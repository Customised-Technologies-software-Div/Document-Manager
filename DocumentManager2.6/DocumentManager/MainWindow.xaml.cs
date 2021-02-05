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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NPOI.XSSF.UserModel;
using DocumentManager.com.poco;
using DocumentManager.com.Utility;
using DocumentManager.com.dl;
using DocumentManager.com.exception;
using System.Threading;
using System.ComponentModel;

namespace DocumentManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        private AddressBuffer addressBuffer;
     
        public MainWindow()
        {
            InitializeComponent();
            populateStatesCmb();
            populateCompaniesCmb();
            populateSettings();
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            setCompanyViewMode();
            setViewAddressMode();
            if(cmbCompanies.Items.Count>0) cmbCompanies.SelectedIndex = 0;
        }
        
        /// <summary>
        /// This will take values from database and populate states combo box
        /// </summary>
        private void populateStatesCmb()
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res = dl.GetStates();
                if(res.success)
                {
                    List<State> states = (List<State>)res.body;
                    cmbState.ItemsSource = states;
                    cmbState.DisplayMemberPath = "stateName";
                }
                else if(res.isException)
                {
                    throw new DAOException(res.exception);
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Exception occured in populating states " + ex.Message);
            }
        }

        /// <summary>
        /// This is a helper method to get state object by stateName
        /// </summary>
        /// <param name="stateName"></param>
        private void selectStateByStateName(string stateName)
        {
            int Selected = -1;
            int count = cmbState.Items.Count;
            for (int i = 0; (i <= (count - 1)); i++)
            {
                cmbState.SelectedIndex = i;

                if (((State)(cmbState.SelectedItem)).stateName == stateName)
                {
                    Selected = i;
                    break;
                }
            }
            txtStateCode.Text = getStateCode();
        }

        /// <summary>
        /// This is helper method to get current selected state
        /// </summary>
        /// <returns></returns>
        public State getSelectedState()
        {
            return (State)(cmbState.SelectedItem);
        }

        /// <summary>
        /// It will take companies values from database and populate companies combo box
        /// </summary>
        private void populateCompaniesCmb()
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res = dl.GetAllCompanies();
                if (res.success)
                {
                    List<Company> companies = (List<Company>)res.body;
                    cmbCompanies.ItemsSource = null;
                    // how about we can change in companies pojo, and add and show whatever we want
                    cmbCompanies.ItemsSource = companies;
                    cmbCompanies.DisplayMemberPath = "companyNameToShow";
                    cmbCompanies.SelectionChanged += cmbCompaniesSelectionChanged;
                }
                else if (res.isException)
                {
                    MessageBox.Show("Exception while populating companies : " + res.exception);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception in populate companies cmb : " + exception);
            }
        }

        /// <summary>
        /// It will call every time when selection in combo box changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCompaniesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Response res;
            DataLayer dl = new DataLayer();
            try
            {
                if (cmbCompanies.SelectedIndex == -1)
                {
                    emptyAllAddressFields();
                    gridContacts.ItemsSource = null;
                    return;
                }
                Company company = (Company)cmbCompanies.SelectedItem;
                PopulateFilePaths();
                setAddressComponent(company.companyId);
                populateContacts(company.companyId);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception while selecting company " + exception);
            }
        }

        /// <summary>
        /// It will populate address buffer according to company name
        /// </summary>
        /// <param name="companyId"></param>
        private void setAddressComponent(int companyId)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetAddressByCompanyId(companyId);
            if (res.success)
            {
                List<Address> addresses = (List<Address>)res.body;
                addressBuffer = new AddressBuffer(addresses);
                emptyAllAddressFields();
                if (addressBuffer.GetSize() != 0)
                {
                    Address address = addressBuffer.GetCurrentAddress();
                    FillAddress(address);
                }
                setAddressPanel();
            }
            else if (res.isException)
            {
                MessageBox.Show("Point 2 : " + res.exception);
            }
        }

        /// <summary>
        /// It will populate contacts Data grid according to particular company values from db
        /// </summary>
        /// <param name="companyId"></param>
        private void populateContacts(int companyId)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetContactsByCompanyId(companyId);
            if (res.success)
            {
                List<Contact> contacts = (List<Contact>)res.body;
                gridContacts.ItemsSource = contacts;
                // At every time we are changing value of Item source, we have to hide columns we want to hide.
                if (gridContacts.Columns.Count > 2)
                {
                    gridContacts.Columns[0].Visibility = Visibility.Collapsed;
                    gridContacts.Columns[1].Visibility = Visibility.Collapsed;
                }
                Utility.MakeAllColumnsWidthSame(gridContacts);
                if(gridContacts.Columns.Count>=3) gridContacts.Columns[3].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                //setAllColumnsOfSameWidth();
                //if (gridContacts.Columns.Count > 3)
                //gridContacts.Columns[1].Width = new DataGridLength(3, DataGridLengthUnitType.Star);
                if (contacts.Count > 0) gridContacts.SelectedIndex = 0;
            }
            else if (res.isException)
            {
                MessageBox.Show("Point 3 : " + res.exception);
            }
        }
        
        /// <summary>
        /// It is helper method which will give current selected company object
        /// </summary>
        /// <returns></returns>
        public Company getCurrentSelectedCompany()
        {
            return (Company)cmbCompanies.SelectedItem;
        }

        /// <summary>
        /// It will populate different text fields according to taking values from settings table in db
        /// </summary>
        public void populateSettings()
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetSettings();
            if (res.success)
            {
                Settings settings = (Settings)res.body;
                txtTemplateFolderPath.Text = settings.templateRoot;
                txtRef.Text = settings.refName;
                txtDocumentFolderPath.Text = settings.docRoot;
                txtServiceInvoiceNo.Text = settings.serviceInvoiceNo;
                txtTaxInvoiceNo.Text = settings.taxInvoiceNo;
                populateTemplateFoldersCmb();
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        /// <summary>
        /// It will populate template folder Combobox according to folder path given in template folder text feild
        /// </summary>
        private void populateTemplateFoldersCmb()
        {
            try
            {
                cmbTemplateFiles.ItemsSource = null;
                cmbTemplateFolders.Items.Clear();

                string[] subdirectoryEntries = Directory.GetDirectories(txtTemplateFolderPath.Text.ToString());


                // Loop through them to see if they have any other subdirectories
                foreach (string subdirectory in subdirectoryEntries)
                {
                    string folderName = System.IO.Path.GetFileName(subdirectory);
                    cmbTemplateFolders.Items.Add(folderName);
                }
                cmbTemplateFolders.SelectionChanged += cmbTemplateFolderSelectionChanged;
                if (cmbTemplateFolders.SelectedIndex == -1) cmbTemplateFolders.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception : Populate Template Folder : " + ex.Message);
            }
        }

        /// <summary>
        /// It will call every time template folder combobox value changed, and do whatever need to be done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTemplateFolderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTemplateFolders.SelectedItem == null)
            {
                cmbTemplateFiles.Items.Clear();
                //gridDocuments.Items.Clear();
                return;
            }
            populateTemplateFiles(cmbTemplateFolders.SelectedItem.ToString());
            populateDocumentsDataGrid(cmbTemplateFolders.SelectedItem.ToString());
        }

        /// <summary>
        /// It will populate files in templateFiles cmb, according to value from template folder combo box
        /// </summary>
        /// <param name="docType"></param>
        private void populateTemplateFiles(string docType)
        {
            try
            {
                String folderName = docType;
                string[] fileEntries = Directory.GetFiles(txtTemplateFolderPath.Text + "\\" + folderName);
                cmbTemplateFiles.Items.Clear();
                foreach (var file_name in fileEntries)
                {
                    cmbTemplateFiles.Items.Add(System.IO.Path.GetFileName(file_name));
                }
                cmbTemplateFiles.SelectedIndex = 0;
                txtTemplateFilePath.Text = fileEntries[0];
            }
            catch (NullReferenceException nre)
            {
                MessageBox.Show(nre.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// It will trigger at a time of selection changed in template files cmb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTemplateFilesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PopulateFilePaths();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// It will populate template and save file text field, by taking values from various places
        /// </summary>
        private void PopulateFilePaths()
        {
            if (cmbTemplateFolders.SelectedIndex == -1 || cmbTemplateFiles.SelectedIndex == -1 || getCurrentSelectedCompany() == null) return;
            txtTemplateFilePath.Text = txtTemplateFolderPath.Text + "\\" + cmbTemplateFolders.SelectedItem.ToString() + "\\" + cmbTemplateFiles.SelectedItem.ToString();
            FileInfo fileInfo = new FileInfo(txtTemplateFilePath.Text);

            txtSaveFilePath.Text = txtDocumentFolderPath.Text + "\\" + getCurrentSelectedCompany().companyName + "\\" + cmbTemplateFolders.SelectedItem.ToString() + txtDocumentSerialNumber.Text + txtRef.Text + fileInfo.Extension;
        }

        /// <summary>
        /// It will make changes in UI changes according to add mode
        /// </summary>
        public void setCompanyAddMode()
        {
            cmbCompanies.Visibility = Visibility.Collapsed;
            txtCompanyName.Visibility = Visibility.Visible;
            txtCompanyName.Text = "";
            btnEditCompany.IsEnabled = false;
            btnDeleteCompany.IsEnabled = false;
            btnCancelCompany.IsEnabled = true;
        }

        /// <summary>
        /// It will make changes in UI according to update Mode
        /// </summary>
        public void setCompanyEditMode()
        {
            cmbCompanies.Visibility = Visibility.Collapsed;
            txtCompanyName.Visibility = Visibility.Visible;
            txtCompanyName.Text = ((Company)cmbCompanies.SelectedItem).companyName;
            btnAddCompany.IsEnabled = false;
            btnDeleteCompany.IsEnabled = false;
            btnCancelCompany.IsEnabled = true;
        }

        /// <summary>
        /// View Mode is default mode, when window start company is in view mode
        /// </summary>
        public void setCompanyViewMode()
        {
            cmbCompanies.Visibility = Visibility.Visible;
            txtCompanyName.Visibility = Visibility.Collapsed;
            btnAddCompany.IsEnabled = true;
            btnEditCompany.IsEnabled = true;
            btnDeleteCompany.IsEnabled = true;
            btnCancelCompany.IsEnabled = false;
            btnAddCompany.Content = "Add";
            btnEditCompany.Content = "Edit";
        }

        /// <summary>
        /// It will trigger when we Add button clicked below company cmb. In this firstly it will set company to add mode and make add button as OK button. Then call dl add function when click on ok button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddCompanyDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            if (btn.Content.ToString() == "Add")
            {
                btn.Content = "OK";
                setCompanyAddMode();
            }
            else
            {

                if (txtCompanyName.Text == "" || txtCompanyName.Text.Length == 0)
                {
                    MessageBox.Show("Company name cannot be empty");
                    setCompanyAddMode();
                    return;
                }
                DataLayer dl = new DataLayer();
                Company company = new Company();
                company.companyName = txtCompanyName.Text;
                Response res = dl.AddNewCompany(company);
                if (res.success)
                {
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                    setCompanyViewMode();
                    btn.Content = "Add";
                    lblCompanyStatus.Content = "Company Added Successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        /// <summary>
        /// It will trigger when Edit button below company clicked, it will do same process as for add company function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditCompanyDetails_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }
            Button btn = e.Source as Button;
            if (btn.Content.ToString() == "Edit")
            {
                btn.Content = "OK";
                setCompanyEditMode();
            }
            else
            {
                if (txtCompanyName.Text == "" || txtCompanyName.Text.Length == 0)
                {
                    MessageBox.Show("Company name cannot be empty");
                    return;
                }
                DataLayer dl = new DataLayer();
                Company company = new Company();
                company.companyId = ((Company)cmbCompanies.SelectedItem).companyId;
                company.companyName = txtCompanyName.Text;
                Response res = dl.EditCompany(company);
                if (res.success)
                {
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                    setCompanyViewMode();
                    lblCompanyStatus.Content = "Company updated successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        /// <summary>
        /// It will populate companies combo box again. Actually, if crud operation is performed and updation in combo box not shown, we can refresh with this function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshCompaniesCmb(object sender, RoutedEventArgs e)
        {
            populateCompaniesCmb();
            lblCompanyStatus.Content = "Refreshed";
        }

        /// <summary>
        /// This function will ask if you want to delete particular company, if your ans is yes, it will call delete method from data layer and delete the company.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteCompanyDetails_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Confirm Delete : " + ((Company)cmbCompanies.SelectedItem).companyName, "Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DataLayer dl = new DataLayer();
                Response res = dl.DeleteCompany(((Company)cmbCompanies.SelectedItem).companyId);
                if (res.success)
                {
                    //company deleted
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                    lblCompanyStatus.Content = "Company Deleted Successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        /// <summary>
        /// It will set company to view mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelCompany_Click(object sender, RoutedEventArgs e)
        {
            setCompanyViewMode();
        }

        /// <summary>
        /// It will set address to view mode, and make UI changes accordingly. Also default mode is view mode.
        /// </summary>
        void setViewAddressMode()
        {
            btnAddAddress.IsEnabled = true;
            btnSaveAddress.IsEnabled = true;
            btnDeleteAddress.IsEnabled = true;
            btnCancelAddress.IsEnabled = false;
            setAddressPanel();
            cmbCompanies.IsEnabled = true;
            btnAddAddress.Content = "Add";
        }
        void setAddAddressMode()
        {
            btnAddAddress.IsEnabled = true;
            btnSaveAddress.IsEnabled = false;
            btnDeleteAddress.IsEnabled = false;
            btnCancelAddress.IsEnabled = true;
            addressBuffer.AddAddress(new Address());
            setAddressPanel();
            while (addressBuffer.canGetNext()) btnNext.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            cmbCompanies.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnAddAddress.Content = "Ok";
        }
        private void btnCancelAddressModification_Click(object sender, RoutedEventArgs e)
        {
            setViewAddressMode();
            addressBuffer.RemoveLast();
            setAddressPanel();
            if (addressBuffer.canGetPrev()) btnPrev.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        /// <summary>
        /// It will firstly set address to addAddressMode, and make button as OK button. When we click onn Ok button, it will call addAddress method in DL, by taking values from different address text fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAddressDetails_Click(object sender, RoutedEventArgs e)
        {
            Button b = e.Source as Button;
            if (b.Content.ToString() == "Add")
            {
                setAddAddressMode();
            }
            else if (b.Content.ToString() == "Ok")
            {
                if(cmbCompanies.SelectedIndex==-1)
                {
                    MessageBox.Show("Please select company first");
                    return;
                }
                DataLayer dl = new DataLayer();
                Address address = new Address();
                address.address1 = txtAddress1.Text;
                address.address2 = txtAddress2.Text;
                address.address3 = txtAddress3.Text;
                address.city = txtCity.Text;
                address.GSTNo = txtGSTNo.Text;
                address.phone = txtPhone.Text;
                address.pincode = txtPincode.Text;
                address.state = getSelectedState().stateName;
                address.country = txtCountry.Text;
                if (txtStateCode.Text.Trim() != "") address.stateCode = Int32.Parse(txtStateCode.Text);
                
                address.companyID = ((Company)cmbCompanies.SelectedItem).companyId;
                Response res = dl.AddAddress(address);
                if (res.success)
                {
                    // address added
                    Thread.Sleep(1000);
                    setAddressComponent(getCurrentSelectedCompany().companyId);
                    setViewAddressMode();
                    populateCompaniesCmb();
                    lblAddressStatus.Content = "Address Added Successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        /// <summary>
        /// It will call save address function from DL by taking values from different address fields and also update address buffer details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveAddressDetails_Click(object sender, RoutedEventArgs e)
        {
            if (addressBuffer == null)
            {
                MessageBox.Show("please select a address first");
                return;
            }
            Address oldAddress = addressBuffer.GetCurrentAddress();
            Address newAddress = new Address()
            {
                address1 = txtAddress1.Text,
                address2 = txtAddress2.Text,
                address3 = txtAddress3.Text,
                city = txtCity.Text,
                state = getSelectedState().stateName,
                country = txtCountry.Text,
                pincode = txtPincode.Text,
                phone = txtPhone.Text,
                GSTNo = txtGSTNo.Text
            };
            //stateCode = (txtStateCode.Text == "") ? 0 : Int32.Parse(txtStateCode.Text);
            if (txtStateCode.Text.Trim() != "") newAddress.stateCode = Int32.Parse(txtStateCode.Text);
            newAddress.addressID = oldAddress.addressID;
            DataLayer dl = new DataLayer();
            Response res = dl.EditAddress(newAddress);
            if (res.success)
            {
                // address edited / saved successfully
                Thread.Sleep(1000);
                setAddressComponent(getCurrentSelectedCompany().companyId);
                populateCompaniesCmb();
                lblAddressStatus.Content = "Address Updated Successfully";
            }
            else if (res.isException)
            {
                MessageBox.Show("Exception in saving address : " + res.exception);
            }
        }

        /// <summary>
        /// It will call delete address function from dl, and also delete address from address buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteAddress_Click(object sender, RoutedEventArgs e)
        {
            Address address = addressBuffer.GetCurrentAddress();
            MessageBoxResult result = MessageBox.Show("Confirm Delete whole address line 1 is " + address.address1, "Delete Address", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DataLayer dl = new DataLayer();
                Response res = dl.DeleteAddress(address.addressID);
                if (res.success)
                {
                    // address deleted
                    Thread.Sleep(2000);
                    setAddressComponent(getCurrentSelectedCompany().companyId);
                    setAddressPanel();
                    populateCompaniesCmb();
                    lblAddressStatus.Content = "Address Deleted Successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show("Exception while deleting address : " + res.exception);
                }
            }
        }

        /// <summary>
        /// It will take values from text fields and call Add city function from DL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddCity_Click(object sender, RoutedEventArgs e)
        {
            City city = new City();
            city.cityName = txtCity.Text;
            city.state = getSelectedState().stateName;
            city.pincode = txtPincode.Text;
            city.stdCode = txtPhone.Text;
            city.country = txtCountry.Text;

            DataLayer dl = new DataLayer();
            Response res = dl.AddCity(city);
            if (res.success)
            {
                // city addded
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        /// <summary>
        /// It will empty all the address fields.
        /// </summary>
        private void emptyAllAddressFields()
        {
            txtAddress1.Text = "";
            txtAddress2.Text = "";
            txtAddress3.Text = "";
            txtCity.Text = "";
            cmbState.SelectedIndex = -1;
            txtCountry.Text = "";
            txtPincode.Text = "";
            txtPhone.Text = "";
            txtGSTNo.Text = "";
            txtStateCode.Text = "";
        }

        /// <summary>
        /// It is just a helper method, which will take address object and fill all address fields
        /// </summary>
        /// <param name="address"></param>
        private void FillAddress(Address address)
        {
            txtAddress1.Text = address.address1;
            txtAddress2.Text = address.address2;
            txtAddress3.Text = address.address3;
            txtCity.Text = address.city;
            selectStateByStateName(address.state);
            txtCountry.Text = address.country;
            //txtState.Text = address.state; Changes
            txtPincode.Text = address.pincode;
            txtPhone.Text = address.phone;
            txtGSTNo.Text = address.GSTNo;
            txtStateCode.Text = getStateCode();
        }

        /// <summary>
        /// It will take prev address value from addressBuffer and call fill address for that address value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnPrevAddress_Click(object sender, RoutedEventArgs e)
        {
            // It will show previous address and change value on txtAddressInfo
            try
            {
                if (addressBuffer != null && addressBuffer.canGetPrev())
                {
                    FillAddress(addressBuffer.GetPreviousAddress());
                }
                setAddressPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// It will take next address from address buffer and call fill address for the same
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnNextAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (addressBuffer != null && addressBuffer.canGetNext())
                {
                    FillAddress(addressBuffer.GetNextAddress());
                }
                setAddressPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// It will set address panel, which is located between prev and next address button. This panel will show, out of total addresses from address which address you are currently in.
        /// </summary>
        private void setAddressPanel()
        {
            if (addressBuffer == null || addressBuffer.GetSize() == 0)
            {
                txtAddressInfo.Text = "0 / 0";
                btnPrev.IsEnabled = false;
                btnNext.IsEnabled = false;
            }
            else
            {
                txtAddressInfo.Text = addressBuffer.GetCurrentIndex() + " / " + addressBuffer.GetSize();
                if (addressBuffer != null && !addressBuffer.canGetPrev())
                    btnPrev.IsEnabled = false;
                else
                    btnPrev.IsEnabled = true;
                if (addressBuffer != null && !addressBuffer.canGetNext())
                    btnNext.IsEnabled = false;
                else
                    btnNext.IsEnabled = true;
            }
        }

        /// <summary>
        /// It will populate fields object and then generate xlsx report by calling generate report method from ReportProcessor. Also It will also save information of report generated in documents table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //DataRowView row = (DataRowView)gridContacts.SelectedItem;
                if (gridContacts.SelectedIndex == -1 || cmbCompanies.SelectedIndex == -1)
                {
                    MessageBox.Show("Company/Contact Not Selected");
                    return;
                }
                if (cmbTemplateFolders.SelectedIndex == -1 || cmbTemplateFiles.SelectedIndex == -1)
                {
                    MessageBox.Show("Folder or file is not selected");
                    return;
                }
                if (txtDocumentSerialNumber.Text == "" || !Utility.IsNumber(txtDocumentSerialNumber.Text))
                {
                    MessageBox.Show("Please give valid serial number");
                    return;
                }
                string refName = txtRef.Text;
                //Contact contact = (Contact)gridContacts.SelectedItem;
                Field field = new Field()
                {
                    refNo = txtRef.Text,
                    companyName = getCurrentSelectedCompany().companyName,
                    address1 = txtAddress1.Text,
                    address2 = txtAddress2.Text,
                    address3 = txtAddress3.Text,
                    city = txtCity.Text,
                    pincode = txtPincode.Text,
                    phone = txtPhone.Text,
                    date = txtDate.Text,
                    state = getSelectedState().stateName,
                    country = txtCountry.Text,
                    stateCode = txtStateCode.Text,
                    contactName = txtContactName.Text,
                    contactEmail = txtContactEmail.Text,
                    contactPhone = txtContactPhone.Text,
                    gstNo = txtGSTNo.Text,
                    serviceInvoiceNo = txtServiceInvoiceNo.Text,
                    taxInvoiceNo = txtTaxInvoiceNo.Text
                };
                ReportProcessor rp = new ReportProcessor();
                string saveFolder = txtDocumentFolderPath.Text + "\\" + getCurrentSelectedCompany().companyName;
                //rp.GenerateReport(field, txtTemplateFilePath.Text, txtSaveFilePath.Text);
                //return;
                if (File.Exists(txtSaveFilePath.Text))
                {
                    MessageBox.Show("File Exists previously, either change serial number or delete file from " + txtSaveFilePath.Text);
                }
                //Trace.WriteLine("save file path" + txtSaveFilePath.Text);
                //Trace.WriteLine("Template file path" + txtTemplateFilePath.Text);
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }
                rp.GenerateReport(field, txtTemplateFilePath.Text, txtSaveFilePath.Text);
                //Trace.WriteLine("Report Generated");
                SaveInDb(txtSaveFilePath.Text);
                if (btnOpenToggle.IsChecked == true)
                {
                    Process.Start(txtSaveFilePath.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in generating report : " + ex.Message);
            }
        }

        /// <summary>
        /// This function will save details of report generated in db.
        /// </summary>
        /// <param name="documentSavePath"></param>
        public void SaveInDb(string documentSavePath)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetDoctypeIdByName(cmbTemplateFolders.SelectedItem.ToString());
            int docTypeId = 0;
            if (res.success)
            {
                docTypeId = (int)res.body;
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
                return;
            }
            //int docTypeId = Int32.Parse(dl.GetDoctypeIdByName(cmbTemplateFolders.SelectedItem.ToString()).ToString());
            Trace.WriteLine("Doctype id " + docTypeId);
            DLDocument dlDocument = new DLDocument()
            {
                companyId = getCurrentSelectedCompany().companyId,
                docTypeId = docTypeId,
                documentId = Int32.Parse(txtDocumentSerialNumber.Text),
                documentPath = documentSavePath,
                sender = txtRef.Text
            };
            res = dl.AddDocument(dlDocument);
            if (res.success)
            {
                // document added
                //MessageBox.Show("Document added");
                populateDocumentsDataGrid(cmbTemplateFolders.SelectedItem.ToString());
            }
            else if (res.isException)
            {
                MessageBox.Show("Exception in adding document : " + res.exception);
            }
        }

        /// <summary>
        /// This function will feed update setting value in setting table by taking value from appropriate text field and calling Update setting function from DL.
        /// </summary>
        public void UpdateSettingsTable()
        {
            DataLayer dl = new DataLayer();
            Settings setting = new Settings();
            setting.docRoot = txtDocumentFolderPath.Text;
            setting.refName = txtRef.Text;
            setting.serviceInvoiceNo = txtServiceInvoiceNo.Text;
            setting.taxInvoiceNo = txtTaxInvoiceNo.Text;
            setting.templateRoot = txtTemplateFolderPath.Text;
            Response res = dl.UpdateSettings(setting);
            if (res.success)
            {
                Thread.Sleep(1000);
                Trace.WriteLine("Settings Saved");
                populateSettings();
                //settings updated
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        /// <summary>
        /// It will open file open dialog box, where you can select folder for storing templates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectTemplateFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtTemplateFolderPath.Text = dialog.FileName;
                UpdateSettingsTable();
            }
            populateTemplateFoldersCmb();
        }

        /// <summary>
        /// It will open fileOpenDialog where you can select where generated file has to be saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectDocumentFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtDocumentFolderPath.Text = dialog.FileName;
                UpdateSettingsTable();
            }
            PopulateFilePaths();
        }

        /// <summary>
        /// It will save information in settings table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            UpdateSettingsTable();
        }

        /// <summary>
        /// It will give todays, date to date text field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDate_Click(object sender, RoutedEventArgs e)
        {
            txtDate.Text = DateTime.Now.ToString();
            populateSettings();
        }

        /// <summary>
        /// It will open template folder path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenTemplateFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(txtTemplateFolderPath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// It will redirect to next window, where you can create new template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateNewTemplate_Click(object sender, RoutedEventArgs e)
        {
            FieldsCreator fieldsCreator = new FieldsCreator();
            fieldsCreator.Show();
            fieldsCreator.Closed += fieldWindowClose;
        }

        /// <summary>
        /// It will trigger when we close fields window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void fieldWindowClose(object sender, System.EventArgs eventArgs)
        {
            populateTemplateFoldersCmb();
        }

        /// <summary>
        /// It will redirect to window from which we can test our data layer functions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDataLayerTestClick(object sender, RoutedEventArgs e)
        {
            DataLayerTest dlTest = new DataLayerTest();
            dlTest.Show();
        }

        /// <summary>
        /// This function will automatically fill state, pincode, country and stateCode according to city name typed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCityFocusChanged_Click(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetCityByName(txtCity.Text);
            if (res.success)
            {
                City city = (City)res.body;
                selectStateByStateName(city.state); 
                //txtState.Text = city.state;
                txtPincode.Text = city.pincode;
                txtPhone.Text = city.stdCode;
                txtCountry.Text = city.country;
                int stateCode = getSelectedState().stateCode;
                if (stateCode != 0) txtStateCode.Text = getStateCode();
            }
            else if (res.isException)
            {
                //MessageBox.Show("Exception while getting cities " + res.exception);
                // maybe city name does not exist in database
            }
        }

        private string getStateCode(string state)
        {
            string stateCode = "";
            DataLayer dl = new DataLayer();
            Response res = dl.GetStateCodeByName(state);
            if (res.success)
            {
                stateCode = (string)res.body;
            }
            else if (res.isException)
            {
                // state_code not found, maybe state name is not exist in database
            }
            return stateCode;
        }

        /// <summary>
        /// It will redirect to manage cities dailog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManageCities_Click(object sender, RoutedEventArgs e)
        {
            ManageCities manageCities = new ManageCities();
            manageCities.ShowDialog();
        }

        /// <summary>
        /// It will take values from contact fields and save contact detais by calling function from DL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveContactDetails_Click(object sender, RoutedEventArgs e)
        {
            if (gridContacts.SelectedIndex == -1) return;
            Contact contact = new Contact();
            contact.contactId = ((Contact)gridContacts.SelectedItem).contactId;
            contact.contact_name = txtContactName.Text;
            contact.contact_email = txtContactEmail.Text;
            contact.contact_phone = txtContactPhone.Text;

            DataLayer dl = new DataLayer();
            Response res = dl.EditContact(contact);
            if (res.success)
            {
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                lblContactStatus.Content = "Saving...";
                Thread.Sleep(1000);
                lblContactStatus.Content = "Saved";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        /// <summary>
        /// It will firstly ask to confirm deleting contact details, then it will delete contact details by calling DL function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteContactDetails_Click(object sender, RoutedEventArgs e)
        {
            if (gridContacts.SelectedIndex == -1) return;
            int contactId = ((Contact)gridContacts.SelectedItem).contactId;

            DataLayer dl = new DataLayer();
            Response res = dl.DeleteContact(contactId);
            if (res.success)
            {
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                lblContactStatus.Content = "Deleting...";
                Thread.Sleep(1000);
                lblContactStatus.Content = "Deleted";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }

        }

        /// <summary>
        /// It will fill text fields according to selection changed in data grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                if (gridContacts.SelectedIndex == -1) return;
                Contact contact = (Contact)gridContacts.SelectedItem;
                txtContactName.Text = contact.contact_name;
                txtContactEmail.Text = contact.contact_email;
                txtContactPhone.Text = contact.contact_phone;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception in contact selection changed " + exception.Message);
            }
        }

        /// <summary>
        /// This will run first time when grid contact initialized, and we have to hide some contact from our object, so we use this event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridContacts_Loaded(object sender, RoutedEventArgs e)
        {
            if (gridContacts != null && gridContacts.Columns.Count > 0)
            {
                gridContacts.Columns[0].Visibility = Visibility.Collapsed;
                gridContacts.Columns[1].Visibility = Visibility.Collapsed;
                Utility.MakeAllColumnsWidthSame(gridContacts);
                if(gridContacts.Columns.Count>=3) gridContacts.Columns[3].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            }
        }

        /// <summary>
        /// It will take contact details from text fields and add to contacts table by calling appropriate DL function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }
            Contact contact = new Contact();
            contact.companyId = ((Company)cmbCompanies.SelectedItem).companyId;
            contact.contact_name = txtContactName.Text;
            contact.contact_email = txtContactEmail.Text;
            contact.contact_phone = txtContactPhone.Text;

            DataLayer dl = new DataLayer();
            Response res = dl.AddContact(contact);
            Trace.WriteLine(res.success);
            Trace.WriteLine(res.isException);
            if (res.success)
            {
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                lblContactStatus.Content = "Adding...";
                Thread.Sleep(1000);
                lblContactStatus.Content = "Added";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        /// <summary>
        /// It will populate Document data grid, by taking values by calling appropriate DL function
        /// </summary>
        /// <param name="docTypeName"></param>
        private async void populateDocumentsDataGrid(string docTypeName)
        {
            await Task.Delay(1000);
            DataLayer dl = new DataLayer();
            //Trace.WriteLine(cmbTemplateFolders.SelectedItem.ToString());
            Response res = dl.GetDocuementsByDocType(docTypeName);
            if (res.success)
            {
                List<Document> documents = (List<Document>)res.body;
                gridDocuments.ItemsSource = documents;
                Utility.MakeAllColumnsWidthSame(gridDocuments);
                if (gridDocuments.Columns.Count > 3)
                {
                    gridDocuments.Columns[3].Width = new DataGridLength(3, DataGridLengthUnitType.Star);
                }
                if (documents != null && documents.Count > 0)
                {
                    txtDocumentSerialNumber.Text = (documents[0].SNo + 1).ToString();
                }
                else txtDocumentSerialNumber.Text = "1";
                //gridDocuments.Columns[1].Visibility = Visibility.Collapsed;
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        /// <summary>
        /// This will give us access to give different name as what we were given name in poco 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridDocuments_AutoGeneratingColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor)
            {
                e.Column.Header = descriptor.DisplayName ?? descriptor.Name;
            }
        }

        private void gridContacts_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor)
            {
                e.Column.Header = descriptor.DisplayName ?? descriptor.Name;
            }
        }

        /// <summary>
        /// It will trigger when document table is populated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridDocuments_Loaded(object sender, RoutedEventArgs e)
        {
            Utility.MakeAllColumnsWidthSame(gridDocuments);
            if (gridDocuments.Columns.Count > 3)
            {
                gridDocuments.Columns[3].Width = new DataGridLength(3, DataGridLengthUnitType.Star);
            }
        }

        private void txtDocumentSerialNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopulateFilePaths();
        }

        /// <summary>
        /// It will delete currently selected document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            if (gridDocuments.SelectedIndex == -1 || cmbTemplateFolders.SelectedIndex == -1)
            {
                MessageBox.Show("No Document selected, select a document which you want to delete.");
                return;
            }
            Document document = (Document)gridDocuments.SelectedItem;
            Response res = dl.DeleteDocument(document.SNo, cmbTemplateFolders.SelectedItem.ToString());
            if (res.success)
            {
                // document deleted
                populateDocumentsDataGrid(cmbTemplateFolders.SelectedItem.ToString());

            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        /// <summary>
        /// It will open document folder, where our generated file will be saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenDocumentFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(txtDocumentFolderPath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// It will show service invoice window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowServiceInvoiceWindow_Click(object sender,RoutedEventArgs e)
        {
            ServiceInvoiceWindow serviceInvoiceWindow = new ServiceInvoiceWindow();
            serviceInvoiceWindow.Show();
        }

        /// <summary>
        /// it will take currently selected state value and set stateCode accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (getSelectedState() == null) return;
            txtStateCode.Text = getStateCode();
        }

        private string getStateCode()
        {
            string stateCode = getSelectedState().stateCode.ToString();
            if (stateCode.Length == 1) stateCode = "0" + stateCode;
            return stateCode;
        }

        /// <summary>
        /// It will open currently selected document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenDocumentFile_Click(object sender,RoutedEventArgs e)
        {
            // document file is made with these elements
            // documentFolderPath\companyName\ documentName+SerialNumber+SenderName.xlsx
            try
            {
                if (cmbTemplateFolders.SelectedIndex == -1) throw new DAOException("Please select template folder !");
                if (gridDocuments.SelectedIndex == -1) throw new DAOException("Please select document first !");
                Document document = (Document)gridDocuments.SelectedItem;
                string docTypeName = cmbTemplateFolders.SelectedItem.ToString();
                DataLayer dl = new DataLayer();
                Response res = dl.getDocumentFilePath(document, docTypeName);
                if(res.success)
                {
                    // document opened
                    string documentFilePath = res.body.ToString();
                    Process.Start(documentFilePath);
                }
                else if(res.isException)
                {
                    MessageBox.Show("Exception while opening document : " + res.exception);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    
 
}
