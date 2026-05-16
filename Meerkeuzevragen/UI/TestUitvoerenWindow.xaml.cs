using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using System.Windows;
using System.Windows.Controls;

namespace UI
{
    public partial class TestUitvoerenWindow : Window
    {
        private readonly TestManager testManager;
        private readonly ResultaatManager resultaatManager;

        private Test? geladenTest;
        private readonly Dictionary<int, char> gekozenAntwoorden = new();

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
                Test? geselecteerdeTest = ComboBoxTesten.SelectedItem as Test;

                if (geselecteerdeTest == null)
                {
                    MessageBox.Show("Kies eerst een test.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                geladenTest = testManager.GeefTestById(geselecteerdeTest.TestId);

                ToonTest(geladenTest);
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

        private void ToonTest(Test test)
        {
            StackPanelVragen.Children.Clear();
            gekozenAntwoorden.Clear();

            foreach (TestVraag testVraag in test.TestVragen.OrderBy(tv => tv.Volgorde))
            {
                GroupBox groupBox = new GroupBox();
                groupBox.Header = $"{testVraag.Volgorde}. {testVraag.Vraag.Tekst}";
                groupBox.Margin = new Thickness(0, 0, 0, 15);

                StackPanel stackPanelAntwoorden = new StackPanel();
                stackPanelAntwoorden.Margin = new Thickness(10);

                foreach (TestVraagAntwoord testVraagAntwoord in testVraag.TestVraagAntwoorden.OrderBy(tva => tva.Volgorde))
                {
                    RadioButton radioButton = new RadioButton();
                    radioButton.Content = $"{testVraagAntwoord.Letter}. {testVraagAntwoord.Antwoord.Tekst}";
                    radioButton.Margin = new Thickness(0, 3, 0, 3);
                    radioButton.GroupName = $"Vraag_{testVraag.TestVraagId}";
                    radioButton.Tag = new KeuzeInfo(testVraag.TestVraagId, testVraagAntwoord.Letter);
                    radioButton.Checked += RadioButtonAntwoord_Checked;

                    stackPanelAntwoorden.Children.Add(radioButton);
                }

                groupBox.Content = stackPanelAntwoorden;
                StackPanelVragen.Children.Add(groupBox);
            }
        }

        private void RadioButtonAntwoord_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            KeuzeInfo keuzeInfo = (KeuzeInfo)radioButton.Tag;

            gekozenAntwoorden[keuzeInfo.TestVraagId] = keuzeInfo.Letter;
        }

        private void ButtonVerbeteren_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (geladenTest == null)
                {
                    MessageBox.Show("Laad eerst een test.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(TextBoxGebruikerId.Text, out int gebruikerId))
                {
                    MessageBox.Show("GebruikerId moet een getal zijn.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (gekozenAntwoorden.Count != geladenTest.TestVragen.Count)
                {
                    MessageBox.Show("Beantwoord eerst alle vragen.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string antwoorden = MaakAntwoordenString(geladenTest);

                TestResultaat resultaat = resultaatManager.VerbeterTest(
                    geladenTest.TestId,
                    gebruikerId,
                    antwoorden
                );

                string feedback = MaakFeedback(geladenTest);

                string bericht = $"Score: {resultaat.Score}/{resultaat.TotaalAantalVragen}";

                if (!string.IsNullOrWhiteSpace(feedback))
                {
                    bericht += "\n\nFoute antwoorden:\n\n" + feedback;
                }

                MessageBox.Show(
                    bericht,
                    "Resultaat",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
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

        private string MaakAntwoordenString(Test test)
        {
            string antwoorden = "";

            foreach (TestVraag testVraag in test.TestVragen.OrderBy(tv => tv.Volgorde))
            {
                antwoorden += gekozenAntwoorden[testVraag.TestVraagId];
            }

            return antwoorden;
        }

        private void ButtonSluiten_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private class KeuzeInfo
        {
            public int TestVraagId { get; }
            public char Letter { get; }

            public KeuzeInfo(int testVraagId, char letter)
            {
                TestVraagId = testVraagId;
                Letter = letter;
            }
        }
        private string MaakFeedback(Test test)
        {
            string feedback = "";

            foreach (TestVraag testVraag in test.TestVragen.OrderBy(tv => tv.Volgorde))
            {
                char gekozenLetter = gekozenAntwoorden[testVraag.TestVraagId];

                Antwoord gekozenAntwoord = testVraag.GeefAntwoordVoorLetter(gekozenLetter);

                if (!gekozenAntwoord.IsCorrect)
                {
                    TestVraagAntwoord juistTestVraagAntwoord = testVraag.TestVraagAntwoorden
                        .First(tva => tva.Antwoord.IsCorrect);

                    feedback += $"Vraag {testVraag.Volgorde}: {testVraag.Vraag.Tekst}\n";
                    feedback += $"Jouw antwoord: {gekozenLetter}. {gekozenAntwoord.Tekst}\n";
                    feedback += $"Juiste antwoord: {juistTestVraagAntwoord.Letter}. {juistTestVraagAntwoord.Antwoord.Tekst}\n\n";
                }
            }

            return feedback;
        }
    }
}
