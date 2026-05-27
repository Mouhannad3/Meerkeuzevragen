using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using System.Collections.Generic;
using System.Windows;

namespace UI
{
    public partial class VragenImporterenWindow : Window
    {
        private OnderwerpManager onderwerpManager;
        private ImportManager importManager;

        private IReadOnlyList<Onderwerp> onderwerpen;

        public VragenImporterenWindow(OnderwerpManager onderwerpManager, ImportManager importManager)
        {
            InitializeComponent();

            this.onderwerpManager = onderwerpManager;
            this.importManager = importManager;

            onderwerpen = new List<Onderwerp>();

            LaadOnderwerpen();
        }

        private void LaadOnderwerpen()
        {
            ComboBoxOnderwerpen.Items.Clear();

            onderwerpen = onderwerpManager.GeefOnderwerpen();

            foreach (Onderwerp onderwerp in onderwerpen)
            {
                ComboBoxOnderwerpen.Items.Add(onderwerp.Naam);
            }

            if (onderwerpen.Count > 0)
            {
                ComboBoxOnderwerpen.SelectedIndex = 0;
            }
        }

        private void ButtonImporteren_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ComboBoxOnderwerpen.SelectedIndex < 0)
                {
                    MessageBox.Show("Kies eerst een onderwerp.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TextBoxPad.Text))
                {
                    MessageBox.Show("Geef eerst het pad van het bestand in.");
                    return;
                }

                Onderwerp onderwerp = onderwerpen[ComboBoxOnderwerpen.SelectedIndex];

                importManager.ImporteerVragen(TextBoxPad.Text, onderwerp.OnderwerpId);

                MessageBox.Show("Vragen werden geïmporteerd.");

                DialogResult = true;
                Close();
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij importeren: " + ex.Message);
            }
        }

        private void ButtonAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}