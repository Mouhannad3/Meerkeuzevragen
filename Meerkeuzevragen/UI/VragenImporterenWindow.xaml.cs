using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using Microsoft.Win32;
using System.Windows;

namespace UI
{
    public partial class VragenImporterenWindow : Window
    {
        private readonly OnderwerpManager onderwerpManager;
        private readonly ImportManager importManager;

        public VragenImporterenWindow(OnderwerpManager onderwerpManager, ImportManager importManager)
        {
            InitializeComponent();

            this.onderwerpManager = onderwerpManager;
            this.importManager = importManager;

            LaadOnderwerpen();
        }

        private void LaadOnderwerpen()
        {
            ComboBoxOnderwerpen.ItemsSource = onderwerpManager.GeefOnderwerpen();
        }

        private void ButtonBladeren_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Tekstbestanden (*.txt)|*.txt|Alle bestanden (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                TextBoxPad.Text = dialog.FileName;
            }
        }

        private void ButtonImporteren_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Onderwerp? onderwerp = ComboBoxOnderwerpen.SelectedItem as Onderwerp;

                if (onderwerp == null)
                {
                    MessageBox.Show("Kies eerst een onderwerp.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                importManager.ImporteerVragen(TextBoxPad.Text, onderwerp.OnderwerpId);

                MessageBox.Show("Vragen werden geïmporteerd.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

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
