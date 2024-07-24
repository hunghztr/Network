using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class RegisterForm : Form
    {
        string conn = "Data Source=DESKTOP-CDO0SQ2;Initial Catalog=ACCOUNTMANAGEMENT;Integrated Security=True;Encrypt=False";
        SqlConnection connection;
        SqlCommand command;
        public RegisterForm()
        {
            InitializeComponent();
            connection = new SqlConnection(conn);
            connection.Open();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if(txtIPassword.Text.CompareTo(txtPassword.Text) != 0)
            {
                lbShow.Text = "Mật khẩu không trùng khớp";
            }
            else
            {
                bool isSuccess = addAccount();
                if(isSuccess)
                {
                    MessageBox.Show("đăng kí thành công");
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("tài khoản đã tồn tại");
                }
            }
        }
        private bool addAccount()
        {
            string sql = "INSERT INTO ACCOUNTLIST VALUES (@acc,@pass)";
            using(command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@acc",txtAccount.Text);
                command.Parameters.Add("@pass", txtPassword.Text);
                try
                {
                    int row = command.ExecuteNonQuery();
                    if (row > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }
    }
}
