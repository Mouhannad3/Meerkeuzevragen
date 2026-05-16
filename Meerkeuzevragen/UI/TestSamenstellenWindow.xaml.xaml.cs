using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using System.Windows;

namespace UI
{
    public partial class TestSamenstellenWindow : Window
    {
        private readonly OnderwerpManager onderwerpManager;
        private readonly TestManager testManager;

        public TestSamenstellenWindow(OnderwerpManager onderwerpManager, TestManager testManager)
        {
            InitializeComponent();

            this.onderwerpManager = onderwerpManager;
            this.testManager = testManager;

            LaadOnderwerpen();
        }

        private void LaadOnderwerpen()
        {
            ComboBoxOnderwerpen.ItemsSource = onderwerpManager.GeefOnderwerpen();
        }

        private void ButtonSamenstellen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string naam = TextBoxNaam.Text;

                Onderwerp? onderwerp = ComboBoxOnderwerpen.SelectedItem as Onderwerp;

                if (onderwerp == null)
                {
                    MessageBox.Show("Kies eerst een onderwerp.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(TextBoxAantalVragen.Text, out int aantalVragen))
                {
                    MessageBox.Show("Aantal vragen moet een getal zijn.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Test test = testManager.StelTestSamen(naam, onderwerp.OnderwerpId, aantalVragen);

                MessageBox.Show("Test werd samengesteld.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
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
