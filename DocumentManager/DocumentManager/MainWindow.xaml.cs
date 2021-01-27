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
using System.ComponentModel;

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
        //private string documentFolderPath = @"E:\Vinay\Training\Document Manager\DocMgr Files\";
        //private string saveFolderPath= @"E:\Vinay\Training\Document Manager\Reports\";
        //private string _SerialNumber = "S025";
        //private string _ReferenceNumber = "B";
        private AddressBuffer addressBuffer;
        public MainWindow()
        {
            InitializeComponent();
            populateCompaniesCmb();
            populateSettings();
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            setCompanyViewMode();
            setViewAddressMode();
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
                    //for (int i = 0; i < companies.Count; i++) Trace.WriteLine(companies[i].companyName);
                    cmbCompanies.ItemsSource = companies;
                    cmbCompanies.DisplayMemberPath = "companyName";
                    cmbCompanies.SelectionChanged += cmbCompaniesSelectionChanged;
                    cmbCompanies.SelectedIndex = 1;
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
        private void cmbCompaniesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Response res;
            DataLayer dl = new DataLayer();
            try
            {
                if (cmbCompanies.SelectedIndex == -1) return;
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
                setAllColumnsOfSameWidth();

                if (contacts.Count > 0) gridContacts.SelectedIndex = 0;
            }
            else if (res.isException)
            {
                MessageBox.Show("Point 3 : " + res.exception);
            }
        }
        private Company getCurrentSelectedCompany()
        {
            return (Company)cmbCompanies.SelectedItem;
        }

        public void populateSettings()
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetSettings();
            if(res.success)
            {
                Settings settings = (Settings)res.body;
                txtTemplateFolderPath.Text = settings.templateRoot;
                txtRef.Text = settings.refName;
                txtDocumentFolderPath.Text = settings.docRoot;
                populateTemplateFoldersCmb();
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
        private void PopulateFilePaths()
        {
            if (cmbTemplateFolders.SelectedIndex == -1 || cmbTemplateFiles.SelectedIndex == -1 || getCurrentSelectedCompany()==null) return;
            txtTemplateFilePath.Text = txtTemplateFolderPath.Text + "\\"+ cmbTemplateFolders.SelectedItem.ToString() + "\\" + cmbTemplateFiles.SelectedItem.ToString();
            FileInfo fileInfo = new FileInfo(txtTemplateFilePath.Text);

            txtSaveFilePath.Text = txtDocumentFolderPath.Text + "\\" + getCurrentSelectedCompany().companyName + "\\" + cmbTemplateFolders.SelectedItem.ToString() + txtDocumentSerialNumber.Text + txtRef.Text + fileInfo.Extension ;
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
            if (btn.Content.ToString() == "Add")
            {
                btn.Content = "OK";
                setCompanyAddMode();
            }
            else
            {

                if(txtCompanyName.Text=="" || txtCompanyName.Text.Length==0)
                {
                    MessageBox.Show("Company name cannot be empty");
                    setCompanyAddMode();
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
                    btn.Content = "Add";
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
                if (txtDocumentSerialNumber.Text == "" || !Utility.IsNumber(txtDocumentSerialNumber.Text))
                {
                    MessageBox.Show("Please give valid serial number");
                    return;
                }
                string refName= txtRef.Text;
                Contact contact = (Contact)gridContacts.SelectedItem;
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
                    state = txtState.Text,
                    stateCode = (txtStateCode.Text == "") ? 0 : int.Parse(txtStateCode.Text),
                    contactName = contact.contact_name,
                    contactEmail = contact.contact_email,
                    contactPhone = contact.contact_phone,
                    gstNo = txtGSTNo.Text,
                };
                ReportProcessor rp = new ReportProcessor();
                string saveFolder=txtDocumentFolderPath.Text+"\\"+getCurrentSelectedCompany().companyName;

                if(File.Exists(txtSaveFilePath.Text))
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
                Trace.WriteLine("Report Generated");
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
        private void SaveInDb(string documentSavePath)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetDoctypeIdByName(cmbTemplateFolders.SelectedItem.ToString());
            int docTypeId = 0;
            if (res.success)
            {
                docTypeId = (int)res.body;
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
                return;
            }
            //int docTypeId = Int32.Parse(dl.GetDoctypeIdByName(cmbTemplateFolders.SelectedItem.ToString()).ToString());
            Trace.WriteLine("Doctype id "+docTypeId);
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
                MessageBox.Show("Exception in adding document : "+res.exception);
            }
        }
        private void UpdateSettingsTable()
        {
            DataLayer dl = new DataLayer();
            Settings setting = new Settings();
            setting.docRoot = txtDocumentFolderPath.Text;
            setting.refName = txtRef.Text;
            Trace.WriteLine(setting.refName);
            setting.templateRoot = txtTemplateFolderPath.Text;
            Response res = dl.UpdateSettings(setting);
            if(res.success)
            {
                Trace.WriteLine("Settings Saved");
                //settings updated
            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
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

        private void btnSelectDocumentFolder_Click(object sender,RoutedEventArgs e)
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

        private void btnSaveSettings_Click(object sender,RoutedEventArgs e)
        {
            UpdateSettingsTable();
        }

        private void RefreshDate_Click(object sender, RoutedEventArgs e)
        {
            txtDate.Text = DateTime.Now.ToString();
        }

        private void btnOpenTemplateFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(txtTemplateFolderPath.Text);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            gridContacts.Columns[0].Visibility = Visibility.Collapsed;
            gridContacts.Columns[1].Visibility = Visibility.Collapsed;
            setAllColumnsOfSameWidth();
        }
        private void setAllColumnsOfSameWidth()
        {
            foreach (var column in gridContacts.Columns)
            {
                column.MinWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
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

        private async void populateDocumentsDataGrid(string docTypeName)
        {
            await Task.Delay(1000);
            DataLayer dl = new DataLayer();
            //Trace.WriteLine(cmbTemplateFolders.SelectedItem.ToString());
            Response res = dl.GetDocuementsByDocType(docTypeName);
            if(res.success)
            {
                List<Document> documents = (List<Document>)res.body;
                gridDocuments.ItemsSource = documents;
                setAllColumnsOfSameWidth();
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
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

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

        private void gridDocuments_Loaded(object sender, RoutedEventArgs e)
        {
            setAllColumnsOfSameWidth();
            if (gridDocuments.Columns.Count > 3)
            {
                gridDocuments.Columns[3].Width = new DataGridLength(3, DataGridLengthUnitType.Star);
            }
        }

        private void txtDocumentSerialNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopulateFilePaths();
        }

        private void btnDeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            if(gridDocuments.SelectedIndex==-1 || cmbTemplateFolders.SelectedIndex==-1)
            {
                MessageBox.Show("No Document selected, select a document which you want to delete.");
                return;
            }
            Document document = (Document)gridDocuments.SelectedItem;
            Response res = dl.DeleteDocument(document.SNo, cmbTemplateFolders.SelectedItem.ToString());
            if(res.success)
            {
                // document deleted
                populateDocumentsDataGrid(cmbTemplateFolders.SelectedItem.ToString());

            }
            else if(res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void btnOpenDocumentFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(txtDocumentFolderPath.Text);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        public string gstNo { set; get; }
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
            string extension = Utility.GetExtension(reportName);
            if (extension == ".xlsx") workbook = new XSSFWorkbook(fileStream);
            else if (extension == ".xls")
            {
                throw new DAOException(extension + " files are not supported. Please change them to .xlsx file format. \n Thanks");
            }
            else throw new DAOException("File format " + extension + " not supported.");
            ISheet sheet = null;
            while (reader.Read())
            {
                if (sheet == null) sheet = workbook.GetSheet(reader["field_sheet"].ToString());
                IRow currRow = sheet.GetRow(int.Parse(reader["field_row"].ToString()) - 1);
                ICell cell = currRow.GetCell(int.Parse(reader["field_column"].ToString()) - 1);
                SetValue(cell, reader["field_name"].ToString(), field);
            }
            using (var fileData = new FileStream(saveFileName, FileMode.Create))
            {
                workbook.Write(fileData);
                workbook.Close();
            }
            //string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Solution.xls");
            connection.Close();
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
            if (fieldName.Equals("State Code")) cell.SetCellValue(field.stateCode);
            if (fieldName.Equals("GST No")) cell.SetCellValue(field.gstNo);
            // Add for contact email also if required
            //if (fieldName.Equals("State Code")) cell.SetCellValue(field.stateCode);
        }
    }

    
 
}
