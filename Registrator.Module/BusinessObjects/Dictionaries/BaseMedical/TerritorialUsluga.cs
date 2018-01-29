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
using DevExpress.Persistent.Base.General;
using System.Windows.Forms;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    [XafDisplayName("��������������� ������")]
    public class TerritorialUsluga : DevExpress.Persistent.BaseImpl.BaseObject, ITreeNode
    {
        public TerritorialUsluga() { }
        public TerritorialUsluga(Session session) : base(session) { }

        /// <summary>
        /// ��� ������
        /// </summary>
        [XafDisplayName("���")]
        public string Code { get; set; }

        /// <summary>
        /// ������������ ������
        /// </summary>
        [Size(255)]
        [XafDisplayName("������������")]
        public string Name { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} (��� {1})", Name, Code);
        }

        /// <summary>
        /// ������������ ������� ������������
        /// ��������� ������������ ������
        /// </summary>
        [Browsable(false)]
        public float? UET { get; set; }

        /// <summary>
        /// �����
        /// </summary>
        [Browsable(false)]
        public Decimal? Tarif { get; set; }

        /// <summary>
        /// ������� ������������ ��� ������
        /// </summary>
        [Browsable(false)]
        public bool? PriznakOplati { get; set; }

        /// <summary>
        /// �������� �� ��������������� �������
        /// </summary>
        [XafDisplayName("���")]
        public bool? MUR { get; set; }

        /// <summary>
        /// ������ ����������� �������� ����
        /// Null - ����
        /// </summary>
        [XafDisplayName("���")]
        public Gender? ForGender { get; set; }

        [XafDisplayName("����������� �� ����")]
        [NonPersistent]
        public bool Home
        {
            get
            {
                return Name.Contains("�� ����");
            }
        }

        /// <summary>
        /// ������ ����������� �������� �� ����. ��������������
        /// </summary>
        [Association("DoctorSpec-TerrServices")]
        public XPCollection<DoctorSpecTree> DoctorSpec
        {
            get { return GetCollection<DoctorSpecTree>("DoctorSpec"); }
        }

        [Association("RecordType-Service")]
        [Browsable(false)]
        public XPCollection<ProtocolRecordType> ProtocolRecordTypes
        {
            get { return GetCollection<ProtocolRecordType>("ProtocolRecordTypes"); }
        }

        #region ITreeNode
        [Association("TerUslParent-TerUslChildren")]
        [Browsable(false)]
        public TerritorialUsluga parent { get; set; }
        [XafDisplayName("���������")]
        public ITreeNode Parent { get { return parent as ITreeNode; } }

        [Association("TerUslParent-TerUslChildren"), DevExpress.Xpo.Aggregated]
        [Browsable(false)]
        public XPCollection<TerritorialUsluga> children
        {
            get
            {
                return GetCollection<TerritorialUsluga>("children");
            }
        }

        [XafDisplayName("������")]
        public System.ComponentModel.IBindingList Children
        {
            get
            {
                return children as System.ComponentModel.IBindingList;
            }

        }
        #endregion

        private void CopyValueFrom(TerrUslTemp temp)
        {
            this.Code = temp.Code;
            this.Name = temp.Name;
            this.MUR = temp.MUR;
            this.Tarif = temp.Tarif;
            this.PriznakOplati = temp.PriznakOplati;
            this.UET = temp.UET;
            this.ForGender = temp.ForGender;
        }

        /// <summary>
        /// ��������� � ���� �������������� �� ����� XML
        /// </summary>
        /// <param name="updater">������������ �������� ObjectSpace</param>
        /// <param name="xmlPath">���� �� ����� ��������������</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            List<TerrUslTemp> list = TerritorialUsluga.GetListFromXML(xmlPath);
            /*
             * ����� ���� ������ ����������� (���� 3, 4, 5 � 6-� ������� ����)
             * ������ � ������� ����� �������� ���������� ������, 4�, 5-� ������� - ������������
             */

            var cat3 = list.Where(t => t.Code.Length == 3);
            var cat4 = list.Where(t => t.Code.Length == 4);
            var cat5 = list.Where(t => t.Code.Length == 5);
            var cat6 = list.Where(t => t.Code.Length == 6);
            
            int createdCount = 0;
            foreach (var cat in cat3)
            {
                var terrUsl = objSpace.FindObject<TerritorialUsluga>(CriteriaOperator.Parse("Code=?", cat.Code));
                if (terrUsl == null)
                {
                    terrUsl = objSpace.CreateObject<TerritorialUsluga>();
                    terrUsl.CopyValueFrom(cat);
                    createdCount++;
                }
            }

            if (createdCount!=0)
                objSpace.CommitChanges();

            createdCount = 0;
            foreach(var cat in cat4)
            {
                var terrUsl = objSpace.FindObject<TerritorialUsluga>(CriteriaOperator.Parse("Code=?", cat.Code));
                if (terrUsl == null)
                {
                    terrUsl = objSpace.CreateObject<TerritorialUsluga>();
                    terrUsl.CopyValueFrom(cat);
                    createdCount++;

                    var parent = TerritorialUsluga.FindParent(objSpace, cat.Code);
                    if (parent!=null)
                    {
                        terrUsl.parent = parent;
                        parent.children.Add(terrUsl);
                    }
                }
            }

            if (createdCount!=0)
                objSpace.CommitChanges();

            createdCount = 0;
            foreach(var cat in cat5)
            {
                var terrUsl = objSpace.FindObject<TerritorialUsluga>(CriteriaOperator.Parse("Code=?", cat.Code));
                if (terrUsl == null)
                {
                    terrUsl = objSpace.CreateObject<TerritorialUsluga>();
                    terrUsl.CopyValueFrom(cat);
                    createdCount++;

                    var parent = TerritorialUsluga.FindParent(objSpace, cat.Code);
                    if (parent!=null)
                    {
                        terrUsl.parent = parent;
                        parent.children.Add(terrUsl);
                    }
                }
            }

            if (createdCount!=0)
                objSpace.CommitChanges();

            createdCount = 0;
            foreach(var cat in cat6)
            {
                var terrUsl = objSpace.FindObject<TerritorialUsluga>(CriteriaOperator.Parse("Code=?", cat.Code));
                if (terrUsl == null)
                {
                    terrUsl = objSpace.CreateObject<TerritorialUsluga>();
                    terrUsl.CopyValueFrom(cat);
                    createdCount++;

                    var parent = TerritorialUsluga.FindParent(objSpace, cat.Code);
                    if (parent!=null)
                    {
                        terrUsl.parent = parent;
                        parent.children.Add(terrUsl);
                    }
                }
            }

            if (createdCount!=0)
                objSpace.CommitChanges();
        }

        private static TerritorialUsluga FindParent (DevExpress.ExpressApp.IObjectSpace objSpace, string code)
        {
            TerritorialUsluga parent = null;
            // ���� ���� ��������� ������ � ������ ���� 6
            if (code.Length > 5)
            {
                parent = objSpace.FindObject<TerritorialUsluga>(CriteriaOperator.Parse("Code=?", code.Substring(0, 5)));
                if (parent != null)
                    return parent;
            }

            // ���� �� ������ �������� � ��������� � ������, ����� ������� = 5
            // ��� ���� ��������� ��� ������������ (����� 5)
            if (code.Length > 4)
            {
                parent = objSpace.FindObject<TerritorialUsluga>(CriteriaOperator.Parse("Code=?", code.Substring(0, 4)));
                if (parent != null)
                    return parent;
            }

            // ���� �� ������ �������� � ��������� � ������, ����� ������� = 4 ��� 5
            // ��� ���� ��������� ��� ������������ (����� 4 ��� 5)

            if (code.Length > 3)
            {
                parent = objSpace.FindObject<TerritorialUsluga>(CriteriaOperator.Parse("Code=?", code.Substring(0, 3)));
                if (parent != null)
                    return parent;
            }

            // �������� �� ������
            return null;
        }

        struct TerrUslTemp
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public float? UET { get; set; }
            public Decimal? Tarif { get; set; }
            public bool? PriznakOplati { get; set; }
            public bool? MUR { get; set; }
            public Gender? ForGender { get; set; }
        }

        private static List<TerrUslTemp> GetListFromXML(string path)
        {
            var result = new List<TerrUslTemp>();

            XDocument xml = XDocument.Load(path);
            foreach(var xmlUsl in xml.Descendants("usl"))
            {
                var usl = new TerrUslTemp();
                    usl.Code = xmlUsl.Attribute("code").Value;
                    usl.Name = xmlUsl.Attribute("name").Value;
                try
                {
                    usl.Tarif = Utils.GetDecimalFromString(xmlUsl.Attribute("tarif").Value);

                    usl.MUR = xmlUsl.Attribute("mur").Value == "" ? null : (bool?)xmlUsl.Attribute("mur").Value.Equals("��");

                    var payment = xmlUsl.Attribute("pay");
                    var gender = xmlUsl.Attribute("gender");
                    var uet = xmlUsl.Attribute("uet");
                    usl.PriznakOplati = (payment == null || string.IsNullOrEmpty(payment.Value)) ? null : (bool?)payment.Value.ToString().Equals("True");
                    usl.ForGender = (gender == null || string.IsNullOrEmpty(gender.Value))
                                    ? null
                                    : (Gender?)(gender.Value.Equals("�") ? Gender.Female : Gender.Male);
                    usl.UET = (uet == null || string.IsNullOrEmpty(uet.Value)) ? null : (float?)Utils.GetDecimalFromString(uet.Value.ToString());

                    result.Add(usl);
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("{0}: {1}", usl.Code, e.Message));
                }
            }

            return result;
        }
    }
}
