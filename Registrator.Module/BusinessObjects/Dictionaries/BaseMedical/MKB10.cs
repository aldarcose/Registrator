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
    [DefaultClassOptions]
    public class MKB10 : DevExpress.Persistent.BaseImpl.BaseObject
    {
        public MKB10(Session session) : base(session) { }

        [XafDisplayName("Код")]
        public string CODE { get; set; }

        [XafDisplayName("Название")]
        public string NAME { get; set; }
        [XafDisplayName("Key")]
        public string KEY { get; set; }

        [XafDisplayName("High")]
        public string High { get; set; }

        [XafDisplayName("NXT")]
        public string NXT { get; set; }
        [XafDisplayName("MKB")]
        public string MKB { get; set; }
        [XafDisplayName("SEX")]
        public string SEX { get; set; }
        [XafDisplayName("SP")]
        public int SP { get; set; }
        [XafDisplayName("AUTOP")]
        public string AUTOP { get; set; }
        [XafDisplayName("OMS")]
        public int OMS { get; set; }
        [XafDisplayName("OMSD")]
        public int OMSD { get; set; }
        [XafDisplayName("REG")]
        public int REG { get; set; }
        [XafDisplayName("KD")]
        public float KD { get; set; }
        [XafDisplayName("MIN_KD")]
        public float MIN_KD { get; set; }
        [XafDisplayName("KP")]
        public int KP { get; set; }
        [XafDisplayName("DOKD")]
        public float DOKD { get; set; }
        [XafDisplayName("DOKP")]
        public int DOKP { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", MKB, NAME);
        }
        
        /// <summary>
        /// Добавляет в базу классификаторы из файла XML
        /// </summary>
        /// <param name="updater">Пространство объектов ObjectSpace</param>
        /// <param name="xmlPath">Путь до файла классификатора</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            foreach (XElement el in doc.Root.Element("ROWDATA").Elements("ROW"))
            {
                // int id = Convert.ToInt32(el.Attribute("mkb").Value);
                // MKB10 obj = ObjectSpace.FindObject<MKB10>(CriteriaOperator.Parse("mkb=?", id));
                // if (obj == null)

                MKB10 obj = objSpace.CreateObject<MKB10>();
                obj.CODE = el.Attribute("code").Value;
                obj.NAME = el.Attribute("name").Value;
                obj.KEY = el.Attribute("key").Value;
                obj.High = el.Attribute("high").Value;
                obj.NXT = el.Attribute("nxt").Value;
                obj.MKB = el.Attribute("mkb").Value;
                obj.SEX = el.Attribute("sex").Value;
                obj.SP = Convert.ToInt32(el.Attribute("sp").Value);
                obj.AUTOP = el.Attribute("autop").Value;
                obj.OMS = Convert.ToInt32(el.Attribute("oms").Value);
                obj.OMSD = Convert.ToInt32(el.Attribute("omsd").Value);
                obj.REG = Convert.ToInt32(el.Attribute("reg").Value);
                obj.KD = Convert.ToSingle(el.Attribute("kd").Value);
                obj.MIN_KD = Convert.ToSingle(el.Attribute("min_kd").Value);
                obj.KP = Convert.ToInt32(el.Attribute("kp").Value);
                obj.DOKD = Convert.ToSingle(el.Attribute("dokd").Value);
                obj.DOKP = Convert.ToInt32(el.Attribute("dokp").Value);
            }
        }
    }
}
