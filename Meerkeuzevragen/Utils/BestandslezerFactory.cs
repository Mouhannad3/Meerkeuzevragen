using BL.Interfaces;
using DL.Bestandslezer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class BestandslezerFactory
    {
        public static IMeerkeuzeBestandslezer GeefMeerkeuzeBestandslezer(string fileType)
        {
            switch (fileType)
            {
                case "TXT":
                    return new MeerkeuzeBestandslezer();
                default:
                    throw new Exception("Onbekend fileType voor MeerkeuzeBestandslezer.");
            }
        }
        public static IBulkResultaatBestandslezer GeefBulkResultaatBestandslezer(string fileType)
        {
            switch (fileType)
            {
                case "TXT":
                    return new BulkResultaatBestandslezer();
                default:
                    throw new Exception("Onbekend fileType voor BulkResultaatBestandslezer.");
            }
        }
    }
}
