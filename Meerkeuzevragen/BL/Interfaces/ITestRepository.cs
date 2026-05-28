using BL.Domein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface ITestRepository
    {
        void VoegTestToe(Test test);

        Test? GeefTestById(int testId);

        List<Test> GeefTesten();

        List<Test> GeefTestenByOnderwerp(int onderwerpId);

        bool BestaatTestMetNaam(string naam);
    }
}
