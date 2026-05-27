using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using System.Windows;

namespace UI
{
    public partial class TestUitvoerenWindow : Window
    {
        private readonly TestManager testManager;
        private readonly ResultaatManager resultaatManager;

        private Test? geladenTest;

        public TestUitvoerenWindow(TestManager testManager, ResultaatManager resultaatManager)
        {
            InitializeComponent();

            this.testManager = testManager;
            this.resultaatManager = resultaatManager;

            LaadTesten();
        }

        private void LaadTesten()
        {
            ComboBoxTesten.ItemsSource = testManager.GeefTesten();
        }

        private void ButtonTestLaden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Test? test = ComboBoxTesten.SelectedItem as Test;

                if (test == null)
                {
                    MessageBox.Show("Kies eerst een test.");
                    return;
                }

                geladenTest = testManager.GeefTestById(test.TestId);

                string tekst = testManager.MaakExportTekst(test.TestId);

                TextBoxTest.Text = tekst;
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij laden van test: " + ex.Message);
            }
        }

        private void ButtonVerbeteren_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (geladenTest == null)
                {
                    MessageBox.Show("Laad eerst een test.");
                    return;
                }

                if (!int.TryParse(TextBoxGebruikerId.Text, out int gebruikerId))
                {
                    MessageBox.Show("GebruikerId moet een getal zijn.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TextBoxAntwoorden.Text))
                {
                    MessageBox.Show("Geef de antwoorden in.");
                    return;
                }

                string antwoorden = TextBoxAntwoorden.Text.Trim().ToUpper();

                TestResultaat resultaat = resultaatManager.VerbeterTest(
                    geladenTest.TestId,
                    gebruikerId,
                    antwoorden
                );

                string bericht = "Score: " +
                                 resultaat.Score +
                                 "/" +
                                 resultaat.TotaalAantalVragen;

                MessageBox.Show(bericht);
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij verbeteren: " + ex.Message);
            }
        }

        private void ButtonSluiten_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}