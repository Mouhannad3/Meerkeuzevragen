using BL.Exceptions;

namespace BL.Domein
{
    public class Antwoord
    {
        private int antwoordId;
        private string tekst = "";
        private bool isCorrect;

        public int AntwoordId
        {
            get { return antwoordId; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("AntwoordId moet groter zijn dan 0.");
                }

                antwoordId = value;
            }
        }

        public string Tekst
        {
            get { return tekst; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new MeerkeuzeException("Tekst van antwoord mag niet leeg zijn.");
                }

                tekst = value.Trim();
            }
        }

    public bool IsCorrect
    {
        get { return isCorrect; }
        private set { isCorrect = value; }
    }

        // Voor een nieuw antwoord vóór opslaan in de database
        public Antwoord(string tekst, bool isCorrect)
        {
            Tekst = tekst;
            IsCorrect = isCorrect;
        }

        // Voor het lezen van de database
        public Antwoord(int antwoordId, string tekst, bool isCorrect)
        {
            AntwoordId = antwoordId;
            Tekst = tekst;
            IsCorrect = isCorrect;
        }
    }
}