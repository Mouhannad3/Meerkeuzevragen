using BL.Exceptions;

namespace BL.Domein
{
    public class Onderwerp
    {
        private int onderwerpId;
        private string naam = string.Empty;

        public int OnderwerpId
        {
            get { return onderwerpId; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("OnderwerpId moet groter zijn dan 0.");
                }

                onderwerpId = value;
            }
        }

        public string Naam
        {
            get { return naam; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new MeerkeuzeException("Naam mag niet leeg zijn.");
                }

                naam = value.Trim();
            }
        }

        // Voor een nieuw onderwerp vóór opslaan in de database
        public Onderwerp(string naam)
        {
            Naam = naam;
        }

        // Voor het lezen van de database
        public Onderwerp(int onderwerpId, string naam)
        {
            OnderwerpId = onderwerpId;
            Naam = naam;
        }
        public override string ToString()
        {
            return Naam;
        }
    }
}
