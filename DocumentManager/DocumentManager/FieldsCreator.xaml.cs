//using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Xps.Packaging;
using unvell.ReoGrid;
using unvell.ReoGrid.Events;
using DocumentManager.com.exception;

namespace DocumentManager
{
    /// <summary>
    /// Interaction logic for FieldsCreator.xaml
    /// </summary>
    public partial class FieldsCreator : System.Windows.Window
    {
        private string xpsFileName = "";
        //private string path = @"E:\Vinay\Training\Document Manager\DocMgr Files\Envelop\Envelop.xls";
        private string path = @"E:\Vinay\Training\Document Manager\DocMgr Files\GST Tax Invoice\GST Tax Invoice 21 Jul 2017.xlsx";
        private DataTable _DataTable;
        private int _DeleteCellIndex = 0;
        private string _RootPath = "";
        public FieldsCreator()
        {
            InitializeComponent();
            populateCmbFieldNames();
            populateFieldDataGrid();
        }
        private void populateCmbFieldNames()
        {
            OleDbConnection connection = DatabaseConnection.GetConnection();
            connection.Open();
            string sqlString = "select distinct field_name from FIELDS";
            OleDbCommand command = new OleDbCommand(sqlString, connection);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                System.Diagnostics.Trace.WriteLine(reader["field_name"].ToString());
                cmbFieldNames.Items.Add(reader["field_name"].ToString());
            }
            connection.Close();
        }
        private void populateFieldDataGrid()
        {
            _DataTable = new DataTable();
            _DataTable.Columns.Add("Field Name");
            _DataTable.Columns.Add("Row");
            _DataTable.Columns.Add("Column");
            _DataTable.Columns.Add("Sheet");
            _DataTable.Columns.Add("Delete Item");
            fieldDataGrid.ItemsSource = _DataTable.DefaultView;

            _DeleteCellIndex = _DataTable.Columns.Count - 1;
        }
        private void sheet_CellMouseDown(object sender,CellMouseEventArgs e)
        {
            unvell.ReoGrid.Worksheet sheet = reoGridControl.CurrentWorksheet;
            Cell cell = sheet.CreateAndGetCell(e.CellPosition);
            //MessageBox.Show((cell.Row+1)+" "+(cell.Column+1));
            txtRow.Text = (cell.Row + 1).ToString();
            txtColumn.Text = (cell.Column + 1).ToString();
            txtSheet.Text = sheet.Name;
        }

        private void btnSelectTemplateFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Select Template",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "xlsx",
                    Filter = "xlsx files (*.xlsx)|*.xlsx",
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    txtSelectTemplate.Text = openFileDialog.FileName;
                    path = txtSelectTemplate.Text;
                    reoGridControl.Load(path, unvell.ReoGrid.IO.FileFormat.Excel2007);
                    var sheet = reoGridControl.CurrentWorksheet;
                    sheet.CellMouseDown += sheet_CellMouseDown;
                }
                else
                {
                    MessageBox.Show("File Not Selected");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void reoGridControl_CurrentWorksheetChanged(object sender, EventArgs e)
        {
            var sheet = reoGridControl.CurrentWorksheet;
            sheet.CellMouseDown += sheet_CellMouseDown;
        }

        private void btnResetFieldsInfo_Click(object sender, RoutedEventArgs e)
        {
            txtRow.Text = "";
            txtColumn.Text = "";
            txtSheet.Text = "";
            cmbFieldNames.SelectedIndex = -1;
        }

        private void btnAddField_Click(object sender, RoutedEventArgs e)
        {
            string fieldName = cmbFieldNames.SelectedItem.ToString();
            string rowNumber = txtRow.Text;
            string columnNumber = txtColumn.Text;
            string sheet = txtSheet.Text;
            if(fieldName=="" || rowNumber=="" || columnNumber=="" || sheet=="")
            {
                MessageBox.Show("Cannot Add, one or more fields are empty");
                return;
            }
            DataRow row = _DataTable.NewRow();
            row["Field Name"] = fieldName;
            row["Row"] = rowNumber;
            row["Column"] = columnNumber;
            row["Sheet"] = sheet;
            row["Delete Item"] = "Delete";
            _DataTable.Rows.Add(row);
            clearFields();
        }
        private void clearFields()
        {
            cmbFieldNames.SelectedIndex = -1;
            txtRow.Text = "";
            txtColumn.Text = "";
            txtSheet.Text = "";
        }

        private void fieldDataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView dataRow = (DataRowView)fieldDataGrid.SelectedItem;
            if (dataRow == null)
            {
                MessageBox.Show("Some Error");
                return;
            }
            int currentColumnIndex = fieldDataGrid.CurrentCell.Column.DisplayIndex;
            int currentRowIndex = fieldDataGrid.SelectedIndex;
            string cellValue = dataRow.Row.ItemArray[0].ToString();
            Trace.WriteLine(currentColumnIndex + " " + _DeleteCellIndex);
            if (currentColumnIndex == _DeleteCellIndex)
            {
                MessageBoxResult result = MessageBox.Show("Confirm Delete : " + cellValue, "", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    _DataTable.Rows.RemoveAt(currentRowIndex);
                    fieldDataGrid.Items.Refresh();
                }
            }
        }
        private string getTemplateName()
        {
            string templateName = txtTemplateName.Text;
            if (templateName == "" || templateName.Length == 0)
            {
                throw new DAOException("Template name not found, please give valid template name");
            }
            return templateName;
        }
        private void checkIfTemplateNameExists(OleDbConnection connection,string templateName)
        {
            OleDbCommand command = null;
            OleDbDataReader reader = null;
            string sqlString = "select * from DocTypes where doctype_name=@NAME";
            command = new OleDbCommand(sqlString, connection);
            command.Parameters.AddWithValue("@NAME", templateName);
            reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                throw new DAOException("Template name already exist, cannot add with this name");
            }
        }
        private int getTemplateDataIdByName(OleDbConnection connection,string templateName)
        {
            OleDbCommand command = null;
            OleDbDataReader reader = null;
            string sqlString = "";
            sqlString = "select doctype_id from doctypes where doctype_name=@NAME";
            command = new OleDbCommand(sqlString, connection);
            command.Parameters.AddWithValue("@NAME", templateName);
            reader = command.ExecuteReader();
            reader.Read();
            int documentId = Int32.Parse(reader["doctype_id"].ToString());
            return documentId;
        }
        private void checkIfDirectoryExists(OleDbConnection connection,string templateName)
        {
            string sqlString = "select setting,svalue from settings";
            try
            {
                OleDbCommand command = new OleDbCommand(sqlString, connection);
                OleDbDataReader reader = command.ExecuteReader();
                reader.Read();
                _RootPath = reader["svalue"].ToString();
                reader.Read();
                string refText = reader["svalue"].ToString();
                Trace.WriteLine(_RootPath + " " + refText);
                if (Directory.Exists(_RootPath + "\\" + templateName))
                {
                    throw new DAOException("Directory exists with name " + templateName + ". Please firstly change template name or delete this directory then save");
                }
            }catch(DAOException dao)
            {
                throw new DAOException("Exception while checking directory exist : "+dao.Message);
            }

        }
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            OleDbConnection connection = null;
            OleDbCommand command = null;
            OleDbDataReader reader = null;
            string sqlString = "";
            try
            {
                string templateName = getTemplateName();
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                checkIfTemplateNameExists(connection,templateName);
                checkIfDirectoryExists(connection, templateName);
                // insert template name in doctypes table
                sqlString = "insert into doctypes(doctype_name) values(@NAME)";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@NAME", templateName);
                reader = command.ExecuteReader();
                if (reader.RecordsAffected != 1)
                {
                    MessageBox.Show("Some error occured and record cannot be added");
                    return;
                }
                //done
                int documentId = getTemplateDataIdByName(connection,templateName);
                int rowCount = _DataTable.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    //Trace.WriteLine(_DataTable.Rows[i]["Field Name"]);
                    //Trace.WriteLine(_DataTable.Rows[i]["Row"]);
                    //Trace.WriteLine(_DataTable.Rows[i]["Column"]);
                    //Trace.WriteLine(_DataTable.Rows[i]["Sheet"]);
                    string fieldName = _DataTable.Rows[i]["Field Name"].ToString();
                    int fieldType = (fieldName == "Mode of Transport") ? 0 : 1;
                    int row = Int32.Parse(_DataTable.Rows[i]["Row"].ToString());
                    int column = Int32.Parse(_DataTable.Rows[i]["Column"].ToString());
                    string sheet = _DataTable.Rows[i]["Sheet"].ToString();
                    sqlString = "insert into fields(doctype_id,field_name,field_type,field_row,field_column,field_sheet,append,isList) values(@1,@2,@3,@4,@5,@6,'False','False')";
                    command = new OleDbCommand(sqlString, connection);
                    command.Parameters.AddWithValue("@1",documentId);
                    command.Parameters.AddWithValue("@2", fieldName);
                    command.Parameters.AddWithValue("@3", fieldType);
                    command.Parameters.AddWithValue("@4", row);
                    command.Parameters.AddWithValue("@5", column);
                    command.Parameters.AddWithValue("@6", sheet);
                    reader = command.ExecuteReader();
                    if(reader.RecordsAffected!=1)
                    {
                        throw new DAOException("Some Error occured and records are not added");
                    }
                }
                Directory.CreateDirectory(_RootPath + "\\" + templateName);
                File.Copy(path, _RootPath + "\\" + templateName + "\\" + templateName + ".xlsx");
                MessageBox.Show("Information saved and template is created");
                connection.Close();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in saving : " + ex.Message);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception in cancel button " + ex.Message);
            }
        }
    }
}
