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
using System.Data;
using System.Data.OleDb;
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

namespace DocumentManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OleDbConnection connection = null;
        private OleDbCommand command = null;
        private OleDbDataReader reader = null;
        private string documentFolderPath = @"E:\Vinay\Training\Document Manager\DocMgr Files\";
        private string saveFolderPath= @"E:\Vinay\Training\Document Manager\Reports\";
        private string _SerialNumber = "S025";
        private string _ReferenceNumber = "B";
        private AddressBuffer addressBuffer;
        public MainWindow()
        {
            InitializeComponent();
            populateCompaniesCmb();
            txtFolderPath.Text = documentFolderPath;
            populateTemplateFoldersCmb();
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            setCompanyViewMode();
            setViewAddressMode();
            cmbCompanies.SelectedIndex = 1;
        }
        public void setCompanyAddMode()
        {
            cmbCompanies.Visibility = Visibility.Collapsed;
            txtCompanyName.Visibility = Visibility.Visible;
            txtCompanyName.Text = "";
            btnEditCompany.IsEnabled = false;
            btnDeleteCompany.IsEnabled = false;
            btnCancelCompany.IsEnabled = true;
        }
        public void setCompanyEditMode()
        {
            cmbCompanies.Visibility = Visibility.Collapsed;
            txtCompanyName.Visibility = Visibility.Visible;
            txtCompanyName.Text = ((Company)cmbCompanies.SelectedItem).companyName;
            btnAddCompany.IsEnabled = false;
            btnDeleteCompany.IsEnabled = false;
            btnCancelCompany.IsEnabled = true;
        }
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
        private void btnAddCompanyDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            if (btn.Content == "Add")
            {
                btn.Content = "OK";
                setCompanyAddMode();
            }
            else
            {
                btn.Content = "Add";
                if(txtCompanyName.Text=="" || txtCompanyName.Text.Length==0)
                {
                    MessageBox.Show("Company name cannot be empty");
                    setCompanyEditMode();
                    return;
                }
                DataLayer dl = new DataLayer();
                Company company = new Company();
                company.companyName = txtCompanyName.Text;
                Response res = dl.AddNewCompany(company);
                if(res.success)
                {
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                    setCompanyViewMode();
                }
                else if(res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }
        private void btnEditCompanyDetails_Click(object sender,RoutedEventArgs e)
        {
            if (cmbCompanies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }
            Button btn = e.Source as Button;
            if (btn.Content == "Edit")
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
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }
        private void btnRefreshCompaniesCmb(object sender,RoutedEventArgs e)
        {
            populateCompaniesCmb();
        }
        private void btnDeleteCompanyDetails_Click(object sender,RoutedEventArgs e)
        {
            if(cmbCompanies.SelectedIndex==-1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }
            
            MessageBoxResult result= MessageBox.Show("Confirm Delete : " + ((Company)cmbCompanies.SelectedItem).companyName,"Delete",MessageBoxButton.YesNo);
            if(result==MessageBoxResult.Yes)
            {
                DataLayer dl = new DataLayer();
                Response res=dl.DeleteCompany(((Company)cmbCompanies.SelectedItem).companyId);
                if(res.success)
                {
                    //company deleted
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                }
                else if(res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }
        private void btnCancelCompany_Click(object sender, RoutedEventArgs e)
        {
        
            setCompanyViewMode();
        }

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
        private void btnAddAddressDetails_Click(object sender, RoutedEventArgs e)
        {
            Button b = e.Source as Button;
            if (b.Content.ToString() == "Add")
            {
                setAddAddressMode();
            }
            else if(b.Content.ToString()=="Ok")
            {
                DataLayer dl = new DataLayer();
                Address address = new Address();
                address.address1 = txtAddress1.Text;
                address.address2 = txtAddress2.Text;
                address.address3 = txtAddress3.Text;
                address.city = txtCity.Text;
                address.GSTNo = txtGSTNo.Text;
                address.phone = txtPhone.Text;
                address.pincode = txtPincode.Text;
                address.state = txtState.Text;
                if(txtStateCode.Text.Trim()!="") address.stateCode = Int32.Parse(txtStateCode.Text);
                address.companyID = ((Company)cmbCompanies.SelectedItem).companyId;
                Response res=dl.AddAddress(address);
                if(res.success)
                {
                    // address added
                    Thread.Sleep(1000);
                    setAddressComponent(getCurrentSelectedCompany().companyId);
                    setViewAddressMode();
                }
                else if(res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }
        private void btnSaveAddressDetails_Click(object sender,RoutedEventArgs e)
        {
            if(addressBuffer==null)
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
                state = txtState.Text,
                pincode = txtPincode.Text,
                phone = txtPhone.Text,
                GSTNo = txtGSTNo.Text
            };
            //stateCode = (txtStateCode.Text == "") ? 0 : Int32.Parse(txtStateCode.Text);
            if (txtStateCode.Text.Trim() != "") newAddress.stateCode = Int32.Parse(txtStateCode.Text);
            newAddress.addressID = oldAddress.addressID;
            DataLayer dl = new DataLayer();
            Response res = dl.EditAddress(newAddress);
            if(res.success)
            {
                // address edited / saved successfully
                Thread.Sleep(1000);
                setAddressComponent(getCurrentSelectedCompany().companyId);
            }
            else if(res.isException)
            {
                MessageBox.Show("Exception in saving address : " + res.exception);
            }
        }

        private void btnDeleteAddress_Click(object sender,RoutedEventArgs e)
        {
            Address address = addressBuffer.GetCurrentAddress();
            MessageBoxResult result=MessageBox.Show("Confirm Delete whole address line 1 is " + address.address1,"Delete Address",MessageBoxButton.YesNo);
            if(result==MessageBoxResult.Yes)
            {
                DataLayer dl = new DataLayer();
                Response res=dl.DeleteAddress(address.addressID);
                if(res.success)
                {
                    // address deleted
                    Thread.Sleep(2000);
                    setAddressComponent(getCurrentSelectedCompany().companyId);
                    setAddressPanel();
                   
                }
                else if(res.isException)
                {
                    MessageBox.Show("Exception while deleting address : " + res.exception);
                }
            }
        }

        private void btnAddCity_Click(object sender, RoutedEventArgs e)
        {
            City city = new City();
            city.cityName = txtCity.Text;
            city.state = txtState.Text;
            city.pincode = txtPincode.Text;
            city.stdCode = txtPhone.Text;

            DataLayer dl = new DataLayer();
            Response res = dl.AddCity(city);
            if(res.success)
            {
                // city addded
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void populateTemplateFoldersCmb()
        {
            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(txtFolderPath.Text.ToString());

                cmbTemplateFolders.Items.Clear();

                // Loop through them to see if they have any other subdirectories
                foreach (string subdirectory in subdirectoryEntries)
                {
                    string folderName=System.IO.Path.GetFileName(subdirectory);
                    cmbTemplateFolders.Items.Add(folderName);
                }
                cmbTemplateFolders.SelectionChanged += cmbTemplateSelectionChanged;
                if (cmbTemplateFolders.SelectedIndex == -1) cmbTemplateFolders.SelectedIndex = 0;
            }
            catch (DAOException ex)
            {
                MessageBox.Show("Exception : Populate Companies : " + ex.Message);
            }

        }

        private void cmbTemplateSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(cmbTemplateFolders.SelectedItem==null)
                {
                    cmbTemplateFiles.Items.Clear();
                    return;
                }
                String folderName = cmbTemplateFolders.SelectedItem.ToString();
                string[] fileEntries = Directory.GetFiles(documentFolderPath + folderName);
                cmbTemplateFiles.Items.Clear();
                foreach (var file_name in fileEntries)
                {
                    cmbTemplateFiles.Items.Add(System.IO.Path.GetFileName(file_name));
                }
                cmbTemplateFiles.SelectedIndex = 0;
            }catch(NullReferenceException nre)
            {
                MessageBox.Show(nre.Message);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

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

        private Company getCurrentSelectedCompany()
        {
            return (Company)cmbCompanies.SelectedItem;
        }
        private void cmbCompaniesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Response res;
            DataLayer dl = new DataLayer();
            try
            {
                if (cmbCompanies.SelectedIndex == -1) return;
                Company company = (Company) cmbCompanies.SelectedItem;
                setAddressComponent(company.companyId);
                populateContacts(company.companyId);
            }
            catch(Exception exception)
            {
                MessageBox.Show("Exception while selecting company " + exception);
            }
        }
        private void populateContacts(int companyId)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetContactsByCompanyId(companyId);
            if (res.success)
            {
                List<Contact> contacts = (List<Contact>)res.body;
                gridContacts.ItemsSource = contacts;
                if(contacts.Count>0) gridContacts.SelectedIndex = 0;
            }
            else if (res.isException)
            {
                MessageBox.Show("Point 3 : " + res.exception);
            }
        }
        private void emptyAllAddressFields()
        {
            txtAddress1.Text = "";
            txtAddress2.Text = "";
            txtAddress3.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtPincode.Text = "";
            txtPhone.Text = "";
            txtGSTNo.Text = "";
            txtStateCode.Text = "0";
        }
        private void FillAddress(Address address)
        {
            txtAddress1.Text = address.address1;
            txtAddress2.Text = address.address2;
            txtAddress3.Text = address.address3;
            txtCity.Text = address.city;
            txtState.Text = address.state;
            txtPincode.Text = address.pincode;
            txtPhone.Text = address.phone;
            txtGSTNo.Text = address.GSTNo;
            txtStateCode.Text = address.stateCode.ToString();
        }

        public void btnPrevAddress_Click(object sender, RoutedEventArgs e)
        {
            // It will show previous address and change value on txtAddressInfo
            try
            {
                if (addressBuffer!=null && addressBuffer.canGetPrev())
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
        public void btnNextAddress_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                if (addressBuffer!=null && addressBuffer.canGetNext())
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
        private void setAddressPanel()
        {
            if (addressBuffer == null || addressBuffer.GetSize()==0)
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
                    for (int i = 0; i < companies.Count; i++) Trace.WriteLine(companies[i].companyName);
                    cmbCompanies.ItemsSource = companies;
                    cmbCompanies.DisplayMemberPath = "companyName";
                    cmbCompanies.SelectionChanged += cmbCompaniesSelectionChanged;
                }
                else if(res.isException)
                {
                    MessageBox.Show("Exception while populating companies : " + res.exception);
                }
            }catch(Exception exception)
            {
                MessageBox.Show("Exception in populate companies cmb : " + exception);
            }
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //DataRowView row = (DataRowView)gridContacts.SelectedItem;
                if (gridContacts.SelectedIndex==-1 || cmbCompanies.SelectedIndex==-1)
                {
                    MessageBox.Show("Company/Contact Not Selected");
                    return;
                }
                if (cmbTemplateFolders.SelectedIndex == -1 || cmbTemplateFiles.SelectedIndex == -1)
                {
                    MessageBox.Show("Folder or file is not selected");
                    return;
                }
                Contact contact = (Contact)gridContacts.SelectedItem;
                Field field = new Field()
                {
                    refNo = _ReferenceNumber,
                    companyName = cmbCompanies.SelectedItem.ToString(),
                    address1 = txtAddress1.Text,
                    address2 = txtAddress2.Text,
                    address3 = txtAddress3.Text,
                    city = txtCity.Text,
                    pincode = txtPincode.Text,
                    phone = txtPhone.Text,
                    date = txtDate.Text,
                    state = txtState.Text,
                    stateCode = (txtStateCode.Text=="")?0:int.Parse(txtStateCode.Text),
                    contactName = contact.contact_name,
                    contactEmail = contact.contact_email,
                    contactPhone = contact.contact_phone
                };
                ReportProcessor rp = new ReportProcessor();
                string companyName = cmbCompanies.SelectedItem.ToString();
                if (!Directory.Exists(saveFolderPath + companyName))
                {
                    Directory.CreateDirectory(saveFolderPath + companyName);
                }
                rp.GenerateReport(field, documentFolderPath + cmbTemplateFolders.Text + "\\" + cmbTemplateFiles.Text, saveFolderPath + companyName + "\\" + cmbTemplateFolders.Text + _ReferenceNumber + _SerialNumber + ".xlsx");
                if (btnOpenToggle.IsChecked == true)
                {
                    Process.Start(saveFolderPath +companyName+"\\"+ cmbTemplateFolders.Text + _ReferenceNumber + _SerialNumber + ".xlsx");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in generating report : " + ex.Message);
            }
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtFolderPath.Text = dialog.FileName;
            }
            populateTemplateFoldersCmb();

            //SaveFileDialog fileDialog = new SaveFileDialog()
            //{
            //    Title = "Save As",

            //    CheckFileExists = true,
            //    CheckPathExists = true,

            //    DefaultExt = "xsl",
            //    Filter = "xsl files (*.xsl)|*.xsl",
            //};
            //if (fileDialog.ShowDialog() == true)
            //{
            //    lblSaveFolderPath.Content=fileDialog.FileName;
            //}

        }

        private void RefreshDate_Click(object sender, RoutedEventArgs e)
        {
            txtDate.Text = DateTime.Now.ToString();
        }

        private void btnOpenTemplateFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(documentFolderPath);
            }catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void btnCreateNewTemplate_Click(object sender, RoutedEventArgs e)
        {
            FieldsCreator fieldsCreator = new FieldsCreator();
            fieldsCreator.Show();
            fieldsCreator.Closed += fieldWindowClose;
        }
        private void fieldWindowClose(object sender,System.EventArgs eventArgs)
        {
            populateTemplateFoldersCmb();
        }

        private void btnDataLayerTestClick(object sender, RoutedEventArgs e)
        {
            DataLayerTest dlTest = new DataLayerTest();
            dlTest.Show();
        }

        private void txtCityFocusChanged_Click(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetCityByName(txtCity.Text);
            if(res.success)
            {
                City city = (City)res.body;
                txtState.Text = city.state;
                txtPincode.Text = city.pincode;
                //txtPhone.Text = city.stdCode;
                string stateCode=getStateCode(txtState.Text);
                if (stateCode != "") txtStateCode.Text = stateCode;
            }
            else if(res.isException)
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
            else if(res.isException)
            {
                // state_code not found, maybe state name is not exist in database
            }
            return stateCode;
        }

        private void btnManageCities_Click(object sender, RoutedEventArgs e)
        {
            ManageCities manageCities = new ManageCities();
            manageCities.ShowDialog();
        }
        private void btnSaveContactDetails_Click(object sender,RoutedEventArgs e)
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
                txtContactStatus.Content = "Saving...";
                Thread.Sleep(1000);
                txtContactStatus.Content = "Saved";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
        private void btnDeleteContactDetails_Click(object sender,RoutedEventArgs e)
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
                txtContactStatus.Content = "Deleting...";
                Thread.Sleep(1000);
                txtContactStatus.Content = "Deleted";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }

        }

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
            }catch(Exception exception)
            {
                MessageBox.Show("Exception in contact selection changed " + exception.Message);
            }
        }

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
                txtContactStatus.Content = "Adding...";
                Thread.Sleep(1000);
                txtContactStatus.Content = "Added";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
    }
    public class Field
    {
        public string companyName { set; get; }
        public string address1 { set; get; }
        public string address2 { set; get; }
        public string address3 { set; get; }
        public string city { set; get; }
        public string pincode { set; get; }
        public string phone { set; get; }
        public string state { set; get; }
        public string refNo { set; get; }
        public string date { set; get; }
        public int stateCode { set; get; }
        public string contactName { set; get; }
        public string contactEmail { set; get; }
        public string contactPhone { set; get; }
    }
    public class ReportProcessor
    {
        //string reportName = @"E:\Vinay\Training\Document Manager\DocMgr Files\GST Tax Invoice\GST Tax Invoice 21 Jul 2017.xls";
        //string reportName = @"E:\Vinay\Training\Document Manager\DocMgr Files\Envelop\Envelop.xls";
        OleDbConnection connection = null;
        OleDbCommand command = null;
        OleDbDataReader reader = null;
        public void GenerateReport(Field field, string reportName, string saveFileName)
        {
            try
            {
                string documentName = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(reportName));
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                //string sqlString = "select * from qryAddress where companyName=@NAME";
                command = new OleDbCommand("select * from qryDocumentFields where doctype_name=@DOCUMENT", connection);
                //command.Parameters.AddWithValue("@DOCUMENT", "GST Tax Invoice");
                //Trace.WriteLine("Report Name "+reportName);
                //Trace.WriteLine("Directory name "+System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(reportName)));
                command.Parameters.AddWithValue("@DOCUMENT", documentName);
                reader = command.ExecuteReader();
                FileStream fileStream = new FileStream(reportName, FileMode.Open, FileAccess.Read);
                IWorkbook workbook = null;
                if (reportName.IndexOf(".xlsx") > 0) workbook = new XSSFWorkbook(fileStream);
                else if (reportName.IndexOf(".xls") > 0) workbook = new HSSFWorkbook(fileStream);
                ISheet sheet = null;
                while (reader.Read())
                {
                    if (sheet == null) sheet = workbook.GetSheet(reader["field_sheet"].ToString());
                    IRow currRow = sheet.GetRow(int.Parse(reader["field_row"].ToString()) - 1);
                    ICell cell = currRow.GetCell(int.Parse(reader["field_column"].ToString()) - 1);
                    SetValue(cell, reader["field_name"].ToString(), field);
                    //Trace.WriteLine(cell.ToString() + " " + (cell.RowIndex + 1) + " " + (cell.ColumnIndex + 1));
                    //Trace.WriteLine("Field name : " + reader["field_name"]);
                    //Trace.WriteLine("Field row : " + reader["field_row"]);
                    //Trace.WriteLine("Field column : " + reader["field_column"]);
                }
                using (var fileData = new FileStream(saveFileName, FileMode.Create))
                {
                    workbook.Write(fileData);
                    workbook.Close();
                }
                //string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Solution.xls");
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception while generating report : " + ex.Message);
            }
        }
        public void SetValue(ICell cell, string fieldName, Field field)
        {
            if (fieldName.Equals("Ref No")) cell.SetCellValue(field.refNo);
            if (fieldName.Equals("Date")) cell.SetCellValue(field.date);
            if (fieldName.Equals("Company Name")) cell.SetCellValue(field.companyName);
            if (fieldName.Equals("Address 1")) cell.SetCellValue(field.address1);
            if (fieldName.Equals("Address 2")) cell.SetCellValue(field.address2);
            if (fieldName.Equals("Address 3")) cell.SetCellValue(field.address3);
            if (fieldName.Equals("City")) cell.SetCellValue(field.city);
            if (fieldName.Equals("Pincode")) cell.SetCellValue(field.pincode);
            if (fieldName.Equals("Address Block")) cell.SetCellValue(field.address1 + "\n" + field.address2 + "," + field.address3 + "\n" + field.city + " - " + field.pincode + "\n" + field.state);
            if (fieldName.Equals("Phone")) cell.SetCellValue(field.phone);
            if (fieldName.Equals("State")) cell.SetCellValue(field.state);
            if (fieldName.Equals("Contact Name")) cell.SetCellValue(field.contactName);
            if (fieldName.Equals("Contact Phone")) cell.SetCellValue(field.contactPhone);
            // Add for contact email also if required
            //if (fieldName.Equals("State Code")) cell.SetCellValue(field.stateCode);
        }
    }

    
 
}
