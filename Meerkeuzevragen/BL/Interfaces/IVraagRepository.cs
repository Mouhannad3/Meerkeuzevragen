using BL.Domein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface IVraagRepository
    {
        void VoegVraagToe(Vraag vraag);

        void UpdateVraag(Vraag vraag);

        Vraag? GeefVraagById(int vraagId);

        List<Vraag> GeefVragenByOnderwerp(int onderwerpId);

        List<Vraag> GeefBeschikbareVragenByOnderwerp(int onderwerpId);

        List<Vraag> GeefBeschikbareVragenByOnderwerpEnAantalAntwoorden(int onderwerpId, int aantalAntwoorden);

        bool BestaatVraagMetTekst(string tekst, int onderwerpId, int? vraagId);
    }
}
