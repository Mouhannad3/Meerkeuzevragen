using BL.Domein;
using BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMeerKeuzevragen
{
    public class TestVraagTests
    {


        [Fact]
        public void Test_ctor_valid()
        {
            // Arrange
            Test test = MaakTest();
            Vraag vraag = MaakVraag();

            // Act
            TestVraag testVraag = new TestVraag(test, vraag, 1);

            // Assert
            Assert.Equal(test, testVraag.Test);
            Assert.Equal(vraag, testVraag.Vraag);
            Assert.Equal(1, testVraag.Volgorde);
            Assert.Empty(testVraag.TestVraagAntwoorden);
        }

        [Fact]
        public void Test_ctor_met_id_valid()
        {
            // Arrange
            Test test = MaakTest();
            Vraag vraag = MaakVraag();

            // Act
            TestVraag testVraag = new TestVraag(1, test, vraag, 1);

            // Assert
            Assert.Equal(1, testVraag.TestVraagId);
            Assert.Equal(test, testVraag.Test);
            Assert.Equal(vraag, testVraag.Vraag);
            Assert.Equal(1, testVraag.Volgorde);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_met_id_invalid_id(int testVraagId)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestVraag(testVraagId, MaakTest(), MaakVraag(), 1));
        }

        [Fact]
        public void Test_ctor_invalid_test_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestVraag(null, MaakVraag(), 1));
        }

        [Fact]
        public void Test_ctor_invalid_vraag_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestVraag(MaakTest(), null, 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_invalid_volgorde(int volgorde)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestVraag(MaakTest(), MaakVraag(), volgorde));
        }

        [Fact]
        public void Test_VoegTestVraagAntwoordToe_valid()
        {
            // Arrange
            TestVraag testVraag = MaakTestVraag();
            Antwoord antwoord = testVraag.Vraag.Antwoorden[0];

            TestVraagAntwoord testVraagAntwoord =
                new TestVraagAntwoord(testVraag, antwoord, 'A', 1);

            // Act
            testVraag.VoegTestVraagAntwoordToe(testVraagAntwoord);

            // Assert
            Assert.Single(testVraag.TestVraagAntwoorden);
            Assert.Contains(testVraagAntwoord, testVraag.TestVraagAntwoorden);
        }

        [Fact]
        public void Test_VoegTestVraagAntwoordToe_invalid_null_state_blijft_ongewijzigd()
        {
            // Arrange
            TestVraag testVraag = MaakTestVraag();

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                testVraag.VoegTestVraagAntwoordToe(null));

            // Assert
            Assert.Empty(testVraag.TestVraagAntwoorden);
        }

        [Fact]
        public void Test_VoegTestVraagAntwoordToe_invalid_dubbele_letter_state_blijft_ongewijzigd()
        {
            // Arrange
            TestVraag testVraag = MaakTestVraag();

            TestVraagAntwoord antwoordA =
                new TestVraagAntwoord(testVraag, testVraag.Vraag.Antwoorden[0], 'A', 1);

            TestVraagAntwoord antwoordMetZelfdeLetter =
                new TestVraagAntwoord(testVraag, testVraag.Vraag.Antwoorden[1], 'A', 2);

            testVraag.VoegTestVraagAntwoordToe(antwoordA);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                testVraag.VoegTestVraagAntwoordToe(antwoordMetZelfdeLetter));

            // Assert
            Assert.Single(testVraag.TestVraagAntwoorden);
            Assert.Contains(antwoordA, testVraag.TestVraagAntwoorden);
        }

        [Fact]
        public void Test_VoegTestVraagAntwoordToe_invalid_dubbel_antwoord_state_blijft_ongewijzigd()
        {
            // Arrange
            TestVraag testVraag = MaakTestVraag();

            Antwoord antwoord = testVraag.Vraag.Antwoorden[0];

            TestVraagAntwoord antwoordA =
                new TestVraagAntwoord(testVraag, antwoord, 'A', 1);

            TestVraagAntwoord zelfdeAntwoordAndereLetter =
                new TestVraagAntwoord(testVraag, antwoord, 'B', 2);

            testVraag.VoegTestVraagAntwoordToe(antwoordA);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                testVraag.VoegTestVraagAntwoordToe(zelfdeAntwoordAndereLetter));

            // Assert
            Assert.Single(testVraag.TestVraagAntwoorden);
            Assert.Contains(antwoordA, testVraag.TestVraagAntwoorden);
        }

        [Fact]
        public void Test_GeefAntwoordVoorLetter_valid()
        {
            // Arrange
            TestVraag testVraag = MaakTestVraag();

            Antwoord correctAntwoord = testVraag.Vraag.Antwoorden[0];
            Antwoord foutAntwoord = testVraag.Vraag.Antwoorden[1];

            testVraag.VoegTestVraagAntwoordToe(
                new TestVraagAntwoord(testVraag, correctAntwoord, 'D', 1));

            testVraag.VoegTestVraagAntwoordToe(
                new TestVraagAntwoord(testVraag, foutAntwoord, 'A', 2));

            // Act
            Antwoord antwoordVoorD = testVraag.GeefAntwoordVoorLetter('D');
            Antwoord antwoordVoorA = testVraag.GeefAntwoordVoorLetter('a');

            // Assert
            Assert.Equal(correctAntwoord, antwoordVoorD);
            Assert.Equal(foutAntwoord, antwoordVoorA);
        }

        [Fact]
        public void Test_GeefAntwoordVoorLetter_invalid()
        {
            // Arrange
            TestVraag testVraag = MaakTestVraag();

            testVraag.VoegTestVraagAntwoordToe(
                new TestVraagAntwoord(testVraag, testVraag.Vraag.Antwoorden[0], 'A', 1));

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                testVraag.GeefAntwoordVoorLetter('B'));
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
            return new TestVraag(1, MaakTest(), MaakVraag(), 1);
        }
    }
}
