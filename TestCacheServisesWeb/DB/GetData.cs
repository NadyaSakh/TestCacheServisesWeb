using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TestCacheServisesWeb.DB
{
    public class GetData
    {
        public static async Task<List<string>> QueryGetCitiesAsync()
        {
            List<string> shipCityList = new List<string>();
            using (SqlConnection conn = DBUtils.GetDBConnection())
            {
                await conn.OpenAsync();
                try
                {
                    string sqlExpression = "SELECT ShipCity FROM Orders;";

                    SqlCommand cmd = new SqlCommand(sqlExpression, conn);
                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync()) // построчно считываем данные
                        {
                            object city = reader.GetValue(0);
                            shipCityList.Add(city.ToString());
                        }
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in QueryGetCities: " + e);
                    Console.WriteLine(e.StackTrace);
                }
                conn.Close();
            }
            return shipCityList;
        }

        public static async Task<Dictionary<string, object>> QueryGetOrderInfoAsync()
        {
            Dictionary<string, object> orderInfo = new Dictionary<string, object>(14);
            using (SqlConnection conn = DBUtils.GetDBConnection())
            {
                await conn.OpenAsync();
                try
                {
                    string sqlExpression = "SELECT * FROM Orders where OrderID = 10248;";

                    SqlCommand cmd = new SqlCommand(sqlExpression, conn);
                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            object orderID = reader["OrderID"];
                            object customerID = reader["CustomerID"];
                            object employeeID = reader["EmployeeID"];
                            object orderDate = reader["OrderDate"];
                            object requiredDate = reader["RequiredDate"];
                            object shippedDate = reader["ShippedDate"];
                            object shipVia = reader["ShipVia"];
                            object freight = reader["Freight"];
                            object shipName = reader["ShipName"];
                            object shipAddress = reader["ShipAddress"];
                            object shipCity = reader["ShipCity"];
                            object shipRegion = reader["ShipRegion"];
                            object shipPostalCode = reader["ShipPostalCode"];
                            object shipCountry = reader["ShipCountry"];

                            orderInfo.Add("OrderID", orderID);
                            orderInfo.Add("CustomerID", customerID);
                            orderInfo.Add("EmployeeID", employeeID);
                            orderInfo.Add("OrderDate", orderDate);
                            orderInfo.Add("RequiredDate", requiredDate);
                            orderInfo.Add("ShippedDate", shippedDate);
                            orderInfo.Add("ShipVia", shipVia);
                            orderInfo.Add("Freight", freight);
                            orderInfo.Add("ShipName", shipName);
                            orderInfo.Add("ShipAddress", shipAddress);
                            orderInfo.Add("ShipCity", shipCity);
                            orderInfo.Add("ShipRegion", shipRegion);
                            orderInfo.Add("ShipPostalCode", shipPostalCode);
                            orderInfo.Add("ShipCountry", shipCountry);
                        }
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in QueryGetCities: " + e);
                    Console.WriteLine(e.StackTrace);
                }
                conn.Close();
            }
            return orderInfo;
        }

        public static async Task<List<Dictionary<string, object>>> QueryGetOrdersInfoAsync()
        {
            List<Dictionary<string, object>> shipCityList = new List<Dictionary<string, object>>();
            Dictionary<string, object> orderInfo;
            using (SqlConnection conn = DBUtils.GetDBConnection())
            {
                await conn.OpenAsync();
                try
                {// 10250
                    string sqlExpression = "SELECT * FROM Orders where OrderID >= 10248 and OrderID <= 10300;";

                    SqlCommand cmd = new SqlCommand(sqlExpression, conn);
                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            orderInfo = new Dictionary<string, object>(14);

                            orderInfo.Add("OrderID", reader["OrderID"]);
                            orderInfo.Add("CustomerID", reader["CustomerID"]);
                            orderInfo.Add("EmployeeID", reader["EmployeeID"]);
                            orderInfo.Add("OrderDate", reader["OrderDate"]);
                            orderInfo.Add("RequiredDate", reader["RequiredDate"]);
                            orderInfo.Add("ShippedDate", reader["ShippedDate"]);
                            orderInfo.Add("ShipVia", reader["ShipVia"]);
                            orderInfo.Add("Freight", reader["Freight"]);
                            orderInfo.Add("ShipName", reader["ShipName"]);
                            orderInfo.Add("ShipAddress", reader["ShipAddress"]);
                            orderInfo.Add("ShipCity", reader["ShipCity"]);
                            orderInfo.Add("ShipRegion", reader["ShipRegion"]);
                            orderInfo.Add("ShipPostalCode", reader["ShipPostalCode"]);
                            orderInfo.Add("ShipCountry", reader["ShipCountry"]);

                            shipCityList.Add(orderInfo);
                        }
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in QueryGetCities: " + e);
                    Console.WriteLine(e.StackTrace);
                }
                conn.Close();
            }
            return shipCityList;
        }
    }
}
