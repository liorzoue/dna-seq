using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace data_access
{
    public class cSequence : dataAccess
    {

        public enum POSITIONS
        {
            RNAME = 2
        }

        private string _amorce_name;

        public long idSequence { get; set; }
        public long idIndividu { get; set; }
        // ReSharper disable once InconsistentNaming
        public string RNAME { get; set; }

        public List<cSequenceRead> Reads { get; set; }

        public cSequence() : base()
        {

        }

        public cSequence(long idSequence)
        {
            this.idSequence = idSequence;
            this.LoadReads();
        }

        private void LoadReads()
        {
            MySqlCommand cmd = this.CreateCommand();
            List<cSequenceRead> lsReads = new List<cSequenceRead>();

            cmd.CommandText = "SELECT ID_SEQUENCE_READ, ID_SEQUENCE, QNAME, FLAG, POS, MAPQ, CIGAR, MRNM, MPOS, ISIZE, SEQ, QUAL FROM SEQUENCE_READ WHERE ID_SEQUENCE = @ID_SEQUENCE;";
            cmd.Parameters.AddWithValue("@ID_SEQUENCE", this.idSequence);

            try
            {
                this.OpenConnection();

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        cSequenceRead read = new cSequenceRead
                        {
                            idSequenceRead = dr.GetInt64("ID_SEQUENCE_READ"),
                            QNAME = dr.GetString("QNAME"),
                            FLAG = dr.GetInt32("FLAG"),
                            POS = dr.GetInt32("POS"),
                            MAPQ = dr.GetInt32("MAPQ"),
                            CIGAR = dr.GetString("CIGAR"),
                            MRNM = dr.GetInt32("MRNM"),
                            MPOS = dr.GetInt32("MPOS"),
                            ISIZE = dr.GetInt32("ISIZE"),
                            SEQ = dr.GetString("SEQ"),
                            QUAL = dr.GetString("QUAL")
                        };

                        read.GestionINDEL();

                        lsReads.Add(read);
                    }
                }

                this.CloseConnection();

            }
            catch (Exception ex)
            {

                this.CloseConnection();
                throw;
            }


            this.Reads =  lsReads;
        }

        public void Save()
        {
            this.saveSequence();
            this.saveSequenceReads();
        }
        
        private void saveSequence()
        {
            MySqlCommand cmd = this.CreateCommand();

            cmd.CommandText = "INSERT INTO SEQUENCE (ID_INDIVIDU, RNAME) " +
                    "VALUES (@ID_INDIVIDU, @RNAME);";

            cmd.Parameters.AddWithValue("@ID_INDIVIDU", idIndividu);
            cmd.Parameters.AddWithValue("@RNAME", this.RNAME);

            this.ExecuteNonQuery(cmd);

            this.idSequence = cmd.LastInsertedId;
        }

        private void saveSequenceReads()
        {
            foreach (cSequenceRead read in this.Reads)
            {
                read.idSequence = this.idSequence;
                read.Save();
            }
        }
    }
}
