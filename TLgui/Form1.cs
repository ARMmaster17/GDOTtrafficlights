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
using TLpserver;

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
            string result = pipeManager.sendMsg(textBox1.Text);
            textBox1.Text = "";
            textBox2.Text += result + Environment.NewLine;
        }
    }
}
