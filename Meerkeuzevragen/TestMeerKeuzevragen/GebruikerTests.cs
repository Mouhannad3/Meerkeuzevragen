using BL.Domein;
using BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMeerKeuzevragen
{
    public class GebruikerTests
    {
        [Fact]
        public void Test_ctor_valid()
        {
            // Arrange + Act
            Gebruiker gebruiker = new Gebruiker("Mouhannad");

            // Assert
            Assert.Equal("Mouhannad", gebruiker.Naam);
        }

        [Theory]
        [InlineData(" Mouhannad ", "Mouhannad")]
        [InlineData("Student 105", "Student 105")]
        public void Test_ctor_valid_naam_wordt_getrimd(string naam, string verwachteNaam)
        {
            // Arrange + Act
            Gebruiker gebruiker = new Gebruiker(naam);

            // Assert
            Assert.Equal(verwachteNaam, gebruiker.Naam);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_ctor_invalid_naam(string naam)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Gebruiker(naam));
        }

        [Fact]
        public void Test_ctor_met_id_valid()
        {
            // Arrange + Act
            Gebruiker gebruiker = new Gebruiker(1, "Mouhannad");

            // Assert
            Assert.Equal(1, gebruiker.GebruikerId);
            Assert.Equal("Mouhannad", gebruiker.Naam);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_met_id_invalid_id(int gebruikerId)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Gebruiker(gebruikerId, "Mouhannad"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_Naam_invalid_state_blijft_ongewijzigd(string naam)
        {
            // Arrange
            Gebruiker gebruiker = new Gebruiker("Mouhannad");

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => gebruiker.Naam = naam);

            // Assert
            Assert.Equal("Mouhannad", gebruiker.Naam);
        }
    }
}
