using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        string conn = "Data Source=DESKTOP-CDO0SQ2;Initial Catalog=ACCOUNTMANAGEMENT;Integrated Security=True;Encrypt=False";
        SqlConnection connection;
        SqlCommand cmd;
        SqlDataAdapter adapter;

        public static string account = "";
        public static string password = "";
        string group = "";
        IPEndPoint end;
        Socket socket;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            account = LogInForm.account.Trim();
            lbAcc.Text = "Tên đăng nhập :" + account;
            password = LogInForm.password.Trim();
            connect();
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            if (group == "")
            {
                send(account+" : "+txtInfo.Text,false);
            }
            else
            {
                sendGroup();
            }
        }
        void connect()
        {
            
            end = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(end);


            }
            catch (Exception ex)
            {
                MessageBox.Show("lôi " + ex);
            }
            Thread listen = new Thread(receive);


            listen.IsBackground = true;

            listen.Start();

        }
        void close()
        {
            socket.Close();
        }
        void send(string text,bool isGroup)
        {
            if (text != "")
            {
                 
                socket.Send(serialize(text));

                if (isGroup)
                {
                    string temp = text.Substring(10);
                    listView2.Items.Add(temp);
                }
                else
                {
                    listView1.Items.Add(text);
                }
                txtInfo.Text = "";

            }
        }
        void sendGroup()
        {
            send("GROUP:"+group+":"+account+" : "+txtInfo.Text,true);
        }
        void receive()
        {


            try
            {
                while (true)
                {
                    var buffer = new byte[1024];
                    var bytesRead = socket.Receive(buffer);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (message.StartsWith("FILE:"))
                    {
                        string fileName = message.Substring(5);
                        receiveFile(fileName);
                    }else if (message.StartsWith("GROUP"))
                    {
                        
                        string temp = message.Substring(6,3);
                        message = message.Substring(10);
                        
                        if (group.CompareTo(temp) == 0)
                        {
                            listView2.Items.Add(message);
                            var filePath = createFile(group);
                            writeToFile(filePath,message);
                        }
                    }
                    else
                    {
                        listView1.Items.Add(message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        void writeToFile(string filePath,string mess)
        {
            using(FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(mess);
                }
            }
        }
        string createFile(string group)
        {
            string filePath = "D:\\Lập Trình\\Lập Trình Mạng\\file\\chathistory\\"+group;
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) ;
            }
            return filePath;
        }
        void receiveFile(string fileName)
        {
            string path = "D:\\Lập Trình\\Lập Trình Mạng\\file";
            using (FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.Create, FileAccess.Write))
            {
                var buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = socket.Receive(buffer)) > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    if (bytesRead < buffer.Length) break;
                }
            }

            listView1.Items.Add("Đã nhận file: " + fileName);
        }
        byte[] serialize(object obj)
        {
            string text = (string)obj;
            return Encoding.UTF8.GetBytes(text);
        }
        object deserialize(byte[] data, int temp)
        {
            string text = Encoding.UTF8.GetString(data, 0, temp);
            return text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable dt = getElements();
            dataGridView1.DataSource = dt;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            close();
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {


        }

        private void btnBrowser_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    sendFile(filePath);
                }
            }
        }
        void sendFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                byte[] fileNameBytes = Encoding.UTF8.GetBytes("FILE:" + Path.GetFileName(filePath));
                socket.Send(fileNameBytes);

                byte[] fileBytes = File.ReadAllBytes(filePath);
                socket.Send(fileBytes);

                listView1.Items.Add("Đã gửi file: " + filePath);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            CreateGroupForm form = new CreateGroupForm();
            form.ShowDialog();
        }

        private void cbo_Click(object sender, EventArgs e)
        {
            
            
        }
        public DataTable getElements()
        {
            DataTable dt = new DataTable();
            using (connection = new SqlConnection(conn))
            {
                connection.Open();
                var sql = "SELECT * FROM CHATGROUP";
                using (cmd = new SqlCommand(sql, connection))
                {
                    using (adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private void cbo_SelectionChangeCommitted(object sender, EventArgs e)
        {
           
            
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            
        }

        private void cbo_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        void readFromFile(string filePath,ListView list)
        {
            using(FileStream fs = new FileStream(filePath,FileMode.Open, FileAccess.Read))
            {
                string line;
                using(StreamReader reader = new StreamReader(fs))
                {
                    
                    while((line = reader.ReadLine()) != null)
                    {
                        list.Items.Add(line);
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dataGridView1.SelectedRows[0];
            var detail = row.Cells[0].Value.ToString().Trim();
            if(group.CompareTo(detail) != 0)
            {
                group = detail;
                string filePath = createFile(group);

                readFromFile(filePath, listView2);
            }
        }
    }
}
