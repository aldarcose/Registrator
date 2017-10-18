using System;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// ������������� �������� ������ ������
    /// </summary>
    public class F005 : BaseObject
    { 
        public F005(Session session)
            : base(session)
        {
        }

        [XafDisplayName("��� �������� ������ ����������� ������")]
        public int IDIDST { get; set; }

        [XafDisplayName("������������ �������� ������ ����������� ������")]
        [Size(254)]
        public string STNAME { get; set; }

        [XafDisplayName("���� ������ �������� ������")]
        public DateTime? DATEBEG { get; set; }

        [XafDisplayName("���� ��������� �������� ������")]
        public DateTime? DATEEND { get; set; }

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            foreach (XElement el in doc.Root.Elements())
            {
                int id = Convert.ToInt32(el.Attribute("IDIDST").Value);
                F005 obj = objSpace.FindObject<F005>(DevExpress.Data.Filtering.CriteriaOperator.Parse("IDIDST=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<F005>();
                    obj.IDIDST = Convert.ToInt32(el.Attribute("IDIDST").Value);
                    obj.DATEBEG = el.Attribute("DATEBEG").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEBEG").Value);
                    obj.DATEEND = el.Attribute("DATEEND").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEEND").Value);
                    obj.STNAME = el.Attribute("STNAME").Value;
                }
            }
        }
    }
}
