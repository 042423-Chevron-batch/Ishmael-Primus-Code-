using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Prj1_Models;

namespace Prj1_Repository
{
    public class Prj1_RepoLayerClassLibrary
    {

        private static SqlConnection flubby { get; set; } = new SqlConnection("Server=tcp:052123-batch-server.database.windows.net,1433; Initial Catalog=052123-batch-db;Persist Security Info=False;User ID=batch052123;Password=Nubian19;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        /// <summary>
        /// Get a list of all the stores. This doesn not include the inventory.
        /// </summary>
        /// <returns></returns>
        public List<Store> GetStores()
        {
            SqlCommand comm = new SqlCommand("SELECT * FROM Stores;", flubby);
            flubby.Open();
            List<Store> stores = new List<Store>();// create the person obj
            SqlDataReader res;
            try
            {
                res = comm.ExecuteReader();// envoke the query
                while (res.Read())
                {
                    stores.Add(new Store(res.GetGuid(0), res.GetString(1), res.GetString(2)));
                }
                flubby.Close();
                return stores!;
            }
            catch (SqlException ex)
            {
                // write this exception to a file, exception.
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return stores;// in the business layer, we will check to see if the list length is 0. if 0, there was an error.
            }
        }

        /// <summary>
        /// takes the username and password of a user and returns an instance of hte user Person obj.
        /// of no user exists, returns null.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Person GetUserByUnamePword(string userName, string password)
        {
            // check to see if thise username/password combo is in the Db.
            SqlCommand comm = new SqlCommand("SELECT TOP 1 * FROM Customer WHERE UserName = @username AND Password = @password;", flubby);
            comm.Parameters.AddWithValue("@username", userName);
            comm.Parameters.AddWithValue("@password", password);
            flubby.Open();
            Person? p = null;// create the person obj
            SqlDataReader res;
            try
            {
                res = comm.ExecuteReader();// envoke the query
                if (res.Read())
                {
                    p = new Person(res.GetGuid(0), res.GetString(1), res.GetString(2), res.GetDateTime(3), res.GetString(4), res.GetString(5), res.GetString(6));
                }
                flubby.Close();
                return p!;
            }
            catch (SqlException ex)
            {
                // write this exception to a file, exception.
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return p!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return p!;
            }
        }


        public int RegisterNewCustomer(Person p)
        {
            // create the query
            SqlCommand comm = new SqlCommand("INSERT INTO Customer values(@cid, @fn, @ln, @lod, @r, @un, @pw);", flubby);
            comm.Parameters.AddWithValue("@fn", p.FirstName);
            comm.Parameters.AddWithValue("@ln", p.LastName);
            comm.Parameters.AddWithValue("@cid", p.CustomerId);
            comm.Parameters.AddWithValue("@r", p.Remarks);
            comm.Parameters.AddWithValue("@lod", p.LastOrderDate);
            comm.Parameters.AddWithValue("@un", p.UserName);
            comm.Parameters.AddWithValue("@pw", p.Password);
            flubby.Open();
            int res;
            try
            {
                res = comm.ExecuteNonQuery();
                if (res == 1)
                {
                    flubby.Close();
                    return res;
                }
                else
                {
                    flubby.Close();
                    return 0;
                }
            }
            catch (SqlException ex)
            {
                // write this exception to a file, exception.
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return 0;
            }
        }

        /// <summary>
        /// this method uses the store id to retrieve all the store data including it's inventory.
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public Store StoreInfo(Guid storeId)
        {
            SqlCommand comm = new SqlCommand("SELECT s.StoreId, s.Name, s.Address, p.ProductId, p.Name,p.Description, spj.Quantity FROM [dbo].[Stores] s LEFT JOIN [dbo].[StoreProductJunction] spj ON s.StoreId = spj.StoreId LEFT JOIN [dbo].[Products] p ON spj.ProductId = p.ProductId WHERE s.StoreId = @storeId;", flubby);
            comm.Parameters.AddWithValue("@storeId", storeId);
            flubby.Open();
            Store? s = null;// create the person obj
            SqlDataReader res;
            try
            {
                res = comm.ExecuteReader();// envoke the query
                if (res.Read())
                {//get the store data and send it to the Mapper
                    s = Mapper.AdoToStore(res.GetGuid(0), res.GetString(1), res.GetString(2));
                    do
                    {// a do/while loop will run at least once.
                        //Console.WriteLine($"the values are {res.GetGuid(3)} {res.GetString(4)} {res.GetInt32(6)} {res.GetString(5)}");
                        Guid id = res.GetGuid(3);
                        string name = res.GetString(4);
                        int q = res.GetInt32(6);
                        string desc = res.GetString(5);
                        Product p = Mapper.AdoToProduct(id, name, q, desc);
                        s.Inventory?.Add(p);

										} while (res.Read());
                    flubby.Close();
                    return s!;
                }
                else
                {
                    flubby.Close();
                    return s!;
                }
            }
            catch (SqlException ex)
            {
                // write this exception to a file, exception.
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return s!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return s!;
            }
        }

        /// <summary>
        /// takes a username and password combination.
        /// Returns false of that combination is not in the Db.
        /// Otherwise, returns true
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool UserNamePasswordInDb(string username, string password)
        {
            // check to see if this username/password combo is in the Db.
            SqlCommand comm = new SqlCommand("SELECT * FROM Customer WHERE UserName = @username AND Password = @password;", flubby);
            comm.Parameters.AddWithValue("@username", username);
            comm.Parameters.AddWithValue("@password", password);
            flubby.Open();
            SqlDataReader res;
            try
            {
                res = comm.ExecuteReader();
                if (!res.Read())
                {
                    flubby.Close();
                    return false;
                }
                else
                {
                    flubby.Close();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                // write this exception to a file, exception.
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return false;
            }
        }

        //Get the products from the Db
        public List<Product> GetProducts()
        {
            SqlCommand comm = new SqlCommand("SELECT * FROM Products;", flubby);
            flubby.Open();
            List<Product> products = new List<Product>();
            SqlDataReader res;
            try
            {
                res = comm.ExecuteReader();
                if (res.Read())
                {
                    do
                    {
                        Guid id = res.GetGuid(0);
                        string name = res.GetString(1);
                        int q = res.GetInt32(2);
                        string desc = res.GetString(3);
                        Product p = Mapper.AdoToProduct(id, name, q, desc);
                        products.Add(p);
                    } while (res.Read());
                    flubby.Close();
                    return products;
                }
                else
                {
                    flubby.Close();
                    return products;
                }
            }
            catch (SqlException ex)
            {
                // write this exception to a file, exception.
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return products;
            }
        }

        // Select a store by its address
        public Store GetStoreByAddress(string address)
        {
            SqlCommand comm = new SqlCommand("SELECT * FROM Stores WHERE Address = @address;", flubby);
            comm.Parameters.AddWithValue("@address", address);
            flubby.Open();
            SqlDataReader res;
            try
            {
                res = comm.ExecuteReader();
                if (res.Read())
                {
                    Guid id = res.GetGuid(0);
                    string name = res.GetString(1);
                    string addr = res.GetString(2);
                    Store s = Mapper.AdoToStore(id, name, addr);
                    flubby.Close();
                    return s;
                }
                else
                {
                    flubby.Close();
                    return null;
                }
            }
            catch (SqlException ex)
            {
                // write this exception to a file, exception.
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return null;
            }
        }

        // Registered user can select a store by its address
        public Store GetStoreByAddress(string address, Guid customerId)
        {
            SqlCommand comm = new SqlCommand("SELECT * FROM Stores WHERE Address = @address;", flubby);
            comm.Parameters.AddWithValue("@address", address);
            flubby.Open();
            SqlDataReader res;
            try
            {
                res = comm.ExecuteReader();
                if (res.Read())
                {
                    Guid id = res.GetGuid(0);
                    string name = res.GetString(1);
                    string addr = res.GetString(2);
                    Store s = Mapper.AdoToStore(id, name, addr);
                    flubby.Close();
                    return s;
                }
                else
                {
                    flubby.Close();
                    return null;
                }
            }
            catch (SqlException ex)
            {
                // write this exception to a file, exception.
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"the exception was {ex.Message} - {ex.InnerException}");
                flubby.Close();
                return null;
            }
        }


    }// EoC
}// EoN