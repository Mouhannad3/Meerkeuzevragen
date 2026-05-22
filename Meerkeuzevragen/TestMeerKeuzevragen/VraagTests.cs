using BL.Domein;
using BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMeerKeuzevragen
{
    public class VraagTests
    {

        [Fact]
        public void Test_ctor_valid()
        {
            // Arrange
            Onderwerp onderwerp = MaakOnderwerp();

            // Act
            Vraag vraag = new Vraag("Waar staat SQL voor?", onderwerp);

            // Assert
            Assert.Equal("Waar staat SQL voor?", vraag.Tekst);
            Assert.Equal(onderwerp, vraag.Onderwerp);
            Assert.True(vraag.IsBeschikbaar);
            Assert.Empty(vraag.Antwoorden);
        }

        [Theory]
        [InlineData(" Waar staat SQL voor? ", "Waar staat SQL voor?")]
        [InlineData("Welke clausule gebruik je?", "Welke clausule gebruik je?")]
        public void Test_ctor_valid_tekst_wordt_getrimd(string tekst, string verwachteTekst)
        {
            // Arrange + Act
            Vraag vraag = new Vraag(tekst, MaakOnderwerp());

            // Assert
            Assert.Equal(verwachteTekst, vraag.Tekst);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_ctor_invalid_tekst(string tekst)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Vraag(tekst, MaakOnderwerp()));
        }

        [Fact]
        public void Test_ctor_invalid_onderwerp_null()
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() => new Vraag("Waar staat SQL voor?", null));
        }

        [Fact]
        public void Test_ctor_met_id_valid()
        {
            // Arrange
            Onderwerp onderwerp = MaakOnderwerp();

            // Act
            Vraag vraag = new Vraag(1, "Waar staat SQL voor?", true, onderwerp);

            // Assert
            Assert.Equal(1, vraag.VraagId);
            Assert.Equal("Waar staat SQL voor?", vraag.Tekst);
            Assert.True(vraag.IsBeschikbaar);
            Assert.Equal(onderwerp, vraag.Onderwerp);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_ctor_met_id_invalid_id(int vraagId)
        {
            // Arrange + Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                new Vraag(vraagId, "Waar staat SQL voor?", true, MaakOnderwerp()));
        }

        [Fact]
        public void Test_VoegAntwoordToe_valid()
        {
            // Arrange
            Vraag vraag = new Vraag("Waar staat SQL voor?", MaakOnderwerp());
            Antwoord antwoord = new Antwoord("Structured Query Language", true);

            // Act
            vraag.VoegAntwoordToe(antwoord);

            // Assert
            Assert.Single(vraag.Antwoorden);
            Assert.Contains(antwoord, vraag.Antwoorden);
        }

        [Fact]
        public void Test_VoegAntwoordToe_invalid_null_state_blijft_ongewijzigd()
        {
            // Arrange
            Vraag vraag = MaakGeldigeVraag();
            int aantalAntwoorden = vraag.Antwoorden.Count;

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => vraag.VoegAntwoordToe(null));

            // Assert
            Assert.Equal(aantalAntwoorden, vraag.Antwoorden.Count);
        }

        [Fact]
        public void Test_VoegAntwoordToe_invalid_dubbele_tekst_state_blijft_ongewijzigd()
        {
            // Arrange
            Vraag vraag = new Vraag("Waar staat SQL voor?", MaakOnderwerp());
            vraag.VoegAntwoordToe(new Antwoord("SELECT", true));

            int aantalAntwoorden = vraag.Antwoorden.Count;

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                vraag.VoegAntwoordToe(new Antwoord("select", false)));

            // Assert
            Assert.Equal(aantalAntwoorden, vraag.Antwoorden.Count);
            Assert.Single(vraag.Antwoorden);
        }

        [Fact]
        public void Test_VoegAntwoordToe_invalid_meer_dan_een_correct_antwoord_state_blijft_ongewijzigd()
        {
            // Arrange
            Vraag vraag = new Vraag("Welke opdracht haalt gegevens op?", MaakOnderwerp());
            vraag.VoegAntwoordToe(new Antwoord("SELECT", true));

            int aantalAntwoorden = vraag.Antwoorden.Count;

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() =>
                vraag.VoegAntwoordToe(new Antwoord("GET", true)));

            // Assert
            Assert.Equal(aantalAntwoorden, vraag.Antwoorden.Count);
            Assert.Single(vraag.Antwoorden);
        }

        [Fact]
        public void Test_ControleerOfVraagGeldigIs_valid()
        {
            // Arrange
            Vraag vraag = MaakGeldigeVraag();

            // Act
            vraag.ControleerOfVraagGeldigIs();

            // Assert
            Assert.Equal(2, vraag.Antwoorden.Count);
            Assert.Single(vraag.Antwoorden.Where(a => a.IsCorrect));
        }

        [Fact]
        public void Test_ControleerOfVraagGeldigIs_invalid_geen_antwoorden()
        {
            // Arrange
            Vraag vraag = new Vraag("Waar staat SQL voor?", MaakOnderwerp());

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => vraag.ControleerOfVraagGeldigIs());
        }

        [Fact]
        public void Test_ControleerOfVraagGeldigIs_invalid_minder_dan_twee_antwoorden()
        {
            // Arrange
            Vraag vraag = new Vraag("Waar staat SQL voor?", MaakOnderwerp());
            vraag.VoegAntwoordToe(new Antwoord("Structured Query Language", true));

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => vraag.ControleerOfVraagGeldigIs());
        }

        [Fact]
        public void Test_ControleerOfVraagGeldigIs_invalid_geen_correct_antwoord()
        {
            // Arrange
            Vraag vraag = new Vraag("Waar staat SQL voor?", MaakOnderwerp());
            vraag.VoegAntwoordToe(new Antwoord("Simple Query Language", false));
            vraag.VoegAntwoordToe(new Antwoord("System Query Logic", false));

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => vraag.ControleerOfVraagGeldigIs());
        }

        [Fact]
        public void Test_IsBeschikbaar_valid()
        {
            // Arrange
            Vraag vraag = MaakGeldigeVraag();

            // Act
            vraag.IsBeschikbaar = false;

            // Assert
            Assert.False(vraag.IsBeschikbaar);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_Tekst_invalid_state_blijft_ongewijzigd(string tekst)
        {
            // Arrange
            Vraag vraag = new Vraag("Waar staat SQL voor?", MaakOnderwerp());

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => vraag.Tekst = tekst);

            // Assert
            Assert.Equal("Waar staat SQL voor?", vraag.Tekst);
        }

        [Fact]
        public void Test_Onderwerp_invalid_null_state_blijft_ongewijzigd()
        {
            // Arrange
            Onderwerp onderwerp = MaakOnderwerp();
            Vraag vraag = new Vraag("Waar staat SQL voor?", onderwerp);

            // Act + Assert
            Assert.Throws<MeerkeuzeException>(() => vraag.Onderwerp = null);

            // Assert
            Assert.Equal(onderwerp, vraag.Onderwerp);
        }
        private Onderwerp MaakOnderwerp()
        {
            return new Onderwerp(1, "SQL");
        }

        private Vraag MaakGeldigeVraag()
        {
            Vraag vraag = new Vraag("Waar staat SQL voor?", MaakOnderwerp());

            vraag.VoegAntwoordToe(new Antwoord("Structured Query Language", true));
            vraag.VoegAntwoordToe(new Antwoord("Simple Query Language", false));

            return vraag;
        }
    }
}
