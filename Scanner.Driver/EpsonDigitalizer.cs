using System;
using com.epson.bank.driver;

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

        public EpsonDigitalizer()
        {
            _mObjmfDevice = new MFDevice();
            _mObjmfBase = new MFBase();
            _mObjmfScanFront = new MFScan();
            _mObjmfScanBack = new MFScan();
            _mObjmfMicr = new MFMicr();
            _mObjmfProcess = new MFProcess();
            _mObjmfDevice.SCNMICRStatusCallback += ScnmicrSetStatusBack;
        }

        private static void ScnmicrSetStatusBack(int iTransactionNumber, MainStatus mainStatus, ErrorCode subStatus, string portName)
        {
            switch (mainStatus)
            {
                case MainStatus.MF_FUNCTION_START:
                    break;
                case MainStatus.MF_CHECKPAPER_PROCESS_START:
                    break;
                case MainStatus.MF_DATARECEIVE_START:
                    break;
                case MainStatus.MF_DATARECEIVE_DONE:
                    break;
                case MainStatus.MF_CHECKPAPER_PROCESS_DONE:
                    break;
                case MainStatus.MF_FUNCTION_DONE:
                    break;
                case MainStatus.MF_ERROR_OCCURED:
                    break;
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
            CheckResponse(_mObjmfDevice.SCNMICRFunctionContinuously(_mObjmfBase, FunctionType.MF_GET_BASE_DEFAULT));
            _mObjmfBase.Timeout = 1;
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
    }
}