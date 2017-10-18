using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Registrator.Module.BusinessObjects;
using DevExpress.XtraEditors;

namespace Registrator.Module.Win
{
    public partial class AddressSelectorForm : DevExpress.XtraEditors.XtraForm
    {
        private Address _editAddress = null;
        public AddressSelectorForm(Address editAddress)
        {
            InitializeComponent();
            _editAddress = editAddress;

            Init();
        }

        public Address GetAddress()
        {
            return _editAddress;
        }

        public void Init()
        {
            /*regionText.Text = _editAddress.Level1.Name;
            areaText.Text = _editAddress.Level2.Name;
            cityText.Text = _editAddress.Level3.Name;
            townText.Text = _editAddress.Level4.Name;

            houseText.Text = _editAddress.House;
            buildText.Text = _editAddress.Build;
            flatText.Text = _editAddress.Flat;*/
        }
    }
}