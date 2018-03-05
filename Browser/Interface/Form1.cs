using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Browser.Database;
using Browser.Utilities;

namespace Browser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            splitContainer1.Top = 200;
            this.Size = new Size(1000, 600);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            splitContainer1.Top = 10;
            this.Size = new Size(1000, 800);
            Crawler cr = new Crawler();
            cr.indexFilesAndDirectories();
        }
    }
}
