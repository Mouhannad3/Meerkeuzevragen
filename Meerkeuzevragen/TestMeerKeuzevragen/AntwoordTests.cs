using BL.Domein;
using BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMeerKeuzevragen
{
    public class AntwoordTests
    {
        [Fact]
        public void Test_ctor_valid()
        {
            // Arrange + Act
            Antwoord antwoord = new Antwoord("SELECT", true);

            // Assert
            Assert.Equal("SELECT", antwoord.Tekst);
            Assert.True(antwoord.IsCorrect);
        }

        [Theory]
        [InlineData(" SELECT ", "SELECT")]
        [InlineData("WHERE", "WHERE")]
        public void Test_ctor_valid_tekst_wordt_getrimd(string tekst, string verwachteTekst)
        {
            // Arrange + Act
            Antwoord antwoord = new Antwoord(tekst, false);

            // Assert
            Assert.Equal(verwachteTekst, antwoord.Tekst);
            Assert.False(antwoord.IsCorrect);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_ctor_invalid_tekst(string tekst)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Antwoord(tekst, true));
        }

        [Fact]
        public void Test_ctor_met_id_valid()
        {
            // Arrange + Act
            Antwoord antwoord = new Antwoord(1, "SELECT", true);

            // Assert
            Assert.Equal(1, antwoord.AntwoordId);
            Assert.Equal("SELECT", antwoord.Tekst);
            Assert.True(antwoord.IsCorrect);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_met_id_invalid_id(int antwoordId)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Antwoord(antwoordId, "SELECT", true));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_Tekst_invalid_state_blijft_ongewijzigd(string tekst)
        {
            // Arrange
            Antwoord antwoord = new Antwoord("SELECT", true);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => antwoord.Tekst = tekst);

            // Assert
            Assert.Equal("SELECT", antwoord.Tekst);
            Assert.True(antwoord.IsCorrect);
        }

        [Fact]
        public void Test_IsCorrect_valid()
        {
            // Arrange
            Antwoord antwoord = new Antwoord("SELECT", false);

            // Act
            antwoord.IsCorrect = true;

            // Assert
            Assert.True(antwoord.IsCorrect);
        }
    }
}
