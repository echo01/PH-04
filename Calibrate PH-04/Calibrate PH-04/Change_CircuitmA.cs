using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calibrate_PH_04
{
    public partial class Change_CircuitmA : Form
    {
        public void set_text(string msg)
        {
            label1.Text = msg;
        }

        public Change_CircuitmA()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
