using BL.Domein;
using BL.Exceptions;
using BL.Interfaces;

namespace BL.Managers
{
    public class ImportManager
    {
        private readonly IMeerkeuzeBestandslezer meerkeuzeBestandslezer;
        private readonly IVraagRepository vraagRepository;
        private readonly IOnderwerpRepository onderwerpRepository;

        public ImportManager(
            IMeerkeuzeBestandslezer meerkeuzeBestandslezer,
            IVraagRepository vraagRepository,
            IOnderwerpRepository onderwerpRepository)
        {
            this.meerkeuzeBestandslezer = meerkeuzeBestandslezer
                ?? throw new ArgumentNullException(nameof(meerkeuzeBestandslezer));

            this.vraagRepository = vraagRepository
                ?? throw new ArgumentNullException(nameof(vraagRepository));

            this.onderwerpRepository = onderwerpRepository
                ?? throw new ArgumentNullException(nameof(onderwerpRepository));
        }

        public void ImporteerVragen(string pad, int onderwerpId)
        {
            if (string.IsNullOrWhiteSpace(pad))
            {
                throw new MeerkeuzeException("Pad mag niet leeg zijn.");
            }

            if (onderwerpId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig onderwerpId.");
            }

            Onderwerp? onderwerp = onderwerpRepository.GeefOnderwerpById(onderwerpId);

            if (onderwerp == null)
            {
                throw new MeerkeuzeException("Onderwerp niet gevonden.");
            }

            List<Vraag> vragen = meerkeuzeBestandslezer.LeesVragen(pad, onderwerp);

            if (vragen.Count == 0)
            {
                throw new MeerkeuzeException("Er werden geen vragen gevonden in het bestand.");
            }

            foreach (Vraag vraag in vragen)
            {
                vraag.ControleerOfVraagGeldigIs();

                if (!vraagRepository.BestaatVraagMetTekst(vraag.Tekst, onderwerp.OnderwerpId, null))
                {
                    vraagRepository.VoegVraagToe(vraag);
                }
            }
        }
    }
}