using BL.Domein;
using BL.Exceptions;
using BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class VraagManager
    {
        private readonly IVraagRepository vraagRepository;
        private readonly IOnderwerpRepository onderwerpRepository;

        public VraagManager(IVraagRepository vraagRepository, IOnderwerpRepository onderwerpRepository)
        {
            this.vraagRepository = vraagRepository;

            this.onderwerpRepository = onderwerpRepository;
        }

        public void VoegVraagToe(Vraag vraag)
        {
            if (vraag == null)
            {
                throw new MeerkeuzeException("Vraag mag niet null zijn.");
            }

            vraag.ControleerOfVraagGeldigIs();

            if (vraag.Onderwerp.OnderwerpId <= 0)
            {
                throw new MeerkeuzeException("OnderwerpId is ongeldig.");
            }

            if (!onderwerpRepository.BestaatOnderwerpMetNaam(vraag.Onderwerp.Naam))
            {
                throw new MeerkeuzeException("Onderwerp bestaat niet.");
            }

            if (vraagRepository.BestaatVraagMetTekst(vraag.Tekst, vraag.Onderwerp.OnderwerpId, null))
            {
                throw new MeerkeuzeException("Er bestaat al een vraag met dezelfde tekst binnen dit onderwerp.");
            }

            vraagRepository.VoegVraagToe(vraag);
        }

        public void UpdateVraag(Vraag vraag)
        {
            if (vraag == null)
            {
                throw new MeerkeuzeException("Vraag mag niet null zijn.");
            }

            if (vraag.VraagId <= 0)
            {
                throw new MeerkeuzeException("VraagId is ongeldig.");
            }

            vraag.ControleerOfVraagGeldigIs();

            if (vraagRepository.BestaatVraagMetTekst(vraag.Tekst, vraag.Onderwerp.OnderwerpId, vraag.VraagId))
            {
                throw new MeerkeuzeException("Er bestaat al een andere vraag met dezelfde tekst binnen dit onderwerp.");
            }

            vraagRepository.UpdateVraag(vraag);
        }

        public Vraag GeefVraagById(int vraagId)
        {
            if (vraagId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig vraagId.");
            }

            Vraag? vraag = vraagRepository.GeefVraagById(vraagId);

            if (vraag == null)
            {
                throw new MeerkeuzeException("Vraag niet gevonden.");
            }

            return vraag;
        }

        public IReadOnlyList<Vraag> GeefVragenByOnderwerp(int onderwerpId)
        {
            if (onderwerpId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig onderwerpId.");
            }

            return vraagRepository.GeefVragenByOnderwerp(onderwerpId);
        }

        public IReadOnlyList<Vraag> GeefBeschikbareVragenByOnderwerp(int onderwerpId)
        {
            if (onderwerpId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig onderwerpId.");
            }

            return vraagRepository.GeefBeschikbareVragenByOnderwerp(onderwerpId);
        }

        public IReadOnlyList<Vraag> GeefBeschikbareVragenByOnderwerpEnAantalAntwoorden(int onderwerpId, int aantalAntwoorden)
        {
            if (onderwerpId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig onderwerpId.");
            }

            if (aantalAntwoorden < 2)
            {
                throw new MeerkeuzeException("Aantal antwoorden moet minstens 2 zijn.");
            }

            return vraagRepository.GeefBeschikbareVragenByOnderwerpEnAantalAntwoorden(onderwerpId, aantalAntwoorden);
        }

        public void ZetVraagOnbeschikbaar(int vraagId)
        {
            Vraag vraag = GeefVraagById(vraagId);

            vraag.IsBeschikbaar = false;

            vraagRepository.UpdateVraag(vraag);
        }
    }
}
