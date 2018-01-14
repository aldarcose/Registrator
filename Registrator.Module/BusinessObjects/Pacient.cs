using System.Reflection;
using System.Security.Authentication.ExtendedProtection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Registrator.Module.BusinessObjects.Abstract;
using Registrator.Module.BusinessObjects.Dictionaries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Registrator.Module.BusinessObjects.Interfaces;

namespace Registrator.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("�������")]
    [DefaultProperty("FullName")]
    public class Pacient : DevExpress.Persistent.BaseImpl.BaseObject, IReestrTFoms
    {
        private Polis curPolis;
        private int? age;

        public Pacient(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            IsNewBorn = true;
        }

        public void PolisesCollectionChanged(object sender, XPCollectionChangedEventArgs e)
        {
            OnChanged("CurrentPolis");
            OnChanged("IsInogorodniy");
        }

        public void CasesCollectionChanged(object sender, XPCollectionChangedEventArgs e)
        {
            OnChanged("VisitCases");
            OnChanged("HospitalCases");
            OnChanged("DispanserizaionCases");
        }

        #region ������� �������� �������
        /// <summary>
        /// ������� ��������������
        /// </summary>
        [XafDisplayName("�������������")]
        [Appearance("NewBornShow", Criteria = "!IsNewBorn", Visibility = ViewItemVisibility.Hide, Context="DetailView")]
        [NonPersistent]
        public bool IsNewBorn { get; set; }

        /// <summary>
        /// ������� ������������� �������
        /// </summary>
        [XafDisplayName("������������")]
        [Appearance("EarlyBornShow", Criteria = "!IsNewBorn", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        [ImmediatePostData]
        [NonPersistent]
        public bool IsEarlyBorn { get; set; }

        /// <summary>
        /// ��� ��� ��������, ����������� ���� ������� ������������
        /// </summary>
        [XafDisplayName("��� ��� ��������")]
        [Appearance("BornWeightShow", Criteria = "!IsEarlyBorn", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        [NonPersistent]
        public int BornWeight { get; set; }

        /// <summary>
        /// ������ �� ����� ������
        /// </summary>
        [XafDisplayName("��������")]
        [Appearance("NewBornShowParent", Criteria = "!IsNewBorn", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        [NonPersistent]
        public Pacient ChildParent { get; set; }
        #endregion

        #region ������ ��������
        /// <summary>
        /// �������
        /// </summary>
        [Size(100)]
        [XafDisplayName("�������")]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public string LastName { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        [Size(100)]
        [XafDisplayName("���")]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public string FirstName { get; set; }
        
        /// <summary>
        /// ��������
        /// </summary>
        [Size(100)]
        [XafDisplayName("��������")]
        public string MiddleName { get; set; }

        /// <summary>
        /// ������ ��� ��������
        /// </summary>
        [XafDisplayName("���")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Concat(LastName, ' ', FirstName, ' ', MiddleName)")]
        public string FullName
        {
            get { return (string)EvaluateAlias("FullName"); }
        }

        /// <summary>
        /// ���� ��������
        /// </summary>
        [XafDisplayName("���� ��������")]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public DateTime? Birthdate { get; set; }

        [NonPersistent]
        public Polis CurrentPolis
        {
            get 
            {
                if (curPolis == null)
                    curPolis = GetCurrentPolis();
                return curPolis; 
            }
        }

        private Polis GetCurrentPolis()
        {
            return Polises.FirstOrDefault(t => t.IsActive);
        }

        [XafDisplayName("�����������")]
        public bool? IsInogorodniy
        {
            get
            {
                var polis = CurrentPolis;
                if (CurrentPolis == null)
                    return null;

                // ���� ��������� � Kladr � ����������� ��� ���������. ��������������
                return CurrentPolis.IsFromAnotherRegion;
            }
        }

        /// <summary>
        /// ����� ��������
        /// </summary>
        [Size(200)]
        [XafDisplayName("����� ��������")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string BirthPlace { get; set; }

        /// <summary>
        /// ��� ��������
        /// </summary>
        [XafDisplayName("���")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public Gender Gender { get; set; }
        
        /// <summary>
        /// ����� �������� ��������
        /// </summary>
        [Delayed(true)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [XafDisplayName("����� ��������")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public Address Address
        {
            get { return GetDelayedPropertyValue<Address>("Address"); }
            set { SetDelayedPropertyValue<Address>("Address", value); } 
        }

        /// <summary>
        /// ����� ���������� ��������
        /// </summary>
        [Delayed(true)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [XafDisplayName("����� ����������")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public Address AddressFact 
        {
            get { return GetDelayedPropertyValue<Address>("AddressFact"); }
            set { SetDelayedPropertyValue<Address>("AddressFact", value); } 
        } 

        /// <summary>
        /// ��������, �������������� �������� ��������
        /// </summary>
        [DevExpress.Xpo.Aggregated]
        [XafDisplayName("��������")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public Document Document { get; set; }

        /// <summary>
        /// ����� ��������
        /// </summary>
        [ModelDefault("EditMask", "000-000-000 00")]
        [Size(14)]
        [XafDisplayName("�����")]
        [VisibleInDetailView(true)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField("", DefaultContexts.Save, "������� ����� ��������", ResultType = ValidationResultType.Warning)]
        public string SNILS { get; set; }

        /// <summary>
        /// ���, � �������� ���������� �������
        /// </summary>
        [XafDisplayName("������������")]
        [DataSourceCriteriaProperty("LPUCriteria")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public MedOrg Prikreplenie { get; set; }

        [NonPersistent]
        [Browsable(false)]
        private CriteriaOperator LPUCriteria
        {
            get
            {
                var okato = Settings.RegionSettings.GetCurrentRegionOKATO(Session);
                return CriteriaOperator.Parse("TF_OKATO=?", okato);
            }
        }

        /// <summary>
        /// ���� ������������
        /// </summary>
        [XafDisplayName("���� ������������")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? PrikreplenieDate { get; set; }

        /// <summary>
        /// ���������� ����, ������� �� ������ ������
        /// </summary>
        [Browsable(false)]
        public string LPUAuto { get; set; }

        /// <summary>
        /// ����� ������ ��������
        /// </summary>
        [Size(200)]
        [XafDisplayName("����� ������")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string WorkPlace { get; set; }

        /// <summary>
        /// ����� ����� ��������
        /// </summary>
        [Size(200)]
        [XafDisplayName("����� �����")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string LearningPlace { get; set; }

        /// <summary>
        /// ������� ��������
        /// </summary>
        [Size(100)]
        [XafDisplayName("�������")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        [XafDisplayName("�������")]
        public Uchastok Uchastok { get; set; }
        /// <summary>
        /// ��������� �������, � ������� ��������� �������
        /// </summary>
        [XafDisplayName("���������")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public Kategoriya CitizenCategory { get; set; }
        
        /// <summary>
        /// ���������� ������ ��������
        /// </summary>
        [XafDisplayName("���. ������")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public SocStatus SocStatus { get; set; }
        
        /// <summary>
        /// ���-�� ������ ���
        /// </summary>
        [XafDisplayName("���-�� ������ ���")]
        public int Age
        {
            get 
            {
                if (!age.HasValue) age = GetAge();
                return age.Value; 
            }
        }

        /// <summary> 
        /// ������ ������������
        /// </summary>
        [XafDisplayName("������������")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public DisablityData Disability { get; set; }
        #endregion

        [DevExpress.Xpo.Aggregated, Association("Pacient_Polis")]
        [XafDisplayName("������")]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public XPCollection<Polis> Polises
        {
            get { return GetCollection<Polis>("Polises"); }
        }

        /// <summary>
        /// ������ ������� ����������� (��������������)
        /// </summary>
        [XafDisplayName("������� ���������")]
        [DevExpress.Xpo.Aggregated, Association("Pacient_DnevnoyStacionar")]
        public XPCollection<DnevnoyStacionar> DnevnieStacionari
        {
            get
            {
                return GetCollection<DnevnoyStacionar>("DnevnieStacionari");
            }
        }
        
        /// <summary>
        /// ������ �������
        /// </summary>
        [XafDisplayName("���������������")]
        public IList<DispanserizaionCase> DispanserizaionCases
        {
            get
            {
                // �������� ��� ����������� ������
                var list = new List<DispanserizaionCase>();
                // ����� �� ������ ����, ������� ������������� ��������
                foreach (var dispCase in Cases.OfType<DispanserizaionCase>())
                    list.Add(dispCase);
                return list;
            }
        }

        [XafDisplayName("��������������")]
        public IList<HospitalCase> HospitalCases
        {
            get
            {
                // �������� ��� ����������� ������
                var list = new List<HospitalCase>();
                // ����� �� ������ ����, ������� ������������� ��������
                foreach (var hospitalCase in Cases.OfType<HospitalCase>())
                    list.Add(hospitalCase);
                return list;
            }
        }

        [XafDisplayName("���������")]
        public IList<VisitCase> VisitCases
        {
            get
            {
                // �������� ��� ����������� ������
                var list = new List<VisitCase>();
                foreach(var visitCase in Cases.OfType<VisitCase>())
                    list.Add(visitCase);
                return list;
            }
        }

        /// <summary>
        /// ������ ���������
        /// </summary>
        public IList<MedService> VisitCaseServices
        {
            get
            {
                List<MedService> list = new List<MedService>();
                foreach (var visitCase in Cases.OfType<VisitCase>())
                    list.AddRange(visitCase.Services.OfType<MedService>());
                return list;
            }
        }
        
        /// <summary>
        /// ������ ���� �������
        /// </summary>
        [XafDisplayName("��� ������")]
        [DevExpress.Xpo.Aggregated, Association("AbstractCase-Pacient")]
        [Browsable(false)]
        public XPCollection<AbstractCase> Cases
        {
            get
            {
                var coll = GetCollection<AbstractCase>("Cases");
                coll.CollectionChanged += CasesCollectionChanged;
                return coll;
            }
        }

        #region ������ ������
        public override string ToString()
        {
            return FullName;
        }

        /// <summary>
        /// ���������� ���-�� ������ ���
        /// </summary>
        /// <returns></returns>
        public int GetAge()
        {
            return GetAge(DateTime.Now);
        }

        /// <summary>
        /// ���������� ���-�� ������ ���
        /// </summary>
        /// <param name="Today">�� ������ ����</param>
        /// <returns></returns>
        public int GetAge(DateTime Today)
        {
            int result = 0;
            if (Birthdate.HasValue)
            {
                // ���������� ���������� ���
                var years = Today.Year - Birthdate.Value.Year;

                // ��������� ������� �������
                var month = Today.Month - Birthdate.Value.Month;
                // ���� �� �� � ���� ������
                if (month != 0)
                {
                    // ���� �� � ����. �������
                    return (month < 0) ? years - 1 : years;
                }

                // ���� �� � ������� ������, ��������� ������� ����
                var days = DateTime.Now.Day - Birthdate.Value.Day;
                // ���� �� ��� �� ���� � ���� ������
                return (days < 0) ? years - 1 : years;
            }
            return result;
        }

        /// <summary>
        /// ���������� ������ ������� �� ������� ����
        /// ���� �������� 1 ��� � 3 ������, ������� ������ 3
        /// </summary>
        /// <returns></returns>
        public int GetMonthWithNoAge()
        {
            return GetMonthWithNoAge(DateTime.Now);
        }

        /// <summary>
        /// ���������� ������ ������� �� ������� ����
        /// ���� �������� 1 ��� � 3 ������, ������� ������ 3
        /// </summary>
        /// <returns></returns>
        public int GetMonthWithNoAge(DateTime Today)
        {
            int result = 0;
            if (Birthdate.HasValue)
            {

                // ��������� ������� �������
                var month = Today.Month - Birthdate.Value.Month;

                // ���� ������� ������, �� ���-�� �������� ������� ��������� ���
                if (month < 0) month = 12 + month;

                // ���� �� ��� �������� �������� ������ ������ (������� �� ����), �� 
                if (Today.Day <= Birthdate.Value.Day) month--;

                return month;

            }
            return result;
        }

        public void AddPolis(Polis polis)
        {
            // "���������" ��������� ������ ��������
            if (this.Polises.Count > 0)
            {
                foreach (var curPolise in this.Polises)
                {
                    if (curPolise.Oid != polis.Oid)
                    {
                        if (curPolise.DateEnd == null || DateTime.Now < curPolise.DateEnd)
                        {
                            // ������������� ����� �������� ������ ���������� ����
                            curPolise.DateEnd = DateTime.Now.AddDays(-1);
                        }
                    }
                }
            }

            // ��������� �����, ���� ��� �� ����
            if (this.Polises.Contains<Polis>(polis) == false)
                this.Polises.Add(polis);
        }

        #endregion

        #region ������������� ������
        /*
         * FAM, IM, OT, DR - ��� � �� ��������.
         * W - ��� (1 - �., 2 - �.)
         * DOCTP - ��� ��������� �����. ��������
         * DOCS - �����
         * DOCN - �����
         * RN - ��� ����� (�����)
         * NP, UL, DOM, KOR, KV - ���. �����, �����, ���, ������, ��������
         * PRN - ���� �����-�� ��� �����
         * Q - ��� ��������� ���. �����������
         * PRZ - ����� ����������� ���������������
         * SPOL - ����� ������
         * NPOL - ����� ������
         * OPDOC - ��� ������
         * DBEG - ���� ������ �������� ������
         * LPU - ������������� ���
         * LPUDT - ���� ������������ � ���
         * LPUAUTO - ?
         */
        // ������ ������ �� XML
        // <ROWDATA>
        //  <ROW FAM="���" IM="����������" OT="���" DR="01.10.1947" W="1" DOCTP="14" DOCS="81&#160;01" DOCN="141050" RN="81401373000" NP="����-���" UL="�������" DOM="54" KOR="�" KV="2" PRN="81401373000" Q="03101" PRZ="137" SPOL="" NPOL="0369250848000218" OPDOC="3" DBEG="30.01.2015" LPU="032021" LPUDT="01.01.2011" LPUAUTO="0"/>

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument xDoc = XDocument.Load(xmlPath);
            const string elementsContainer = "ROWDATA";
            const string elementNameStartsWith = "ROW";
            int counter = 0;
            int countToLoad = 120;
            foreach (var element in xDoc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                // ���� ��������, �������������� ��������
                var docType = objSpace.FindObject<VidDocumenta>(CriteriaOperator.Parse("Code=?", element.Attribute("DOCTP").Value));
                var docSerial = element.Attribute("DOCS").Value;
                var docNumber = element.Attribute("DOCN").Value;
                var document = objSpace.FindObject<Document>(CriteriaOperator.Parse("Type=? AND Serial=? AND Number=?", docType, docSerial, docNumber));

                Pacient pacient = null; 
                // ���� �������� ������
                if (document != null)
                {
                    // ���� �������� � ����� ����������
                    pacient = objSpace.FindObject<Pacient>(CriteriaOperator.Parse("Document=?", document));
                }
                else
                {
                    document = objSpace.CreateObject<Document>();
                    document.Type = docType;
                    document.Serial = docSerial;
                    document.Number = docNumber;
                }

                // ���� ������� �� ������ (��� �� ������ ��������)
                if (pacient == null)
                {
                    // ������� ��������
                    pacient = objSpace.CreateObject<Pacient>();
                    counter++;
                }
                else
                {
                    counter++;
                    //continue;
                }
                
                // ������ ������ ��� ������, ���� ������� ��� ���
                pacient.LastName = element.Attribute("FAM").Value;
                pacient.FirstName = element.Attribute("IM").Value;
                pacient.MiddleName = element.Attribute("OT").Value;

                DateTime birthDate = DateTime.MinValue;
                DateTime.TryParse(element.Attribute("DR").Value, out birthDate);
                pacient.Birthdate = birthDate == DateTime.MinValue ? null : (DateTime?) birthDate;

                pacient.Gender = element.Attribute("W").Value == "1" ? Gender.Male : Gender.Female;

                pacient.Document = document;

                
                var SMO = objSpace.FindObject<StrahMedOrg>(CriteriaOperator.Parse("Code=?", element.Attribute("Q").Value));
                pacient.Prikreplenie = objSpace.FindObject<MedOrg>(CriteriaOperator.Parse("Code=?", element.Attribute("LPU").Value));
                pacient.PrikreplenieDate = (element.Attribute("LPUDT")==null || element.Attribute("LPUDT").Value == "") ? null : (DateTime?) DateTime.Parse(element.Attribute("LPUDT").Value);

                var polisType = objSpace.FindObject<VidPolisa>(CriteriaOperator.Parse("Code=?", element.Attribute("OPDOC").Value));
                var polisSerial = element.Attribute("SPOL").Value;
                var polisNumber = element.Attribute("NPOL").Value;
                var polis = objSpace.FindObject<Polis>(CriteriaOperator.Parse("Type=? AND Serial=? AND Number=?", polisType, polisSerial, polisNumber)) ??
                            objSpace.CreateObject<Polis>();
                polis.Type = polisType;
                polis.Serial = polisSerial;
                polis.Number = polisNumber;
                polis.PRZ = element.Attribute("PRZ").Value;
                polis.SMO = SMO;
                polis.DateBegin = (element.Attribute("DBEG")==null || element.Attribute("DBEG").Value == "") ? null : (DateTime?) DateTime.Parse(element.Attribute("DBEG").Value);

                if (pacient.Polises.Contains<Polis>(polis) == false)
                    pacient.Polises.Add(polis);

                var okato = element.Attribute("RN").Value;
                pacient.Address = Address.GetAddressByOkato(objSpace, okato);
                if (pacient.Address != null)
                {
                    var collection = new CriteriaOperatorCollection();
                    if (pacient.Address.Level1 != null) { collection.Add(new BinaryOperator("City", pacient.Address.Level1)); }
                    if (pacient.Address.Level2 != null) { collection.Add(new BinaryOperator("City", pacient.Address.Level2)); }
                    if (pacient.Address.Level3 != null) { collection.Add(new BinaryOperator("City", pacient.Address.Level3)); }
                    if (pacient.Address.Level4 != null) { collection.Add(new BinaryOperator("City", pacient.Address.Level4)); }

                    var street = objSpace.FindObject<Street>(CriteriaOperator.And(
                                                                CriteriaOperator.Or(collection), 
                                                                CriteriaOperator.Parse(string.Format("Lower(Name) like '{0}%'", element.Attribute("UL").Value.ToLower())))
                                                            );
                    pacient.Address.Street = street;

                    pacient.Address.House = element.Attribute("DOM").Value;
                    pacient.Address.Build = element.Attribute("KOR").Value;
                    pacient.Address.Flat = element.Attribute("KV").Value;
                }

                if (counter > countToLoad)
                {
                    
                    counter = 0;
                    break;
                }
                
            }
        }
        #endregion

        #region ������ ����������
        public bool IsValidForReestr()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// �������� ������ ��� ������� LM � ������� XElement
        /// </summary>
        /// <returns>������� XML</returns>
        public XElement GetReestrElement()
        {
            /*if (IsValidForReestr() == false)
                return null;*/

            const string dateTimeFormat = "{0:yyyy-MM-dd}";

            var element = new XElement("PERS",
                                        new XElement("ID_PAC", this.Oid)
                                    );

            // ������ �79
            // ��� ���������� �����, ��������, �������, �������� � �������� ����� �� ����, �� ��� ���� ������ ��������� ������� DOST �� �������� 1.
            // ���� ��� ��� ������-���� ����, ����������� ��� ���� ������� DOST
            // ��: ������� � DOST-��� �� �������� ��� � �����.
            // ������� ���� ��� �������, ������� ��������� �������� "���".

            if (!string.IsNullOrEmpty(this.LastName))
                // �������
                element.Add(new XElement("FAM", this.LastName));
            if (!string.IsNullOrEmpty(this.FirstName))
                // ���
                element.Add(new XElement("IM", this.FirstName));
            if (!string.IsNullOrEmpty(this.MiddleName))
                // ��������
                element.Add(new XElement("OT", this.MiddleName));

            element.Add(new XElement("W", this.Gender == BusinessObjects.Gender.Male ? 1 : 2));

            if (this.Birthdate != null)
                element.Add(new XElement("DR", String.Format(dateTimeFormat, this.Birthdate)));
            if (!string.IsNullOrEmpty(this.BirthPlace))
                element.Add(new XElement("MR", this.BirthPlace));

            if (Document != null && Document.Type != null)
            {
                element.Add(new XElement("DOCTYPE", this.Document.Type.Code));
                element.Add(new XElement("DOCSER", this.Document.Serial));
                element.Add(new XElement("DOCNUM", this.Document.Number));
            }
            if (!string.IsNullOrEmpty(this.SNILS))
                element.Add(new XElement("SNILS", this.SNILS));

            return element;
        }

        public XElement GetReestrElement(int zapNumber)
        {
            return GetReestrElement();
        }
        #endregion
    }
}
