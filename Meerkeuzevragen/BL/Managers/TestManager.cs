using BL.Domein;
using BL.Exceptions;
using BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class TestManager
    {
        private readonly ITestRepository testRepository;
        private readonly IVraagRepository vraagRepository;
        private readonly IOnderwerpRepository onderwerpRepository;

        public TestManager(
            ITestRepository testRepository,
            IVraagRepository vraagRepository,
            IOnderwerpRepository onderwerpRepository)
        {
            this.testRepository = testRepository;

            this.vraagRepository = vraagRepository;

            this.onderwerpRepository = onderwerpRepository;
        }

        public Test StelTestSamen(string naam, int onderwerpId, int aantalVragen)
        {
            if (string.IsNullOrWhiteSpace(naam))
            {
                throw new MeerkeuzeException("Naam van test mag niet leeg zijn.");
            }

            if (onderwerpId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig onderwerpId.");
            }

            if (aantalVragen <= 0)
            {
                throw new MeerkeuzeException("Aantal vragen moet groter zijn dan 0.");
            }

            Onderwerp? onderwerp = onderwerpRepository.GeefOnderwerpById(onderwerpId);

            if (onderwerp == null)
            {
                throw new MeerkeuzeException("Onderwerp niet gevonden.");
            }

            if (testRepository.BestaatTestMetNaam(naam.Trim()))
            {
                throw new MeerkeuzeException("Er bestaat al een test met deze naam.");
            }

            IReadOnlyList<Vraag> beschikbareVragen =
                vraagRepository.GeefBeschikbareVragenByOnderwerp(onderwerpId);

            if (beschikbareVragen.Count < aantalVragen)
            {
                throw new MeerkeuzeException("Er zijn niet genoeg beschikbare vragen voor dit onderwerp.");
            }

            List<Vraag> gekozenGroep = KiesVragenMetZelfdeAantalAntwoorden(beschikbareVragen, aantalVragen);

            int aantalAntwoordenPerVraag = gekozenGroep[0].Antwoorden.Count;

            Test test = new Test(naam.Trim(), onderwerp, aantalAntwoordenPerVraag);

            Random random = new Random();

            for (int i = 0; i < gekozenGroep.Count; i++)
            {
                Vraag vraag = gekozenGroep[i];

                TestVraag testVraag = new TestVraag(test, vraag, i + 1);

                List<Antwoord> gemengdeAntwoorden = vraag.Antwoorden
                    .OrderBy(a => random.Next())
                    .ToList();

                for (int j = 0; j < gemengdeAntwoorden.Count; j++)
                {
                    char letter = (char)('A' + j);

                    TestVraagAntwoord testVraagAntwoord =
                        new TestVraagAntwoord(testVraag, gemengdeAntwoorden[j], letter, j + 1);

                    testVraag.VoegTestVraagAntwoordToe(testVraagAntwoord);
                }

                test.VoegTestVraagToe(testVraag);
            }

            testRepository.VoegTestToe(test);

            return test;
        }

        public Test GeefTestById(int testId)
        {
            if (testId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig testId.");
            }

            Test? test = testRepository.GeefTestById(testId);

            if (test == null)
            {
                throw new MeerkeuzeException("Test niet gevonden.");
            }

            return test;
        }

        public IReadOnlyList<Test> GeefTesten()
        {
            return testRepository.GeefTesten();
        }

        private List<Vraag> KiesVragenMetZelfdeAantalAntwoorden(IReadOnlyList<Vraag> vragen, int aantalVragen)
        {
            Random random = new Random();

            foreach (Vraag vraag in vragen)
            {
                int aantalAntwoorden = vraag.Antwoorden.Count;

                List<Vraag> vragenMetZelfdeAantalAntwoorden = new();

                foreach (Vraag andereVraag in vragen)
                {
                    if (andereVraag.Antwoorden.Count == aantalAntwoorden)
                    {
                        vragenMetZelfdeAantalAntwoorden.Add(andereVraag);
                    }
                }

                if (vragenMetZelfdeAantalAntwoorden.Count >= aantalVragen)
                {
                    return vragenMetZelfdeAantalAntwoorden
                        .OrderBy(v => random.Next())
                        .Take(aantalVragen)
                        .ToList();
                }
            }

            throw new MeerkeuzeException("Er zijn niet genoeg vragen met hetzelfde aantal antwoorden.");
        }
        public string MaakExportTekst(int testId)
        {
            Test test = GeefTestById(testId);

            string tekst = $"Test: {test.Naam}\n";
            tekst += $"Onderwerp: {test.Onderwerp.Naam}\n\n";

            foreach (TestVraag testVraag in test.TestVragen.OrderBy(tv => tv.Volgorde))
            {
                tekst += $"{testVraag.Volgorde}. {testVraag.Vraag.Tekst}\n\n";

                foreach (TestVraagAntwoord antwoord in testVraag.TestVraagAntwoorden.OrderBy(a => a.Volgorde))
                {
                    tekst += $"{antwoord.Letter}. {antwoord.Antwoord.Tekst}\n";
                }

                tekst += "\n";
            }

            return tekst;
        }
    }
}
