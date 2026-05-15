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

        IReadOnlyList<Vraag> GeefVragenByOnderwerp(int onderwerpId);

        IReadOnlyList<Vraag> GeefBeschikbareVragenByOnderwerp(int onderwerpId);

        IReadOnlyList<Vraag> GeefBeschikbareVragenByOnderwerpEnAantalAntwoorden(int onderwerpId, int aantalAntwoorden);

        bool BestaatVraagMetTekst(string tekst, int onderwerpId, int? vraagId);
    }
}
