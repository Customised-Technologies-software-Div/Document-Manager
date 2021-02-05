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
using System.Diagnostics;
namespace DocumentManager
{
    /// <summary>
    /// Interaction logic for DataLayerTest.xaml
    /// </summary>
    public partial class DataLayerTest : Window
    {
        public DataLayerTest()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Company company = new Company();
            company.companyName = txtCompanyName.Text;
            Response response = dl.AddNewCompany(company);
            if (response.success) MessageBox.Show("Company Added");
            else if (response.isException) MessageBox.Show("Comapany Not added " + response.exception);
            else MessageBox.Show("Error");
        }

        private void tempBtnEditClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Company company;
                Button b = e.Source as Button;
                if (b.Content.ToString() == "Edit")
                {
                    DataLayer dl = new DataLayer();
                    Response res = dl.GetCompanyByCompanyName(txtCompanyName.Text);
                    if (res.success)
                    {
                        company = (Company)res.body;
                        b.Content = "Update";
                        txtCompanyId.Text = company.companyId.ToString();
                        MessageBox.Show(company.companyId + " " + company.companyName);
                    }
                    else if (res.isException)
                    {
                        MessageBox.Show("Exception occured : " + res.exception);
                    }
                }
                else if (b.Content.ToString() == "Update")
                {
                    DataLayer dl = new DataLayer();
                    company = new Company();
                    company.companyId = Int32.Parse(txtCompanyId.Text);
                    company.companyName = txtCompanyName.Text;
                    //MessageBox.Show(company.companyId + " " + company.companyName);
                    Response res = dl.EditCompany(company);
                    if (res.success)
                    {
                        b.Content = "Edit";
                        //MessageBox.Show("Company name updated");
                        //MessageBox.Show("Company name updated with id "+res.body.ToString());

                    }
                    else if (res.isException)
                    {
                        // I dont know whether to change button name or not
                        MessageBox.Show("Exception occured " + res.exception);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("button exception " + ex.Message);
            }
        }
        private void tempBtnDeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res = dl.GetCompanyByCompanyName(txtCompanyName.Text);
                int companyId = 0;
                if (res.success)
                {
                    Company company = (Company)res.body;
                    companyId = company.companyId;
                }
                else if (res.isException)
                {
                    MessageBox.Show("Exception occured : " + res.exception);
                    return;
                }
                Response res2 = dl.DeleteCompany(companyId);
                if (res2.success)
                {
                    MessageBox.Show("Record deleted");
                }
                else if (res2.isException)
                {
                    MessageBox.Show(res2.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void tempBtnGetAll(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res = dl.GetAllCompanies();
                if (res.success)
                {
                    List<Company> companies = (List<Company>)res.body;
                    for (int i = 0; i < companies.Count; i++)
                    {
                        Trace.WriteLine(i + " " + companies[i].companyId + " " + companies[i].companyName);
                    }
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void tempBtnAddAddressClick(object sender, RoutedEventArgs e)
        {
            Response res = null;
            try
            {
                DataLayer dl = new DataLayer();
                Address address = new Address();
                address.addressID = 1135;
                address.companyID = ((Company)dl.GetCompanyByCompanyName("vinay2").body).companyId;
                address.address1 = "address12";
                address.address2 = "address22";
                address.address3 = "address32";
                address.city = "mumbai";
                address.state = "Maharastra";
                address.country = "India";
                address.pincode = "123456";
                address.phone = "1234567890";
                address.GSTNo = "123GST";
                res = dl.AddAddress(address);
                //res = dl.EditAddress(address);
                if (res.success)
                {
                    MessageBox.Show("address added");
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void tempBtnDeleteAddressClick(object sender, RoutedEventArgs e)
        {
            Response res = null;
            try
            {
                DataLayer dl = new DataLayer();
                int addressID = 1135;
                res = dl.DeleteAddress(addressID);
                if (res.success)
                {
                    MessageBox.Show("address deleted");
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void tempBtnGetAddressByCompanyId(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res = dl.GetAddressByCompanyId(1106);
                if (res.success)
                {
                    List<Address> addresses = (List<Address>)res.body;
                    for (int i = 0; i < addresses.Count; i++)
                    {
                        // just printing two things for checking this method
                        Trace.WriteLine(i + " " + addresses[i].address1 + " " + addresses[i].city);
                    }
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void tempBtnGetAddressById(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res = dl.GetAddressById(1137);
                if (res.success)
                {
                    Address address = (Address)res.body;
                    // just printing two things for checking this method
                    Trace.WriteLine(address.address1 + " " + address.city);
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void tempBtnAddCityClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                City city = new City();
                city.cityName = txtCompanyName.Text;
                city.state = "temp";
                city.pincode = "000";
                city.stdCode = "1234";
                city.country = "India";
                Response res = dl.AddCity(city);
                if (res.success)
                {
                    MessageBox.Show("City added with name " + city.cityName);
                }
                else if (res.isException)
                {
                    MessageBox.Show("Cannot add : " + res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void tempBtnGetAllCitiesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res = dl.GetAllCities();
                if (res.success)
                {
                    List<City> cities = (List<City>)res.body;
                    for (int i = 0; i < cities.Count; i++)
                    {
                        Trace.WriteLine(cities[i].cityName + " " + cities[i].pincode + " " + cities[i].state + " " + cities[i].stdCode+" "+cities[i].country);
                    }
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void tempBtnEditCityClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                City city = new City();
                city.cityName = txtCompanyName.Text;
                city.state = "temp1";
                city.pincode = "001";
                city.stdCode = "12341";
                city.country = "India";
                Response res = dl.EditCity(city);
                if (res.success)
                {
                    MessageBox.Show("City edited with name " + city.cityName);
                }
                else if (res.isException)
                {
                    MessageBox.Show("Cannot edit : " + res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void tempBtnDeleteCityClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                string cityName = txtCompanyName.Text;
                Response res = dl.DeleteCity(cityName);
                if (res.success)
                {
                    MessageBox.Show("City deleted with name " + cityName);
                }
                else if (res.isException)
                {
                    MessageBox.Show("Cannot delete : " + res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tempBtnAddContactClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                Contact contact = new Contact();
                contact.companyId = 1106;
                contact.contact_name = "vinayContact2";
                contact.contact_email = "email2";
                contact.contact_phone = "phone2";
                Response res = dl.AddContact(contact);
                if (res.success)
                {
                    MessageBox.Show("Contact Added with name : " + contact.contact_name);
                }
                else if (res.isException)
                {
                    MessageBox.Show("Cannot add contact : " + res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void tempBtnEditContactClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                Contact contact = new Contact();
                contact.contactId = 1330;
                contact.contact_name = "vinayContact5";
                contact.contact_email = "email5";
                contact.contact_phone = "phone5";
                Response res = dl.EditContact(contact);
                if (res.success)
                {
                    MessageBox.Show("Contact Edited ");
                }
                else if (res.isException)
                {
                    MessageBox.Show("Cannot edit contact : " + res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void tempBtnDeleteContactClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                int contactId = Int32.Parse(txtCompanyName.Text);
                Response res = dl.DeleteContact(contactId);
                if (res.success)
                {
                    MessageBox.Show("Contact deleted with Id : " + contactId);
                }
                else if (res.isException)
                {
                    MessageBox.Show("Cannot delete contact : " + res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void tempBtnGetContactByCompanyIdClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                int companyId = Int32.Parse(txtCompanyName.Text);
                Response res = dl.GetContactsByCompanyId(companyId);
                if (res.success)
                {
                    List<Contact> contacts = (List<Contact>)res.body;
                    for (int i = 0; i < contacts.Count; i++)
                    {
                        Trace.WriteLine(contacts[i].companyId + " " + contacts[i].contactId + " " + contacts[i].contact_name + " " + contacts[i].contact_email + " " + contacts[i].contact_phone);
                    }
                }
                else if (res.isException)
                {
                    MessageBox.Show("Cannot add contact : " + res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tempBtnGetCityByNameClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetCityByName(txtCompanyName.Text);
            if(res.success)
            {
                City city = (City)res.body;
                Trace.WriteLine(city.cityName);
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
        private void tempBtnGetCompanyByCompanyIdClick(object sender,RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetCompanyByCompanyId(Int32.Parse(txtCompanyName.Text));
            if(res.success)
            {
                Company company = (Company)res.body;
                MessageBox.Show("Company Name is " + company.companyName);
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void tempBtnGetDocTypeIdClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            // In company name text field we will enter docTypeName
            Response res = dl.GetDoctypeIdByName(txtCompanyName.Text);
            if (res.success)
            {
                MessageBox.Show(res.body.ToString());
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
        private void tempBtnGetSettingsClick(object sender,RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetSettings();
            if(res.success)
            {
                Settings settings = (Settings)res.body;
                MessageBox.Show("Root : " + settings.docRoot + " Ref Name : " + settings.refName);
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
        private void tempBtnUpdateSettingsClick(object sender,RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Settings settings = new Settings();
            settings.docRoot = @"c:\abcd";
            settings.refName = "vinay";
            Response res = dl.UpdateSettings(settings);
            if (res.success)
            {
                MessageBox.Show("Records Updated");
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
        private void tempBtnGetDocumentsClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetDocuementsByDocType("RDC");
            if (res.success)
            {
                List<Document> documents = (List<Document>)res.body;
                for (int i = 0; i < documents.Count; i++)
                {
                    Trace.WriteLine(documents[i].SNo + " " + documents[i].date + " " + documents[i].companyName);
                }
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
        private void tempBtnAddDocumentClick(object sender,RoutedEventArgs e)
        {
            DLDocument dlDocument = new DLDocument()
            {
                companyId = 1111,
                docTypeId = 71,
                documentDate = DateTime.Now,
                documentId = 2,
                documentPath = "\\vinay1\\rdc.xlsx",
                rev_no = 0,
                sender = "V"
            };
            DataLayer dl = new DataLayer();
            Response res=dl.AddDocument(dlDocument);
            if(res.success)
            {
                // document added
                MessageBox.Show("Document added");
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void tempBtnDeleteDocumentClick(object sender,RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.DeleteDocument(1,"vinay1");
            if(res.success)
            {
                MessageBox.Show("Document deleted");
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void tempBtnGetAllModelsClick(object sender,RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetAllModels();
            if(res.success)
            {
                List<Model> models = (List<Model>)res.body;
                for(int i=0;i<models.Count;i++)
                {
                    Trace.WriteLine(models[i].id + " " + models[i].modelName);
                }
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void tempBtnAddModelClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.AddModel(txtCompanyName.Text);
            if (res.success)
            {
                MessageBox.Show("Model added");
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }

        }

        private void tempBtnUpdateModelClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Model model = new Model();
            model.id = 3;
            model.modelName = "abcd";
            Response res = dl.UpdateModel(model);
            if (res.success)
            {
                MessageBox.Show("Model Updated");
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void tempBtnDeleteModelClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.DeleteModel(txtCompanyName.Text);
            if (res.success)
            {
                MessageBox.Show("Model deleted");
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void tempBtnGetAllServicesClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetAllServices();
            if (res.success)
            {
                List<Service> services = (List<Service>)res.body;
                for (int i = 0; i < services.Count; i++)
                {
                    Trace.WriteLine(services[i].id + " " + services[i].serviceName);
                }
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void tempBtnAddServiceClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.AddService(txtCompanyName.Text);
            if (res.success)
            {
                MessageBox.Show("Service added");
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void tempBtnUpdateServiceClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Service service = new Service();
            service.id = 3;
            service.serviceName = "abcd";
            Response res = dl.UpdateService(service);
            if (res.success)
            {
                MessageBox.Show("Service Updated");
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void tempBtnDeleteServiceClick(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.DeleteService(txtCompanyName.Text);
            if (res.success)
            {
                MessageBox.Show("Service deleted");
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
    }
}
