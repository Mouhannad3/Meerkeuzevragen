using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using Microsoft.Win32;
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
                    MessageBox.Show("Kies eerst een test.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Tekstbestand (*.txt)|*.txt";
                dialog.FileName = test.Naam + ".txt";

                if (dialog.ShowDialog() == true)
                {
                    string tekst = testManager.MaakExportTekst(test.TestId);

                    using StreamWriter sw = new StreamWriter(dialog.FileName);
                    sw.Write(tekst);

                    MessageBox.Show("Test werd geëxporteerd.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Onverwachte fout: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
