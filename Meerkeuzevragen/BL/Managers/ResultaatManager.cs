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
    public class ResultaatManager
    {
        private readonly IBulkResultaatBestandslezer bulkResultaatBestandslezer;
        private readonly IResultaatRepository resultaatRepository;
        private readonly ITestRepository testRepository;
        public ResultaatManager(
            IResultaatRepository resultaatRepository,
            ITestRepository testRepository,
            IBulkResultaatBestandslezer bulkResultaatBestandslezer)
        { 
            this.resultaatRepository = resultaatRepository
                ?? throw new ArgumentNullException(nameof(resultaatRepository));

            this.testRepository = testRepository
                ?? throw new ArgumentNullException(nameof(testRepository));

            this.bulkResultaatBestandslezer = bulkResultaatBestandslezer
                ?? throw new ArgumentNullException(nameof(bulkResultaatBestandslezer));
        }

        public TestResultaat VerbeterTest(int testId, int gebruikerId, string antwoorden)
        {
            if (testId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig testId.");
            }

            if (gebruikerId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig gebruikerId.");
            }

            if (string.IsNullOrWhiteSpace(antwoorden))
            {
                throw new MeerkeuzeException("Antwoorden mogen niet leeg zijn.");
            }

            Test? test = testRepository.GeefTestById(testId);

            if (test == null)
            {
                throw new MeerkeuzeException("Test niet gevonden.");
            }

            string opgeschoondeAntwoorden = antwoorden.Trim().ToUpper();

            if (opgeschoondeAntwoorden.Length != test.TestVragen.Count)
            {
                throw new MeerkeuzeException("Aantal antwoorden komt niet overeen met het aantal vragen in de test.");
            }

            TestResultaat testResultaat = new TestResultaat(test, gebruikerId, test.TestVragen.Count);

            List<TestVraag> testVragen = test.TestVragen
                .OrderBy(tv => tv.Volgorde)
                .ToList();

            for (int i = 0; i < testVragen.Count; i++)
            {
                char gekozenLetter = opgeschoondeAntwoorden[i];

                GebruikerAntwoord gebruikerAntwoord =
                    new GebruikerAntwoord(testResultaat, testVragen[i], gekozenLetter);

                testResultaat.VoegGebruikerAntwoordToe(gebruikerAntwoord);
            }

            resultaatRepository.VoegTestResultaatToe(testResultaat);

            return testResultaat;
        }

        public TestResultaat GeefTestResultaatById(int testResultaatId)
        {
            if (testResultaatId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig testResultaatId.");
            }

            TestResultaat? testResultaat =
                resultaatRepository.GeefTestResultaatById(testResultaatId);

            if (testResultaat == null)
            {
                throw new MeerkeuzeException("TestResultaat niet gevonden.");
            }

            return testResultaat;
        }

        public IReadOnlyList<TestResultaat> GeefResultatenByTest(int testId)
        {
            if (testId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig testId.");
            }

            return resultaatRepository.GeefResultatenByTest(testId);
        }

        public IReadOnlyList<TestResultaat> GeefResultatenByGebruiker(int gebruikerId)
        {
            if (gebruikerId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig gebruikerId.");
            }

            return resultaatRepository.GeefResultatenByGebruiker(gebruikerId);
        }
        public List<TestResultaat> VerwerkBulkResultaten(string pad)
        {
            if (string.IsNullOrWhiteSpace(pad))
            {
                throw new MeerkeuzeException("Pad mag niet leeg zijn.");
            }

            List<(int TestId, int GebruikerId, string Antwoorden)> lijnen =
                bulkResultaatBestandslezer.LeesBulkResultaten(pad);

            if (lijnen.Count == 0)
            {
                throw new MeerkeuzeException("Bulkbestand bevat geen resultaten.");
            }

            List<TestResultaat> resultaten = new();

            foreach (var lijn in lijnen)
            {
                TestResultaat resultaat = VerbeterTest(
                    lijn.TestId,
                    lijn.GebruikerId,
                    lijn.Antwoorden
                );

                resultaten.Add(resultaat);
            }

            return resultaten;
        }
    }
}
