using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace data_access
{
    public class cCIGAR
    {
        private string _code;
        private List<cInDel> _detail;

        public enum InDel
        {
            Match = 'M',
            Insertion = 'I',
            Deletion = 'D',
            Skipped = 'N',
            SoftClipping = 'S',
            HardClipping = 'H',
            Padding = 'P',
            SequenceMatch = '=',
            SequenceMismatch = 'X'
        }

        public cCIGAR(string code)
        {

            this.howCIGARWork();

            this._code = code;
            this._detail = new List<cInDel>();

            this.GenereInDelList();
                        
        }

        public List<cInDel> GetInDels()
        {
            return this._detail;
        }

        public string Corrige(string pSEQ)
        {

            string tSEQ = string.Empty;
            int index = 0;

            foreach (cInDel item in this._detail)
            {
                switch (item.Indel)
                {
                    case 'M':
                        tSEQ += pSEQ.Substring(index, item.Length);
                        index += item.Length;
                        break;
                    case 'I':
                    case 'S':
                        tSEQ += string.Empty;
                        index += item.Length;
                        break;
                    case 'D':
                    case 'N':
                        tSEQ += GenereDumbData(item.Length);
                        index += 0;
                        break;
                    default:
                        break;
                }

            }

            return tSEQ;
        }

        private void GenereInDelList()
        {

            string tNumber = string.Empty;

            foreach (char item in this._code)
            {
                if (char.IsDigit(item))
                {
                    tNumber += item;
                }
                else
                {
                    cInDel tIndel = new cInDel();
                    tIndel.Length = int.Parse(tNumber);
                    tIndel.Indel = item;
                    this._detail.Add(tIndel);

                    tNumber = string.Empty;
                }
            }
        }

        private string GenereDumbData(int length)
        {
            string output = string.Empty;
            char remplissage = ConfigurationManager.AppSettings["CharRemplissage"].FirstOrDefault();

            for (int i = 0; i < length; i++)
            {
                output += remplissage;
            }

            return output;
        }

        private void howCIGARWork()
        {
            /*
            Op  Description
            ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾
            M   alignment match (can be a sequence match or mismatch)
            I   insertion to the reference
            D   deletion from the reference
            N   skipped region from the reference
            S   soft clipping (clipped sequences present in SEQ)
            H   hard clipping (clipped sequences NOT present in SEQ)
            P   padding (silent deletion from padded reference)
            =   sequence match
            X   sequence mismatch

            • H can only be present as the first and/or last operation.
            • S may only have H operations between them and the ends of the string.
            • For mRNA-to-genome alignment, an N operation represents an intron.
              For other types of alignments, the interpretation of N is not defined.
            • Sum of the lengths of the M/I/S/=/X operations shall equal the length of SEQ.
            

            valid = (
                "1M",
                "2H1S1M2S",
                "1S1M1S",
                "1H1H",
                "1H2S1H",
                "1M2S",
            );
            
            invalid = (
                "bogus",
                "1M2H1M",
                "1M2S1M",
            );
            */
        }
    }
}