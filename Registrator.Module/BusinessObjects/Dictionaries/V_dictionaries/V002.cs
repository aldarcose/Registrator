using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using System.Xml.Linq;
using DevExpress.Persistent.Validation;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// ���������� ���������
    /// </summary>
    public class V002 : DevExpress.Persistent.BaseImpl.BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (http://documentation.devexpress.com/#Xaf/CustomDocument3146).
        public V002(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (http://documentation.devexpress.com/#Xaf/CustomDocument2834).
        }


        [XafDisplayName("��� ����������� ���������")]
        public int IDPR { get; set; }

        [XafDisplayName("������������ ����������� ���������")]
        [Size(512)]
        public string PRNAME { get; set; }

        [XafDisplayName("���� ������ �������� ������")]
        public DateTime? DATEBEG { get; set; }

        [XafDisplayName("���� ��������� �������� ������")]
        public DateTime? DATEEND { get; set; }

        public override string ToString()
        {
            return PRNAME;
        }

        // ������ ������ � XML:
        // <zap IDPR="1" PRNAME="�������� (�������������)" DATEBEG="01.01.2011" DATEEND="" />
        // <zap IDPR="2" PRNAME="���������� � �����������" DATEBEG="01.01.2011" DATEEND="08.07.2013" />
        // <zap IDPR="3" PRNAME="����������� ����" DATEBEG="01.01.2011" DATEEND="" />
        // <zap IDPR="4" PRNAME="������������ � �����������" DATEBEG="01.01.2011" DATEEND="" />
        // <zap IDPR="5" PRNAME="�������������� � ��������������" DATEBEG="01.01.2011" DATEEND="" />
        // <zap IDPR="6" PRNAME="�������������" DATEBEG="01.01.2011" DATEEND="" />

        /// <summary>
        /// ��������� � ���� �������������� �� ����� XML
        /// </summary>
        /// <param name="updater">������������ �������� ObjectSpace</param>
        /// <param name="xmlPath">���� �� ����� ��������������</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            const string elementNameStartsWith = "rec";

            const string code_attr = "IDPR";
            const string name_attr = "PRNAME";

            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                V002 obj = objSpace.FindObject<V002>(DevExpress.Data.Filtering.CriteriaOperator.Parse("IDPR=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<V002>();
                    obj.IDPR = int.Parse(element.Attribute(code_attr).Value);
                    obj.PRNAME = element.Attribute(name_attr).Value;

                    obj.DATEBEG = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DATEEND = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}
