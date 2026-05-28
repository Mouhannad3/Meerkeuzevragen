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
          
        }

        private void ButtonSluiten_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}