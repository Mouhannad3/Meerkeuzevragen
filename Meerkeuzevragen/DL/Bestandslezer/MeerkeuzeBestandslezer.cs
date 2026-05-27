using BL.Domein;
using BL.Exceptions;
using BL.Interfaces;

namespace DL.Bestandslezer
{
    public class MeerkeuzeBestandslezer : IMeerkeuzeBestandslezer
    {
        public List<Vraag> LeesVragen(string pad, Onderwerp onderwerp)
        {
            if (string.IsNullOrWhiteSpace(pad))
            {
                throw new MeerkeuzeException("Pad mag niet leeg zijn.");
            }

            if (onderwerp == null)
            {
                throw new MeerkeuzeException("Onderwerp mag niet null zijn.");
            }

            List<string> lijnen = LeesLijnen(pad);

            bool heeftCorrectPerVraag = HeeftCorrectPerVraag(lijnen);
            bool heeftAntwoordenOpEinde = HeeftAntwoordenOpEinde(lijnen);

            if (heeftCorrectPerVraag)
            {
                return LeesMetCorrectPerVraag(lijnen, onderwerp);
            }
            else if (heeftAntwoordenOpEinde)
            {
                return LeesMetAntwoordenOpEinde(lijnen, onderwerp);
            }
            else
            {
                throw new MeerkeuzeException("Onbekend bestandsformaat.");
            }
        }

        private List<string> LeesLijnen(string pad)
        {
            List<string> lijnen = new();

            try
            {
                using StreamReader sr = new StreamReader(pad);

                string? lijn;
                while ((lijn = sr.ReadLine()) != null)
                {
                    lijnen.Add(lijn);
                }
            }
            catch (Exception ex)
            {
                throw new MeerkeuzeException("Fout bij lezen van bestand.", ex);
            }

            return lijnen;
        }

        private bool HeeftCorrectPerVraag(List<string> lijnen)
        {
            foreach (string lijn in lijnen)
            {
                if (lijn.Trim().ToLower().StartsWith("correct:"))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HeeftAntwoordenOpEinde(List<string> lijnen)
        {
            foreach (string lijn in lijnen)
            {
                if (lijn.Trim().ToLower() == "antwoorden")
                {
                    return true;
                }
            }

            return false;
        }

        private List<Vraag> LeesMetCorrectPerVraag(List<string> lijnen, Onderwerp onderwerp)
        {
            List<Vraag> vragen = new();
            int index = 0;

            while (index < lijnen.Count)
            {
                string lijn = lijnen[index].Trim();

                if (IsVraagStart(lijn))
                {
                    string vraagTekst = HaalVraagTekstUitVraagLijn(lijn);
                    index++;

                    while (index < lijnen.Count &&
                           !IsAntwoordLijn(lijnen[index]) &&
                           !lijnen[index].Trim().ToLower().StartsWith("correct:"))
                    {
                        if (!string.IsNullOrWhiteSpace(lijnen[index]))
                        {
                            vraagTekst = VoegRegelToe(vraagTekst, lijnen[index].Trim());
                        }

                        index++;
                    }

                    Dictionary<char, string> antwoorden = new();

                    while (index < lijnen.Count && IsAntwoordLijn(lijnen[index]))
                    {
                        char letter = char.ToUpper(lijnen[index].Trim()[0]);
                        string antwoordTekst = HaalAntwoordTekstUitAntwoordLijn(lijnen[index]);

                        antwoorden.Add(letter, antwoordTekst);
                        index++;
                    }

                    while (index < lijnen.Count && string.IsNullOrWhiteSpace(lijnen[index]))
                    {
                        index++;
                    }

                    if (index >= lijnen.Count || !lijnen[index].Trim().ToLower().StartsWith("correct:"))
                    {
                        throw new MeerkeuzeException("Correct antwoord ontbreekt bij een vraag.");
                    }

                    string correctTekst = HaalCorrectTekstUitCorrectLijn(lijnen[index]);
                    char correcteLetter = HaalEersteLetter(correctTekst);
                    index++;

                    Vraag vraag = MaakVraag(vraagTekst, antwoorden, correcteLetter, onderwerp);
                    vragen.Add(vraag);
                }
                else
                {
                    index++;
                }
            }

            return vragen;
        }

        private List<Vraag> LeesMetAntwoordenOpEinde(List<string> lijnen, Onderwerp onderwerp)
        {
            int antwoordenIndex = ZoekAntwoordenIndex(lijnen);

            if (antwoordenIndex < 0)
            {
                throw new MeerkeuzeException("Antwoordenlijst ontbreekt.");
            }

            List<string> vraagLijnen = new();

            for (int i = 0; i < antwoordenIndex; i++)
            {
                vraagLijnen.Add(lijnen[i]);
            }

            List<char> correcteLetters = new();

            for (int i = antwoordenIndex + 1; i < lijnen.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(lijnen[i]))
                {
                    correcteLetters.Add(HaalEersteLetter(lijnen[i].Trim()));
                }
            }

            List<Vraag> vragen = new();

            int index = 0;
            int vraagNummer = 0;

            while (index < vraagLijnen.Count)
            {
                string lijn = vraagLijnen[index].Trim();

                if (IsVraagStart(lijn))
                {
                    if (vraagNummer >= correcteLetters.Count)
                    {
                        throw new MeerkeuzeException("Er zijn minder antwoorden dan vragen.");
                    }

                    string vraagTekst = HaalVraagTekstUitVraagLijn(lijn);
                    index++;

                    while (index < vraagLijnen.Count && !IsAntwoordLijn(vraagLijnen[index]))
                    {
                        if (!string.IsNullOrWhiteSpace(vraagLijnen[index]))
                        {
                            vraagTekst = VoegRegelToe(vraagTekst, vraagLijnen[index].Trim());
                        }

                        index++;
                    }

                    Dictionary<char, string> antwoorden = new();

                    while (index < vraagLijnen.Count && IsAntwoordLijn(vraagLijnen[index]))
                    {
                        char letter = char.ToUpper(vraagLijnen[index].Trim()[0]);
                        string antwoordTekst = HaalAntwoordTekstUitAntwoordLijn(vraagLijnen[index]);

                        antwoorden.Add(letter, antwoordTekst);
                        index++;
                    }

                    Vraag vraag = MaakVraag(vraagTekst, antwoorden, correcteLetters[vraagNummer], onderwerp);
                    vragen.Add(vraag);

                    vraagNummer++;
                }
                else
                {
                    index++;
                }
            }

            if (vragen.Count != correcteLetters.Count)
            {
                throw new MeerkeuzeException("Aantal antwoorden in de antwoordenlijst komt niet overeen met het aantal vragen.");
            }

            return vragen;
        }

        private int ZoekAntwoordenIndex(List<string> lijnen)
        {
            for (int i = 0; i < lijnen.Count; i++)
            {
                if (lijnen[i].Trim().ToLower() == "antwoorden")
                {
                    return i;
                }
            }

            return -1;
        }

        private Vraag MaakVraag(string vraagTekst, Dictionary<char, string> antwoorden, char correcteLetter, Onderwerp onderwerp)
        {
            if (string.IsNullOrWhiteSpace(vraagTekst))
            {
                throw new MeerkeuzeException("Vraagtekst mag niet leeg zijn.");
            }

            if (antwoorden.Count == 0)
            {
                throw new MeerkeuzeException("Vraag zonder antwoorden gevonden.");
            }

            if (!antwoorden.ContainsKey(correcteLetter))
            {
                throw new MeerkeuzeException("Correcte letter komt niet voor bij de antwoorden.");
            }

            Vraag vraag = new Vraag(vraagTekst, onderwerp);

            foreach (KeyValuePair<char, string> antwoord in antwoorden)
            {
                Antwoord nieuwAntwoord = new Antwoord(
                    antwoord.Value,
                    antwoord.Key == correcteLetter
                );

                vraag.VoegAntwoordToe(nieuwAntwoord);
            }

            return vraag;
        }

        private bool IsVraagStart(string lijn)
        {
            if (string.IsNullOrWhiteSpace(lijn))
            {
                return false;
            }

            int puntIndex = lijn.IndexOf('.');

            if (puntIndex <= 0)
            {
                return false;
            }

            string nummer = lijn.Substring(0, puntIndex);

            return int.TryParse(nummer, out int resultaat);
        }

        private bool IsAntwoordLijn(string lijn)
        {
            if (string.IsNullOrWhiteSpace(lijn))
            {
                return false;
            }

            string trimmed = lijn.Trim();

            if (trimmed.Length < 3)
            {
                return false;
            }

            return char.IsLetter(trimmed[0]) &&
                   (trimmed[1] == '.' || trimmed[1] == ')');
        }

        private string HaalVraagTekstUitVraagLijn(string lijn)
        {
            int puntIndex = lijn.IndexOf('.');
            return lijn.Substring(puntIndex + 1).Trim();
        }

        private string HaalAntwoordTekstUitAntwoordLijn(string lijn)
        {
            string trimmed = lijn.Trim();
            return trimmed.Substring(2).Trim();
        }

        private string HaalCorrectTekstUitCorrectLijn(string lijn)
        {
            string correctLijn = lijn.Trim();
            int dubbelePuntIndex = correctLijn.IndexOf(':');

            if (dubbelePuntIndex < 0)
            {
                throw new MeerkeuzeException("Correct antwoord heeft geen dubbele punt.");
            }

            return correctLijn.Substring(dubbelePuntIndex + 1).Trim();
        }

        private char HaalEersteLetter(string tekst)
        {
            if (string.IsNullOrWhiteSpace(tekst))
            {
                throw new MeerkeuzeException("Antwoordletter ontbreekt.");
            }

            char letter = char.ToUpper(tekst.Trim()[0]);

            if (letter < 'A' || letter > 'Z')
            {
                throw new MeerkeuzeException("Ongeldige antwoordletter.");
            }

            return letter;
        }

        private string VoegRegelToe(string bestaandeTekst, string nieuweRegel)
        {
            if (string.IsNullOrWhiteSpace(bestaandeTekst))
            {
                return nieuweRegel;
            }

            return bestaandeTekst + "\n" + nieuweRegel;
        }
    }
}