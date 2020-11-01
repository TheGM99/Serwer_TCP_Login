using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Serwer_Echo_Lib
{
    class Operations
    {

        public static string Login (string Login, string Password)
        {
            string result;
            SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=ServerEcho_Login;Integrated Security=True"); // making connection   
            SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM Accounts WHERE Login='" + Login + "' AND password='" + Password + "'", con);

            DataTable dt = new DataTable(); //this is creating a virtual table  
            sda.Fill(dt);
            if (dt.Rows[0][0].ToString() == "1")
            {
                /* I have made a new page called home page. If the user is successfully authenticated then the form will be moved to the next form */
              result = "Logging in was successful !";
            }
            else
               result = "Invalid username or password !";
            return result;
        }

        public static string Register (string Login, string Password)
        {

            string result;
            SqlConnection con = new SqlConnection(@"Data Source=.;Initial Catalog=ServerEcho_Login;Integrated Security=True"); // making connection   
            SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM Accounts WHERE Login='" + Login + "' AND password='" + Password + "'", con);

            DataTable dt = new DataTable(); //this is creating a virtual table  
            sda.Fill(dt);
            if (dt.Rows[0][0].ToString() == "1")
            {
                /* I have made a new page called home page. If the user is successfully authenticated then the form will be moved to the next form */
                result = "This user already exists !";
            }
            else
            {
                con.Open();
                String command = "INSERT INTO Accounts(Login,Password) VALUES ('" + Login + "', '" + Password + "')";
                SqlCommand cmd = new SqlCommand(command, con);
                cmd.ExecuteNonQuery();
                con.Close();
                result = "Account Created !";
            }
            return result;

        }
    }
}
