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
                sqlString = "select c.company_id, c.company_name, a.address3, a.city from companylist as c left join addresslist as a on c.company_id=a.company_id order by c.company_name ASC";
                command = new OleDbCommand(sqlString, connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Company company = new Company();
                    company.companyId = Int32.Parse(reader["company_id"].ToString());
                    company.companyName = reader["company_name"].ToString();
                    company.companyNameToShow = reader["company_name"].ToString()+" , "+reader["address3"]+" , "+reader["city"];
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

        // CRUD Operation on Address Table
        // decide to put addressID in Address pojo or not
        public Response AddAddress(Address address) 
        {
            Response res = new Response();
            string sqlString = "";
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "Insert into addresslist(company_id,address1,address2,address3,city,state,country,pincode,phone,GSTNo,state_code) values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11)";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", address.companyID);
                command.Parameters.AddWithValue("@2", address.address1);
                command.Parameters.AddWithValue("@3", address.address2);
                command.Parameters.AddWithValue("@4", address.address3);
                command.Parameters.AddWithValue("@5", address.city);
                command.Parameters.AddWithValue("@6", address.state);
                command.Parameters.AddWithValue("@7", address.country);
                command.Parameters.AddWithValue("@8", address.pincode);
                command.Parameters.AddWithValue("@9", address.phone);
                command.Parameters.AddWithValue("@10", address.GSTNo);
                command.Parameters.AddWithValue("@11", address.stateCode);

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
                sqlString = "Update addressList set   address1=@1 ,address2=@2,address3=@3,city=@4,state=@5,country=@6,pincode=@7,phone=@8,GSTNo=@9,state_code=@10 where address_id=@11";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", address.address1);
                command.Parameters.AddWithValue("@2", address.address2);
                command.Parameters.AddWithValue("@3", address.address3);
                command.Parameters.AddWithValue("@4", address.city);
                command.Parameters.AddWithValue("@5", address.state);
                command.Parameters.AddWithValue("@6", address.country);
                command.Parameters.AddWithValue("@7", address.pincode);
                command.Parameters.AddWithValue("@8", address.phone);
                command.Parameters.AddWithValue("@9", address.GSTNo);
                command.Parameters.AddWithValue("@10", address.stateCode);
                command.Parameters.AddWithValue("@11", address.addressID);

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
                        country=reader["country"].ToString(),
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
                    country=reader["country"].ToString(),
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

        // CRUD Operation on Cities
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
                string sqlString = "insert into cities(city,state,country,pincode,stdcode) values(@1,@2,@3,@4,@5)";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", city.cityName);
                command.Parameters.AddWithValue("@2", city.state);
                command.Parameters.AddWithValue("@3", city.country);
                command.Parameters.AddWithValue("@4", city.pincode);
                command.Parameters.AddWithValue("@5", city.stdCode);
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
                    city.country = reader["country"].ToString();
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
                sqlString = "update cities set state=@1, pincode=@2, stdCode=@3, country=@4 where city=@5";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", city.state);
                command.Parameters.AddWithValue("@2", city.pincode);
                command.Parameters.AddWithValue("@3", city.stdCode);
                command.Parameters.AddWithValue("@4", city.country);
                command.Parameters.AddWithValue("@5", city.cityName);
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
                city.country = reader["country"].ToString();

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

        // CRUD operation on Contact
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

        public Response getDocumentFilePath(Document document,string docTypeName)
        {
            // I have three things for fetching documentPath ---> S.No companyName DocumentId sender
            Response res = new Response();
            int companyId = 0;
            int docTypeId = 0;
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                res = GetCompanyByCompanyName(document.companyName);
                if(res.success)
                {
                    companyId = ((Company)res.body).companyId;
                    System.Diagnostics.Trace.WriteLine("Company Id : " + companyId);
                }
                else if(res.isException)
                {
                    throw new DAOException("Company name is invalid");
                }
                res = GetDoctypeIdByName(docTypeName);
                if(res.success)
                {
                    docTypeId = (int)res.body;
                    System.Diagnostics.Trace.WriteLine("Doc Type Id : " + docTypeId);
                }
                else if(res.isException)
                {
                    throw new DAOException("DocType Not exist in db");
                }

                conn = DatabaseConnection.GetConnection();
                conn.Open();
                sqlString = "select * from documents where document_id=@1 and doctype_id=@2 and company_id=@3 and sender=@4";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", document.SNo);
                cmd.Parameters.AddWithValue("@2", docTypeId);
                cmd.Parameters.AddWithValue("@3", companyId);
                cmd.Parameters.AddWithValue("@4", document.sender);
                dbReader = cmd.ExecuteReader();
                //System.Diagnostics.Trace.WriteLine("Sql String : " + sqlString);
                //System.Diagnostics.Trace.WriteLine(document.SNo + " " + docTypeId + " " + companyId + " " + document.RNo);
                if (!dbReader.HasRows) throw new DAOException("Cannot able to fetch fileName correctly");
                if(dbReader.Read())
                {
                    res.success = true;
                    res.isException = false;
                    res.body = dbReader["document_path"].ToString();
                }
            }catch(Exception ex)
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
                reader2.Read();
                setting.serviceInvoiceNo = reader2["svalue"].ToString();
                reader2.Read();
                setting.taxInvoiceNo = reader2["svalue"].ToString();
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

                command2 = new OleDbCommand(sqlString, connection2);
                command2.Parameters.AddWithValue("@1", setting.serviceInvoiceNo);
                command2.Parameters.AddWithValue("@2", "ServiceInvoice");
                int recordsUpdated4 = command2.ExecuteNonQuery();

                command2 = new OleDbCommand(sqlString, connection2);
                command2.Parameters.AddWithValue("@1", setting.taxInvoiceNo);
                command2.Parameters.AddWithValue("@2", "ManagementInvoice");
                int recordsUpdated5 = command2.ExecuteNonQuery();

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
        
        // CRUD on States
        public Response GetStates()
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                List<State> states = new List<State>();
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from states";
                command = new OleDbCommand(sqlString, connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    State state = new State();
                    state.id = Int32.Parse(reader["ID"].ToString());
                    state.stateName = reader["state_name"].ToString();
                    state.stateCode=Int32.Parse(reader["state_code"].ToString());
                    states.Add(state);
                }
                res.success = true;
                res.isException = false;
                res.body = states;

            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }

        public Response GetStateByStateName(string stateName)
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                connection = DatabaseConnection.GetConnection();
                connection.Open();
                sqlString = "select * from states where state_name=@1";
                command = new OleDbCommand(sqlString, connection);
                command.Parameters.AddWithValue("@1", stateName);
                reader = command.ExecuteReader();
                reader.Read();

                State state = new State();
                state.id = Int32.Parse(reader["ID"].ToString());
                state.stateName = reader["state_name"].ToString();
                state.stateCode = Int32.Parse(reader["state_code"].ToString());
                
                res.success = true;
                res.isException = false;
                res.body = state;

            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }

        // CRUD Operation in Models
        public Response GetAllModels()
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                List<Model> models = new List<Model>();
                conn = DatabaseConnection.GetConnection();
                conn.Open();
                sqlString = "select * from models order by model_name ASC";
                cmd = new OleDbCommand(sqlString, conn);
                dbReader = cmd.ExecuteReader();
                while (dbReader.Read())
                {
                    Model model = new Model();
                    model.id = Int32.Parse(dbReader["ID"].ToString());
                    model.modelName = dbReader["model_name"].ToString();
                    models.Add(model);
                }
                res.success = true;
                res.isException = false;
                res.body = models;
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }

        public Response AddModel(string modelName)
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                if (modelName.Length == 0 || modelName == "") throw new DAOException("cannot add empty model name");
                conn = DatabaseConnection.GetConnection();
                conn.Open();
                sqlString = "select * from models where model_name=@1";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", modelName);
                dbReader = cmd.ExecuteReader();
                if (dbReader.HasRows)
                {
                    throw new DAOException("Model name already exist");
                }

                sqlString = "insert into models(model_name) values(@1)";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1",modelName);

                dbReader = cmd.ExecuteReader();
                if (dbReader.RecordsAffected >= 0)
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
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }

        public Response UpdateModel(Model model)
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                conn = DatabaseConnection.GetConnection();
                conn.Open();
                sqlString = "select * from models where model_name=@1";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", model.modelName);
                dbReader = cmd.ExecuteReader();
                if (dbReader.HasRows)
                {
                    throw new DAOException("Model name already exist");
                }

                sqlString = "Update models set model_name=@1 where id=@2";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", model.modelName);
                cmd.Parameters.AddWithValue("@2", model.id);

                int recordsUpdated = cmd.ExecuteNonQuery();
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

        public Response DeleteModel(string modelName)
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                conn = DatabaseConnection.GetConnection();
                conn.Open();
                sqlString = "select * from models where model_name=@1";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", modelName);
                dbReader = cmd.ExecuteReader();
                if (!dbReader.HasRows)
                {
                    throw new DAOException("Model name does not already exist, please add model first!");
                }

                sqlString = "Delete from models where model_name=@1";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", modelName);

                int recordsDeleted = cmd.ExecuteNonQuery();
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

        // CRUD operation on services
        public Response GetAllServices()
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                List<Service> services = new List<Service>();
                conn = DatabaseConnection.GetConnection();
                conn.Open();
                sqlString = "select * from services order by service_name ASC";
                cmd = new OleDbCommand(sqlString, conn);
                dbReader = cmd.ExecuteReader();
                while (dbReader.Read())
                {
                    Service service = new Service();
                    service.id = Int32.Parse(dbReader["ID"].ToString());
                    service.serviceName = dbReader["service_name"].ToString();
                    services.Add(service);
                }
                res.success = true;
                res.isException = false;
                res.body = services;
            }
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }

        public Response AddService(string serviceName)
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                conn = DatabaseConnection.GetConnection();
                conn.Open();
                sqlString = "select * from services where service_name=@1";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", serviceName);
                dbReader = cmd.ExecuteReader();
                if (dbReader.HasRows)
                {
                    throw new DAOException("Service name already exist");
                }

                sqlString = "insert into services(service_name) values(@1)";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", serviceName);

                dbReader = cmd.ExecuteReader();
                if (dbReader.RecordsAffected >= 0)
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
            catch (Exception ex)
            {
                res.success = false;
                res.isException = true;
                res.exception = ex.Message;
            }
            return res;
        }

        public Response UpdateService(Service service)
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                conn = DatabaseConnection.GetConnection();
                conn.Open();
                sqlString = "select * from services where service_name=@1";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", service.serviceName);
                dbReader = cmd.ExecuteReader();
                if (dbReader.HasRows)
                {
                    throw new DAOException("Service name already exist");
                }

                sqlString = "Update services set service_name=@1 where id=@2";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", service.serviceName);
                cmd.Parameters.AddWithValue("@2", service.id);

                int recordsUpdated = cmd.ExecuteNonQuery();
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

        public Response DeleteService(string serviceName)
        {
            Response res = new Response();
            string sqlString = "";
            OleDbConnection conn = null;
            OleDbCommand cmd = null;
            OleDbDataReader dbReader = null;
            try
            {
                conn = DatabaseConnection.GetConnection();
                conn.Open();
                sqlString = "Delete from services where service_name=@1";
                cmd = new OleDbCommand(sqlString, conn);
                cmd.Parameters.AddWithValue("@1", serviceName);

                int recordsDeleted = cmd.ExecuteNonQuery();
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
    }
}
