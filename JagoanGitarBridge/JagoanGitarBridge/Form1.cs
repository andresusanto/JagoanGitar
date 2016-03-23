using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading; 

namespace JagoanGitarBridge
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        private string PortName = "COM2";
        private int BaudRate = 9600;
        private StringBuilder tmpData;
        private Socket dataSender;

        public Form1()
        {
            InitializeComponent();
            tmpData = new StringBuilder();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort = new SerialPort(PortName, BaudRate);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
            serialPort.Open();
            System.Console.WriteLine(serialPort.IsOpen);

            dataSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            dataSender.Connect("127.0.0.1", 1231);

            //byte[] msg = Encoding.ASCII.GetBytes("0.24\n");
            //dataSender.Send(msg);

            //Thread.Sleep(1000);
            //dataSender.Send(msg);
        }

        private void dataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            
            string receivedData = serialPort.ReadExisting();
            tmpData.Append(receivedData);

            if (receivedData.Contains("\n"))
            {
                string strToSend = tmpData.ToString();
                if (!strToSend.Contains("clip"))
                {
                    strToSend = strToSend.Replace("hz", "").Replace("\n", "").Trim() + "\n";

                    Console.WriteLine(strToSend);
                    byte[] msg = Encoding.ASCII.GetBytes(strToSend);
                    dataSender.Send(msg);
                }
                tmpData.Clear();
            }
        }
    }
}
