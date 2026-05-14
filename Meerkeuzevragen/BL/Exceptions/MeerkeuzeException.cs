using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    public class MeerkeuzeException : Exception
    {
        public MeerkeuzeException()
        {
        }

        public MeerkeuzeException(string message) : base(message)
        {
        }

        public MeerkeuzeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
