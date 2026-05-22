using BL.Domein;
using BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMeerKeuzevragen
{
    public class TestVraagAntwoordTests
    {

        [Fact]
        public void Test_ctor_valid()
        {
            // Arrange
            TestVraag testVraag = MaakTestVraag();
            Antwoord antwoord = MaakAntwoord();

            // Act
            TestVraagAntwoord testVraagAntwoord =
                new TestVraagAntwoord(testVraag, antwoord, 'A', 1);

            // Assert
            Assert.Equal(testVraag, testVraagAntwoord.TestVraag);
            Assert.Equal(antwoord, testVraagAntwoord.Antwoord);
            Assert.Equal('A', testVraagAntwoord.Letter);
            Assert.Equal(1, testVraagAntwoord.Volgorde);
        }

        [Fact]
        public void Test_ctor_valid_letter_wordt_hoofdletter()
        {
            // Arrange + Act
            TestVraagAntwoord testVraagAntwoord =
                new TestVraagAntwoord(MaakTestVraag(), MaakAntwoord(), 'b', 2);

            // Assert
            Assert.Equal('B', testVraagAntwoord.Letter);
            Assert.Equal(2, testVraagAntwoord.Volgorde);
        }

        [Fact]
        public void Test_ctor_met_id_valid()
        {
            // Arrange
            TestVraag testVraag = MaakTestVraag();
            Antwoord antwoord = MaakAntwoord();

            // Act
            TestVraagAntwoord testVraagAntwoord =
                new TestVraagAntwoord(1, testVraag, antwoord, 'A', 1);

            // Assert
            Assert.Equal(1, testVraagAntwoord.TestVraagAntwoordId);
            Assert.Equal(testVraag, testVraagAntwoord.TestVraag);
            Assert.Equal(antwoord, testVraagAntwoord.Antwoord);
            Assert.Equal('A', testVraagAntwoord.Letter);
            Assert.Equal(1, testVraagAntwoord.Volgorde);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_met_id_invalid_id(int testVraagAntwoordId)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestVraagAntwoord(
                    testVraagAntwoordId,
                    MaakTestVraag(),
                    MaakAntwoord(),
                    'A',
                    1));
        }

        [Fact]
        public void Test_ctor_invalid_testvraag_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestVraagAntwoord(null, MaakAntwoord(), 'A', 1));
        }

        [Fact]
        public void Test_ctor_invalid_antwoord_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestVraagAntwoord(MaakTestVraag(), null, 'A', 1));
        }

        [Theory]
        [InlineData('1')]
        [InlineData('@')]
        [InlineData('[')]
        public void Test_ctor_invalid_letter(char letter)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestVraagAntwoord(MaakTestVraag(), MaakAntwoord(), letter, 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_invalid_volgorde(int volgorde)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new TestVraagAntwoord(MaakTestVraag(), MaakAntwoord(), 'A', volgorde));
        }

        [Fact]
        public void Test_Letter_valid_wordt_hoofdletter()
        {
            // Arrange
            TestVraagAntwoord testVraagAntwoord =
                new TestVraagAntwoord(MaakTestVraag(), MaakAntwoord(), 'A', 1);

            // Act
            testVraagAntwoord.Letter = 'c';

            // Assert
            Assert.Equal('C', testVraagAntwoord.Letter);
        }

        [Theory]
        [InlineData('1')]
        [InlineData('@')]
        [InlineData('[')]
        public void Test_Letter_invalid_state_blijft_ongewijzigd(char letter)
        {
            // Arrange
            TestVraagAntwoord testVraagAntwoord =
                new TestVraagAntwoord(MaakTestVraag(), MaakAntwoord(), 'A', 1);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => testVraagAntwoord.Letter = letter);

            // Assert
            Assert.Equal('A', testVraagAntwoord.Letter);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        public void Test_Volgorde_valid(int volgorde)
        {
            // Arrange
            TestVraagAntwoord testVraagAntwoord =
                new TestVraagAntwoord(MaakTestVraag(), MaakAntwoord(), 'A', 1);

            // Act
            testVraagAntwoord.Volgorde = volgorde;

            // Assert
            Assert.Equal(volgorde, testVraagAntwoord.Volgorde);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_Volgorde_invalid_state_blijft_ongewijzigd(int volgorde)
        {
            // Arrange
            TestVraagAntwoord testVraagAntwoord =
                new TestVraagAntwoord(MaakTestVraag(), MaakAntwoord(), 'A', 1);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => testVraagAntwoord.Volgorde = volgorde);

            // Assert
            Assert.Equal(1, testVraagAntwoord.Volgorde);
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

        private Antwoord MaakAntwoord()
        {
            return new Antwoord(1, "Structured Query Language", true);
        }

    }
}
