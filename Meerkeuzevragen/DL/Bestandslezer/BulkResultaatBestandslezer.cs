using BL.Exceptions;
using BL.Interfaces;

namespace DL.Bestandslezer
{
    public class BulkResultaatBestandslezer : IBulkResultaatBestandslezer
    {
        public List<(int GebruikerId, string Antwoorden)> LeesBulkResultaten(string pad)
        {
            if (string.IsNullOrWhiteSpace(pad))
            {
                throw new MeerkeuzeException("Pad mag niet leeg zijn.");
            }

            List<(int GebruikerId, string Antwoorden)> resultaten = new();

            try
            {
                using StreamReader sr = new StreamReader(pad);

                string? lijn;
                bool eersteLijn = true;
                int lijnNummer = 0;

                while ((lijn = sr.ReadLine()) != null)
                {
                    lijnNummer++;

                    if (eersteLijn)
                    {
                        eersteLijn = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(lijn))
                    {
                        continue;
                    }

                    string[] delen = lijn.Split(',');

                    if (delen.Length != 3)
                    {
                        throw new MeerkeuzeException(
                            $"Ongeldig bulkbestand op lijn {lijnNummer}. Verwacht: TestId,IDGebruiker,Antwoorden."
                        );
                    }

                    if (!int.TryParse(delen[0].Trim(), out int testId))
                    {
                        throw new MeerkeuzeException($"TestId moet een getal zijn op lijn {lijnNummer}.");
                    }

                    if (!int.TryParse(delen[1].Trim(), out int gebruikerId))
                    {
                        throw new MeerkeuzeException($"IDGebruiker moet een getal zijn op lijn {lijnNummer}.");
                    }

                    string antwoorden = delen[2].Trim().ToUpper();

                    if (string.IsNullOrWhiteSpace(antwoorden))
                    {
                        throw new MeerkeuzeException($"Antwoorden mogen niet leeg zijn op lijn {lijnNummer}.");
                    }

                    resultaten.Add((gebruikerId, antwoorden));
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