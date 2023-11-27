using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace Freya_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private TcpClient tcpClient;

        private void ConnectToServer()
        {
            tcpClient = new TcpClient();

            try
            {
                tcpClient.Connect("127.0.0.1", 6606);
                SendClientInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }

        string antivirus = null;

        private void SendClientInfo()
        {
            
            GetAntivirus();

            // Rastgele bilgiler oluştur
            Random random = new Random();
            string clientInfo = GetRandom4DigitNumber() + "|" + Environment.MachineName + "|" + GetLocalIPAddress() + "|" + DateTime.Now.ToString() + "|" + antivirus;

            // Bilgileri sunucuya gönder
            byte[] data = Encoding.UTF8.GetBytes(clientInfo);
            tcpClient.GetStream().Write(data, 0, data.Length);

            tcpClient.Close();
        }

        private string GetLocalIPAddress()
        {
            return "127.0.0.1";
        }

        public void GetAntivirus()
        {
            ManagementObjectSearcher wmiData = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
            ManagementObjectCollection data = wmiData.Get();

            foreach (ManagementObject virusChecker in data)
            {
                antivirus = virusChecker["displayName"].ToString();
            }
        }

        private string GetRandom4DigitNumber()
        {
            Random random = new Random();
            return random.Next(1000, 10000).ToString("D4");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
