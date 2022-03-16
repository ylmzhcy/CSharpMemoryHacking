
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication11
{
    public partial class Form1 : Form
    {

        Fonks _fonks= new Fonks();

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            _fonks.ConnectMe(textBox1.Text);
            //MessageBox.Show("Handle : " + _fonks.deneme.ToString() + " / " + _fonks.GetSunCount().ToString());
            MessageBox.Show(_fonks.message);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _fonks.ChangeSunCount(Convert.ToInt32(textBox2.Text));
        }
    }


}
