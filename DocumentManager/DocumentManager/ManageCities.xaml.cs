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
using DocumentManager.com.dl;
using DocumentManager.com.poco;
namespace DocumentManager
{
    /// <summary>
    /// Interaction logic for ManageCities.xaml
    /// </summary>
    public partial class ManageCities : Window
    {
        public ManageCities()
        {
            InitializeComponent();
            populateCities();
        }
        private void populateCities()
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetAllCities();
            if(res.success)
            {
                List<City> cities = (List<City>)res.body;
                gridCities.ItemsSource = cities;
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void gridCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                City city = (City)gridCities.SelectedItem;
                if (city == null) return;
                txtCity.Text = city.cityName;
                txtState.Text = city.state;
                txtPincode.Text = city.pincode;
                txtStdCode.Text = city.stdCode;
                txtMessage.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void  btnSaveCities_Click(object sender, RoutedEventArgs e)
        {
            if (gridCities.SelectedIndex == -1) return;
            DataLayer dl = new DataLayer();
            City city = new City();
            city.cityName = txtCity.Text;
            city.pincode = txtPincode.Text;
            city.state = txtState.Text;
            city.stdCode = txtStdCode.Text;
            Response res=dl.EditCity(city);
            if(res.success)
            {
                // city editied
                txtMessage.Text = "City saved ";
                txtCity.Text = "";
                txtPincode.Text = "";
                txtState.Text = "";
                txtStdCode.Text = "";
                txtMessage.Visibility = Visibility.Visible;
                System.Threading.Thread.Sleep(1000);
                populateCities();
            }
            else if(res.isException)
            {
                txtMessage.Text = "Exception : " + res.exception;
            }
        }
        private void btnDeleteCities_Click(object sender,RoutedEventArgs e)
        {
            if (gridCities.SelectedIndex == -1) return;
            DataLayer dl = new DataLayer();
            Response res = dl.DeleteCity(txtCity.Text);
            if(res.success)
            {
                txtMessage.Text = "City Deleted with name : " + txtCity.Text;
                txtMessage.Visibility = Visibility.Visible;
                txtCity.Text = "";
                txtPincode.Text = "";
                txtState.Text = "";
                txtStdCode.Text = "";
                System.Threading.Thread.Sleep(1000);
                populateCities();
            }
            else if(res.isException)
            {
                txtMessage.Text = "Exception : " + res.exception;
            }
        }
    }
}
