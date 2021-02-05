using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using DocumentManager.com.exception;
using DocumentManager.com.poco;

namespace DocumentManager.com.Utility
{
    /// <summary>
    /// This class is responsible for taking values from Fields object and generating reports accordingly.
    /// </summary>
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
                //System.Diagnostics.Trace.WriteLine(reader["field_name"]);
            }
            //System.Diagnostics.Trace.WriteLine(field.contactName + " " + field.contactPhone);
            //return;
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
            if (fieldName.Equals("Contact Email")) cell.SetCellValue(field.contactEmail);
            if (fieldName.Equals("State Code")) cell.SetCellValue(field.stateCode);
            if (fieldName.Equals("GST No")) cell.SetCellValue(field.gstNo);
            if (fieldName.Equals("Country")) cell.SetCellValue(field.country);
            if (fieldName.Equals("Service Invoice No"))
            {
                cell.SetCellValue(field.serviceInvoiceNo);
                MainWindow mainWindow = new MainWindow();
                string serviceNo = field.serviceInvoiceNo;
                string newServiceInvoiceNo = Utility.IncreaseStringByOne(serviceNo);
                mainWindow.txtServiceInvoiceNo.Text = newServiceInvoiceNo;
                mainWindow.UpdateSettingsTable();
                field.serviceInvoiceNo = newServiceInvoiceNo;
            }
            if (fieldName.Equals("Tax Invoice No"))
            {
                cell.SetCellValue(field.taxInvoiceNo);
                MainWindow mainWindow = new MainWindow();
                string taxInvoiceNo = field.taxInvoiceNo;
                string newTaxInvoiceNo = Utility.IncreaseStringByOne(taxInvoiceNo);
                mainWindow.txtTaxInvoiceNo.Text = newTaxInvoiceNo;
                mainWindow.UpdateSettingsTable();
                field.taxInvoiceNo = newTaxInvoiceNo;
            }
        }
    }


}
