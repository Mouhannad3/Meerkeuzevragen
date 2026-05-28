using BL.Domein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface IResultaatRepository
    {
        void VoegTestResultaatToe(TestResultaat testResultaat);

        TestResultaat? GeefTestResultaatById(int testResultaatId);

        List<TestResultaat> GeefResultatenByTest(int testId);

        List<TestResultaat> GeefResultatenByGebruiker(int gebruikerId);
    }
}
