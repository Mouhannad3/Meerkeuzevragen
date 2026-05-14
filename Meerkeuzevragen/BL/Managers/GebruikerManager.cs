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
    public class GebruikerManager
    {
        private readonly IGebruikerRepository gebruikerRepository;

        public GebruikerManager(IGebruikerRepository gebruikerRepository)
        {
            this.gebruikerRepository = gebruikerRepository
                ?? throw new ArgumentNullException(nameof(gebruikerRepository));
        }

        public void VoegGebruikerToe(Gebruiker gebruiker)
        {
            if (gebruiker == null)
            {
                throw new MeerkeuzeException("Gebruiker mag niet null zijn.");
            }

            gebruikerRepository.VoegGebruikerToe(gebruiker);
        }

        public Gebruiker GeefGebruikerById(int gebruikerId)
        {
            if (gebruikerId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig gebruikerId.");
            }

            Gebruiker? gebruiker = gebruikerRepository.GeefGebruikerById(gebruikerId);

            if (gebruiker == null)
            {
                throw new MeerkeuzeException("Gebruiker niet gevonden.");
            }

            return gebruiker;
        }

        public bool BestaatGebruiker(int gebruikerId)
        {
            if (gebruikerId <= 0)
            {
                throw new MeerkeuzeException("Ongeldig gebruikerId.");
            }

            return gebruikerRepository.BestaatGebruiker(gebruikerId);
        }
    }
}
