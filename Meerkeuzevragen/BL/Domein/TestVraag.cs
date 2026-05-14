using BL.Exceptions;

namespace BL.Domein
{
    public class TestVraag
    {
        private int testVraagId;
        private Test test = null!;
        private Vraag vraag = null!;
        private int volgorde;

        private readonly List<TestVraagAntwoord> testVraagAntwoorden = new();

        public int TestVraagId
        {
            get { return testVraagId; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("TestVraagId moet groter zijn dan 0.");
                }

                testVraagId = value;
            }
        }

        public Test Test
        {
            get { return test; }
            set
            {
                if (value == null)
                {
                    throw new MeerkeuzeException("Test mag niet null zijn.");
                }

                test = value;
            }
        }

        public Vraag Vraag
        {
            get { return vraag; }
            set
            {
                if (value == null)
                {
                    throw new MeerkeuzeException("Vraag mag niet null zijn.");
                }

                vraag = value;
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

        public IReadOnlyList<TestVraagAntwoord> TestVraagAntwoorden
        {
            get { return testVraagAntwoorden.AsReadOnly(); }
        }

        public TestVraag(Test test, Vraag vraag, int volgorde)
        {
            Test = test;
            Vraag = vraag;
            Volgorde = volgorde;
        }

        public TestVraag(int testVraagId, Test test, Vraag vraag, int volgorde)
        {
            TestVraagId = testVraagId;
            Test = test;
            Vraag = vraag;
            Volgorde = volgorde;
        }

        public void VoegTestVraagAntwoordToe(TestVraagAntwoord testVraagAntwoord)
        {
            if (testVraagAntwoord == null)
            {
                throw new MeerkeuzeException("TestVraagAntwoord mag niet null zijn.");
            }

            if (testVraagAntwoorden.Any(tva => tva.Letter == testVraagAntwoord.Letter))
            {
                throw new MeerkeuzeException("Deze letter bestaat al bij deze testvraag.");
            }

            if (testVraagAntwoorden.Any(tva => tva.Antwoord.AntwoordId > 0 &&
                                               tva.Antwoord.AntwoordId == testVraagAntwoord.Antwoord.AntwoordId))
            {
                throw new MeerkeuzeException("Dit antwoord bestaat al bij deze testvraag.");
            }

            testVraagAntwoorden.Add(testVraagAntwoord);
        }

        public Antwoord GeefAntwoordVoorLetter(char letter)
        {
            TestVraagAntwoord? testVraagAntwoord = testVraagAntwoorden
                .FirstOrDefault(tva => tva.Letter == char.ToUpper(letter));

            if (testVraagAntwoord == null)
            {
                throw new MeerkeuzeException("Er bestaat geen antwoord voor deze letter.");
            }

            return testVraagAntwoord.Antwoord;
        }
    }
}
