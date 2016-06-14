using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using file_reader;
using log4net;
using log4net.Config;

namespace data_access
{
    public class cAmorceInfo : dataAccess
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(dataAccess));

        public string NAME { get; set; }
        public string RNAME { get; set; }
        public int FLAG { get; set; }
        public int LNG { get; set; }
        public int POS { get; set; }

        public enum POSITIONS
        {
            NAME = 1,
            RNAME = 3,
            FLAG = 4,
            LNG = 5,
            POS = 7
        }
        
        public static List<cAmorceInfo> LoadFromFile(string fileName)
        {
            fileReader fr = new fileReader(fileName);
            List<string> lsLines = fr.getAllLinesFromCSV();

            return lsLines.Skip(1).Select(line => line.Split(';')).Select(aLine => new cAmorceInfo
            {
                NAME = aLine[(int)POSITIONS.NAME],
                RNAME = aLine[(int) POSITIONS.RNAME],
                FLAG = int.Parse(aLine[(int) POSITIONS.FLAG]),
                LNG = int.Parse(aLine[(int) POSITIONS.LNG]),
                POS = int.Parse(aLine[(int) POSITIONS.POS])
            }).ToList();
        }

        public static void TruncateData()
        {
            dataAccess da = new dataAccess();
            MySqlCommand cmd = da.CreateCommand();

            cmd.CommandText = "TRUNCATE TABLE AMORCES;";
            da.ExecuteNonQuery(cmd);
        }

        public void Save()
        {
            MySqlCommand cmd = this.CreateCommand();

            cmd.CommandText = "INSERT INTO AMORCES (NOM, RNAME, FLAG, LNG, POS) VALUES (@NAME, @RNAME, @FLAG, @LNG, @POS);";
            cmd.Parameters.AddWithValue("@NAME", this.NAME);
            cmd.Parameters.AddWithValue("@RNAME", this.RNAME);
            cmd.Parameters.AddWithValue("@FLAG", this.FLAG);
            cmd.Parameters.AddWithValue("@LNG", this.LNG);
            cmd.Parameters.AddWithValue("@POS", this.POS);

            this.ExecuteNonQuery(cmd);
            
        }


        public int getLongAmorce(int sequenceLength = 0)
        {
            MySqlCommand cmd = this.CreateCommand();

            int output = 0;
            int erreurPOS;
            if (this.FLAG == 99 || this.FLAG == 163)
            {
                sequenceLength = 0;
            }
            
            int.TryParse(ConfigurationManager.AppSettings["ErreurPos"], out erreurPOS);

            cmd.CommandText = "SELECT NOM, LNG FROM AMORCES " +
                "WHERE RNAME = @RNAME AND FLAG = @FLAG AND " +
                "((POS + LNG < @POS + @ERR AND POS + LNG > @POS - @ERR) " +
                "OR " +
                "(POS < @POS + @ERR AND POS > @POS - @ERR)) " +
                "ORDER BY ABS( POS - @POS ) ASC LIMIT 1;";

            cmd.Parameters.AddWithValue("@ERR", erreurPOS);
            cmd.Parameters.AddWithValue("@RNAME", this.RNAME);
            cmd.Parameters.AddWithValue("@POS", this.POS + sequenceLength);
            cmd.Parameters.AddWithValue("@FLAG", this.FLAG);
            
            this.OpenConnection();

            MySqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    output = dr.GetInt32("LNG");
                    this.NAME = dr.GetString("NOM");
                }

            }
            else
            {
                log.Error("Il n'y a pas d'amorces correspondante !");
                log.Error("Informations : RNAME='" + this.RNAME + "' FLAG='" + this.FLAG + "' POS='" + this.POS + "'");
                Console.WriteLine("Il n'y a pas d'amorces correspondante !");
                Console.WriteLine("Informations : RNAME='" + this.RNAME + "' FLAG='" + this.FLAG + "' POS='" + this.POS + "'");
                this.CloseConnection();
                throw new ApplicationException("Amorce inconnue : RNAME='" + this.RNAME + "' FLAG='" + this.FLAG + "' POS='" + this.POS + "'");
            }

            this.CloseConnection();

            this.LNG = output;
            return output;
        }

        public string truncateSequence(cSequenceRead sequence)
        {
            return truncateSequence(sequence, this.FLAG == 99 || this.FLAG == 163);
        }

        private string reverseString(string input)
        {
            return new string(input.ToCharArray().Reverse().ToArray());
        }

        private string truncateSequence(cSequenceRead read, bool isDebut)
        {
            int lng = 0;
            if (read.SEQCorrige.Length > 0)
            {
                lng = this.getLongAmorce(read.SEQCorrige.Length);
            }

            if (lng == 0)
            {
                return read.SEQCorrige;
            }

            string sequence;
            try
            {
                if (lng > read.SEQCorrige.Length) return string.Empty;
                if (!isDebut)
                {
                    sequence = reverseString(reverseString(read.SEQCorrige).Remove(0, lng));
                }
                else
                {
                    sequence = read.SEQCorrige.Remove(0, lng);
                }
                return sequence;
            } catch (Exception ex)
            {
                return read.SEQCorrige;
            }
        }

    }
}