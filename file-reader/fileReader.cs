using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace file_reader
{
    public class fileReader
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FileNameShort { get; set; }
        public int Count { get; set; }

        public List<cLine> AllLines { get
            {
                return readAllLines();
            }
        }

        public fileReader(string file)
        {
            this.FileName = file;
            this.FileExtension = file.Split('.')[1];
            this.FileNameShort = file.Split('.')[0].Replace("_clipped", string.Empty).Replace("clipped", string.Empty).Split(Convert.ToChar("\\")).LastOrDefault();

        }

        public List<string> getAllLinesFromCSV()
        {
            List<string> lsLines;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(@FileName);
                lsLines = lines.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

            return lsLines;
        }
        
        private List<cLine> readAllLines ()
        {
            List<cLine> lsLines = new List<cLine>();

            try
            {
                string[] lines = System.IO.File.ReadAllLines(@FileName);
                lsLines = formatLines(lines);
            }
            catch (Exception ex)
            {
                throw;
            }

            this.Count = lsLines.Count;
            return lsLines;
        }

        private List<cLine> formatLines(string[] lines)
        {
            List<cLine> lsLines = new List<cLine>();

            foreach (string line in lines)
            {
                lsLines.Add(new cLine(line));
            }

            return lsLines;
        }
    }
}
