﻿using System;
using System.Collections.Generic;
using com.epson.bank.driver;
using Color = com.epson.bank.driver.Color;

namespace Scanner.Driver
{
    public class EpsonDigitalizer : IDigitalizer
    {
        private readonly MFDevice _mObjmfDevice;
        private readonly MFBase _mObjmfBase;
        private readonly MFScan _mObjmfScanFront;
        private readonly MFScan _mObjmfScanBack;
        private readonly MFMicr _mObjmfMicr;
        private readonly MFProcess _mObjmfProcess;
        private IList<Document> _documents;

        public EpsonDigitalizer()
        {
            _mObjmfDevice = new MFDevice();
            _mObjmfBase = new MFBase();
            _mObjmfScanFront = new MFScan();
            _mObjmfScanBack = new MFScan();
            _mObjmfMicr = new MFMicr();
            _mObjmfProcess = new MFProcess();
            _mObjmfDevice.SCNMICRStatusCallback += ScnmicrSetStatusBack;
            _documents = new List<Document>();
        }

        private void ScnmicrSetStatusBack(int iTransactionNumber, MainStatus mainStatus, ErrorCode subStatus, string portName)
        {
            ErrorCode ret;
            switch (mainStatus)
            {
                case MainStatus.MF_FUNCTION_START:
                    break;
                case MainStatus.MF_CHECKPAPER_PROCESS_START:
                    break;
                case MainStatus.MF_DATARECEIVE_START:
                    _documents.Add( new Document() );
                    break;
                case MainStatus.MF_DATARECEIVE_DONE:
                    var document = _documents[_documents.Count - 1];
                    document.Cmc7 = "";
                    document.Id = _mObjmfDevice.TransactionNumber;
                    ret = _mObjmfDevice.GetMicrText(iTransactionNumber, _mObjmfMicr);
                    if (ret != ErrorCode.ERR_MICR_NODATA)
                    {
                        if (!string.IsNullOrEmpty(_mObjmfMicr.MicrStr))
                        {
                            document.Cmc7 = _mObjmfMicr.MicrStr.Substring(0, _mObjmfMicr.MicrStr.IndexOf('\0'));
                            document.Cmc7 = document.Cmc7.Trim();
                        }
                    }
                    byte[][] aux;
                    if (_mObjmfDevice.SCNSelectScanFace(ScanSide.MF_SCAN_FACE_FRONT) == ErrorCode.SUCCESS
                        && _mObjmfDevice.GetScanImage(iTransactionNumber, _mObjmfScanFront) == ErrorCode.SUCCESS
                        && _mObjmfDevice.SCNSelectScanFace(ScanSide.MF_SCAN_FACE_BACK) == ErrorCode.SUCCESS
                        && _mObjmfDevice.GetScanImage(iTransactionNumber, _mObjmfScanBack) == ErrorCode.SUCCESS)
                    {
                        aux = new byte[2][];
                        aux[0] = new byte[_mObjmfScanFront.Data.Length];
                        _mObjmfScanFront.Data.Read(aux[0], 0, aux[0].Length);
                        aux[1] = new byte[_mObjmfScanBack.Data.Length];
                        _mObjmfScanBack.Data.Read(aux[1], 0, aux[1].Length);
                        document.RawImage = TiffUtil.mergeTiffPages(aux);
                    }
                    break;
                case MainStatus.MF_CHECKPAPER_PROCESS_DONE:
                    break;
                case MainStatus.MF_FUNCTION_DONE:
                    Scan();
                    break;
                case MainStatus.MF_ERROR_OCCURED:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mainStatus), mainStatus, null);
            }
        }

        private static void CheckResponse(ErrorCode error)
        {
            if (error != ErrorCode.SUCCESS)
            {
                throw new Exception(error.ToString());
            }
        }

        private void ConfigureMulti()
        {
            // Base
            _mObjmfBase.Timeout = 0;
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfBase, FunctionType.MF_GET_BASE_DEFAULT));
            
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfBase, FunctionType.MF_SET_BASE_PARAM));


            // ScanFront
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfScanFront, FunctionType.MF_GET_SCAN_FRONT_DEFAULT));
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfScanFront, FunctionType.MF_SET_SCAN_FRONT_PARAM));

            CheckResponse(_mObjmfDevice.SCNSelectScanFace(ScanSide.MF_SCAN_FACE_FRONT));
            CheckResponse(_mObjmfDevice.SCNSetImageQuality(ColorDepth.EPS_BI_SCN_1BIT, 0D, Color.EPS_BI_SCN_MONOCHROME, ExOption.EPS_BI_SCN_SHARP));
            CheckResponse(_mObjmfDevice.SCNSetImageFormat(Format.EPS_BI_SCN_TIFF));


            // ScanBack
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfScanBack, FunctionType.MF_GET_SCAN_BACK_DEFAULT));
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfScanBack, FunctionType.MF_SET_SCAN_BACK_PARAM));

            // Micr
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfMicr, FunctionType.MF_GET_MICR_DEFAULT));
            _mObjmfMicr.Font = MfMicrFont.MF_MICR_FONT_CMC7;
            _mObjmfMicr.MicrOcrSelect = MfMicrType.MF_MICR_USE_MICR;
            _mObjmfMicr.Parsing = false;
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfMicr, FunctionType.MF_SET_MICR_PARAM));


            
            // Operational settings when an error occurs
            // Process
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfProcess, FunctionType.MF_GET_PROCESS_DEFAULT));

            _mObjmfProcess.PaperType = MfPaperType.MF_PAPER_TYPE_OTHER;
            _mObjmfProcess.PaperMisInsertionErrorSelect = MfErrorSelect.MF_ERROR_SELECT_NODETECT;
            _mObjmfProcess.DoubleFeedErrorSelect = MfErrorSelect.MF_ERROR_SELECT_DETECT;
            _mObjmfProcess.BaddataErrorSelect = MfErrorSelect.MF_ERROR_SELECT_NODETECT; //Para que no fallen los cmc7            
            _mObjmfProcess.NodataErrorSelect = MfErrorSelect.MF_ERROR_SELECT_NODETECT;
            _mObjmfProcess.ActivationMode = MfActivateMode.MF_ACTIVATE_MODE_HIGH_SPEED;

            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfProcess, FunctionType.MF_SET_PROCESS_PARAM));

        }

        public void Connect()
        {
            CheckResponse(MFDevice.ESCNEnable(Storage.CROP_STORE_MEMORY));
            CheckResponse(_mObjmfDevice.OpenMonPrinter(OpenType.TYPE_PRINTER, "TM-S1000U"));
            CheckResponse(_mObjmfDevice.SCNMICRSetStatusBack());
            ConfigureMulti();
        }

        public void Disconnect()
        {
            _mObjmfDevice.SCNMICRCancelFunction(MfEjectType.MF_EJECT_DISCHARGE);
            _mObjmfDevice.SCNMICRCancelStatusBack();
            _mObjmfDevice.CloseMonPrinter();
        }

        public void Scan()
        {
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(FunctionType.MF_EXEC));
        }

        public IList<Document> Documents => _documents;
    }
}