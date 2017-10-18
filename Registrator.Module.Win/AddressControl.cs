using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Registrator.Module.BusinessObjects;

namespace Registrator.Module.Win
{
    public partial class AddressControl : UserControl
    {
        public AddressControl()
        {
            InitializeComponent();
            
        }

        public Address EditAddress { get; set; }

        public void Init()
        {
            buttonEdit1.Text = EditAddress.Level1.Name;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("123123");
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Clicked!");
        }
    }
}
