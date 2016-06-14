using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using slack_client;
using log4net;
using log4net.Config;

using MySql.Data.MySqlClient;

namespace data_access
{
    public partial class dataAccess
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(dataAccess));

        private MySqlConnection _conn;
        private string _connectionString;

        #region Constructeur
        public dataAccess()
        {
            this._connectionString = ConfigurationManager.ConnectionStrings["database"].ConnectionString;
            this.InitConnexion();
        }
        
        private void InitConnexion()
        {
            this._conn = new MySqlConnection(this._connectionString);
        }
        #endregion

        public MySqlCommand CreateCommand()
        {
            return this._conn.CreateCommand();
        }

        internal void OpenConnection()
        {
            try
            {
                if (this._conn.State != System.Data.ConnectionState.Open)
                {
                    this._conn.Open();
                }
            } catch (Exception ex)
            {
                log.Error("La connection n'a pas pu être ouverte");
                log.Error(ex.Message);
                SlackErrorReport("OpenConnection");
                SlackErrorReport(ex.Message);
                throw;
            }
        }

        internal void CloseConnection()
        {
            try
            {
                if (this._conn.State != System.Data.ConnectionState.Closed)
                {
                    this._conn.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("La connection n'a pas pu être fermée");
                log.Error(ex.Message);
                SlackErrorReport("CloseConnection");
                SlackErrorReport(ex.Message);
                throw;
            }
        }

        public void ExecuteNonQuery(MySqlCommand cmd)
        {
            this.OpenConnection();

            try
            {
                cmd.ExecuteNonQuery();
            } catch (Exception ex)
            {
                log.Error("Erreur lors de l'execution de la requete !");
                log.Error(ex.Message);
                SlackErrorReport("ExecuteNonQuery");
                SlackErrorReport(ex.Message);
                throw;
            }

            this.CloseConnection();
        }

        public MySqlDataReader ExecuteSelect(MySqlCommand cmd)
        {
            MySqlDataReader reader;
            this.OpenConnection();

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                log.Error("Erreur lors de l'execution du select");
                log.Error(ex.Message);
                SlackErrorReport("ExecuteSelect");
                SlackErrorReport(ex.Message);
                throw;
            }

            this.CloseConnection();

            return reader;
        }

        public void SlackErrorReport(string message)
        {
            SlackClient client = new SlackClient();
            client.PostMessage($"ERROR: {message}");
        }

    }
}
