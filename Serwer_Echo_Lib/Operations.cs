using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace Serwer_Echo_Lib
{
    class Operations
    {

        public static string Login (string Login, string Password)
        {
            string result;
            SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=ServerEcho_Login;Integrated Security=True"); // making connection   
            SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM Accounts WHERE Login='" + Login + "' AND password='" + Password + "'", con);

            DataTable dt = new DataTable(); 
            sda.Fill(dt);
            if (dt.Rows[0][0].ToString() == "1")
            {             
              result = "Logging in was successful !\r\n";
            }
            else
               result = "Invalid username or password !\r\n";
            return result;
        }

        public static string Register (string Login, string Password)
        {

            string result;
            //Użyto 
            SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=ServerEcho_Login;Integrated Security=True"); // making connection (DATABASE NOT PROVIDED!!)
            SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM Accounts WHERE Login='" + Login + "' AND password='" + Password + "'", con);

            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows[0][0].ToString() == "1")
            {
  
                result = "This user already exists !\r\n";
            }
            else
            {
                con.Open();
                String command = "INSERT INTO Accounts(Login,Password) VALUES ('" + Login + "', '" + Password + "')";
                SqlCommand cmd = new SqlCommand(command, con);
                cmd.ExecuteNonQuery();
                con.Close();
                result = "Account Created !\r\n";
            }
            return result;

        }

        
    }
}
