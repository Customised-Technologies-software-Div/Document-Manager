using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DocumentManager
{
    /// <summary>
    /// Interaction logic for CreateTemplate.xaml
    /// </summary>
    public partial class CreateTemplate : Window
    {
        public CreateTemplate()
        {
            InitializeComponent();
        }

        private void btnSelectTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Select Template",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "xls",
                    Filter = "xls files (*.xls)|*.xls",
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    txtSelectTemplate.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("File Not Selected");
                    return;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCreateFieldList_Click(object sender, RoutedEventArgs e)
        {
            FieldsCreator fieldsCreator = new FieldsCreator();
            fieldsCreator.Show();
        }
    }
}
