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

            IResultaatRepository resultaatRepository =
                RepositoryFactory.GeefResultaatRepository(databaseType, connectionString);

            IMeerkeuzeBestandslezer bestandslezer =
                BestandslezerFactory.GeefMeerkeuzeBestandslezer(fileType);

            IBulkResultaatBestandslezer bulkBestandslezer =
                BestandslezerFactory.GeefBulkResultaatBestandslezer(fileType);

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

            resultaatManager = new ResultaatManager(
                resultaatRepository,
                testRepository,
                bulkBestandslezer
            );

            importManager = new ImportManager(
                bestandslezer,
                vraagRepository,
                onderwerpRepository
            );
        }

        private void ButtonOnderwerpToevoegen_Click(object sender, RoutedEventArgs e)
        {
            OnderwerpToevoegenWindow window = new OnderwerpToevoegenWindow(onderwerpManager);

            if (window.ShowDialog() == true)
            {
                MessageBox.Show(
                    "Onderwerp werd toegevoegd.",
                    "Info",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void ButtonVragenImporteren_Click(object sender, RoutedEventArgs e)
        {
            VragenImporterenWindow window = new VragenImporterenWindow(
                onderwerpManager,
                importManager
            );

            window.ShowDialog();
        }

        private void ButtonVragenBeheren_Click(object sender, RoutedEventArgs e)
        {
            VragenBeheren window = new VragenBeheren(
                onderwerpManager,
                vraagManager
            );

            window.ShowDialog();
        }

        private void ButtonTestSamenstellen_Click(object sender, RoutedEventArgs e)
        {
            TestSamenstellenWindow window = new TestSamenstellenWindow(
                onderwerpManager,
                testManager
            );

            window.ShowDialog();
        }

        private void ButtonTestExporteren_Click(object sender, RoutedEventArgs e)
        {
            TestExporterenWindow window = new TestExporterenWindow(testManager);

            window.ShowDialog();
        }

        private void ButtonTestUitvoeren_Click(object sender, RoutedEventArgs e)
        {
            TestUitvoerenWindow window = new TestUitvoerenWindow(
                testManager,
                resultaatManager
            );

            window.ShowDialog();
        }

        private void ButtonBulkResultaten_Click(object sender, RoutedEventArgs e)
        {
            BulkResultatenWindow window = new BulkResultatenWindow(resultaatManager);

            window.ShowDialog();
        }
    }
}