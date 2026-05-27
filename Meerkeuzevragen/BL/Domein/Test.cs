using BL.Exceptions;

namespace BL.Domein
{
    public class Test
    {
        private int testId;
        private string naam = "";
        private DateTime aangemaaktOp;
        private int aantalAntwoordenPerVraag;
        private Onderwerp onderwerp = null!;

        private readonly List<TestVraag> testVragen = new();

        public int TestId
        {
            get { return testId; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("TestId moet groter zijn dan 0.");
                }

                testId = value;
            }
        }

        public string Naam
        {
            get { return naam; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new MeerkeuzeException("Naam van test mag niet leeg zijn.");
                }

                naam = value.Trim();
            }
        }

        public DateTime AangemaaktOp
        {
            get { return aangemaaktOp; }
            private set
            {
                if (value == default)
                {
                    throw new MeerkeuzeException("AangemaaktOp moet een geldige datum zijn.");
                }

                aangemaaktOp = value;
            }
        }

        public int AantalAntwoordenPerVraag
        {
            get { return aantalAntwoordenPerVraag; }
            private set
            {
                if (value < 2)
                {
                    throw new MeerkeuzeException("Aantal antwoorden per vraag moet minstens 2 zijn.");
                }

                aantalAntwoordenPerVraag = value;
            }
        }

        public Onderwerp Onderwerp
        {
            get { return onderwerp; }
            set
            {
                if (value == null)
                {
                    throw new MeerkeuzeException("Onderwerp mag niet null zijn.");
                }

                onderwerp = value;
            }
        }

        public IReadOnlyList<TestVraag> TestVragen
        {
            get { return testVragen.AsReadOnly(); }
        }

        // Voor een nieuwe test vóór opslaan in de database
        public Test(string naam, Onderwerp onderwerp, int aantalAntwoordenPerVraag)
        {
            Naam = naam;
            Onderwerp = onderwerp;
            AangemaaktOp = DateTime.Now;
            AantalAntwoordenPerVraag = aantalAntwoordenPerVraag;
        }

        // Voor het lezen van de database
        public Test(int testId, string naam, DateTime aangemaaktOp, Onderwerp onderwerp, int aantalAntwoordenPerVraag)
        {
            TestId = testId;
            Naam = naam;
            AangemaaktOp = aangemaaktOp;
            Onderwerp = onderwerp;
            AantalAntwoordenPerVraag = aantalAntwoordenPerVraag;
        }

        public void VoegTestVraagToe(TestVraag testVraag)
        {
            if (testVraag == null)
            {
                throw new MeerkeuzeException("TestVraag mag niet null zijn.");
            }

            if (testVragen.Any(tv => tv.Vraag.VraagId > 0 && tv.Vraag.VraagId == testVraag.Vraag.VraagId))
            {
                throw new MeerkeuzeException("Deze vraag bestaat al in deze test.");
            }

            if (testVraag.Vraag.Antwoorden.Count != AantalAntwoordenPerVraag)
            {
                throw new MeerkeuzeException("Elke vraag in een test moet hetzelfde aantal antwoorden hebben.");
            }

            testVragen.Add(testVraag);
        }
        public override string ToString()
        {
            return Naam;
        }
    }
}
