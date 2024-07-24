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
using AccountList;
using Server;
namespace Client
{
    
    public partial class LogInForm : Form
    {
        public static string account = "";
        public static string password = "";
        string conn = "Data Source=DESKTOP-CDO0SQ2;Initial Catalog=ACCOUNTMANAGEMENT;Integrated Security=True;Encrypt=False";
        SqlConnection connection;  
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable dt;

        public LogInForm()
        {
            InitializeComponent();
            
            
            connection = new SqlConnection(conn);
            connection.Open();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            if (!isCheck())
            {
                lbShow.ForeColor = Color.Red;

                lbShow.Text = "thông tin không chính xác";
            }
            else
            {
                AccountList.AccountList acc = getAccount();
                account = acc.Account;
                password = acc.Password;
                Form1 form = new Form1();
                form.ShowDialog();
            }
        }
        private  bool isCheck()
        {
            DataTable dt = getElements();
            AccountList.AccountList acc = new AccountList.AccountList(txtAcc.Text,txtPass.Text);
            for(int i  = 0; i < dt.Rows.Count; i++)
            {
                string temp = (string)dt.Rows[i][0];
                string temp1 = (string)dt.Rows[i][1];
                if (acc.Account.CompareTo(temp.Trim()) == 0 
                    && acc.Password.CompareTo(temp1.Trim()) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        private AccountList.AccountList getAccount()
        {
            AccountList.AccountList account = new AccountList.AccountList();
            var sql = "SELECT * FROM ACCOUNTLIST WHERE ACCOUNT = @acc AND PASSWORDD = @pass";
            using(command = new SqlCommand(sql,connection))
            {
                command.Parameters.Add("@acc",txtAcc.Text);
                command.Parameters.Add("@pass", txtPass.Text);
                using(adapter = new SqlDataAdapter(command))
                {
                    dt = new DataTable();
                    adapter.Fill(dt);
                    account.Account = (string)dt.Rows[0][0];
                    account.Password = (string)dt.Rows[0][1];
                    return account;
                }
            }
        }
        private DataTable getElements()
        {
            string sql = "SELECT * FROM ACCOUNTLIST";
            using (command = new SqlCommand(sql, connection))
            {
               
                using(adapter = new SqlDataAdapter(command))
                {
                    dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        private void rbtnShow_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            if(cbShow.Checked)
            {
                txtPass.UseSystemPasswordChar = false;
            }
            else
            {
                txtPass.UseSystemPasswordChar = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            
            
            
        }

        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            RegisterForm form = new RegisterForm();
            form.ShowDialog();
        }

        private void LogInForm_Load(object sender, EventArgs e)
        {

        }
    }
}
