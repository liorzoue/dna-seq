using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using data_access;
using slack_client;
using fastq_generator;

using log4net;
using System.Configuration;

namespace fastq_generator
{
    public class fastq
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(fastq));

        public fastq(string pIndividu)
        {
            this.genereFile(pIndividu);
        }

        private void genereFile(string individu)
        {
            cIndividu oIndividu = new cIndividu(individu);
            List<string> output = new List<string>();
            string idChar = ConfigurationManager.AppSettings["CharIdentificationLigne"];
            int unknownAmorces = 0;

            LogMe($"Individu chargé");

            foreach (cSequence item in oIndividu.Sequences)
            {
                var reads = item.Reads.GroupBy(i => i.QNAME);
                foreach (var read in reads)
                {
                    if (read.Count() != 2)
                    {
                        LogMe($"WARNING : Il y a plus de deux reads");
                        LogMe($"QNAME : {read.Key}");
                        LogMe($"Sequence : {item.RNAME}");
                    }
                    else
                    {
                        try
                        {
                            
                            foreach (cSequenceRead sequenceRead in read)
                            {
                                cAmorceInfo amorceInfo = new cAmorceInfo
                                {
                                    RNAME = item.RNAME,
                                    FLAG = sequenceRead.FLAG,
                                    POS = sequenceRead.POS
                                };

                                sequenceRead.SEQ = amorceInfo.truncateSequence(sequenceRead);
                            }

                            var retour = cSequenceRead.ContigeSequence(read.ToList());
                            var fRead = read.FirstOrDefault();
                            if (fRead != null) {
                                fRead.GestionAmorces(item.RNAME);
                                output.Add(idChar + read.Key + "|" + fRead.Amorce.NAME);
                                output.Add(retour);
                                output.Add(string.Empty);
                            }

                        }
                        catch (ApplicationException ex)
                        {
                            unknownAmorces++;
                        }
                        catch (Exception ex)
                        {
                            LogMe(ex.Message);
                        }
                    }
                }
            }

            LogMe($"{unknownAmorces} amorces inconnue(s)");

            writeFile(individu, output.ToArray());
        }

        private void writeFile(string fileName, string[] preFile)
        {
            string ext = "fasta";

            try
            {
                if (System.IO.File.Exists(fileName + "." + ext))
                    System.IO.File.Delete(fileName + "." + ext);
                System.IO.File.WriteAllLines(@fileName+".tmp", preFile);
                System.IO.File.Move(fileName + ".tmp", fileName + "." + ext);
            } catch (Exception ex)
            {
                throw;
            }
        }

        private static void LogMe(string message)
        {
            SlackClient client = new SlackClient();
            Console.WriteLine(message);
            Log.Info(message);

            client.PostMessage(message);
        }
    }
}
