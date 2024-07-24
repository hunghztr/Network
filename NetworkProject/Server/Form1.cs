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
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Server
{
    public partial class Form1 : Form
    {

        IPEndPoint end;
        Socket socket;
        List<Socket> sockets;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            connect();
        }





        void connect()
        {
            sockets = new List<Socket>();
            end = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(end);
            Thread listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        socket.Listen(100);
                        Socket client = socket.Accept();


                        sockets.Add(client);
                        Thread re = new Thread(receive);


                        re.IsBackground = true;

                        re.Start(client);

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("lỗi " + ex);
                }
            });
            listen.IsBackground = true;
            listen.Start();
        }
        void close()
        {
            socket.Close();
        }
        void send(Socket s, string text)
        {

            s.Send(serialize(text));



        }
        void receive(object obj)
        {
            Socket s = (Socket)obj;
            try
            {
                while (true)
                {
                    var buffer = new byte[1024];
                    var temp = s.Receive(buffer);
                    string text = (string)deserialize(buffer, temp);

                    if (text.StartsWith("FILE:"))
                    {
                        string fileName = text.Substring(5);
                        receiveFile(s, fileName);
                    }
                    
                    
                    else
                    {
                        foreach (var so in sockets)
                        {
                            if (so != s)
                            {
                                send(so, text);
                            }
                        }
                        listView1.Items.Add(text);
                       

                    }
                }
            }
            catch (Exception ex)
            {
                sockets.Remove(s);
                s.Close();
            }

        }



        private void btnSend_Click(object sender, EventArgs e)
        {

            foreach (var s in sockets)
            {
                send(s, txtInfo.Text);
            }
            listView1.Items.Add(txtInfo.Text);
            txtInfo.Text = "";
        }

        byte[] serialize(object obj)
        {
            string text = (string)obj;
            return Encoding.UTF8.GetBytes(text);
        }
        object deserialize(byte[] data, int temp)
        {
            string text = Encoding.ASCII.GetString(data, 0, temp);
            return text;
        }
       

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            close();
        }
        void sendFile(Socket s, string filePath)
        {
            if (File.Exists(filePath))
            {
                byte[] fileNameBytes = Encoding.UTF8.GetBytes("FILE:" + Path.GetFileName(filePath));
                s.Send(fileNameBytes);

                byte[] fileBytes = File.ReadAllBytes(filePath);
                s.Send(fileBytes);

                listView1.Items.Add("Đã gửi file: " + filePath);
            }
        }

        void receiveFile(Socket s, string fileName)
        {
            string path = "D:\\Lập Trình\\Lập Trình Mạng\\file\\fileforserver";
            string filePath = Path.Combine(path, fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = s.Receive(buffer)) > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    if (bytesRead < buffer.Length) break;
                }

            }
            listView1.Items.Add("Đã nhận file: " + fileName);
            foreach (var client in sockets)
            {
                if (client != s)
                {
                    sendFile(client, filePath);
                }
            }
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    foreach (var s in sockets)
                    {
                        sendFile(s, filePath);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
