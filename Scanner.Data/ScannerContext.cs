using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Scanner.Data
{
    public class ScannerContext
    {
        private readonly MySqlConnection _conn;
        private MySqlCommand _cmd;
        private bool _saveImageToTemp;

        public ScannerContext(string connectionString)
        {
            _conn = new MySqlConnection(connectionString);
            var appSettingsKey = ConfigurationManager.AppSettings["SaveImageToTempPath"];
            _saveImageToTemp = appSettingsKey != null && bool.Parse(appSettingsKey);
        }

        private MySqlCommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = _conn.CreateCommand();
                    _cmd.CommandText =
                            @"insert into Scannergeneral 
                        (ScannerFecha, Scannercmc7, ScannerFotoFrente, ScannerFotoDorso, ScannerEstado) 
                        values (@fecha, @cmc7, @frente, @dorso, @estado)";
                    _cmd.Parameters.Add(new MySqlParameter("estado", MySqlDbType.Int64));
                    _cmd.Parameters.Add(new MySqlParameter("fecha", MySqlDbType.Timestamp));
                    _cmd.Parameters.Add(new MySqlParameter("cmc7", MySqlDbType.VarChar));
                    _cmd.Parameters.Add(new MySqlParameter("frente", MySqlDbType.Blob));
                    _cmd.Parameters.Add(new MySqlParameter("dorso", MySqlDbType.Blob));
                }
                return _cmd;
            }
        }

        public void Save(Document document)
        {
            SaveImagesToDisk(document);
            _conn.Open();
            Command.Parameters["estado"].Value = document.State;
            Command.Parameters["fecha"].Value = document.Date;
            Command.Parameters["cmc7"].Value = document.Cmc7;
            Command.Parameters["frente"].Value = document.FrontImage;
            Command.Parameters["dorso"].Value = document.BackImage;            
            _cmd.ExecuteNonQuery();
            _conn.Close();
        }

        private void SaveImagesToDisk(Document document)
        {
            if (!_saveImageToTemp)
                return;

            const string extension = ".jpg";
            Parallel.Invoke(
                () => SaveImageToFile($"_frente{extension}", document.FrontImage)
                , () => SaveImageToFile($"_dorso{extension}", document.BackImage));
        }

        private static void SaveImageToFile(string fileNamePostFix, byte[] imageStream)
        {
            try
            {
                var frontFile = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), $"{DateTime.Now.Ticks}{fileNamePostFix}");
                using (var fos = new BinaryWriter((new StreamWriter(frontFile)).BaseStream))
                {
                    fos.Write(imageStream);
                    fos.Close();
                }
            }
            catch
            {

            }
        }
    }
}
