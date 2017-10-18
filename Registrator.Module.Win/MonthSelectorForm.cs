using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registrator.Module.Win
{
    public partial class MonthSelectorForm : Form
    {
        public MonthSelectorForm()
        {
            InitializeComponent();
        }

        public DateTime SelectedDateTime
        {
            get { return dateEdit1.DateTime; }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            this.DialogResult = DialogResult.OK;
        }
    }
}
