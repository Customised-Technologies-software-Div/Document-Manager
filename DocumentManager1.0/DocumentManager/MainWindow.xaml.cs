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
        public MainWindow()
        {
            InitializeComponent();
            populateCompaniesCmb();
            txtFolderPath.Text = documentFolderPath;
            populateTemplateFoldersCmb();
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
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

        private void cmbCompaniesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string companyName = cmbCompanies.SelectedItem.ToString();
            CompanyAddress companyAddress = GetCompanyAddressFromAccessQuery(companyName);
            FillTextBoxWithCompanyAddress(companyAddress);
            populateCompanyContactPersonDetails(companyName);
        }
        public void populateCompanyContactPersonDetails(string companyName)
        {
            connection = DatabaseConnection.GetConnection();
            connection.Open();
            //string sqlString = "select * from qryAddress where companyName=@NAME";

            command = new OleDbCommand("select contact_name,contact_email,contact_phone from qryContacts Query where company_name=@NAME", connection);
            //command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@NAME", companyName);
            ////command.Parameters["@ID"].Value = companyId;
            //Trace.WriteLine(command.CommandText);
            //reader = command.ExecuteReader();
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            gridCompanyContactPersons.ItemsSource = dataTable.DefaultView;
            gridCompanyContactPersons.SelectedIndex = 0;
            connection.Close();
        }
        public CompanyAddress GetCompanyAddressFromAccessQuery(string companyName)
        {
            CompanyAddress companyAddress = null;
            connection = DatabaseConnection.GetConnection();
            connection.Open();
            //string sqlString = "select * from qryAddress where companyName=@NAME";

            command = new OleDbCommand("select * from qryAddress3 Query where company_name=@NAME", connection);
            //command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@NAME", companyName);
            ////command.Parameters["@ID"].Value = companyId;
            //Trace.WriteLine(command.CommandText);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                companyAddress = new CompanyAddress();
                companyAddress.companyName = companyName;
                companyAddress.address1 = reader["address1"].ToString();
                companyAddress.address2 = reader["address2"].ToString();
                companyAddress.address3 = reader["address3"].ToString();
                companyAddress.city = reader["city"].ToString();
                companyAddress.state = reader["state"].ToString();
                companyAddress.pincode = reader["pincode"].ToString();
                companyAddress.phone = reader["phone"].ToString();
                txtStateCode.Text = reader["state_code"].ToString();
            }
            connection.Close();
            // We currently don't know information of GST and stateCode
            return companyAddress;
        }
        private void FillTextBoxWithCompanyAddress(CompanyAddress address)
        {
            txtAddress1.Text = address.address1;
            txtAddress2.Text = address.address2;
            txtAddress3.Text = address.address3;
            txtCity.Text = address.city;
            txtPhone.Text = address.phone;
            txtPincode.Text = address.pincode;
            txtState.Text = address.state;
        }
        private int GetCompanyId(string companyName)
        {
            int companyId = -1;
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                command = new OleDbCommand($"select company_id from companylist where company_name='{companyName}'", connection);
                Trace.WriteLine($"select company_id from companylist where company_name='{companyName}'");
                reader = command.ExecuteReader();
                while(reader.Read())
                {
                    companyId = reader.GetInt32(0);
                }
                connection.Close();
                return companyId;
            }catch(DAOException ex)
            {
                Trace.WriteLine(ex.Message);
                return companyId;
            }
        }
        private CompanyAddress GetCompanyAddress(string companyName)
        {
            CompanyAddress companyAddress = null;
            int companyId = GetCompanyId(companyName);
            connection = DatabaseConnection.GetConnection();
            connection.Open();
            string sqlString = "select * from addresslist where company_id=@ID";

            command = new OleDbCommand(sqlString, connection);
            command.Parameters.AddWithValue("@ID", companyId);
            ////command.Parameters["@ID"].Value = companyId;
            //Trace.WriteLine(command.CommandText);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                companyAddress = new CompanyAddress();
                companyAddress.companyName = companyName;
                companyAddress.address1 = reader["address1"].ToString();
                companyAddress.address2 = reader["address2"].ToString();
                companyAddress.address3 = reader["address3"].ToString();
                companyAddress.city = reader["city"].ToString();
                companyAddress.state = reader["state"].ToString();
                companyAddress.pincode = reader["pincode"].ToString();
                companyAddress.phone = reader["phone"].ToString();
            }
            connection.Close();
            // We currently don't know information of GST and stateCode
            return companyAddress;
        }
        private void populateCompaniesCmb()
        {
            try
            {
                connection = DatabaseConnection.GetConnection();
                command = new OleDbCommand("select company_name from companylist order by company_name ASC", connection);
                connection.Open();
                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbCompanies.Items.Add(reader["company_name"]);
                    }
                }
                cmbCompanies.SelectionChanged += cmbCompaniesSelectionChanged;

                connection.Close();
            }catch(DAOException ex)
            {
                MessageBox.Show("Exception : Populate Companies : " + ex.Message);
            }
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)gridCompanyContactPersons.SelectedItem;

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
                    stateCode = int.Parse(txtStateCode.Text),
                    contactName = row[0].ToString(),
                    contactEmail = row[1].ToString(),
                    contactPhone = row[2].ToString()
                };
                ReportProcessor rp = new ReportProcessor();
                if (cmbTemplateFolders.SelectedIndex == -1 || cmbTemplateFiles.SelectedIndex == -1)
                {
                    MessageBox.Show("Folder or file is not selected");
                    return;
                }
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
            }catch(Exception ex)
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
        private void btnAddCompanyDetails_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Company company = new Company();
            company.companyName = txtCompanyName.Text;
            Response response=dl.AddNewCompany(company);
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
                    else if(res.isException)
                    {
                        // I dont know whether to change button name or not
                        MessageBox.Show("Exception occured " + res.exception);
                    }

                }
            }catch(Exception ex)
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
                int companyId=0;
                if (res.success)
                {
                    Company company = (Company)res.body;
                    companyId= company.companyId;
                }
                else if (res.isException)
                {
                    MessageBox.Show("Exception occured : " + res.exception);
                    return;
                }
                Response res2=dl.DeleteCompany(companyId);
                if(res2.success)
                {
                    MessageBox.Show("Record deleted");
                }
                else if(res2.isException)
                {
                    MessageBox.Show(res2.exception);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void tempBtnGetAll(object sender,RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res= dl.GetAllCompanies();
                if(res.success)
                {
                    List<Company> companies = (List<Company>)res.body;
                    for(int i=0;i<companies.Count;i++)
                    {
                        Trace.WriteLine(i + " " + companies[i].companyId + " " + companies[i].companyName);
                    }
                }
                else if(res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void tempBtnAddAddressClick(object sender,RoutedEventArgs e)
        {
            Response res = null;
            try
            {
                DataLayer dl = new DataLayer();
                Address address = new Address();
                address.addressID = 1135;
                address.companyID=((Company)dl.GetCompanyByCompanyName("vinay2").body).companyId;
                address.address1 = "address12";
                address.address2 = "address22";
                address.address3 = "address32";
                address.city = "mumbai";
                address.state = "Maharastra";
                address.pincode = "123456";
                address.phone = "1234567890";
                address.GSTNo = "123GST";
                res = dl.AddAddress(address);
                //res = dl.EditAddress(address);
                if(res.success)
                {
                    MessageBox.Show("address added");
                }
                else if(res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }catch(Exception exception)
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
                Response res=dl.AddCity(city);
                if(res.success)
                {
                    MessageBox.Show("City added with name " + city.cityName);
                }
                else if(res.isException)
                {
                    MessageBox.Show("Cannot add : "+res.exception);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void tempBtnGetAllCitiesClick(object sender,RoutedEventArgs e)
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res=dl.GetAllCities();
                if (res.success)
                {
                    List<City> cities = (List<City>)res.body;
                    for (int i = 0; i < cities.Count; i++)
                    {
                        Trace.WriteLine(cities[i].cityName + " " + cities[i].pincode + " " + cities[i].state + " " + cities[i].stdCode);
                    }
                }
                else if(res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }catch(Exception ex)
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
                    MessageBox.Show("Contact Added with name : "+contact.contact_name);
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
                int contactId=Int32.Parse(txtCompanyName.Text);
                Response res = dl.DeleteContact(contactId);
                if (res.success)
                {
                    MessageBox.Show("Contact deleted with Id : "+contactId);
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
                int companyId=Int32.Parse(txtCompanyName.Text);
                Response res = dl.GetContactsByCompanyId(companyId);
                if (res.success)
                {
                    List<Contact> contacts = (List<Contact>)res.body;
                    for(int i=0;i<contacts.Count;i++)
                    {
                        Trace.WriteLine(contacts[i].companyId+" "+contacts[i].contactId+" "+contacts[i].contact_name+" "+contacts[i].contact_email+" "+contacts[i].contact_phone);
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

    }
    public class DAOException:Exception
    {
            public DAOException(string message) : base(message)
            {

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
        public void GenerateReport(Field field,string reportName,string saveFileName)
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
        public void SetValue(ICell cell,string fieldName,Field field)
        {
            if(fieldName.Equals("Ref No")) cell.SetCellValue(field.refNo);
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

    public class CompanyAddress
    {
        public string companyName { set; get; }
        public string address1 { set; get; }
        public string address2 { set; get; }
        public string address3 { set; get; }
        public string city { set; get; }
        public string state { set; get; }
        public string pincode { set; get; }
        public string phone { set; get; }
        public string GST_IN { set; get; }
        public string stateCode { set; get; }
        public CompanyAddress()
        {
            this.companyName = "";
            this.address1 = "";
            this.address2 = "";
            this.address3 = "";
            this.city = "";
            this.state = "";
            this.pincode = "";
            this.phone = "";
            this.GST_IN = "";
            this.stateCode = "";
        }
    }
    public class Utility
    {
        public static string ConvertNumbertoWords(int number)
        {
            if (number == 0)
                return "ZERO";
            if (number < 0)
                return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";

            if ((number / 1000000000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000000000) + " Billion ";
                number %= 1000000000;
            }

            if ((number / 10000000) > 0)
            {
                words += ConvertNumbertoWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += ConvertNumbertoWords(number / 100000) + " Lakh ";
                number %= 100000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
                number %= 100;
            }
            if (number > 0)
            {
                if (words != "")
                    words += "AND ";
                var unitsMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
                var tensMap = new[] { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }
    }
    public class Company
    {
        public int companyId { set; get; }
        public string companyName { set; get; }
        public Address address { set; get; }
    }
    public class City
    {
        public string cityName { set; get; }
        public string state { set; get; }
        public string pincode { set; get; }
        public string stdCode { set; get; }
    }
    public class Address
    {
        public int addressID { set; get; }
        public int companyID { set; get; }
        public string address1 { set; get; }
        public string address2 { set; get; }
        public string address3 { set; get; }
        public string city { set; get; }
        public string state { set; get; }
        public string pincode { set; get; }
        public string stateCode { set; get; }
        public string phone { set; get; }
        public string GSTNo { set; get; }
    }
    public class Contact
    {
        public int contactId { set; get; }
        public int companyId { set; get; }
        public string contact_name { set; get; }
        public string contact_email { set; get; }
        public string contact_phone { set; get; }
    }
    public class Response
    {
        public bool success { set; get; }
        public bool isException { set; get; }
        public string exception { set; get; }
        public object body { set; get; }
    }


    public class DataLayer
    {
        public OleDbConnection connection = null;
        public OleDbCommand command = null;
        public OleDbDataReader reader = null;
        public Response AddNewCompany(Company company)
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                // check if company name exist or not
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select company_id from companylist where company_name=@COMPANY_NAME";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@COMPANY_NAME", company.companyName);
                reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    throw new DAOException("Company name already exist");
                }
                sqlString = "Insert into companyList(company_name) values(@COMPANY_NAME)";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@COMPNAY_NAME", company.companyName);
                reader = command.ExecuteReader();
                if (reader.RecordsAffected == 1)
                {
                    res.success = true;
                }
                else
                {
                    res.success = false;
                    res.exception = "Some error occured, try closing window, contact admin";
                }
            }catch(DAOException exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = exception.Message;
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }

            return res;
        }
        public Response EditCompany(Company company)
        {
            // check company name exist
            // update companyName with use of given Id
            Response res = new Response();
            string sqlString = "";
            try
            {
                // check if company name exist or not
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from companylist where company_NAME=@COMPANY_NAME";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@COMPANY_NAME", company.companyName);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    throw new DAOException("Company name already exist");
                }
                connection.Close();
                connection.Open();
                sqlString = "Update companylist set company_name=@1 where company_id=@2;";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", company.companyName);
                command.Parameters.AddWithValue("@2", company.companyId);
                int recordsUpdated = command.ExecuteNonQuery();
                if (recordsUpdated > 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, or contact admin";
                }
            }
            catch (DAOException exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = exception.Message;
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;

        }
        public Response DeleteCompany(int companyId)
        {
            // check if company exist, if not throw exception
            // delete the company name
            // it will automatically delete entry where company_id used like from contact list and address list
            // it will automatically delete record from these two database when we delete entry from companylist
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from companylist where company_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", companyId);
                reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    throw new DAOException("Company does not exist");
                }
                sqlString = "delete from companylist where company_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", companyId);
                int recordsDeleted = command.ExecuteNonQuery();
                MessageBox.Show(recordsDeleted+"");
                if (recordsDeleted > 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, or contact admin";
                }
            }
            catch (DAOException exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = exception.Message;
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response GetCompanyByCompanyName(string companyName)
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from companylist where company_name=@COMPANY_NAME";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@COMPANY_NAME", companyName);
                reader = command.ExecuteReader();
                reader.Read();
                Company company2 = new Company();
                company2.companyId = Int32.Parse(reader["company_id"].ToString());
                company2.companyName = reader["company_name"].ToString();
                res.success = true;
                res.isException = false;
                res.body = company2;
            }
            catch (DAOException ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            catch(Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response GetAllCompanies()
        {
            // here we send response as list<string> in return, when success status is true
            Response res = new Response();
            string sqlString = "";
            try
            {
                List<Company> companies = new List<Company>();
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from companylist";
                command = new OleDbCommand(sqlString, connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Company company = new Company();
                    company.companyId = Int32.Parse(reader["company_id"].ToString());
                    company.companyName = reader["company_name"].ToString();
                    companies.Add(company);
                }
                res.success = true;
                res.isException = false;
                res.body = companies;

            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response AddAddress(Address address) // decide to put addressID in Address pojo or not
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "Insert into addresslist(company_id,address1,address2,address3,city,state,pincode,phone,GSTNo) values(@1,@2,@3,@4,@5,@6,@7,@8,@9)";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", address.companyID);
                command.Parameters.AddWithValue("@2", address.address1);
                command.Parameters.AddWithValue("@3", address.address2);
                command.Parameters.AddWithValue("@4", address.address3);
                command.Parameters.AddWithValue("@5", address.city);
                command.Parameters.AddWithValue("@6", address.state);
                command.Parameters.AddWithValue("@7", address.pincode);
                command.Parameters.AddWithValue("@8", address.phone);
                command.Parameters.AddWithValue("@9", address.GSTNo);

                reader = command.ExecuteReader();
                if (reader.RecordsAffected == 1)
                {
                    res.success = true;
                }
                else
                {
                    res.success = false;
                    res.exception = "Some error occured, try closing window, contact admin";
                }
            }
            catch (Exception exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = "Add address exception : "+exception.Message;
            }
            return res;
        }
        public Response EditAddress(Address address) // here maybe we need address ID
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                // check if company name exist or not
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "Update addressList set   address1=@1 ,address2=@2,address3=@3,city=@4,state=@5,pincode=@6,phone=@7,GSTNo=@8 where address_id=@9";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", address.address1);
                command.Parameters.AddWithValue("@2", address.address2);
                command.Parameters.AddWithValue("@3", address.address3);
                command.Parameters.AddWithValue("@4", address.city);
                command.Parameters.AddWithValue("@5", address.state);
                command.Parameters.AddWithValue("@6", address.pincode);
                command.Parameters.AddWithValue("@7", address.phone);
                command.Parameters.AddWithValue("@8", address.GSTNo);
                command.Parameters.AddWithValue("@9", address.addressID);

                int recordsUpdated = command.ExecuteNonQuery();
                if (recordsUpdated > 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, or contact admin";
                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response DeleteAddress(int addressID)
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from addresslist where address_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", addressID);
                reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    throw new DAOException("Address does not exist");
                }
                sqlString = "delete from addresslist where address_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", addressID);
                int recordsDeleted = command.ExecuteNonQuery();
                if (recordsDeleted > 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, or contact admin";
                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response GetAddressByCompanyId(int companyId) // retrun list of address
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                List<Address> addresses = new List<Address>();
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from addressList where company_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", companyId);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Address address = new Address()
                    {
                        addressID = Int32.Parse(reader["address_id"].ToString()),
                        address1 = reader["address1"].ToString(),
                        address2 = reader["address2"].ToString(),
                        address3 = reader["address3"].ToString(),
                        city = reader["city"].ToString(),
                        state = reader["state"].ToString(),
                        pincode = reader["pincode"].ToString(),
                        phone = reader["phone"].ToString(),
                        GSTNo = reader["GSTNo"].ToString()
                    };
                    addresses.Add(address);
                }
                res.success = true;
                res.isException = false;
                res.body = addresses;
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;

        }
        public Response GetAddressById(int addressID) // response will be object of address
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from addressList where address_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", addressID);
                reader = command.ExecuteReader();
                reader.Read();
                Address address = new Address()
                {
                    addressID = Int32.Parse(reader["address_id"].ToString()),
                    address1 = reader["address1"].ToString(),
                    address2 = reader["address2"].ToString(),
                    address3 = reader["address3"].ToString(),
                    city = reader["city"].ToString(),
                    state = reader["state"].ToString(),
                    pincode = reader["pincode"].ToString(),
                    phone = reader["phone"].ToString(),
                    GSTNo = reader["GSTNo"].ToString()
                };
                res.success = true;
                res.isException = false;
                res.body = address;
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }

            return res;
        }
        public Response AddCity(City city)
        {
            Response res = new Response();
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                string sqlString = "insert into cities(city,state,pincode,stdcode) values(@1,@2,@3,@4)";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", city.cityName);
                command.Parameters.AddWithValue("@2", city.state);
                command.Parameters.AddWithValue("@3", city.pincode);
                command.Parameters.AddWithValue("@4", city.stdCode);
                reader = command.ExecuteReader();
                if (reader.RecordsAffected >= 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.exception = "Some error occured, try closing window, contact admin";
                }
            }
            catch (Exception exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = "Add city exception : "+exception.Message;
            }
            return res;
        }
        public Response GetAllCities()
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                List<City> cities = new List<City>();
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from cities";
                command = new OleDbCommand(sqlString, connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    City city = new City();
                    city.cityName = reader["city"].ToString();
                    city.pincode = reader["pincode"].ToString();
                    city.state = reader["state"].ToString();
                    city.stdCode = reader["stdCode"].ToString();
                    cities.Add(city);
                }
                res.success = true;
                res.isException = false;
                res.body = cities;

            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response EditCity(City city) // here fix city name as it is
        {
            // here we will update according to city name, as if city name changes then whole record will change
            // firstly, I will check if city name exist or not
            // then i will edit according to city name
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from cities where city=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1",city.cityName);
                reader = command.ExecuteReader();
                if(!reader.HasRows)
                {
                    throw new DAOException("City name does not exist, please add city first");
                }
                sqlString = "update cities set state=@1, pincode=@2, stdCode=@3 where city=@4";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", city.state);
                command.Parameters.AddWithValue("@2", city.pincode);
                command.Parameters.AddWithValue("@3", city.stdCode);
                command.Parameters.AddWithValue("@4", city.cityName);
                int recordUpdated = command.ExecuteNonQuery();
                if(recordUpdated>0)
                { 
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, or contact admin";
                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response DeleteCity(string cityName)
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from cities where city=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", cityName);
                reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    throw new DAOException("City does not exist with name "+cityName+", Please add city first");
                }
                sqlString = "delete from cities where city=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", cityName);
                int recordsDeleted = command.ExecuteNonQuery();
                if (recordsDeleted > 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, or contact admin";
                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response AddContact(Contact contact)
        {
            Response res = new Response();
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                string sqlString = "insert into contactList(company_id, contact_name, contact_email, contact_phone) values(@1,@2,@3,@4)";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", contact.companyId);
                command.Parameters.AddWithValue("@2", contact.contact_name);
                command.Parameters.AddWithValue("@3", contact.contact_email);
                command.Parameters.AddWithValue("@4", contact.contact_phone);
                reader = command.ExecuteReader();
                if (reader.RecordsAffected >= 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.exception = "Some error occured, try closing window, contact admin";
                }
            }
            catch (Exception exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = "Add contact exception : " + exception.Message;
            }
            return res;
        }
        public Response EditContact(Contact contact) // change contact with use of contact ID
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from contactList where contact_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", contact.contactId);
                reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    throw new DAOException("Contact does not exist, please add contact first");
                }
                sqlString = "update contactList set contact_name=@1, contact_email=@2, contact_phone=@3 where contact_id=@4";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", contact.contact_name);
                command.Parameters.AddWithValue("@2", contact.contact_email);
                command.Parameters.AddWithValue("@3", contact.contact_phone);
                command.Parameters.AddWithValue("@4", contact.contactId);
                int recordUpdated = command.ExecuteNonQuery();
                if (recordUpdated > 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, or contact admin";
                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response DeleteContact(int contactID)
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from contactList where contact_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", contactID);
                reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    throw new DAOException("Contact does not exist, Please add contact first");
                }
                sqlString = "delete from contactlist where contact_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", contactID);
                int recordsDeleted = command.ExecuteNonQuery();
                if (recordsDeleted > 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, or contact admin";
                }
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response GetContactsByCompanyId(int companyID)
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from contactlist where company_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", companyID);
                reader = command.ExecuteReader();
                List<Contact> contacts = new List<Contact>();
                while(reader.Read())
                {
                    Contact contact = new Contact();
                    contact.companyId = companyID;
                    contact.contactId = Int32.Parse(reader["contact_id"].ToString());
                    contact.contact_name = reader["contact_name"].ToString();
                    contact.contact_email = reader["contact_email"].ToString();
                    contact.contact_phone = reader["contact_phone"].ToString();
                    contacts.Add(contact);
                }
                res.success = true;
                res.isException = false;
                res.body = contacts;
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
    }

}
