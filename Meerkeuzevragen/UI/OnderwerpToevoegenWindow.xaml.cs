using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using System.Windows;

namespace UI
{
    public partial class OnderwerpToevoegenWindow : Window
    {
        private readonly OnderwerpManager onderwerpManager;

        public OnderwerpToevoegenWindow(OnderwerpManager onderwerpManager)
        {
            InitializeComponent();

            this.onderwerpManager = onderwerpManager;

        }


        private void ButtonOpslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string naam = TextBoxNaam.Text;

                Onderwerp onderwerp = new Onderwerp(naam);

                onderwerpManager.VoegOnderwerpToe(onderwerp);

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