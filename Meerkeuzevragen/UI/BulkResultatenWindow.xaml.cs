using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using Microsoft.Win32;
using System.Windows;

namespace UI
{
    public partial class BulkResultatenWindow : Window
    {
        private readonly ResultaatManager resultaatManager;

        public BulkResultatenWindow(ResultaatManager resultaatManager)
        {
            InitializeComponent();

            this.resultaatManager = resultaatManager;
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

        private void ButtonVerwerken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<TestResultaat> resultaten =
                    resultaatManager.VerwerkBulkResultaten(TextBoxPad.Text);

                DataGridResultaten.ItemsSource = resultaten;

                MessageBox.Show("Bulkbestand werd verwerkt.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ButtonSluiten_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}