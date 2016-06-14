using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using file_reader;
using data_access;
using slack_client;
using fastq_generator;

using log4net;
using log4net.Config;
using System.IO;
using System.Diagnostics;

namespace console_dna
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            ConfigureLog();

            if (args.Length < 2)
            {
                Console.WriteLine("Il n'y a pas assez d'arguments.");
                args[0] = "help";
            }

            switch (args[0])
            {
                case "-f":
                case "--file":
                    if (args.Length == 2) args[2] = string.Empty;
                    EnregistreFichier(args[1], args[2]);
                    break;
                case "-d":
                case "--folder":
                    if (args.Length == 2) args[2] = string.Empty;
                    EnregistreDossier(args[1], args[2]);
                    break;
                case "-a":
                case "--amorces":
                    if (args.Length == 2) args[2] = string.Empty;
                    EnregistreAmorces(args[1], args[2]);
                    break;
                case "-i":
                case "--individu":
                    LoadIndividu(args[1]);
                    break;
                case "-e":
                case "--export":
                    if (args.Length != 3)
                    {
                        Console.WriteLine("Nombre d'arguments invalide");
                        break;
                    }
                    var individu = args[2];
                    var format = args[1];
                    ExportIndividu(individu, format);
                    break;
                default:
                    Console.WriteLine("Usage :");
                    Console.WriteLine("dna-seq.exe [argument] [options]");
                    Console.WriteLine("  -f [filename.sam] [--append]");
                    Console.WriteLine("  -d [foldername] [--append]");
                    Console.WriteLine("  -a [filename.csv] [--truncate] ");
                    Console.WriteLine("  -e fastq [individu]");
                    break;
            }

            Log.Info("Fin");
            
        }

        private static void ExportIndividu(string individu, string format)
        {
            LogMe($"Traitement de l'individu {individu}");
            LogMe($"Format de sortie : {format}");


            switch (format)
            {
                case "fastq":
                case "fasta":
                    var fastqFile = new fastq(individu);
                    break;
                default:
                    break;
            }

            LogMe($"");
        }

        private static void EnregistreDossier(string dir, string option)
        {
            LogMe($"Traitement du dossier : {dir}");
            int i = 1;
            var files = Directory.GetFiles(dir, "*.sam");
            foreach (string filename in files)
            {
                LogMe($"Fichier {i}/{files.Count()}");
                EnregistreFichier(filename, option);
                i++;
            }

            LogMe("Fin");
        }

        private static void LoadIndividu(string pIndividu)
        {
            Console.WriteLine("Informations sur l'individu :" + pIndividu);
            log4net.GlobalContext.Properties["iName"] = pIndividu;
            cIndividu individu = new cIndividu(pIndividu);
            foreach (cSequence item in individu.Sequences)
            {
                var groupBy = item.Reads.GroupBy(i => i.QNAME);
            }

            Console.WriteLine("Enregistrement terminé ! (" + individu.Sequences.Count + " sequences)");
        }

        private static void EnregistreAmorces(string fileName, string truncate)
        {
            Log.Info("Fichier d'entrée : " + fileName);

            Console.WriteLine("Traitement du fichier : " + fileName);

            List<cAmorceInfo> lsAmorces = cAmorceInfo.LoadFromFile(fileName);

            Console.WriteLine("Enregistrement dans la base de données...");

            if (truncate == "--truncate")
            {
                cAmorceInfo.TruncateData();
            }
            lsAmorces.ForEach(item => item.Save());

            Console.WriteLine("Enregistrement terminé ! (" + lsAmorces.Count + " informations sur les amorces)");

            Log.Info("Amorces : " + lsAmorces.Count + " informations enregistrées.");
        }

        private static void EnregistreFichier(string fileName, string option)
        {
            bool isAppend = false;

            LogMe("Fichier d'entrée : " + fileName);

            switch (option)
            {
                case "--append":
                    isAppend = true;
                    break;
                default:
                    break;
            }

            //Start Chrono
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            LogMe("Traitement du fichier : " + fileName);

            cIndividu individu = new cIndividu();
            individu.LoadFromFile(fileName);

            LogMe("Enregistrement dans la base de données...");

            try
            {
                individu.Save(isAppend);
            }
            catch (Exception ex)
            {
                Log.Error("Erreur d'enregistrement de l'individu");
                Log.Error(ex);

                LogMe("Erreur d'enregistrement de l'individu");
                LogMe($"{ex.Message}");

                individu.MoveError(fileName);
            }

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            Process proc = Process.GetCurrentProcess();

            LogMe("RunTime " + elapsedTime);

            LogMe("L'individu '" + individu.Libelle + "' a été enregistré.");
            LogMe("Il contient " + individu.Sequences.Count + " séquences.");
            LogMe($"EXECTIME;{individu.Libelle};{elapsedTime};{individu.FileLinesCount}");
            LogMe($"MEM-PROC;{proc.PrivateMemorySize64.ToString()};{proc.TotalProcessorTime.TotalSeconds.ToString()}");

            proc.Dispose();
        }

        private static void ConfigureLog()
        {
            XmlConfigurator.Configure();
        }

        private static void LogMe(string message)
        {
            Console.WriteLine(message);
            Log.Info(message);

            SlackClient client = new SlackClient();

            client.PostMessage(username: "Mr. Cicharpe",
                       text: message,
                       channel: "#dev");
        }
    }
}
