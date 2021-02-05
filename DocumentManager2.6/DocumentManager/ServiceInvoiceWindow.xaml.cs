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
using DocumentManager.com.poco;
using DocumentManager.com.Utility;
using System.Diagnostics;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using DocumentManager.com.exception;
using System.Data.OleDb;
using DocumentManager.com.dl;
using System.Text.RegularExpressions;

namespace DocumentManager
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class ServiceInvoiceWindow : Window
    {
        private Field f;
        private List<Purchase> purchases;
        private DataLayer dl;
        private MainWindow mainWindow;
        public ServiceInvoiceWindow()
        {
            InitializeComponent();
            // From this we will get context of MainWindow, and we can do whatever we want from this context
            dl = new DataLayer();
            purchases = new List<Purchase>();
            populateTemplate();
            populateModels();
            if(cmbModels.Items.Count>0) cmbModels.SelectedIndex = 0;
            populateServices();
            if (cmbServices.Items.Count > 0) cmbServices.SelectedIndex = 0;
            populateDates();
            DisableGSTByState(txtState1.Text);
            populatePurchaseGrid();
        }

        private void populatePurchaseGrid()
        {
            purchaseDataGrid.ItemsSource = null;
            purchaseDataGrid.ItemsSource = purchases;
            Utility.MakeAllColumnsWidthSame(purchaseDataGrid);
            if (purchaseDataGrid.Columns.Count > 0)
            {
                purchaseDataGrid.Columns[1].Width = new DataGridLength(10, DataGridLengthUnitType.Star);
                purchaseDataGrid.Columns[3].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            }
        }

        /// <summary>
        /// It will enable only GST fields required according to state only
        /// </summary>
        /// <param name="state"></param>
        private void DisableGSTByState(string state)
        {
            Trace.WriteLine(state);
            Trace.WriteLine(state == "Karnataka");
            if (state == "Karnataka")
            {
                txtCGST.IsEnabled = false;
                txtSGST.IsEnabled = false;
            }
            else
            {
                txtIGST.IsEnabled = false;
            }
        }

        /// <summary>
        /// It will populate date text field, according to today's date.
        /// </summary>
        private void populateDates()
        {
            dateFrom.Text = DateTime.Now.ToString();
            dateTo.Text = DateTime.Now.AddDays(364).ToString();
        }

        /// <summary>
        /// This function will take values from mainWindow text field, populate fields object and then populate text fields accordingly
        /// </summary>
        private void populateTemplate()
        {
            mainWindow = ((MainWindow)Application.Current.MainWindow);
            f = new Field()
            {
                refNo = mainWindow.txtRef.Text,
                companyName = mainWindow.getCurrentSelectedCompany().companyName,
                address1 = mainWindow.txtAddress1.Text,
                address2 = mainWindow.txtAddress2.Text,
                address3 = mainWindow.txtAddress3.Text,
                city = mainWindow.txtCity.Text,
                pincode = mainWindow.txtPincode.Text,
                phone = mainWindow.txtPhone.Text,
                date = mainWindow.txtDate.Text,
                state = mainWindow.getSelectedState().stateName,
                country = mainWindow.txtCountry.Text,
                //stateCode = (mainWindow.txtStateCode.Text == "") ? 0 : int.Parse(mainWindow.txtStateCode.Text),
                stateCode = mainWindow.txtStateCode.Text,
                contactName = mainWindow.txtContactName.Text,
                contactEmail = mainWindow.txtContactEmail.Text,
                contactPhone = mainWindow.txtContactPhone.Text,
                gstNo = mainWindow.txtGSTNo.Text,
                serviceInvoiceNo = mainWindow.txtServiceInvoiceNo.Text,
                taxInvoiceNo=mainWindow.txtTaxInvoiceNo.Text
            };
            string addressBlock = f.companyName + "\n" + f.address1 + "\n" + f.address2 + "\n" + f.address3 + "\n" + f.city + " - " + f.pincode + "\n" + f.state + " , " + f.country + "\n" + "Tel : " + f.phone;
            txtAddressBox1.Text = addressBlock;
            txtAddressBox2.Text = addressBlock;
            txtInvoiceDate.Text = f.date;
            txtState1.Text = f.state;
            txtState2.Text = "State : " + f.state;
            txtStateCode.Text = "State Code : " + f.stateCode;
            txtGST.Text = f.gstNo;
        }

        /// <summary>
        /// This function will take modal values from DL, and show them in combo box
        /// </summary>
        private void populateModels()
        {
            //List<string> models = new List<string>
            //{
            //    "V3.4J","V3.4M","V2015J LX","V2015J CX","V2015J HT","V2015M","V4020J CX","V4020J LX","V4020J HT","V4030J LX"
            //};
            try
            {
                Response res = dl.GetAllModels();
                if (res.success)
                {
                    List<Model> models = (List<Model>)res.body;
                    cmbModels.ItemsSource = models;
                    cmbModels.DisplayMemberPath = "modelName";
                }
                else if(res.isException)
                {
                    throw new DAOException(res.exception);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// This function will take services from DL and show then in combo box.
        /// </summary>
        private void populateServices()
        {
            //List<string> services = new List<string>
            //{
            //    "Annual Comprehensive Service Contract (ASCS) for Vision Measuring System.",
            //    "Annual Maintenance Contract (AMC) for Vision Measuring System.",
            //    "Calibration Service for Vision Measuring System",
            //    "Service Visit Charges",
            //    "Service Charges for Vision Measuring System",
            //    "Training Charges for \"Rapid-I\" Vision Measuring System"
            //};
            try
            {
                Response res = dl.GetAllServices();
                if(res.success)
                {
                    List<Service> services = (List<Service>)res.body;
                    cmbServices.ItemsSource = services;
                    cmbServices.DisplayMemberPath = "serviceName";
                }
                else if(res.isException)
                {
                    throw new DAOException(res.exception);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void rbtnCalibration_Checked(object sender, RoutedEventArgs e)
        {
            CreateDescription();
        }

        private void rbtnContract_Checked(object sender, RoutedEventArgs e)
        {
            CreateDescription();
        }
        
        /// <summary>
        /// This function will create description and put it in description text block, it will create description according to various text fields.
        /// </summary>
        public void CreateDescription()
        {
            if (txtBlockDescription == null) return;
            if(cmbServices==null || cmbModels==null || cmbServices.SelectedIndex==-1 || cmbModels.SelectedIndex==-1)
            {
                txtBlockDescription.Text = "Please select service and model No.";
                return;
            }
            txtBlockDescription.Text = ((Service)cmbServices.SelectedItem).serviceName + "\n" + txtBrand.Text + " " + ((Model)cmbModels.SelectedItem).modelName + " Machine No. " + txtMachineNo.Text;
            txtBlockDescription.Text += "\n\n";
            if (rbtnCalibration.IsChecked == true) txtBlockDescription.Text += "Calibration Validity : " + dateFrom.Text + " To " + dateTo.Text;
            else if (rbtnContract.IsChecked == true) txtBlockDescription.Text += "Contract Period : " + dateFrom.Text + " To " + dateTo.Text;
        }

        private void txtBrand_TextChanged(object sender, TextChangedEventArgs e)
        {
            CreateDescription();
        }

        private void txtMachineNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            CreateDescription();
        }

        private void cmbModels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateDescription();
        }

        private void cmbServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateDescription();
        }

        private void dateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dateTo.Text = dateFrom.SelectedDate.Value.Date.AddDays(364).ToString();
            CreateDescription();
        }


        private void dateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateDescription();
        }

        /// <summary>
        /// This function will add new model in Models table in DB, and then it will update models combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Utility.IsNumber(txtAmount.Text))
                {
                    MessageBox.Show("Invalid value in amount");
                    return;
                }
                if (!Utility.IsNumber(txtQty.Text))
                {
                    MessageBox.Show("Invalid value in Quantity");
                    return;
                }
                Purchase p = new Purchase();
                p.SNo = purchases.Count + 1;
                p.description = txtBlockDescription.Text;
                p.amount = Int32.Parse(txtAmount.Text);
                p.quantity = Int32.Parse(txtQty.Text);
                purchases.Add(p);
                populateMainTemplate();
                populatePurchaseGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in Add Button " + ex.Message);
            }
        }

        /// <summary>
        /// This function will take value from purchases object and  put appropriate values in Sno, desc, Qty and amount text block.
        /// </summary>
        private void populateMainTemplate()
        {
            txtBlockSNo.Text = "";
            txtBlockDesc.Text = "";
            txtBlockQty.Text = "";
            txtBlockAmt.Text = "";
            float total = 0;
            for (int i = 0; i < purchases.Count; i++)
            {
                txtBlockSNo.Text += purchases[i].SNo;
                txtBlockDesc.Text += purchases[i].description;
                txtBlockQty.Text += purchases[i].quantity;
                float calculatedAmount = purchases[i].amount * purchases[i].quantity;
                total += calculatedAmount;
                txtBlockAmt.Text += Utility.ConvertfloatToString(calculatedAmount);
                int count = purchases[i].description.Count(s => s == '\n') + 1;
                string escapeChar = Utility.repeatString("\n", count);
                txtBlockSNo.Text += escapeChar;
                txtBlockQty.Text += escapeChar;
                txtBlockAmt.Text += escapeChar;
                txtBlockDesc.Text += "\n";
            }
            txtTotalAmount.Text = Utility.ConvertfloatToString(total);
            float grandTotal = total;
            float gstAmount = 0;
            if (txtCGST.IsEnabled == true)
            {
                txtCGST.Text = Utility.ConvertfloatToString(total * 9 / 100);
                grandTotal += float.Parse(txtCGST.Text);
                gstAmount += float.Parse(txtCGST.Text);
            }
            if (txtSGST.IsEnabled == true)
            {
                txtSGST.Text = Utility.ConvertfloatToString(total * 9 / 100);
                grandTotal += float.Parse(txtSGST.Text);
                gstAmount += float.Parse(txtSGST.Text);
            }
            if (txtIGST.IsEnabled == true)
            {
                txtIGST.Text = Utility.ConvertfloatToString(total * 18 / 100);
                grandTotal += float.Parse(txtIGST.Text);
                gstAmount += float.Parse(txtIGST.Text);
            }
            txtGrandTotal.Text = Utility.ConvertfloatToString(grandTotal);
            txtGSTAmountInWords.Text = "Rupees " + Utility.CapitaliseString(Utility.ConvertNumbertoWords((int)gstAmount).ToLower()) + " only";
            txtTotalAmountInWords.Text = "Rupees " + Utility.CapitaliseString(Utility.ConvertNumbertoWords((int)grandTotal).ToLower()) + " only";
        }


        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAmount == null || txtQty == null || txtQty.Text.Length==0 || txtAmount.Text.Length==0) return;
            if (Utility.IsNumber(txtQty.Text))
            {
                txtAmount.Text = (Int32.Parse(txtAmount.Text) * Int32.Parse(txtQty.Text)).ToString();
            }
        }

        /// <summary>
        /// This will generate xlsx reports, according to values from fields and purchases
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);
                string saveFolder = mainWindow.txtDocumentFolderPath.Text + "\\" + mainWindow.getCurrentSelectedCompany().companyName;
                if (File.Exists(mainWindow.txtSaveFilePath.Text))
                {
                    MessageBox.Show("File Exists previously, either change serial number or delete file from " + mainWindow.txtSaveFilePath.Text);
                }
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }


                string reportName = mainWindow.txtTemplateFilePath.Text.ToString();
                GenerateReport(f, reportName, mainWindow.txtSaveFilePath.Text.ToString());
                mainWindow.SaveInDb(mainWindow.txtSaveFilePath.Text.ToString());
                if (btnOpenToggle.IsChecked == true)
                {
                    Process.Start(mainWindow.txtSaveFilePath.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in generating report : " + ex.Message);
            }
        }
        
        /// <summary>
        /// This generate function do same work as of reportProcessor generate function. But it is different from that generate function, as it will also used to generate multiline report, when taking values from purchase.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="reportName"></param>
        /// <param name="saveFileName"></param>
        public void GenerateReport(Field field, string reportName, string saveFileName)
        {
            string documentName = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(reportName));
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

            if (sheet == null) sheet = workbook.GetSheet("Invoice");
            IRow currRow;
            ICell cell;

            for (int i = 0, rowNum = 20; i < purchases.Count; i++)
            {
                Purchase p = purchases[i];
                currRow = sheet.GetRow(rowNum);
                cell = currRow.GetCell(0);
                //SetValue(cell, "1", field);
                cell.SetCellValue(p.SNo);

                string[] strs = p.description.Split('\n');
                for (int j = 0; j < strs.Length; j++)
                {
                    currRow = sheet.GetRow(rowNum + j);
                    cell = currRow.GetCell(1);
                    cell.SetCellValue(strs[j]);
                }

                currRow = sheet.GetRow(rowNum);
                cell = currRow.GetCell(6);
                cell.SetCellValue(p.quantity);

                currRow = sheet.GetRow(rowNum);
                cell = currRow.GetCell(8);
                cell.SetCellValue(p.amount);
                int count = p.description.Count(s => s == '\n') + 1;
                rowNum += count;
            }
            currRow = sheet.GetRow(40);
            cell = currRow.GetCell(0);
            cell.SetCellValue(txtGSTAmountInWords.Text);

            currRow = sheet.GetRow(42);
            cell = currRow.GetCell(0);
            cell.SetCellValue(txtTotalAmountInWords.Text);

            currRow = sheet.GetRow(38);
            cell = currRow.GetCell(8);
            cell.SetCellValue(txtTotalAmount.Text);

            currRow = sheet.GetRow(39);
            cell = currRow.GetCell(8);
            cell.SetCellValue(txtCGST.Text);

            currRow = sheet.GetRow(40);
            cell = currRow.GetCell(8);
            cell.SetCellValue(txtSGST.Text);

            currRow = sheet.GetRow(41);
            cell = currRow.GetCell(8);
            cell.SetCellValue(txtIGST.Text);

            currRow = sheet.GetRow(42);
            cell = currRow.GetCell(8);
            cell.SetCellValue(txtGrandTotal.Text);

            OleDbConnection connection = DatabaseConnection.GetConnection();
            connection.Open();
            OleDbCommand command = new OleDbCommand("select * from qryDocumentFields where doctype_name=@DOCUMENT", connection);
            //Trace.WriteLine("Document Name " + documentName);
            command.Parameters.AddWithValue("@DOCUMENT", documentName);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (sheet == null) sheet = workbook.GetSheet(reader["field_sheet"].ToString());
                currRow = sheet.GetRow(int.Parse(reader["field_row"].ToString()) - 1);
                cell = currRow.GetCell(int.Parse(reader["field_column"].ToString()) - 1);
                //Trace.WriteLine("Field Name "+reader["field_name"]);
                SetValue(cell, reader["field_name"].ToString(), field);
            }


            using (var fileData = new FileStream(saveFileName, FileMode.Create))
            {
                workbook.Write(fileData);
                workbook.Close();
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
            if (fieldName.Equals("Contact Email")) cell.SetCellValue(field.contactEmail);
            if (fieldName.Equals("State Code")) cell.SetCellValue(field.stateCode);
            if (fieldName.Equals("GST No")) cell.SetCellValue(field.gstNo);
            if (fieldName.Equals("Country")) cell.SetCellValue(field.country);
            if (fieldName.Equals("Service Invoice No"))
            {
                cell.SetCellValue(field.serviceInvoiceNo);
                string serviceNo = field.serviceInvoiceNo;
                string newServiceInvoiceNo = Utility.IncreaseStringByOne(serviceNo);
                mainWindow.txtServiceInvoiceNo.Text = newServiceInvoiceNo;
                mainWindow.UpdateSettingsTable();
                field.serviceInvoiceNo = newServiceInvoiceNo;
            }
            if (field.Equals("Tax Invoice No"))
            {
                cell.SetCellValue(field.taxInvoiceNo);
                string taxInvoiceNo = field.taxInvoiceNo;
                string newTaxInvoiceNo = Utility.IncreaseStringByOne(taxInvoiceNo);
                mainWindow.txtTaxInvoiceNo.Text = newTaxInvoiceNo;
                mainWindow.UpdateSettingsTable();
                field.taxInvoiceNo = newTaxInvoiceNo;
            }
            
        }

        /// <summary>
        /// This function will fill required text fields, according to selected value in purchase data grid changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void purchaseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (purchaseDataGrid.SelectedIndex == -1)
                {
                    txtQty.Text = "";
                    txtAmount.Text = "";
                    txtBlockDescription.Text = "";
                    btnSave.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                    return;
                }
                btnSave.IsEnabled = true;
                btnDelete.IsEnabled = true;
                Purchase purchase = (Purchase)purchaseDataGrid.SelectedItem;
                if (purchase == null) return;
                txtBlockDescription.Text = purchase.description;
                txtQty.Text = purchase.quantity.ToString();
                txtAmount.Text = purchase.amount.ToString();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void validateFields()
        {
            if(txtQty.Text=="")
            {
                throw new DAOException("Please enter quantity");
            }
            if(!Utility.IsNumber(txtQty.Text))
            {
                throw new DAOException("Quantity must be a number");
            }
            if(txtAmount.Text=="")
            {
                throw new DAOException("Please enter amount");
            }
            if(!Utility.IsNumber(txtAmount.Text))
            {
                throw new DAOException("Amount must be a number");
            }
        }

        /// <summary>
        /// It will update values in purchase data grid, and main template, according to values in text fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                validateFields();
                Purchase selectedPurchase = (Purchase)purchaseDataGrid.SelectedItem;
                if (selectedPurchase == null) throw new DAOException("Please select a field");
                for (int i = 0; i < purchases.Count; i++)
                {
                    if (purchases[i].SNo == selectedPurchase.SNo)
                    {
                        purchases[i].amount = Int32.Parse(txtAmount.Text);
                        purchases[i].description = txtBlockDescription.Text;
                        purchases[i].quantity = Int32.Parse(txtQty.Text);
                    }
                }
                populateMainTemplate();
                populatePurchaseGrid();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        /// <summary>
        /// It will delete currently selected item from data grid, and from main template also.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                Purchase selectedPurchase = (Purchase)purchaseDataGrid.SelectedItem;
                if (selectedPurchase == null) throw new DAOException("Please select a field");
                purchases.Remove(selectedPurchase);
                for(int i=0;i<purchases.Count;i++)
                {
                    purchases[i].SNo = i + 1;
                }
                populateMainTemplate();
                populatePurchaseGrid();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //CRUD on cmb Models
        private void setAddModelsMode()
        {
            txtModels.Visibility = Visibility.Visible;
            btnAddModels.IsEnabled = true;
            btnAddModels.Content = "Ok";
            btnEditModels.IsEnabled = false;
            btnDeleteModels.IsEnabled = false;
            btnCancelModel.IsEnabled = true;
        }
        private void setEditModelsMode()
        {
            txtModels.Visibility = Visibility.Visible;
            btnAddModels.IsEnabled = false;
            btnEditModels.IsEnabled = true;
            btnEditModels.Content = "Ok";
            btnDeleteModels.IsEnabled = false;
            btnCancelModel.IsEnabled = true;
        }

        private void setViewModelsMode()
        {
            txtModels.Visibility = Visibility.Collapsed;
            txtModels.Text = "";
            btnAddModels.IsEnabled = true;
            btnAddModels.Content = "Add";
            btnEditModels.IsEnabled = true;
            btnEditModels.Content = "Edit";
            btnDeleteModels.IsEnabled = true;
            btnCancelModel.IsEnabled = false;
        }

        private void setCancelModelsMode()
        {
            setViewModelsMode();
        }

        private void btnAddModels_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnAddModels.Content.ToString() == "Add")
                {
                    setAddModelsMode();
                }
                else
                {
                    string modelName = txtModels.Text;
                    if (modelName.Length == 0 || modelName.Trim() == "") throw new DAOException("Model Name was invalid");
                    Response res = dl.AddModel(modelName);
                    if(res.success)
                    {
                        System.Threading.Thread.Sleep(1000);
                        setViewModelsMode();
                        populateModels();
                    }
                    else if(res.isException)
                    {
                        throw new DAOException(res.exception);
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEditModels_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                if(btnEditModels.Content.ToString()=="Edit")
                {
                    if (cmbModels.SelectedIndex == -1) throw new DAOException("Please select a model first");
                    setEditModelsMode();
                    txtModels.Text = ((Model)cmbModels.SelectedItem).modelName;
                }
                else
                {
                    Model model = new Model();
                    model.id = ((Model)cmbModels.SelectedItem).id;
                    model.modelName = txtModels.Text;
                    Response res = dl.UpdateModel(model);
                    if(res.success)
                    {
                        System.Threading.Thread.Sleep(1000);
                        setViewModelsMode();
                        populateModels();
                    }
                    else if(res.isException)
                    {
                        throw new DAOException(res.exception);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void btnDeleteModels_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                if (cmbModels.SelectedIndex == -1) throw new DAOException("Please select a model first");
                string modelName=((Model)cmbModels.SelectedItem).modelName;
                MessageBoxResult msg = MessageBox.Show("Confirm Delete : " + modelName,"Delete",MessageBoxButton.YesNo);
                if (msg == MessageBoxResult.Yes)
                {
                    Response res = dl.DeleteModel(modelName);
                    if (res.success)
                    {
                        System.Threading.Thread.Sleep(1000);
                        setCancelModelsMode();
                        populateModels();
                    }
                    else if (res.isException)
                    {
                        throw new DAOException(res.exception);
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancelModels_Click(object sender,RoutedEventArgs e)
        {
            setCancelModelsMode();
        }


        //CRUD operation on services
        private void setViewServiceMode()
        {
            txtService.Visibility = Visibility.Collapsed;
            txtService.Text = "";
            btnAddService.IsEnabled = true;
            btnAddService.Content = "Add";
            btnEditService.IsEnabled = true;
            btnEditService.Content = "Edit";
            btnDeleteService.IsEnabled = true;
            btnCancelService.IsEnabled = false;
        }
        private void setAddServiceMode()
        {
            txtService.Visibility = Visibility.Visible;
            btnAddService.IsEnabled = true;
            btnAddService.Content = "Ok";
            btnEditService.IsEnabled = false;
            btnDeleteService.IsEnabled = false;
            btnCancelService.IsEnabled = true;
        }
        private void setEditServiceMode()
        {
            txtService.Visibility = Visibility.Visible;
            btnAddService.IsEnabled = false;
            btnEditService.IsEnabled = true;
            btnEditService.Content = "Ok";
            btnDeleteService.IsEnabled = false;
            btnCancelService.IsEnabled = true;
        }
        private void setCancelServiceMode()
        {
            setViewServiceMode();
        }

        private void btnAddService_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                if (btnAddService.Content.ToString() == "Add")
                {
                    setAddServiceMode();
                }
                else
                {
                    string serviceName = txtService.Text;
                    if (serviceName.Length == 0 || serviceName.Trim() == "") throw new DAOException("Service name was invalid!");
                    Response res = dl.AddService(serviceName);
                    if(res.success)
                    {
                        System.Threading.Thread.Sleep(1000);
                        setViewServiceMode();
                        populateServices();
                    }
                    else if(res.isException)
                    {
                        throw new DAOException(res.exception);
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEditService_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                if (btnEditService.Content.ToString() == "Edit")
                {
                    if (cmbServices.SelectedIndex == -1) throw new DAOException("Please select a Service first");
                    setEditServiceMode();
                    txtService.Text = ((Service)cmbServices.SelectedItem).serviceName;
                }
                else
                {
                    Service service = new Service();
                    service.id = ((Service)cmbServices.SelectedItem).id;
                    service.serviceName = txtService.Text;
                    Response res = dl.UpdateService(service);
                    if (res.success)
                    {
                        System.Threading.Thread.Sleep(1000);
                        setViewServiceMode();
                        populateServices();
                    }
                    else if (res.isException)
                    {
                        throw new DAOException(res.exception);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDeleteService_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                if (cmbServices.SelectedIndex == -1) throw new DAOException("Please select a service first");
                string serviceName = ((Service)cmbServices.SelectedItem).serviceName;
                MessageBoxResult msg = MessageBox.Show("Confirm Delete : " + serviceName, "Delete", MessageBoxButton.YesNo);
                if (msg == MessageBoxResult.Yes)
                {
                    Response res = dl.DeleteService(serviceName);
                    if (res.success)
                    {
                        System.Threading.Thread.Sleep(1000);
                        setCancelServiceMode();
                        populateServices();
                    }
                    else if (res.isException)
                    {
                        throw new DAOException(res.exception);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancelService_Click(object sender,RoutedEventArgs e)
        {
            setViewServiceMode();
        }
    }
}
