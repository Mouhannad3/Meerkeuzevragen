using BL.Exceptions;

namespace BL.Domein
{
    public class TestVraagAntwoord
    {
        private int testVraagAntwoordId;
        private TestVraag testVraag = null!;
        private Antwoord antwoord = null!;
        private char letter;
        private int volgorde;

        public int TestVraagAntwoordId
        {
            get { return testVraagAntwoordId; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("TestVraagAntwoordId moet groter zijn dan 0.");
                }

                testVraagAntwoordId = value;
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

        public Antwoord Antwoord
        {
            get { return antwoord; }
            set
            {
                if (value == null)
                {
                    throw new MeerkeuzeException("Antwoord mag niet null zijn.");
                }

                antwoord = value;
            }
        }

        public char Letter
        {
            get { return letter; }
            set
            {
                char hoofdletter = char.ToUpper(value);

                if (hoofdletter < 'A' || hoofdletter > 'Z')
                {
                    throw new MeerkeuzeException("Letter moet tussen A en Z liggen.");
                }

                letter = hoofdletter;
            }
        }

        public int Volgorde
        {
            get { return volgorde; }
            set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("Volgorde moet groter zijn dan 0.");
                }

                volgorde = value;
            }
        }

        // Voor een nieuw testvraagantwoord vóór opslaan in de database
        public TestVraagAntwoord(TestVraag testVraag, Antwoord antwoord, char letter, int volgorde)
        {
            TestVraag = testVraag;
            Antwoord = antwoord;
            Letter = letter;
            Volgorde = volgorde;
        }

        // Voor het lezen van de database
        public TestVraagAntwoord(int testVraagAntwoordId, TestVraag testVraag, Antwoord antwoord, char letter, int volgorde)
        {
            TestVraagAntwoordId = testVraagAntwoordId;
            TestVraag = testVraag;
            Antwoord = antwoord;
            Letter = letter;
            Volgorde = volgorde;
        }
    }
}