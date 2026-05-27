using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using System.Collections.Generic;
using System.Windows;

namespace UI
{
    public partial class BulkResultatenWindow : Window
    {
        private ResultaatManager resultaatManager;

        public BulkResultatenWindow(ResultaatManager resultaatManager)
        {
            InitializeComponent();

            this.resultaatManager = resultaatManager;
        }

        private void ButtonVerwerken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TextBoxPad.Text))
                {
                    MessageBox.Show("Geef eerst het pad van het bestand in.");
                    return;
                }

                List<TestResultaat> resultaten =
                    resultaatManager.VerwerkBulkResultaten(TextBoxPad.Text);

                List<ResultaatRij> rijen = new List<ResultaatRij>();

                foreach (TestResultaat resultaat in resultaten)
                {
                    ResultaatRij rij = new ResultaatRij(
                        resultaat.Test.Naam,
                        resultaat.GebruikerId,
                        resultaat.Score,
                        resultaat.TotaalAantalVragen
                    );

                    rijen.Add(rij);
                }

                DataGridResultaten.ItemsSource = rijen;

                MessageBox.Show("Bulkbestand werd verwerkt.");
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij verwerken van bulkbestand: " + ex.Message);
            }
        }

        private void ButtonSluiten_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private class ResultaatRij
        {
            public string TestNaam { get; set; }
            public int GebruikerId { get; set; }
            public int Score { get; set; }
            public int Totaal { get; set; }

            public ResultaatRij(string testNaam, int gebruikerId, int score, int totaal)
            {
                TestNaam = testNaam;
                GebruikerId = gebruikerId;
                Score = score;
                Totaal = totaal;
            }
        }
    }
}