using BL.Domein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface IGebruikerRepository
    {
        void VoegGebruikerToe(Gebruiker gebruiker);

        Gebruiker? GeefGebruikerById(int gebruikerId);

        bool BestaatGebruiker(int gebruikerId);
    }
}
