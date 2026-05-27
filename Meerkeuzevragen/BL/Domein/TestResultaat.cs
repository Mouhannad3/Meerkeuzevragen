using BL.Exceptions;

namespace BL.Domein
{
    public class TestResultaat
    {
        private int testResultaatId;
        private Test test = null!;
        private int gebruikerId;
        private int score;
        private int totaalAantalVragen;
        private DateTime uitgevoerdOp;

        private readonly List<GebruikerAntwoord> gebruikerAntwoorden = new();

        public int TestResultaatId
        {
            get { return testResultaatId; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("TestResultaatId moet groter zijn dan 0.");
                }

                testResultaatId = value;
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

        public int Score
        {
            get { return score; }
            private set
            {
                if (value < 0)
                {
                    throw new MeerkeuzeException("Score mag niet negatief zijn.");
                }

                score = value;
            }
        }

        public int TotaalAantalVragen
        {
            get { return totaalAantalVragen; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("Totaal aantal vragen moet groter zijn dan 0.");
                }

                totaalAantalVragen = value;
            }
        }

        public DateTime UitgevoerdOp
        {
            get { return uitgevoerdOp; }
            private set
            {
                if (value == default)
                {
                    throw new MeerkeuzeException("UitgevoerdOp moet een geldige datum zijn.");
                }

                uitgevoerdOp = value;
            }
        }

        public IReadOnlyList<GebruikerAntwoord> GebruikerAntwoorden
        {
            get { return gebruikerAntwoorden.AsReadOnly(); }
        }

        // Nieuwe uitvoering vóór opslaan in database
        public TestResultaat(Test test, int gebruikerId, int totaalAantalVragen)
        {
            Test = test;
            GebruikerId = gebruikerId;
            TotaalAantalVragen = totaalAantalVragen;
            Score = 0;
            UitgevoerdOp = DateTime.Now;
        }

        // Voor lezen uit database
        public TestResultaat(int testResultaatId, Test test, int gebruikerId, int score, int totaalAantalVragen, DateTime uitgevoerdOp)
        {
            TestResultaatId = testResultaatId;
            Test = test;
            GebruikerId = gebruikerId;
            TotaalAantalVragen = totaalAantalVragen;
            Score = score;
            UitgevoerdOp = uitgevoerdOp;
        }

        public void VoegGebruikerAntwoordToe(GebruikerAntwoord gebruikerAntwoord)
        {
            if (gebruikerAntwoord == null)
            {
                throw new MeerkeuzeException("GebruikerAntwoord mag niet null zijn.");
            }

            if (gebruikerAntwoorden.Any(ga => ga.TestVraag.TestVraagId > 0 &&
                                               ga.TestVraag.TestVraagId == gebruikerAntwoord.TestVraag.TestVraagId))
            {
                throw new MeerkeuzeException("Er bestaat al een antwoord voor deze testvraag.");
            }

            gebruikerAntwoorden.Add(gebruikerAntwoord);

            if (gebruikerAntwoord.IsCorrect)
            {
                Score++;
            }
        }
        public void LaadGebruikerAntwoordToe(GebruikerAntwoord gebruikerAntwoord)
        {
            if (gebruikerAntwoord == null)
            {
                throw new MeerkeuzeException("GebruikerAntwoord mag niet null zijn.");
            }

            if (gebruikerAntwoorden.Any(ga => ga.TestVraag.TestVraagId > 0 &&
                                               ga.TestVraag.TestVraagId == gebruikerAntwoord.TestVraag.TestVraagId))
            {
                throw new MeerkeuzeException("Er bestaat al een antwoord voor deze testvraag.");
            }

            gebruikerAntwoorden.Add(gebruikerAntwoord);
        }
    }
}
