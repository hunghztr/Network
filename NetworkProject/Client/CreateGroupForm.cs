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
    public partial class CreateGroupForm : Form
    {
        public static string acc;
        string conn = "Data Source=DESKTOP-CDO0SQ2;Initial Catalog=ACCOUNTMANAGEMENT;Integrated Security=True;Encrypt=False";
        SqlConnection connection;
        SqlCommand cmd;
        public CreateGroupForm()
        {
            InitializeComponent();
            acc = Form1.account;
            connection = new SqlConnection(conn);
            connection.Open();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
           bool isSuccess = addGroup();
           
            if(isSuccess) {
                MessageBox.Show("tạo thành công");
                
                txtName.Text = "";
            }
            else
            {
                MessageBox.Show("tạo thất bại");
            }
        }
       
        private bool addGroup()
        {
            var sql = "INSERT INTO CHATGROUP VALUES (@name)";
            using (cmd = new SqlCommand(sql, connection))
            {
                
                cmd.Parameters.Add("@name", txtName.Text);
                int row = cmd.ExecuteNonQuery();
                if(row > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void CreateGroupForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
