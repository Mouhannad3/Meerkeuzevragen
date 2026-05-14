using BL.Exceptions;

namespace BL.Domein
{
    public class Vraag
    {
        private int vraagId;
        private string tekst = string.Empty;
        private bool isBeschikbaar;
        private Onderwerp onderwerp = null!;

        private readonly List<Antwoord> antwoorden = new();

        public int VraagId
        {
            get { return vraagId; }
            private set
            {
                if (value <= 0)
                {
                    throw new MeerkeuzeException("VraagId moet groter zijn dan 0.");
                }

                vraagId = value;
            }
        }

        public string Tekst
        {
            get { return tekst; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new MeerkeuzeException("Tekst van vraag mag niet leeg zijn.");
                }

                tekst = value.Trim();
            }
        }

        public bool IsBeschikbaar
        {
            get { return isBeschikbaar; }
            set { isBeschikbaar = value; }
        }

        public Onderwerp Onderwerp
        {
            get { return onderwerp; }
            set
            {
                if (value == null)
                {
                    throw new MeerkeuzeException("Onderwerp mag niet null zijn.");
                }

                onderwerp = value;
            }
        }

        public IReadOnlyList<Antwoord> Antwoorden
        {
            get { return antwoorden.AsReadOnly(); }
        }

        // Voor een nieuwe vraag vóór opslaan in de database
        public Vraag(string tekst, Onderwerp onderwerp)
        {
            Tekst = tekst;
            Onderwerp = onderwerp;
            IsBeschikbaar = true;
        }

        // Voor het lezen van de database
        public Vraag(int vraagId, string tekst, bool isBeschikbaar, Onderwerp onderwerp)
        {
            VraagId = vraagId;
            Tekst = tekst;
            IsBeschikbaar = isBeschikbaar;
            Onderwerp = onderwerp;
        }

        public void VoegAntwoordToe(Antwoord antwoord)
        {
            if (antwoord == null)
            {
                throw new MeerkeuzeException("Antwoord mag niet null zijn.");
            }

            if (antwoorden.Any(a => a.Tekst.Equals(antwoord.Tekst, StringComparison.OrdinalIgnoreCase)))
            {
                throw new MeerkeuzeException("Dit antwoord bestaat al bij deze vraag.");
            }

            if (antwoord.IsCorrect && antwoorden.Any(a => a.IsCorrect))
            {
                throw new MeerkeuzeException("Een vraag mag maar één juist antwoord hebben.");
            }

            antwoorden.Add(antwoord);
        }

        public void ControleerOfVraagGeldigIs()
        {
            if (antwoorden.Count < 2)
            {
                throw new MeerkeuzeException("Een vraag moet minstens twee antwoorden hebben.");
            }

            if (antwoorden.Count(a => a.IsCorrect) != 1)
            {
                throw new MeerkeuzeException("Een vraag moet exact één juist antwoord hebben.");
            }
        }
    }
}