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
        private readonly IResultaatRepository resultaatRepository;
        private readonly ITestRepository testRepository;
        private readonly IGebruikerRepository gebruikerRepository;

        public ResultaatManager(
            IResultaatRepository resultaatRepository,
            ITestRepository testRepository,
            IGebruikerRepository gebruikerRepository)
        {
            this.resultaatRepository = resultaatRepository
                ?? throw new ArgumentNullException(nameof(resultaatRepository));

            this.testRepository = testRepository
                ?? throw new ArgumentNullException(nameof(testRepository));

            this.gebruikerRepository = gebruikerRepository
                ?? throw new ArgumentNullException(nameof(gebruikerRepository));
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

            Gebruiker? gebruiker = gebruikerRepository.GeefGebruikerById(gebruikerId);

            if (gebruiker == null)
            {
                throw new MeerkeuzeException("Gebruiker niet gevonden.");
            }

            string opgeschoondeAntwoorden = antwoorden.Trim().ToUpper();

            if (opgeschoondeAntwoorden.Length != test.TestVragen.Count)
            {
                throw new MeerkeuzeException("Aantal antwoorden komt niet overeen met het aantal vragen in de test.");
            }

            TestResultaat testResultaat = new TestResultaat(test, gebruiker, test.TestVragen.Count);

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
    }
}
