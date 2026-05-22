using BL.Domein;
using BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMeerKeuzevragen
{
    public class TestResultaatTests
    {

        [Fact]
        public void Test_ctor_valid()
        {
            // Arrange
            Test test = MaakTest();
            Gebruiker gebruiker = MaakGebruiker();

            // Act
            TestResultaat testResultaat = new TestResultaat(test, gebruiker, 2);

            // Assert
            Assert.Equal(test, testResultaat.Test);
            Assert.Equal(gebruiker, testResultaat.Gebruiker);
            Assert.Equal(0, testResultaat.Score);
            Assert.Equal(2, testResultaat.TotaalAantalVragen);
            Assert.True(testResultaat.UitgevoerdOp != default);
            Assert.Empty(testResultaat.GebruikerAntwoorden);
        }

        [Fact]
        public void Test_ctor_invalid_test_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestResultaat(null, MaakGebruiker(), 2));
        }

        [Fact]
        public void Test_ctor_invalid_gebruiker_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestResultaat(MaakTest(), null, 2));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_invalid_totaalAantalVragen(int totaalAantalVragen)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestResultaat(MaakTest(), MaakGebruiker(), totaalAantalVragen));
        }

        [Fact]
        public void Test_ctor_met_id_valid()
        {
            // Arrange
            Test test = MaakTest();
            Gebruiker gebruiker = MaakGebruiker();
            DateTime datum = DateTime.Now;

            // Act
            TestResultaat testResultaat =
                new TestResultaat(1, test, gebruiker, 1, 2, datum);

            // Assert
            Assert.Equal(1, testResultaat.TestResultaatId);
            Assert.Equal(test, testResultaat.Test);
            Assert.Equal(gebruiker, testResultaat.Gebruiker);
            Assert.Equal(1, testResultaat.Score);
            Assert.Equal(2, testResultaat.TotaalAantalVragen);
            Assert.Equal(datum, testResultaat.UitgevoerdOp);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_met_id_invalid_id(int testResultaatId)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestResultaat(
                    testResultaatId,
                    MaakTest(),
                    MaakGebruiker(),
                    1,
                    2,
                    DateTime.Now));
        }

        [Fact]
        public void Test_ctor_met_id_invalid_score()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestResultaat(
                    1,
                    MaakTest(),
                    MaakGebruiker(),
                    -1,
                    2,
                    DateTime.Now));
        }

        [Fact]
        public void Test_ctor_met_id_invalid_totaalAantalVragen()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestResultaat(
                    1,
                    MaakTest(),
                    MaakGebruiker(),
                    0,
                    0,
                    DateTime.Now));
        }

        [Fact]
        public void Test_ctor_met_id_invalid_datum()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestResultaat(
                    1,
                    MaakTest(),
                    MaakGebruiker(),
                    1,
                    2,
                    default));
        }

        [Fact]
        public void Test_VoegGebruikerAntwoordToe_valid_correct_verhoogt_score()
        {
            // Arrange
            Test test = MaakTest();
            TestResultaat testResultaat = new TestResultaat(test, MaakGebruiker(), 1);
            TestVraag testVraag = MaakTestVraag(test, 1, 1, 1);

            GebruikerAntwoord gebruikerAntwoord =
                new GebruikerAntwoord(testResultaat, testVraag, 'B');

            // Act
            testResultaat.VoegGebruikerAntwoordToe(gebruikerAntwoord);

            // Assert
            Assert.Single(testResultaat.GebruikerAntwoorden);
            Assert.Equal(1, testResultaat.Score);
            Assert.Contains(gebruikerAntwoord, testResultaat.GebruikerAntwoorden);
        }

        [Fact]
        public void Test_VoegGebruikerAntwoordToe_valid_fout_verhoogt_score_niet()
        {
            // Arrange
            Test test = MaakTest();
            TestResultaat testResultaat = new TestResultaat(test, MaakGebruiker(), 1);
            TestVraag testVraag = MaakTestVraag(test, 1, 1, 1);

            GebruikerAntwoord gebruikerAntwoord =
                new GebruikerAntwoord(testResultaat, testVraag, 'A');

            // Act
            testResultaat.VoegGebruikerAntwoordToe(gebruikerAntwoord);

            // Assert
            Assert.Single(testResultaat.GebruikerAntwoorden);
            Assert.Equal(0, testResultaat.Score);
            Assert.Contains(gebruikerAntwoord, testResultaat.GebruikerAntwoorden);
        }

        [Fact]
        public void Test_VoegGebruikerAntwoordToe_invalid_null_state_blijft_ongewijzigd()
        {
            // Arrange
            TestResultaat testResultaat =
                new TestResultaat(MaakTest(), MaakGebruiker(), 1);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                testResultaat.VoegGebruikerAntwoordToe(null));

            // Assert
            Assert.Empty(testResultaat.GebruikerAntwoorden);
            Assert.Equal(0, testResultaat.Score);
        }

        [Fact]
        public void Test_VoegGebruikerAntwoordToe_invalid_dubbele_testvraag_state_blijft_ongewijzigd()
        {
            // Arrange
            Test test = MaakTest();
            TestResultaat testResultaat = new TestResultaat(test, MaakGebruiker(), 2);
            TestVraag testVraag = MaakTestVraag(test, 1, 1, 1);

            GebruikerAntwoord eersteAntwoord =
                new GebruikerAntwoord(testResultaat, testVraag, 'B');

            GebruikerAntwoord tweedeAntwoordZelfdeVraag =
                new GebruikerAntwoord(testResultaat, testVraag, 'A');

            testResultaat.VoegGebruikerAntwoordToe(eersteAntwoord);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                testResultaat.VoegGebruikerAntwoordToe(tweedeAntwoordZelfdeVraag));

            // Assert
            Assert.Single(testResultaat.GebruikerAntwoorden);
            Assert.Equal(1, testResultaat.Score);
            Assert.Contains(eersteAntwoord, testResultaat.GebruikerAntwoorden);
        }

        [Fact]
        public void Test_LaadGebruikerAntwoordToe_valid_verandert_score_niet()
        {
            // Arrange
            Test test = MaakTest();
            TestResultaat testResultaat =
                new TestResultaat(1, test, MaakGebruiker(), 1, 1, DateTime.Now);

            TestVraag testVraag = MaakTestVraag(test, 1, 1, 1);

            GebruikerAntwoord gebruikerAntwoord =
                new GebruikerAntwoord(1, testResultaat, testVraag, 'B', true);

            // Act
            testResultaat.LaadGebruikerAntwoordToe(gebruikerAntwoord);

            // Assert
            Assert.Single(testResultaat.GebruikerAntwoorden);
            Assert.Equal(1, testResultaat.Score);
            Assert.Contains(gebruikerAntwoord, testResultaat.GebruikerAntwoorden);
        }

        private Onderwerp MaakOnderwerp()
        {
            return new Onderwerp(1, "SQL");
        }

        private Vraag MaakVraag(int vraagId, string tekst)
        {
            Vraag vraag = new Vraag(vraagId, tekst, true, MaakOnderwerp());

            vraag.VoegAntwoordToe(new Antwoord(vraagId * 10 + 1, "Juist antwoord", true));
            vraag.VoegAntwoordToe(new Antwoord(vraagId * 10 + 2, "Fout antwoord", false));

            return vraag;
        }

        private Test MaakTest()
        {
            return new Test(1, "SQL Test", DateTime.Now, MaakOnderwerp(), 2);
        }

        private TestVraag MaakTestVraag(Test test, int testVraagId, int vraagId, int volgorde)
        {
            Vraag vraag = MaakVraag(vraagId, "Vraag " + vraagId);

            TestVraag testVraag = new TestVraag(testVraagId, test, vraag, volgorde);

            testVraag.VoegTestVraagAntwoordToe(
                new TestVraagAntwoord(testVraag, vraag.Antwoorden[1], 'A', 1));

            testVraag.VoegTestVraagAntwoordToe(
                new TestVraagAntwoord(testVraag, vraag.Antwoorden[0], 'B', 2));

            return testVraag;
        }

        private Gebruiker MaakGebruiker()
        {
            return new Gebruiker(1, "Student 1");
        }
    }
}
