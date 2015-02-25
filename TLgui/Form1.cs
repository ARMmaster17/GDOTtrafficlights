using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TLgui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Task.Delay(5000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (NamedPipeClientStream client = new NamedPipeClientStream("GDOTTL")) //creates a disposable interface that allows for other connections when not active
            {
                client.Connect();
                StreamReader reader = new StreamReader(client);
                StreamWriter writer = new StreamWriter(client);
                writer.WriteLine(textBox1.Text);
                writer.Flush();
                textBox1.Text = "";
                textBox2.Text += reader.ReadLine() + Environment.NewLine;
                client.Close();
            }
        }
    }
}
