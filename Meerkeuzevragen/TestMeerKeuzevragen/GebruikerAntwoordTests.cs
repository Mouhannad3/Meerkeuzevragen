using BL.Domein;
using BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMeerKeuzevragen
{
    public class GebruikerAntwoordTests
    {

        [Fact]
        public void Test_ctor_valid_correct_antwoord()
        {
            // Arrange
            TestResultaat testResultaat = MaakTestResultaat();
            TestVraag testVraag = MaakTestVraag();

            // Act
            GebruikerAntwoord gebruikerAntwoord =
                new GebruikerAntwoord(testResultaat, testVraag, 'D');

            // Assert
            Assert.Equal(testResultaat, gebruikerAntwoord.TestResultaat);
            Assert.Equal(testVraag, gebruikerAntwoord.TestVraag);
            Assert.Equal('D', gebruikerAntwoord.GekozenLetter);
            Assert.True(gebruikerAntwoord.IsCorrect);
        }

        [Fact]
        public void Test_ctor_valid_fout_antwoord()
        {
            // Arrange
            TestResultaat testResultaat = MaakTestResultaat();
            TestVraag testVraag = MaakTestVraag();

            // Act
            GebruikerAntwoord gebruikerAntwoord =
                new GebruikerAntwoord(testResultaat, testVraag, 'A');

            // Assert
            Assert.Equal('A', gebruikerAntwoord.GekozenLetter);
            Assert.False(gebruikerAntwoord.IsCorrect);
        }

        [Fact]
        public void Test_ctor_valid_letter_wordt_hoofdletter()
        {
            // Arrange
            TestResultaat testResultaat = MaakTestResultaat();
            TestVraag testVraag = MaakTestVraag();

            // Act
            GebruikerAntwoord gebruikerAntwoord =
                new GebruikerAntwoord(testResultaat, testVraag, 'd');

            // Assert
            Assert.Equal('D', gebruikerAntwoord.GekozenLetter);
            Assert.True(gebruikerAntwoord.IsCorrect);
        }

        [Fact]
        public void Test_ctor_invalid_testResultaat_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new GebruikerAntwoord(null, MaakTestVraag(), 'A'));
        }

        [Fact]
        public void Test_ctor_invalid_testVraag_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new GebruikerAntwoord(MaakTestResultaat(), null, 'A'));
        }

        [Theory]
        [InlineData('1')]
        [InlineData('@')]
        [InlineData('[')]
        public void Test_ctor_invalid_letter_formaat(char letter)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new GebruikerAntwoord(MaakTestResultaat(), MaakTestVraag(), letter));
        }

        [Fact]
        public void Test_ctor_invalid_letter_bestaat_niet()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new GebruikerAntwoord(MaakTestResultaat(), MaakTestVraag(), 'B'));
        }

        [Fact]
        public void Test_ctor_met_id_valid()
        {
            // Arrange
            TestResultaat testResultaat = MaakTestResultaat();
            TestVraag testVraag = MaakTestVraag();

            // Act
            GebruikerAntwoord gebruikerAntwoord =
                new GebruikerAntwoord(1, testResultaat, testVraag, 'D', true);

            // Assert
            Assert.Equal(1, gebruikerAntwoord.GebruikerAntwoordId);
            Assert.Equal(testResultaat, gebruikerAntwoord.TestResultaat);
            Assert.Equal(testVraag, gebruikerAntwoord.TestVraag);
            Assert.Equal('D', gebruikerAntwoord.GekozenLetter);
            Assert.True(gebruikerAntwoord.IsCorrect);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_met_id_invalid_id(int gebruikerAntwoordId)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new GebruikerAntwoord(
                    gebruikerAntwoordId,
                    MaakTestResultaat(),
                    MaakTestVraag(),
                    'D',
                    true));
        }

        private Onderwerp MaakOnderwerp()
        {
            return new Onderwerp(1, "SQL");
        }

        private Vraag MaakVraag()
        {
            Vraag vraag = new Vraag(1, "Waar staat SQL voor?", true, MaakOnderwerp());

            vraag.VoegAntwoordToe(new Antwoord(1, "Structured Query Language", true));
            vraag.VoegAntwoordToe(new Antwoord(2, "Simple Query Language", false));

            return vraag;
        }

        private Test MaakTest()
        {
            return new Test(1, "SQL Test", DateTime.Now, MaakOnderwerp(), 2);
        }

        private TestVraag MaakTestVraag()
        {
            TestVraag testVraag = new TestVraag(1, MaakTest(), MaakVraag(), 1);

            Antwoord juistAntwoord = testVraag.Vraag.Antwoorden[0];
            Antwoord foutAntwoord = testVraag.Vraag.Antwoorden[1];

            testVraag.VoegTestVraagAntwoordToe(
                new TestVraagAntwoord(testVraag, foutAntwoord, 'A', 1));

            testVraag.VoegTestVraagAntwoordToe(
                new TestVraagAntwoord(testVraag, juistAntwoord, 'D', 2));

            return testVraag;
        }

        private TestResultaat MaakTestResultaat()
        {
            return new TestResultaat(
                MaakTest(),
                new Gebruiker(1, "Student 1"),
                1);
        }
    }
}
