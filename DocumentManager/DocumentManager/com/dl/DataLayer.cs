using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.com.poco;
using DocumentManager.com.exception;
namespace DocumentManager.com.dl
{
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
                if (reader.HasRows)
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
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, contact admin";
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
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response GetCompanyByCompanyId(int companyId)
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                OleDbConnection connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from companylist where company_id=@1";
                OleDbCommand command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", companyId);
                OleDbDataReader reader = command.ExecuteReader();
                reader.Read();
                Company company2 = new Company();
                company2.companyId = Int32.Parse(reader["company_id"].ToString());
                company2.companyName = reader["company_name"].ToString();
                res.success = true;
                res.isException = false;
                res.body = company2;
            }
            catch (Exception ex)
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
                sqlString = "select * from companylist order by company_name ASC";
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
                sqlString = "Insert into addresslist(company_id,address1,address2,address3,city,state,pincode,phone,GSTNo,state_code) values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10)";
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
                command.Parameters.AddWithValue("@10", address.stateCode);

                reader = command.ExecuteReader();
                if (reader.RecordsAffected == 1)
                {
                    res.success = true;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, contact admin";
                }
            }
            catch (Exception exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = "Add address exception : " + exception.Message;
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
                sqlString = "Update addressList set   address1=@1 ,address2=@2,address3=@3,city=@4,state=@5,pincode=@6,phone=@7,GSTNo=@8,state_code=@9 where address_id=@10";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", address.address1);
                command.Parameters.AddWithValue("@2", address.address2);
                command.Parameters.AddWithValue("@3", address.address3);
                command.Parameters.AddWithValue("@4", address.city);
                command.Parameters.AddWithValue("@5", address.state);
                command.Parameters.AddWithValue("@6", address.pincode);
                command.Parameters.AddWithValue("@7", address.phone);
                command.Parameters.AddWithValue("@8", address.GSTNo);
                command.Parameters.AddWithValue("@9", address.stateCode);
                command.Parameters.AddWithValue("@10", address.addressID);

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
                    string stateCode = reader["state_code"].ToString();
                    if (stateCode != "") address.stateCode = Int32.Parse(stateCode);
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
                string stateCode = reader["state_code"].ToString();
                if (stateCode != "") address.stateCode = Int32.Parse(stateCode);
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
                res = GetCityByName(city.cityName);
                if(res.success)
                {
                    throw new DAOException("Cannot add city name " + city.cityName + " , as it exist previously, you can update it");
                }
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
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, contact admin";
                }
            }
            catch (Exception exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = "Add city exception : " + exception.Message;
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
                command.Parameters.AddWithValue("@1", city.cityName);
                reader = command.ExecuteReader();
                if (!reader.HasRows)
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
                    throw new DAOException("City does not exist with name " + cityName + ", Please add city first");
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
        public Response GetCityByName(string cityName)
        {
            Response res = new Response();
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                string sqlString = "select * from cities where city=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", cityName);
                reader = command.ExecuteReader();
                reader.Read();

                City city = new City();
                city.cityName = reader["city"].ToString();
                city.pincode = reader["pincode"].ToString();
                city.state = reader["state"].ToString();
                city.stdCode = reader["stdCode"].ToString();

                res.success = true;
                res.isException = false;
                res.body = city;
            }catch(Exception ex)
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
                    res.isException = true;
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
                sqlString = "select contact_id,contact_name,contact_email,contact_phone from contactlist where company_id=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", companyID);
                reader = command.ExecuteReader();
                List<Contact> contacts = new List<Contact>();
                while (reader.Read())
                {
                    Contact contact = new Contact();
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

        public Response GetStateCodeByName(string stateName)
        {
            Response res = new Response();
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                string sqlString = "select state_code from states where state_name=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", stateName);
                reader = command.ExecuteReader();

                if(reader.HasRows)
                {
                    reader.Read();
                    string stateCode = reader["state_code"].ToString();
                    res.success = true;
                    res.isException = false;
                    res.body = stateCode;
                }
            }catch(Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        // CRUD Operation on DocType Table
        public Response GetDoctypeIdByName(string docTypeName)
        {
            Response res = new Response();
            OleDbConnection connection2 = null;
            OleDbCommand command2 = null;
            OleDbDataReader reader2 = null;
            string sqlString = "";
            try
            {
                connection2 = DatabaseConnection.GetConnection();
                connection2.Open();
                sqlString = "select doctype_id from doctypes where doctype_name=@1";
                command2 = new OleDbCommand(sqlString, connection2);
                command2.Parameters.AddWithValue("@1", docTypeName);
                reader2 = command2.ExecuteReader();
                reader2.Read();
                int docTypeId = Int32.Parse(reader2["doctype_id"].ToString());
                res.success = true;
                res.isException = false;
                res.body = docTypeId;
            }catch(Exception exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = exception.Message;
            }
            return res;
        }
        // CRUD Operation on Documents table
        public Response AddDocument(DLDocument dlDocument)
        {
            Response res = new Response();
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                string sqlString = "insert into documents  (document_id, doctype_id, company_id, document_path, sender)  values(@1,@2,@3,@4,@6)";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", dlDocument.documentId);
                command.Parameters.AddWithValue("@2", dlDocument.docTypeId);
                command.Parameters.AddWithValue("@3", dlDocument.companyId);
                command.Parameters.AddWithValue("@4", dlDocument.documentPath);
                command.Parameters.AddWithValue("@6", dlDocument.sender);

                reader = command.ExecuteReader();
                if (reader.RecordsAffected >= 0)
                {
                    res.success = true;
                    res.isException = false;
                }
                else
                {
                    res.success = false;
                    res.isException = true;
                    res.exception = "Some error occured, try closing window, contact admin";
                }
            }
            catch (Exception exception)
            {
                res.success = false;
                res.isException = true;
                res.exception = exception.Message;
            }
            return res;
        }

        public Response DeleteDocument(int serialNumber,string docTypeName)
        {
            Response res = new Response();
            int docTypeId =0;
            try
            {
                Response res2 = GetDoctypeIdByName(docTypeName);
                if(res2.success)
                {
                    docTypeId = Int32.Parse(res2.body.ToString());
                }
                else if(res2.isException)
                {
                    throw new DAOException(res2.exception);
                }

                connection = DatabaseConnection.GetConnection();
                connection.Open();
                string sqlString = "delete from documents where document_id=@1 and doctype_id=@2";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", serialNumber);
                command.Parameters.AddWithValue("@2", docTypeId);
                
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
            catch(Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }

        public Response GetDocuementsByDocType(string docTypeName)
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                List<Document> documents = new List<Document>();
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select doc.document_id, doc.document_date, doc.sender, doc.rev_no,  companylist.company_name from ((documents as doc inner join companylist on companylist.company_id = doc.company_id) inner join doctypes on doc.doctype_id = doctypes.doctype_id) where doctypes.doctype_name = @1 order by doc.document_id DESC";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1",docTypeName);
                OleDbDataReader reader2 = command.ExecuteReader();
                while (reader2.Read())
                {
                    Document document = new Document()
                    {
                        SNo = Int32.Parse(reader2["document_id"].ToString()),
                        RNo = Int32.Parse(reader2["rev_no"].ToString()),
                        sender = reader2["sender"].ToString(),
                    };
                    //System.Diagnostics.Trace.WriteLine("Company Id is " + reader2["company_name"]);
                    document.companyName = reader2["company_name"].ToString();
                    DateTime dt = Convert.ToDateTime(reader2["document_date"]);
                    //String formattedDate = String.Format("dd/MM/yyyy HH/mm/ss", dt);
                    document.date = dt.ToString("dd-MM-yyyy HH:mm:ss");
                    documents.Add(document);
                }
                res.success = true;
                res.isException = false;
                res.body = documents;
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }

        //CRUD operations on settings table
        public Response GetSettings()
        {
            Response res = new Response();
            OleDbConnection connection2 = null;
            OleDbCommand command2 = null;
            OleDbDataReader reader2 = null;
            try
            {
                connection2 = DatabaseConnection.GetConnection();
                connection2.Open();
                string sqlString = "select * from settings";
                command2 = new OleDbCommand(sqlString, connection2);
                reader2 = command2.ExecuteReader();
                reader2.Read();
                Settings setting = new Settings();
                setting.docRoot = reader2["svalue"].ToString();
                reader2.Read();
                setting.refName = reader2["svalue"].ToString();
                reader2.Read();
                setting.templateRoot = reader2["svalue"].ToString();
                res.success = true;
                res.isException = false;
                res.body = setting;
            }catch(Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }
        public Response UpdateSettings(Settings setting)
        {
            Response res = new Response();
            OleDbConnection connection2 = null;
            OleDbCommand command2 = null;
            OleDbDataReader reader2 = null;
            try
            {
                connection2 = DatabaseConnection.GetConnection();
                connection2.Open();
                string sqlString = "update settings set svalue=@1 where setting=@2";
                command2 = new OleDbCommand(sqlString, connection2);
                command2.Parameters.AddWithValue("@1", setting.docRoot);
                command2.Parameters.AddWithValue("@2", "DocRoot");
                int recordsUpdated=command2.ExecuteNonQuery();
                command2 = new OleDbCommand(sqlString, connection2);
                command2.Parameters.AddWithValue("@1", setting.refName);
                command2.Parameters.AddWithValue("@2", "RefText");
                int recordsUpdated2 = command2.ExecuteNonQuery();
                command2 = new OleDbCommand(sqlString, connection2);
                command2.Parameters.AddWithValue("@1", setting.templateRoot);
                command2.Parameters.AddWithValue("@2", "TemplateRoot");
                int recordsUpdated3 = command2.ExecuteNonQuery();
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
    }
}
