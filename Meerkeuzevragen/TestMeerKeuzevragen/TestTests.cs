using BL.Domein;
using BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMeerKeuzevragen
{
    public class TestTests
    {
        [Fact]
        public void Test_ctor_valid()
        {
            // Arrange
            Onderwerp onderwerp = MaakOnderwerp();

            // Act
            Test test = new Test("SQL Test", onderwerp, 4);

            // Assert
            Assert.Equal("SQL Test", test.Naam);
            Assert.Equal(onderwerp, test.Onderwerp);
            Assert.Equal(4, test.AantalAntwoordenPerVraag);
            Assert.Empty(test.TestVragen);
        }

        [Theory]
        [InlineData(" SQL Test ", "SQL Test")]
        [InlineData("C# Test", "C# Test")]
        public void Test_ctor_valid_naam_wordt_getrimd(string naam, string verwachteNaam)
        {
            // Arrange + Act
            Test test = new Test(naam, MaakOnderwerp(), 4);

            // Assert
            Assert.Equal(verwachteNaam, test.Naam);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_ctor_invalid_naam(string naam)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Test(naam, MaakOnderwerp(), 4));
        }

        [Fact]
        public void Test_ctor_invalid_onderwerp_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Test("SQL Test", null, 4));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        public void Test_ctor_invalid_aantalAntwoordenPerVraag(int aantalAntwoordenPerVraag)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new Test("SQL Test", MaakOnderwerp(), aantalAntwoordenPerVraag));
        }

        [Fact]
        public void Test_ctor_met_id_valid()
        {
            // Arrange
            Onderwerp onderwerp = MaakOnderwerp();
            DateTime datum = DateTime.Now;

            // Act
            Test test = new Test(1, "SQL Test", datum, onderwerp, 4);

            // Assert
            Assert.Equal(1, test.TestId);
            Assert.Equal("SQL Test", test.Naam);
            Assert.Equal(datum, test.AangemaaktOp);
            Assert.Equal(onderwerp, test.Onderwerp);
            Assert.Equal(4, test.AantalAntwoordenPerVraag);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_met_id_invalid_id(int testId)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new Test(testId, "SQL Test", DateTime.Now, MaakOnderwerp(), 4));
        }

        [Fact]
        public void Test_ctor_met_id_invalid_datum()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new Test(1, "SQL Test", default, MaakOnderwerp(), 4));
        }

        [Fact]
        public void Test_VoegTestVraagToe_valid()
        {
            // Arrange
            Test test = MaakTest();
            Vraag vraag = MaakVraag(1, "Vraag 1", 4);
            TestVraag testVraag = new TestVraag(test, vraag, 1);

            // Act
            test.VoegTestVraagToe(testVraag);

            // Assert
            Assert.Single(test.TestVragen);
            Assert.Contains(testVraag, test.TestVragen);
        }

        [Fact]
        public void Test_VoegTestVraagToe_invalid_null_state_blijft_ongewijzigd()
        {
            // Arrange
            Test test = MaakTest();

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => test.VoegTestVraagToe(null));

            // Assert
            Assert.Empty(test.TestVragen);
        }

        [Fact]
        public void Test_VoegTestVraagToe_invalid_dubbele_vraag_state_blijft_ongewijzigd()
        {
            // Arrange
            Test test = MaakTest();

            Vraag vraag = MaakVraag(1, "Vraag 1", 4);

            TestVraag testVraag1 = new TestVraag(test, vraag, 1);
            TestVraag testVraag2 = new TestVraag(test, vraag, 2);

            test.VoegTestVraagToe(testVraag1);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => test.VoegTestVraagToe(testVraag2));

            // Assert
            Assert.Single(test.TestVragen);
            Assert.Contains(testVraag1, test.TestVragen);
        }

        [Fact]
        public void Test_VoegTestVraagToe_invalid_verkeerd_aantal_antwoorden_state_blijft_ongewijzigd()
        {
            // Arrange
            Test test = MaakTest();

            Vraag vraagMetVierAntwoorden = MaakVraag(1, "Vraag 1", 4);
            Vraag vraagMetDrieAntwoorden = MaakVraag(2, "Vraag 2", 3);

            TestVraag testVraag1 = new TestVraag(test, vraagMetVierAntwoorden, 1);
            TestVraag testVraag2 = new TestVraag(test, vraagMetDrieAntwoorden, 2);

            test.VoegTestVraagToe(testVraag1);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => test.VoegTestVraagToe(testVraag2));

            // Assert
            Assert.Single(test.TestVragen);
            Assert.Contains(testVraag1, test.TestVragen);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_Naam_invalid_state_blijft_ongewijzigd(string naam)
        {
            // Arrange
            Test test = MaakTest();

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => test.Naam = naam);

            // Assert
            Assert.Equal("SQL Test", test.Naam);
        }

        [Fact]
        public void Test_Onderwerp_invalid_null_state_blijft_ongewijzigd()
        {
            // Arrange
            Onderwerp onderwerp = MaakOnderwerp();
            Test test = new Test("SQL Test", onderwerp, 4);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => test.Onderwerp = null);

            // Assert
            Assert.Equal(onderwerp, test.Onderwerp);
        }

        [Fact]
        public void Test_ToString_valid()
        {
            // Arrange
            Test test = MaakTest();

            // Act
            string tekst = test.ToString();

            // Assert
            Assert.Equal("SQL Test", tekst);
        }

        private Onderwerp MaakOnderwerp()
        {
            return new Onderwerp(1, "SQL");
        }

        private Vraag MaakVraag(int vraagId, string tekst, int aantalAntwoorden)
        {
            Vraag vraag = new Vraag(vraagId, tekst, true, MaakOnderwerp());

            vraag.VoegAntwoordToe(new Antwoord(vraagId * 10 + 1, "Juist antwoord " + vraagId, true));

            for (int i = 2; i <= aantalAntwoorden; i++)
            {
                vraag.VoegAntwoordToe(new Antwoord(vraagId * 10 + i, "Fout antwoord " + i, false));
            }

            return vraag;
        }

        private Test MaakTest()
        {
            return new Test("SQL Test", MaakOnderwerp(), 4);
        }

    }
}
