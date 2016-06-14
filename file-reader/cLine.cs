using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace file_reader
{
    public class cLine
    {
        public enum POSITIONS
        {
            QNAME = 0,
            FLAG = 1,
            RNAME = 2,
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

        public string id_Sequence { get; set; }
        public string QNAME { get; set; }
        public string FLAG { get; set; }
        public string RNAME { get; set; }
        public string POS { get; set; }
        public string MAPQ { get; set; }
        public string CIGAR { get; set; }
        public string MRNM { get; set; }
        public string MPOS { get; set; }
        public string ISIZE { get; set; }
        public string SEQ { get; set; }
        public string QUAL { get; set; }
        public List<string> OPTIONS { get; set; }

        public cLine(string data)
        {

            string[] line = data.Split('\t');

            this.QNAME = line[(int)POSITIONS.QNAME];
            this.FLAG = line[(int)POSITIONS.FLAG];
            this.RNAME = line[(int)POSITIONS.RNAME];
            this.POS = line[(int)POSITIONS.POS];
            this.MAPQ = line[(int)POSITIONS.MAPQ];
            this.CIGAR = line[(int)POSITIONS.CIGAR];
            this.MRNM = line[(int)POSITIONS.MRNM];
            this.MPOS = line[(int)POSITIONS.MPOS];
            this.ISIZE = line[(int)POSITIONS.ISIZE];
            this.SEQ = line[(int)POSITIONS.SEQ];
            this.QUAL = line[(int)POSITIONS.QUAL];

            this.OPTIONS = new List<string>();

            foreach (string option in line.Skip((int)POSITIONS.OPTIONS))
            {
                this.OPTIONS.Add(option);
            }

        }
    }
}
