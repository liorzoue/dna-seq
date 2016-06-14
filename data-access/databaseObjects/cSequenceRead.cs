using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace data_access
{
    public class cSequenceRead : dataAccess
    {
        public enum POSITIONS
        {
            QNAME = 0,
            FLAG = 1,
            POS = 3,
            MAPQ = 4,
            CIGAR = 5,
            MRNM = 6,
            MPOS = 7,
            ISIZE = 8,
            SEQ = 9,
            QUAL = 10,
            OPTIONS = 11
        };

        public long idSequence { get; set; }
        public long idSequenceRead { get; set; }

        public string RNAME
        {
            get {
                cSequence seq = new cSequence();
                //seq.getByID(this.idSequence);
                return seq.RNAME;
            }
        }

        public string QNAME { get; set; }
        public int FLAG { get; set; }
        public int POS { get; set; }
        public int MAPQ { get; set; }
        public string CIGAR { get; set; }
        public long MRNM { get; set; }
        public int MPOS { get; set; }
        public int ISIZE { get; set; }
        public string SEQ { get; set; }
        public string QUAL { get; set; }
        public List<cOption> OPTIONS { get; set; }

        public string SEQCorrige { get; set; }
        public string QUALCorrige { get; set; }

        public cAmorceInfo Amorce { get; set; }


        public void GestionINDEL()
        {
            cCIGAR oCIGAR = new cCIGAR(this.CIGAR);

            this.SEQCorrige = oCIGAR.Corrige(this.SEQ);
            this.QUALCorrige = oCIGAR.Corrige(this.QUAL);
        }

        public void GestionAmorces(string rname = "")
        {
            cAmorceInfo amorce = new cAmorceInfo();
            int lngAmorce; 

            amorce.FLAG = this.FLAG;
            if (rname == string.Empty)
            {
                amorce.RNAME = this.RNAME;
            } else
            {
                amorce.RNAME = rname;
            }
            amorce.POS = this.POS;

            lngAmorce = amorce.getLongAmorce();

            this.Amorce = amorce;
        }

        public void Save()
        {
            this.SaveRead();
            //this.SaveOptions();
        }

        private void SaveOptions()
        {
            foreach (cOption opt in this.OPTIONS)
            {
                opt.idSequenceRead = this.idSequenceRead;
                opt.Save();
            }
        }

        private void SaveRead()
        {
            MySqlCommand cmd = this.CreateCommand();

            cmd.CommandText = "INSERT INTO SEQUENCE_READ (ID_SEQUENCE, QNAME, FLAG, POS, MAPQ, CIGAR, MRNM, MPOS, ISIZE, SEQ, QUAL) " +
                    "VALUES (@ID_SEQUENCE, @QNAME, @FLAG, @POS, @MAPQ, @CIGAR, @MRNM, @MPOS, @ISIZE, @SEQ, @QUAL);";

            cmd.Parameters.AddWithValue("@ID_SEQUENCE", this.idSequence);
            cmd.Parameters.AddWithValue("@QNAME", this.QNAME);
            cmd.Parameters.AddWithValue("@FLAG", this.FLAG);
            cmd.Parameters.AddWithValue("@POS", this.POS);
            cmd.Parameters.AddWithValue("@MAPQ", this.MAPQ);
            cmd.Parameters.AddWithValue("@CIGAR", this.CIGAR);
            cmd.Parameters.AddWithValue("@MRNM", this.MRNM);
            cmd.Parameters.AddWithValue("@MPOS", this.MPOS);
            cmd.Parameters.AddWithValue("@ISIZE", this.ISIZE);
            cmd.Parameters.AddWithValue("@SEQ", this.SEQ);
            cmd.Parameters.AddWithValue("@QUAL", this.QUAL);

            this.ExecuteNonQuery(cmd);

            this.idSequenceRead = cmd.LastInsertedId;
        }

        public bool isDebut ()
        {
            return this.FLAG == 99 || this.FLAG == 163;
        }

        public static string ContigeSequence (List<cSequenceRead> lsSequences)
        {
            if (lsSequences.Count != 2)
                throw new ApplicationException("Nombre de sequences d'entrées incorrectes");

            string seqContige = string.Empty;

            if (lsSequences[0].isDebut())
            {
                seqContige = lsSequences[0].SEQ + lsSequences[1].SEQ;
            } else
            {
                seqContige = lsSequences[1].SEQ + lsSequences[0].SEQ;
            }

            return seqContige;
        }
    }
}