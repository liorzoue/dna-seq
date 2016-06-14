using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace data_access
{
    public class cOption : dataAccess
    {
        public long idSequenceRead { get; set; }

        public long idOption { get; set; }
        public string Valeur { get; set; }

        public cOption(string valeur)
        {
            this.Valeur = valeur;
        }

        public void Save()
        {
            MySqlCommand cmd = this.CreateCommand();

            cmd.CommandText = "INSERT INTO READ_OPTIONS (ID_SEQUENCE_READ, VALEUR) VALUES (@ID_SEQUENCE_READ, @VALEUR);";
            cmd.Parameters.AddWithValue("@ID_SEQUENCE_READ", this.idSequenceRead);
            cmd.Parameters.AddWithValue("@VALEUR", this.Valeur);

            this.ExecuteNonQuery(cmd);

            this.idOption = cmd.LastInsertedId;
        }
    }
}