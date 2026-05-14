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
    public class OnderwerpManager
    {
        private readonly IOnderwerpRepository onderwerpRepository;

        public OnderwerpManager(IOnderwerpRepository onderwerpRepository)
        {
            this.onderwerpRepository = onderwerpRepository
                ?? throw new ArgumentNullException(nameof(onderwerpRepository));
        }

        public void VoegOnderwerpToe(Onderwerp onderwerp)
        {
            if (onderwerp == null)
            {
                throw new MeerkeuzeException("Onderwerp mag niet null zijn.");
            }

            if (onderwerpRepository.BestaatOnderwerpMetNaam(onderwerp.Naam))
            {
                throw new MeerkeuzeException("Er bestaat al een onderwerp met deze naam.");
            }

            onderwerpRepository.VoegOnderwerpToe(onderwerp);
        }

        public Onderwerp GeefOnderwerpById(int onderwerpId)
        {
            if (onderwerpId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig onderwerpId.");
            }

            Onderwerp? onderwerp = onderwerpRepository.GeefOnderwerpById(onderwerpId);

            if (onderwerp == null)
            {
                throw new MeerkeuzeException("Onderwerp niet gevonden.");
            }

            return onderwerp;
        }

        public Onderwerp GeefOnderwerpByNaam(string naam)
        {
            if (string.IsNullOrWhiteSpace(naam))
            {
                throw new MeerkeuzeException("Naam mag niet leeg zijn.");
            }

            Onderwerp? onderwerp = onderwerpRepository.GeefOnderwerpByNaam(naam.Trim());

            if (onderwerp == null)
            {
                throw new MeerkeuzeException("Onderwerp niet gevonden.");
            }

            return onderwerp;
        }

        public IReadOnlyList<Onderwerp> GeefOnderwerpen()
        {
            return onderwerpRepository.GeefOnderwerpen();
        }
    }
}
