using BL.Domein;
using BL.Exceptions;
using BL.Managers;
using System;
using System.Collections.Generic;
using System.Windows;

namespace UI
{
    public partial class BulkResultatenWindow : Window
    {
        private ResultaatManager resultaatManager;
        private TestManager testManager;

        public BulkResultatenWindow(ResultaatManager resultaatManager, TestManager testManager)
        {
            InitializeComponent();

            this.resultaatManager = resultaatManager;
            this.testManager = testManager;

            LaadTesten();
        }

        private void LaadTesten()
        {
            ComboBoxTesten.ItemsSource = testManager.GeefTesten();
        }

        private void ButtonVerwerken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Test? gekozenTest = ComboBoxTesten.SelectedItem as Test;

                if (gekozenTest == null)
                {
                    MessageBox.Show("Kies eerst een test.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TextBoxPad.Text))
                {
                    MessageBox.Show("Geef eerst het pad van het bestand in.");
                    return;
                }

                Test volledigeTest = testManager.GeefTestById(gekozenTest.TestId);

                IReadOnlyList<TestResultaat> resultaten =
                    resultaatManager.VerwerkBulkResultaten(TextBoxPad.Text, volledigeTest);

                MessageBox.Show("Er werden " + resultaten.Count + " resultaten verwerkt.");

                DialogResult = true;
                Close();
            }
            catch (MeerkeuzeException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij verwerken van resultaten: " + ex.Message);
            }
        }

        private void ButtonSluiten_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}