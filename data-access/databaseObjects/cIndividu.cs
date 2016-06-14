using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using file_reader;
using slack_client;
using System.IO;
using log4net;
using log4net.Config;

namespace data_access
{
    public class cIndividu : dataAccess
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(cIndividu));

        public long idIndividu { get; set; }
        public string Libelle { get; set; }
        public List<cSequence> Sequences { get; set; }
        public int FileLinesCount { get; private set; }

        #region Constructor

        public cIndividu ()
        {
            this.InitializeIndividu();
        }

        public cIndividu(string individu)
        {
            this.InitializeIndividu();
            this.LoadFromDatabase(individu);
        }

        #endregion

        public void Save(bool append = false)
        {
            this.idIndividu = 0;

            if (append)
                this.LoadIndividu(this.Libelle);

            if (append && this.idIndividu > 0)
                LogMe("L'individu existait déjà, les données seront ajoutées (commande --append)");

            if (this.idIndividu == 0)
                this.SaveIndividu();
            
            if (this.idIndividu == 0) throw new ApplicationException("Pas d'individu à enregistrer.");

            this.SaveSequences();
        }

        #region Methodes privées

        private void InitializeIndividu()
        {
            this.idIndividu = 0;
            this.Libelle = string.Empty;
            this.Sequences = new List<cSequence>();
        }

        public void LoadFromFile(string fileName)
        {
            fileReader fr = new fileReader(fileName);

            this.Libelle = fr.FileNameShort;
            this.Sequences = ExtractFileData(fr.AllLines);

            this.FileLinesCount = fr.Count;
            this.moveFileLoaded(fileName);
            
        }


        public void MoveError(string fileName)
        {
            moveFile($"{fileName}.loaded", $"{fileName}.error");
        }


        private void moveFileLoaded(string filename)
        {
            var newName = $"{filename}.loaded";
            moveFile(filename, newName);            
        }

        private void moveFile(string oldName, string newName)
        {
            try
            {
                if (!File.Exists(oldName))
                {
                    // This statement ensures that the file is created,
                    // but the handle is not kept.
                    using (FileStream fs = File.Create(oldName)) { }
                }

                // Ensure that the target does not exist.
                if (File.Exists(newName))
                    File.Delete(newName);

                // Move the file.
                File.Move(oldName, newName);
                LogMe($"{oldName} a été déplacé vers {newName}.");

                // See if the original exists now.
                if (File.Exists(oldName))
                {
                    LogMe("Le fichier orignal existe toujours. Ce qui n'est pas normal");
                }

            }
            catch (Exception e)
            {
                LogMe($"Il y a eu une erreur: {e.ToString()}");
            }
        }

        private List<cSequence> ExtractFileData(List<cLine> pLsLines)
        {
            List<cSequence> lsSequences = new List<cSequence>();
            List<cSequenceRead> lsSequenceReads = new List<cSequenceRead>();

            foreach (cLine line in pLsLines)
            {
                if (lsSequences.FindAll(item => item.RNAME == line.RNAME).Count == 0)
                {
                    cSequence newSequence = new cSequence();
                    newSequence.RNAME = line.RNAME;
                    lsSequences.Add(newSequence);
                }

                cSequenceRead newRead = new cSequenceRead();
                newRead.QNAME = line.QNAME.Trim();
                newRead.FLAG = int.Parse(line.FLAG);
                newRead.POS = int.Parse(line.POS);
                newRead.MAPQ = int.Parse(line.MAPQ);
                newRead.CIGAR = line.CIGAR.Trim();
                newRead.MPOS = int.Parse(line.MPOS);
                newRead.ISIZE = int.Parse(line.ISIZE);
                newRead.SEQ = line.SEQ.Trim();
                newRead.QUAL = line.QUAL.Trim();

                newRead.OPTIONS = new List<cOption>();
                foreach (string opt in line.OPTIONS)
                {
                    newRead.OPTIONS.Add(new cOption(opt));
                }

                if (line.MRNM == "=")
                {
                    //newRead.MRNM = lsSequences.First(item => item.RNAME == line.RNAME).idSequence;
                }
                else
                {
                    newRead.MRNM = lsSequences.First(item => item.RNAME == line.MRNM).idSequence;
                }
                if (lsSequences.First(item => item.RNAME == line.RNAME).Reads == null)
                {
                    lsSequences.First(item => item.RNAME == line.RNAME).Reads = new List<cSequenceRead>();
                }

                newRead.GestionINDEL();
                //newRead.GestionAmorces();

                lsSequences.First(item => item.RNAME == line.RNAME).Reads.Add(newRead);
            }

           // lsSequences.ForEach(i => i.Reads.GroupBy(j => j.QNAME));
            
            return lsSequences;
        }

        private void SaveIndividu()
        {
            MySqlCommand cmd = this.CreateCommand();

            cmd.CommandText = "INSERT INTO INDIVIDU (LIBELLE) VALUES (@LIBELLE);";
            cmd.Parameters.AddWithValue("@LIBELLE", this.Libelle);

            this.ExecuteNonQuery(cmd);

            this.idIndividu = cmd.LastInsertedId;
        }

        private void SaveSequences()
        {
            foreach (cSequence seq in this.Sequences)
            {
                seq.idIndividu = this.idIndividu;
                seq.Save();
            }
        }

        #endregion

        private void LoadFromDatabase(string individu)
        {
            this.LoadIndividu(individu);
            if (this.idIndividu > 0)
                this.Sequences = this.LoadSequences(this.idIndividu);

        }

        private void LoadIndividu(string individu)
        {
            MySqlCommand cmd = this.CreateCommand();
            MySqlDataReader dr;

            cmd.CommandText = "SELECT ID_INDIVIDU, LIBELLE FROM INDIVIDU WHERE LIBELLE = @LIBELLE;";
            cmd.Parameters.AddWithValue("@LIBELLE", individu);

            this.idIndividu = 0;

            try
            {
                this.OpenConnection();

                dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        this.idIndividu = dr.GetInt64("ID_INDIVIDU");
                        this.Libelle = dr.GetString("LIBELLE");
                    }

                }

                this.CloseConnection();

            }
            catch (Exception ex)
            {

                this.CloseConnection();
                throw;
            }
        }

        private List<cSequence> LoadSequences(long idIndividu)
        {
            MySqlCommand cmd = this.CreateCommand();
            MySqlDataReader dr;
            List<cSequence> lsSequences = new List<cSequence>();

            cmd.CommandText = "SELECT ID_SEQUENCE, RNAME FROM SEQUENCE WHERE ID_INDIVIDU = @ID_INDIVIDU;";
            cmd.Parameters.AddWithValue("@ID_INDIVIDU", idIndividu);

            try
            {
                this.OpenConnection();

                dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        cSequence seq = new cSequence(dr.GetInt64("ID_SEQUENCE"));
                        seq.idIndividu = idIndividu;
                        seq.RNAME = dr.GetString("RNAME");
                        lsSequences.Add(seq);
                    }
                }

                this.CloseConnection();

            }
            catch (Exception ex)
            {

                this.CloseConnection();
                throw;
            }
            

            return lsSequences;
        }

        private static void LogMe(string message)
        {
            Console.WriteLine(message);
            Log.Info(message);

            SlackClient client = new SlackClient();

            client.PostMessage(message);
        }
    }
}
