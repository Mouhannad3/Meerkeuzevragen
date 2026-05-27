using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using System.Windows;

namespace UI
{
    public partial class VragenBeheren : Window
    {
        private OnderwerpManager onderwerpManager;
        private VraagManager vraagManager;

        private IReadOnlyList<Onderwerp> onderwerpen;
        private IReadOnlyList<Vraag> geladenVragen;

        public VragenBeheren(OnderwerpManager onderwerpManager, VraagManager vraagManager)
        {
            InitializeComponent();

            this.onderwerpManager = onderwerpManager;
            this.vraagManager = vraagManager;

            onderwerpen = new List<Onderwerp>();
            geladenVragen = new List<Vraag>();

            LaadOnderwerpen();
            ComboBoxCorrectAntwoord.SelectedIndex = 0;
        }

        private void LaadOnderwerpen()
        {
            ComboBoxOnderwerpToevoegen.Items.Clear();
            ComboBoxOnderwerpBeheren.Items.Clear();

            onderwerpen = onderwerpManager.GeefOnderwerpen();

            foreach (Onderwerp onderwerp in onderwerpen)
            {
                ComboBoxOnderwerpToevoegen.Items.Add(onderwerp.Naam);
                ComboBoxOnderwerpBeheren.Items.Add(onderwerp.Naam);
            }

            if (onderwerpen.Count > 0)
            {
                ComboBoxOnderwerpToevoegen.SelectedIndex = 0;
                ComboBoxOnderwerpBeheren.SelectedIndex = 0;
            }
        }

        private void ButtonVraagToevoegen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ComboBoxOnderwerpToevoegen.SelectedIndex < 0)
                {
                    MessageBox.Show("Kies een onderwerp.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TextBoxVraag.Text))
                {
                    MessageBox.Show("Geef een vraag in.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TextBoxAntwoordA.Text) ||
                    string.IsNullOrWhiteSpace(TextBoxAntwoordB.Text) ||
                    string.IsNullOrWhiteSpace(TextBoxAntwoordC.Text) ||
                    string.IsNullOrWhiteSpace(TextBoxAntwoordD.Text))
                {
                    MessageBox.Show("Antwoord A, B, C en D zijn verplicht.");
                    return;
                }

                if (ComboBoxCorrectAntwoord.SelectedIndex < 0)
                {
                    MessageBox.Show("Kies het correcte antwoord.");
                    return;
                }

                char correcteLetter = GeefCorrecteLetter();

                if (correcteLetter == 'E' && string.IsNullOrWhiteSpace(TextBoxAntwoordE.Text))
                {
                    MessageBox.Show("Antwoord E is leeg, dus E kan niet correct zijn.");
                    return;
                }

                Onderwerp onderwerp = onderwerpen[ComboBoxOnderwerpToevoegen.SelectedIndex];

                Vraag vraag = new Vraag(TextBoxVraag.Text, onderwerp);

                vraag.VoegAntwoordToe(new Antwoord(TextBoxAntwoordA.Text, correcteLetter == 'A'));
                vraag.VoegAntwoordToe(new Antwoord(TextBoxAntwoordB.Text, correcteLetter == 'B'));
                vraag.VoegAntwoordToe(new Antwoord(TextBoxAntwoordC.Text, correcteLetter == 'C'));
                vraag.VoegAntwoordToe(new Antwoord(TextBoxAntwoordD.Text, correcteLetter == 'D'));

                if (!string.IsNullOrWhiteSpace(TextBoxAntwoordE.Text))
                {
                    vraag.VoegAntwoordToe(new Antwoord(TextBoxAntwoordE.Text, correcteLetter == 'E'));
                }

                vraagManager.VoegVraagToe(vraag);

                MessageBox.Show("Vraag werd toegevoegd.");

                MaakVeldenLeeg();
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij toevoegen van vraag: " + ex.Message);
            }
        }

        private char GeefCorrecteLetter()
        {
            if (ComboBoxCorrectAntwoord.SelectedIndex == 0)
            {
                return 'A';
            }

            if (ComboBoxCorrectAntwoord.SelectedIndex == 1)
            {
                return 'B';
            }

            if (ComboBoxCorrectAntwoord.SelectedIndex == 2)
            {
                return 'C';
            }

            if (ComboBoxCorrectAntwoord.SelectedIndex == 3)
            {
                return 'D';
            }

            return 'E';
        }

        private void MaakVeldenLeeg()
        {
            TextBoxVraag.Text = "";
            TextBoxAntwoordA.Text = "";
            TextBoxAntwoordB.Text = "";
            TextBoxAntwoordC.Text = "";
            TextBoxAntwoordD.Text = "";
            TextBoxAntwoordE.Text = "";

            ComboBoxCorrectAntwoord.SelectedIndex = 0;
        }

        private void ButtonVragenLaden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ComboBoxOnderwerpBeheren.SelectedIndex < 0)
                {
                    MessageBox.Show("Kies een onderwerp.");
                    return;
                }

                Onderwerp onderwerp = onderwerpen[ComboBoxOnderwerpBeheren.SelectedIndex];

                geladenVragen = vraagManager.GeefBeschikbareVragenByOnderwerp(onderwerp.OnderwerpId);

                ListBoxVragen.Items.Clear();

                foreach (Vraag vraag in geladenVragen)
                {
                    ListBoxVragen.Items.Add(vraag.Tekst);
                }

                if (geladenVragen.Count == 0)
                {
                    MessageBox.Show("Er zijn geen beschikbare vragen voor dit onderwerp.");
                }
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij laden van vragen: " + ex.Message);
            }
        }

        private void ButtonNietBeschikbaar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListBoxVragen.SelectedIndex < 0)
                {
                    MessageBox.Show("Kies een vraag.");
                    return;
                }

                Vraag vraag = geladenVragen[ListBoxVragen.SelectedIndex];

                vraagManager.ZetVraagOnbeschikbaar(vraag.VraagId);

                MessageBox.Show("Vraag werd niet beschikbaar gesteld.");

                ButtonVragenLaden_Click(sender, e);
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij niet beschikbaar stellen van vraag: " + ex.Message);
            }
        }
    }
}