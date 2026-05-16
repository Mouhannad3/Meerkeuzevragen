using BL.Exceptions;
using BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Bestandslezer
{
    public class BulkResultaatBestandslezer : IBulkResultaatBestandslezer
    {
        public List<(int TestId, int GebruikerId, string Antwoorden)> LeesBulkResultaten(string pad)
        {
            if (string.IsNullOrWhiteSpace(pad))
            {
                throw new MeerkeuzeException("Pad mag niet leeg zijn.");
            }

            List<(int TestId, int GebruikerId, string Antwoorden)> resultaten = new();

            try
            {
                using StreamReader sr = new StreamReader(pad);

                string? lijn;
                bool eersteLijn = true;

                while ((lijn = sr.ReadLine()) != null)
                {
                    if (eersteLijn)
                    {
                        eersteLijn = false;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(lijn))
                        {
                            string[] delen = lijn.Split(',');

                            if (delen.Length != 3)
                            {
                                throw new MeerkeuzeException("Ongeldig bulkbestand. Verwacht: TestId,IDGebruiker,Antwoorden.");
                            }

                            int testId = int.Parse(delen[0]);
                            int gebruikerId = int.Parse(delen[1]);
                            string antwoorden = delen[2].Trim();

                            resultaten.Add((testId, gebruikerId, antwoorden));
                        }
                    }
                }
            }
            catch (MeerkeuzeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MeerkeuzeException("Fout bij lezen van bulkbestand.", ex);
            }

            return resultaten;
        }
    }
}
