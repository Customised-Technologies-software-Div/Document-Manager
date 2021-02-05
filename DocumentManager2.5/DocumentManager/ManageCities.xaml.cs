using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using DocumentManager.com.Utility;
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
                Utility.MakeAllColumnsWidthSame(gridCities);
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
                txtCountry.Text = city.country;
                lblStatus.Visibility = Visibility.Collapsed;
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
            city.country = txtCountry.Text;
            Response res=dl.EditCity(city);
            if(res.success)
            {
                // city editied
                lblStatus.Content = "City saved ";
                txtCity.Text = "";
                txtPincode.Text = "";
                txtState.Text = "";
                txtStdCode.Text = "";
                txtCountry.Text = "";
                lblStatus.Visibility = Visibility.Visible;
                System.Threading.Thread.Sleep(1000);
                populateCities();
            }
            else if(res.isException)
            {
                lblStatus.Content = "Exception : " + res.exception;
            }
        }
        private void btnDeleteCities_Click(object sender,RoutedEventArgs e)
        {
            if (gridCities.SelectedIndex == -1) return;
            DataLayer dl = new DataLayer();
            Response res = dl.DeleteCity(txtCity.Text);
            if(res.success)
            {
                lblStatus.Content = "City Deleted with name : " + txtCity.Text;
                lblStatus.Visibility = Visibility.Visible;
                txtCity.Text = "";
                txtPincode.Text = "";
                txtState.Text = "";
                txtStdCode.Text = "";
                txtCountry.Text = "";
                System.Threading.Thread.Sleep(1000);
                populateCities();
            }
            else if(res.isException)
            {
                lblStatus.Content = "Exception : " + res.exception;
            }
        }
        private void gridCities_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor)
            {
                e.Column.Header = descriptor.DisplayName ?? descriptor.Name;
            }
        }

        
        private void gridCities_Loaded(object sender, RoutedEventArgs e)
        {
            Utility.MakeAllColumnsWidthSame(gridCities);
        }

    }
}
