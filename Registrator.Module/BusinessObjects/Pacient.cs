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
    [XafDisplayName("Пациент")]
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

        #region Признак детского профиля
        /// <summary>
        /// Признак новорожденного
        /// </summary>
        [XafDisplayName("Новорожденный")]
        [Appearance("NewBornShow", Criteria = "!IsNewBorn", Visibility = ViewItemVisibility.Hide, Context="DetailView")]
        [NonPersistent]
        public bool IsNewBorn { get; set; }

        /// <summary>
        /// Признак недоношенного ребенка
        /// </summary>
        [XafDisplayName("Недоношенный")]
        [Appearance("EarlyBornShow", Criteria = "!IsNewBorn", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        [ImmediatePostData]
        [NonPersistent]
        public bool IsEarlyBorn { get; set; }

        /// <summary>
        /// Вес при рождении, указывается если ребенок недоношенный
        /// </summary>
        [XafDisplayName("Вес при рождении")]
        [Appearance("BornWeightShow", Criteria = "!IsEarlyBorn", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        [NonPersistent]
        public int BornWeight { get; set; }

        /// <summary>
        /// Ссылка на карту матери
        /// </summary>
        [XafDisplayName("Родитель")]
        [Appearance("NewBornShowParent", Criteria = "!IsNewBorn", Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
        [NonPersistent]
        public Pacient ChildParent { get; set; }
        #endregion

        #region Данные пациента
        /// <summary>
        /// Фамилия
        /// </summary>
        [Size(100)]
        [XafDisplayName("Фамилия")]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Size(100)]
        [XafDisplayName("Имя")]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public string FirstName { get; set; }
        
        /// <summary>
        /// Отчество
        /// </summary>
        [Size(100)]
        [XafDisplayName("Отчество")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Полное имя пациента
        /// </summary>
        [XafDisplayName("ФИО")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Concat(LastName, ' ', FirstName, ' ', MiddleName)")]
        public string FullName
        {
            get { return (string)EvaluateAlias("FullName"); }
        }

        /// <summary>
        /// Дата рождения
        /// </summary>
        [XafDisplayName("Дата рождения")]
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

        [XafDisplayName("Иногородний")]
        public bool? IsInogorodniy
        {
            get
            {
                var polis = CurrentPolis;
                if (CurrentPolis == null)
                    return null;

                // Идет обращение к Kladr с последующей его загрузкой. Оптимизировать
                return CurrentPolis.IsFromAnotherRegion;
            }
        }

        /// <summary>
        /// Место рождения
        /// </summary>
        [Size(200)]
        [XafDisplayName("Место рождения")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string BirthPlace { get; set; }

        /// <summary>
        /// Пол пациента
        /// </summary>
        [XafDisplayName("Пол")]
        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public Gender Gender { get; set; }
        
        /// <summary>
        /// Адрес прописки пациента
        /// </summary>
        [Delayed(true)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [XafDisplayName("Адрес прописки")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public Address Address
        {
            get { return GetDelayedPropertyValue<Address>("Address"); }
            set { SetDelayedPropertyValue<Address>("Address", value); } 
        }

        /// <summary>
        /// Адрес проживания пациента
        /// </summary>
        [Delayed(true)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [XafDisplayName("Адрес проживания")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public Address AddressFact 
        {
            get { return GetDelayedPropertyValue<Address>("AddressFact"); }
            set { SetDelayedPropertyValue<Address>("AddressFact", value); } 
        } 

        /// <summary>
        /// Документ, удостоверяющий личность пациента
        /// </summary>
        [DevExpress.Xpo.Aggregated]
        [XafDisplayName("Документ")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public Document Document { get; set; }

        /// <summary>
        /// СНИЛС пациента
        /// </summary>
        [ModelDefault("EditMask", "000-000-000 00")]
        [Size(14)]
        [XafDisplayName("СНИЛС")]
        [VisibleInDetailView(true)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField("", DefaultContexts.Save, "Укажите СНИЛС пациента", ResultType = ValidationResultType.Warning)]
        public string SNILS { get; set; }

        /// <summary>
        /// ЛПУ, к которому прикреплен пациент
        /// </summary>
        [XafDisplayName("Прикрепление")]
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
        /// Дата прикрепления
        /// </summary>
        [XafDisplayName("Дата прикрепления")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime? PrikreplenieDate { get; set; }

        /// <summary>
        /// Непонятное поле, добавим на всякий случай
        /// </summary>
        [Browsable(false)]
        public string LPUAuto { get; set; }

        /// <summary>
        /// Место работы пациента
        /// </summary>
        [Size(200)]
        [XafDisplayName("Место работы")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string WorkPlace { get; set; }

        /// <summary>
        /// Место учебы пациента
        /// </summary>
        [Size(200)]
        [XafDisplayName("Место учебы")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string LearningPlace { get; set; }

        /// <summary>
        /// Телефон пациента
        /// </summary>
        [Size(100)]
        [XafDisplayName("Телефон")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Участок
        /// </summary>
        [XafDisplayName("Участок")]
        public Uchastok Uchastok { get; set; }
        /// <summary>
        /// Категория граждан, к которой относится пациент
        /// </summary>
        [XafDisplayName("Категория")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public Kategoriya CitizenCategory { get; set; }
        
        /// <summary>
        /// Социальный статус пациента
        /// </summary>
        [XafDisplayName("Соц. статус")]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public SocStatus SocStatus { get; set; }
        
        /// <summary>
        /// Кол-во полных лет
        /// </summary>
        [XafDisplayName("Кол-во полных лет")]
        public int Age
        {
            get 
            {
                if (!age.HasValue) age = GetAge();
                return age.Value; 
            }
        }

        /// <summary> 
        /// Данные инвалидности
        /// </summary>
        [XafDisplayName("Инвалидность")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public DisablityData Disability { get; set; }
        #endregion

        [DevExpress.Xpo.Aggregated, Association("Pacient_Polis")]
        [XafDisplayName("Полиса")]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Warning)]
        public XPCollection<Polis> Polises
        {
            get { return GetCollection<Polis>("Polises"); }
        }

        /// <summary>
        /// Список дневных стационаров (госпитализаций)
        /// </summary>
        [XafDisplayName("Дневной стационар")]
        [DevExpress.Xpo.Aggregated, Association("Pacient_DnevnoyStacionar")]
        public XPCollection<DnevnoyStacionar> DnevnieStacionari
        {
            get
            {
                return GetCollection<DnevnoyStacionar>("DnevnieStacionari");
            }
        }
        
        /// <summary>
        /// Список случаев
        /// </summary>
        [XafDisplayName("Диспансеризации")]
        public IList<DispanserizaionCase> DispanserizaionCases
        {
            get
            {
                // Получить все абстрактные случаи
                var list = new List<DispanserizaionCase>();
                // найти те слуачи типы, которых возвращаемого значения
                foreach (var dispCase in Cases.OfType<DispanserizaionCase>())
                    list.Add(dispCase);
                return list;
            }
        }

        [XafDisplayName("Госпитализации")]
        public IList<HospitalCase> HospitalCases
        {
            get
            {
                // Получить все абстрактные случаи
                var list = new List<HospitalCase>();
                // найти те слуачи типы, которых возвращаемого значения
                foreach (var hospitalCase in Cases.OfType<HospitalCase>())
                    list.Add(hospitalCase);
                return list;
            }
        }

        [XafDisplayName("Посещения")]
        public IList<VisitCase> VisitCases
        {
            get
            {
                // Получить все абстрактные случаи
                var list = new List<VisitCase>();
                foreach(var visitCase in Cases.OfType<VisitCase>())
                    list.Add(visitCase);
                return list;
            }
        }

        /// <summary>
        /// Услуги посещений
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
        /// Список всех случаев
        /// </summary>
        [XafDisplayName("Все случаи")]
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

        #region Методы класса
        public override string ToString()
        {
            return FullName;
        }

        /// <summary>
        /// Возвращает кол-во полных лет
        /// </summary>
        /// <returns></returns>
        public int GetAge()
        {
            return GetAge(DateTime.Now);
        }

        /// <summary>
        /// Возвращает кол-во полных лет
        /// </summary>
        /// <param name="Today">На данную дату</param>
        /// <returns></returns>
        public int GetAge(DateTime Today)
        {
            int result = 0;
            if (Birthdate.HasValue)
            {
                // определяем количество лет
                var years = Today.Year - Birthdate.Value.Year;

                // вычисляем разницу месяцев
                var month = Today.Month - Birthdate.Value.Month;
                // если др не в этом месяце
                if (month != 0)
                {
                    // если ДР в след. месяцах
                    return (month < 0) ? years - 1 : years;
                }

                // если др в текущем месяце, вычисляем разницу дней
                var days = DateTime.Now.Day - Birthdate.Value.Day;
                // если др еще не было в этом месяце
                return (days < 0) ? years - 1 : years;
            }
            return result;
        }

        /// <summary>
        /// Количество полных месяцев на текущую дату
        /// если пациенту 1 год и 3 месяца, функция вернет 3
        /// </summary>
        /// <returns></returns>
        public int GetMonthWithNoAge()
        {
            return GetMonthWithNoAge(DateTime.Now);
        }

        /// <summary>
        /// Количество полных месяцев на текущую дату
        /// если пациенту 1 год и 3 месяца, функция вернет 3
        /// </summary>
        /// <returns></returns>
        public int GetMonthWithNoAge(DateTime Today)
        {
            int result = 0;
            if (Birthdate.HasValue)
            {

                // вычисляем разницу месяцев
                var month = Today.Month - Birthdate.Value.Month;

                // если родился раньше, то кол-во прожитых месяцев вычисляем так
                if (month < 0) month = 12 + month;

                // если до дня рождения осталось меньше месяца (сверяем по дням), то 
                if (Today.Day <= Birthdate.Value.Day) month--;

                return month;

            }
            return result;
        }

        public void AddPolis(Polis polis)
        {
            // "закрываем" остальные полисы пациента
            if (this.Polises.Count > 0)
            {
                foreach (var curPolise in this.Polises)
                {
                    if (curPolise.Oid != polis.Oid)
                    {
                        if (curPolise.DateEnd == null || DateTime.Now < curPolise.DateEnd)
                        {
                            // устанавливаем конец действия полиса предыдущим днем
                            curPolise.DateEnd = DateTime.Now.AddDays(-1);
                        }
                    }
                }
            }

            // добавляем полис, если его не было
            if (this.Polises.Contains<Polis>(polis) == false)
                this.Polises.Add(polis);
        }

        #endregion

        #region Инициализация данных
        /*
         * FAM, IM, OT, DR - ФИО и ДР пациента.
         * W - пол (1 - м., 2 - ж.)
         * DOCTP - тип документа удост. личность
         * DOCS - серия
         * DOCN - номер
         * RN - код ОКАТО (Район)
         * NP, UL, DOM, KOR, KV - нас. пункт, улица, дом, корпус, квартира
         * PRN - Тоже какой-то код окато
         * Q - код страховой мед. организации
         * PRZ - Пункт регистрации застрахованного
         * SPOL - серия полиса
         * NPOL - номер полиса
         * OPDOC - тип полиса
         * DBEG - дата начала действия полиса
         * LPU - прикрепленное ЛПУ
         * LPUDT - дата прикрепления к ЛПУ
         * LPUAUTO - ?
         */
        // пример записи из XML
        // <ROWDATA>
        //  <ROW FAM="НЕТ" IM="БАЛАЖИНИМА" OT="НЕТ" DR="01.10.1947" W="1" DOCTP="14" DOCS="81&#160;01" DOCN="141050" RN="81401373000" NP="Улан-Удэ" UL="СМОЛИНА" DOM="54" KOR="А" KV="2" PRN="81401373000" Q="03101" PRZ="137" SPOL="" NPOL="0369250848000218" OPDOC="3" DBEG="30.01.2015" LPU="032021" LPUDT="01.01.2011" LPUAUTO="0"/>

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

                // ищем документ, удостоверяющий личность
                var docType = objSpace.FindObject<VidDocumenta>(CriteriaOperator.Parse("Code=?", element.Attribute("DOCTP").Value));
                var docSerial = element.Attribute("DOCS").Value;
                var docNumber = element.Attribute("DOCN").Value;
                var document = objSpace.FindObject<Document>(CriteriaOperator.Parse("Type=? AND Serial=? AND Number=?", docType, docSerial, docNumber));

                Pacient pacient = null; 
                // если документ найден
                if (document != null)
                {
                    // ищем пациента с таким документом
                    pacient = objSpace.FindObject<Pacient>(CriteriaOperator.Parse("Document=?", document));
                }
                else
                {
                    document = objSpace.CreateObject<Document>();
                    document.Type = docType;
                    document.Serial = docSerial;
                    document.Number = docNumber;
                }

                // если пациент не найден (или не найден документ)
                if (pacient == null)
                {
                    // создаем пациента
                    pacient = objSpace.CreateObject<Pacient>();
                    counter++;
                }
                else
                {
                    counter++;
                    //continue;
                }
                
                // задаем данные или меняем, если пациент уже был
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

        #region Методы интерфейса
        public bool IsValidForReestr()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получаем запись для реестра LM в формате XElement
        /// </summary>
        /// <returns>Элемент XML</returns>
        public XElement GetReestrElement()
        {
            /*if (IsValidForReestr() == false)
                return null;*/

            const string dateTimeFormat = "{0:yyyy-MM-dd}";

            var element = new XElement("PERS",
                                        new XElement("ID_PAC", this.Oid)
                                    );

            // приказ №79
            // при отсутствии полей, например, фамилии, элемента с фамилией может не быть, но при этом должен появиться элемент DOST со значение 1.
            // если нет еще какого-либо поля, добавляется еще один элемент DOST
            // НО: реестры с DOST-ами не проходят ФЛК в ТФОМС.
            // поэтому если нет фамилии, следует указывать значение "НЕТ".

            if (!string.IsNullOrEmpty(this.LastName))
                // фамилия
                element.Add(new XElement("FAM", this.LastName));
            if (!string.IsNullOrEmpty(this.FirstName))
                // имя
                element.Add(new XElement("IM", this.FirstName));
            if (!string.IsNullOrEmpty(this.MiddleName))
                // отчество
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
