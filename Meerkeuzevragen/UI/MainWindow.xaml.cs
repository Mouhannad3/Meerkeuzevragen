using BL.Interfaces;
using BL.Managers;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;
using Utils;

namespace UI
{
    public partial class MainWindow : Window
    {
        private string connectionString = "";
        private string databaseType = "";
        private string fileType = "";

        private OnderwerpManager onderwerpManager;
        private VraagManager vraagManager;
        private TestManager testManager;
        private GebruikerManager gebruikerManager;
        private ResultaatManager resultaatManager;
        private ImportManager importManager;

        public MainWindow()
        {
            InitializeComponent();

            LeesConfig();
            MaakManagers();
        }

        private void LeesConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            connectionString = config.GetConnectionString("SQLServerConnection")!;
            databaseType = config.GetSection("AppSettings")["databaseType"]!;
            fileType = config.GetSection("AppSettings")["fileType"]!;
        }

        private void MaakManagers()
        {
            IOnderwerpRepository onderwerpRepository =
                RepositoryFactory.GeefOnderwerpRepository(databaseType, connectionString);

            IVraagRepository vraagRepository =
                RepositoryFactory.GeefVraagRepository(databaseType, connectionString);

            ITestRepository testRepository =
                RepositoryFactory.GeefTestRepository(databaseType, connectionString);

            IGebruikerRepository gebruikerRepository =
                RepositoryFactory.GeefGebruikerRepository(databaseType, connectionString);

            IResultaatRepository resultaatRepository =
                RepositoryFactory.GeefResultaatRepository(databaseType, connectionString);

            IMeerkeuzeBestandslezer bestandslezer =
                BestandslezerFactory.GeefMeerkeuzeBestandslezer(fileType);

            onderwerpManager = new OnderwerpManager(onderwerpRepository);

            vraagManager = new VraagManager(
                vraagRepository,
                onderwerpRepository
            );

            testManager = new TestManager(
                testRepository,
                vraagRepository,
                onderwerpRepository
            );

            gebruikerManager = new GebruikerManager(
                gebruikerRepository
            );

            resultaatManager = new ResultaatManager(
                resultaatRepository,
                testRepository,
                gebruikerRepository
            );

            importManager = new ImportManager(
                bestandslezer,
                vraagRepository,
                onderwerpRepository
            );
        }
    }
}