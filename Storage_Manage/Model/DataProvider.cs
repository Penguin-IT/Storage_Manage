using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.ComponentModel.DataAnnotations;

namespace Storage_Manage.Model
{
    public class DataProvider
    {
        // 1. Chuỗi kết nối đến SQL Server
        private string connectionString = @"Data Source=localhost\MSSQLSERVER01;Initial Catalog=QL_KHO;Integrated Security=True";

        // 2. Khai báo Instance (Singleton) - ĐÂY LÀ PHẦN BẠN ĐANG THIẾU
        private static DataProvider _instance;
        public static DataProvider Instance
        {
            get
            {
                if (_instance == null) _instance = new DataProvider();
                return _instance;
            }
        }

        // 3. Hàm hỗ trợ gắn tham số (Parameter)
        private void AddParameters(SqlCommand command, string query, object[] parameter)
        {
            if (parameter != null)
            {
                string[] listPara = query.Split(new[] { ' ', ',', '(', ')', '=' }, StringSplitOptions.RemoveEmptyEntries);
                int i = 0;
                foreach (string item in listPara)
                {
                    if (item.StartsWith("@"))
                    {
                        command.Parameters.AddWithValue(item, parameter[i]);
                        i++;
                    }
                }
            }
        }

        // 4. Hàm thực thi lệnh SELECT (trả về DataTable)
        public DataTable ExecuteQuery(string query, object[] parameter = null)
        {
            DataTable data = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    AddParameters(command, query, parameter);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ExecuteQuery: " + ex.Message);
            }
            return data;
        }

        // 5. Hàm thực thi lệnh INSERT/UPDATE/DELETE (trả về số dòng bị ảnh hưởng)
        public int ExecuteNonQuery(string query, object[] parameter = null)
        {
            int data = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    AddParameters(command, query, parameter);
                    data = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ExecuteNonQuery: " + ex.Message);
            }
            return data;
        }
    }
}