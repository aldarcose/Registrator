using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.DTO
{
    [DefaultClassOptions]
    public partial class ZGLV : BaseObject
    {
        public ZGLV()
        {
        }
        public ZGLV(Session session)
            : base(session)
        {
        }

        private string vERSIONField;

        private string dATAField;

        private string fILENAMEField;

        private string fILENAME1Field;

        public string VERSION
        {
            get
            {
                return this.vERSIONField;
            }
            set
            {
                this.vERSIONField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DATA
        {
            get
            {
                return this.dATAField;
            }
            set
            {
                this.dATAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FILENAME
        {
            get
            {
                return this.fILENAMEField;
            }
            set
            {
                this.fILENAMEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FILENAME1
        {
            get
            {
                return this.fILENAME1Field;
            }
            set
            {
                this.fILENAME1Field = value;
            }
        }

        public override string ToString()
        {
            return String.Format("Версия: {0}, дата: {1}, имя файла: {2}", VERSION, DATA, FILENAME);
        }
    }
}
