using BL.Domein;
using BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMeerKeuzevragen
{
    public class OnderwerpTests
    {
        [Fact]
        public void Test_ctor_valid()
        {
            // Arrange + Act
            Onderwerp onderwerp = new Onderwerp("SQL");

            // Assert
            Assert.Equal("SQL", onderwerp.Naam);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_ctor_invalid_naam(string naam)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Onderwerp(naam));
        }

        [Fact]
        public void Test_ctor_met_id_valid()
        {
            // Arrange + Act
            Onderwerp onderwerp = new Onderwerp(1, "SQL");

            // Assert
            Assert.Equal(1, onderwerp.OnderwerpId);
            Assert.Equal("SQL", onderwerp.Naam);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_met_id_invalid_id(int onderwerpId)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Onderwerp(onderwerpId, "SQL"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_ctor_met_id_invalid_naam(string naam)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Onderwerp(1, naam));
        }

        [Theory]
        [InlineData("SQL")]
        [InlineData("C#")]
        [InlineData("WPF")]
        public void Test_Naam_valid(string naam)
        {
            // Arrange
            Onderwerp onderwerp = new Onderwerp("SQL");

            // Act
            onderwerp.Naam = naam;

            // Assert
            Assert.Equal(naam, onderwerp.Naam);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_Naam_invalid(string naam)
        {
            // Arrange
            Onderwerp onderwerp = new Onderwerp("SQL");

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => onderwerp.Naam = naam);

            // Assert state unchanged
            Assert.Equal("SQL", onderwerp.Naam);
        }

        [Fact]
        public void Test_ToString_valid()
        {
            // Arrange
            Onderwerp onderwerp = new Onderwerp("SQL");

            // Act
            string tekst = onderwerp.ToString();

            // Assert
            Assert.Equal("SQL", tekst);
        }

    }
}
