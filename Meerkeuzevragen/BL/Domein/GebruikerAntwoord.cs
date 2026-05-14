using BL.Exceptions;

namespace BL.Domein
{
    public class GebruikerAntwoord
    {
        private int gebruikerAntwoordId;
        private TestResultaat testResultaat = null!;
        private TestVraag testVraag = null!;
        private char gekozenLetter;
        private bool isCorrect;

        public int GebruikerAntwoordId
        {
            get { return gebruikerAntwoordId; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("GebruikerAntwoordId moet groter zijn dan 0.");
                }

                gebruikerAntwoordId = value;
            }
        }

        public TestResultaat TestResultaat
        {
            get { return testResultaat; }
            set
            {
                if (value == null)
                {
                    throw new MeerkeuzeException("TestResultaat mag niet null zijn.");
                }

                testResultaat = value;
            }
        }

        public TestVraag TestVraag
        {
            get { return testVraag; }
            set
            {
                if (value == null)
                {
                    throw new MeerkeuzeException("TestVraag mag niet null zijn.");
                }

                testVraag = value;
            }
        }

        public char GekozenLetter
        {
            get { return gekozenLetter; }
            set
            {
                char hoofdletter = char.ToUpper(value);

                if (hoofdletter < 'A' || hoofdletter > 'Z')
                {
                    throw new MeerkeuzeException("Gekozen letter moet tussen A en Z liggen.");
                }

                gekozenLetter = hoofdletter;
            }
        }

        public bool IsCorrect
        {
            get { return isCorrect; }
            private set { isCorrect = value; }
        }

        // Nieuw antwoord vóór opslaan in database
        public GebruikerAntwoord(TestResultaat testResultaat, TestVraag testVraag, char gekozenLetter)
        {
            TestResultaat = testResultaat;
            TestVraag = testVraag;
            GekozenLetter = gekozenLetter;
            IsCorrect = testVraag.GeefAntwoordVoorLetter(GekozenLetter).IsCorrect;
        }

        // Voor lezen uit database
        public GebruikerAntwoord(int gebruikerAntwoordId, TestResultaat testResultaat, TestVraag testVraag, char gekozenLetter, bool isCorrect)
        {
            GebruikerAntwoordId = gebruikerAntwoordId;
            TestResultaat = testResultaat;
            TestVraag = testVraag;
            GekozenLetter = gekozenLetter;
            IsCorrect = isCorrect;
        }
    }
}
