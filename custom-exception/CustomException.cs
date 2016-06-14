using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace custom_exception
{
    public class CustomException : Exception
    {
        public CustomException()
        {

        }

        public CustomException(string message) : base(message)
        {
            Console.WriteLine(message);
        }

        public CustomException(string message, Exception inner) : base(message, inner)
        {
            Console.WriteLine(message);
            Console.WriteLine(inner.Message);
        }
    }
}
