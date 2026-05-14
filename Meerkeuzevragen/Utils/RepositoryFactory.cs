using BL.Interfaces;
using DL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class RepositoryFactory
    {
        public static IOnderwerpRepository GeefOnderwerpRepository(string databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case "SQL":
                    return new OnderwerpRepository(connectionString);
                default:
                    throw new Exception("Onbekend databaseType voor OnderwerpRepository.");
            }
        }

        public static IVraagRepository GeefVraagRepository(string databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case "SQL":
                    return new VraagRepository(connectionString);
                default:
                    throw new Exception("Onbekend databaseType voor VraagRepository.");
            }
        }

        public static ITestRepository GeefTestRepository(string databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case "SQL":
                    return new TestRepository(connectionString);
                default:
                    throw new Exception("Onbekend databaseType voor TestRepository.");
            }
        }

        public static IGebruikerRepository GeefGebruikerRepository(string databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case "SQL":
                    return new GebruikerRepository(connectionString);
                default:
                    throw new Exception("Onbekend databaseType voor GebruikerRepository.");
            }
        }

        public static IResultaatRepository GeefResultaatRepository(string databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case "SQL":
                    return new ResultaatRepository(connectionString);
                default:
                    throw new Exception("Onbekend databaseType voor ResultaatRepository.");
            }
        }
    }
}
