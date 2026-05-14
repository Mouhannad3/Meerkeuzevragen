using BL.Domein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
     public interface IOnderwerpRepository
    {
        void VoegOnderwerpToe(Onderwerp onderwerp);

        Onderwerp? GeefOnderwerpById(int onderwerpId);

        Onderwerp? GeefOnderwerpByNaam(string naam);

        IReadOnlyList<Onderwerp> GeefOnderwerpen();

        bool BestaatOnderwerpMetNaam(string naam);
    }
}
