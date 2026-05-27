using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using System.IO;
using System.Windows;

namespace UI
{
    public partial class TestExporterenWindow : Window
    {
        private readonly TestManager testManager;

        public TestExporterenWindow(TestManager testManager)
        {
            InitializeComponent();

            this.testManager = testManager;

            LaadTesten();
        }

        private void LaadTesten()
        {
            ComboBoxTesten.ItemsSource = testManager.GeefTesten();
        }

        private void ButtonExporteren_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Test? test = ComboBoxTesten.SelectedItem as Test;

                if (test == null)
                {
                    MessageBox.Show("Kies eerst een test.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TextBoxPad.Text))
                {
                    MessageBox.Show("Geef eerst het pad van het bestand in.");
                    return;
                }

                string tekst = testManager.MaakExportTekst(test.TestId);

                using StreamWriter sw = new StreamWriter(TextBoxPad.Text);
                sw.Write(tekst);

                MessageBox.Show("Test werd geëxporteerd.");

                DialogResult = true;
                Close();
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij exporteren: " + ex.Message);
            }
        }

        private void ButtonAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
