using MySql.Data.MySqlClient;

namespace Scanner.Data
{
    public class ScannerContext
    {
        private readonly MySqlConnection conn;
        private MySqlCommand cmd;

        public ScannerContext(string connectionString)
        {
            conn = new MySqlConnection(connectionString);
        }

        private MySqlCommand Command
        {
            get
            {
                if (cmd == null)
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandText =
                            @"insert into Scannergeneral 
                        (ScannerFecha, Scannercmc7, ScannerFotoFrente, ScannerFotoDorso, ScannerEstado) 
                        values (@fecha, @cmc7, @frente, @dorso, @estado)";
                    cmd.Parameters.Add(new MySqlParameter("estado", MySqlDbType.Int64));
                    cmd.Parameters.Add(new MySqlParameter("fecha", MySqlDbType.Timestamp));
                    cmd.Parameters.Add(new MySqlParameter("cmc7", MySqlDbType.VarChar));
                    cmd.Parameters.Add(new MySqlParameter("frente", MySqlDbType.Blob));
                    cmd.Parameters.Add(new MySqlParameter("dorso", MySqlDbType.Blob));
                }
                return cmd;
            }
        }

        public void Save(Document document) {
            conn.Open();
            Command.Parameters["estado"].Value = document.State;
            Command.Parameters["fecha"].Value = document.Date;
            Command.Parameters["cmc7"].Value = document.Cmc7;
            Command.Parameters["frente"].Value = document.FrontImage;
            Command.Parameters["dorso"].Value = document.BackImage;            
            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }
}
