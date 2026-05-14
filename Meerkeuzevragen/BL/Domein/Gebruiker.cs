using BL.Exceptions;

namespace BL.Domein
{
    public class Gebruiker
    {
        private int gebruikerId;
        private string naam = string.Empty;

        public int GebruikerId
        {
            get { return gebruikerId; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("GebruikerId moet groter zijn dan 0.");
                }

                gebruikerId = value;
            }
        }

        public string Naam
        {
            get { return naam; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new MeerkeuzeException("Naam van gebruiker mag niet leeg zijn.");
                }

                naam = value.Trim();
            }
        }

        // Voor een nieuwe gebruiker vóór opslaan in de database
        public Gebruiker(string naam)
        {
            Naam = naam;
        }

        // Voor het lezen van de database
        public Gebruiker(int gebruikerId, string naam)
        {
            GebruikerId = gebruikerId;
            Naam = naam;
        }
    }
}