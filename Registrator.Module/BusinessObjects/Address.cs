using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registrator.Module.BusinessObjects.Dictionaries;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Xpo;
using System.Xml.Linq;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.DC;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Адрес
    /// </summary>
    [DefaultClassOptions]
    public class Address : BaseObject
    {
        private KladrType level1Type;
        private KladrType level2Type;
        private KladrType level3Type;
        private KladrType level4Type;
        private KladrType streetType;

        private Kladr level1;
        private Kladr level2;
        private Kladr level3;
        private Kladr level4;
        private Street street;

        private string okato;

        public Address() { }
        public Address(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            this.Country = Session.FindObject<Country>(CriteriaOperator.Parse("ShortName='РОССИЯ'"));
            this.level1 = null;
            this.level2 = null;
            this.level3 = null;
            this.level4 = null;
            this.street = null;
        }

        public void Copy(Address address)
        {
            this.level1 = address.level1;
            this.level2 = address.level2;
            this.level3 = address.level3;
            this.level4 = address.level4;

            this.street = address.street;
            this.House = address.House;
            this.Build = address.Build;
            this.Flat = address.Flat;
        }

        [Browsable(false)]
        public string OKATO
        {
            get { return okato; }
            set
            {
                SetPropertyValue("OKATO", ref okato, value);

                /*if (!String.IsNullOrWhiteSpace(value))
                {
                    var kladr = Session.FindObject<Kladr>(new BinaryOperator("CodeOkato", value));
                    if (kladr != null)
                    {
                        if (kladr.Level == 1)
                        {
                            Level1 = kladr;
                        }
                        if (kladr.Level == 2)
                        {
                            Level2 = kladr;
                        }
                        if (kladr.Level == 3)
                        {
                            Level3 = kladr;
                        }
                        if (kladr.Level == 4)
                        {
                            Level4 = kladr;
                        }
                    }
                    else
                    {
                        var str = Session.FindObject<Street>(new BinaryOperator("CodeOkato", value));
                        if (str != null)
                        {
                            Street = str;
                        }
                    }
                }*/
            }
        }

        [Browsable(false)]
        public string OKATOP { get; set; }

        [XafDisplayName("Страна")]
        public Country Country { get; set; }
        [DataSourceCriteria("Level=1")]
        [Appearance("Level1TypeDisabled", Enabled=false, Criteria= "!IsNull(Level1)")]
        [Appearance("Level1TypeInvisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Country) AND IsNull(Level1)")]
        [Browsable(false)]
        public KladrType Level1Type
        {
            get { return level1Type; }
            set { SetPropertyValue("Level1Type", ref level1Type, value); }
        }

        [DataSourceCriteria("Level=2")]
        [Appearance("Level2TypeInvisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Level1) AND IsNull(Level2)")]
        [Browsable(false)]
        public KladrType Level2Type
        {
            get { return level2Type; }
            set { SetPropertyValue("Level2Type", ref level2Type, value); }
        }

        [DataSourceCriteria("Level=3")]
        [Appearance("Level3TypeInvisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Level2) AND IsNull(Level3)")]
        [Browsable(false)]
        public KladrType Level3Type
        {
            get { return level3Type; }
            set { SetPropertyValue("Level3Type", ref level3Type, value); }
        }

        [DataSourceCriteria("Level=4")]
        [Appearance("Level4TypeInvisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Level3) AND IsNull(Level4)")]
        [Browsable(false)]
        public KladrType Level4Type
        {
            get { return level4Type; }
            set { SetPropertyValue("Level4Type", ref level4Type, value); }
        }
        
        [DataSourceCriteria("Level=5")]
        [Appearance("Level5TypeInvisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Level3) AND IsNull(Level4) AND IsNull(Street)")]
        [Browsable(false)]
        public KladrType StreetType
        {
            get { return streetType; }
            set { SetPropertyValue("StreetType", ref streetType, value); }
        }

        [DataSourceCriteria("Level=1")]
        [Appearance("Level1Invisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Country) AND IsNull(Level1)")]
        [ImmediatePostData]
        public Kladr Level1
        {
            get { return level1; }
            set
            {
                SetPropertyValue("Level1", ref level1, value);
                Level1Type = (value != null) ? value.Type : null;

                CheckAndRefreshAddress(value);

                //ReloadOkato();
            }
        }
        [DataSourceCriteriaProperty("Level2Criteria")]
        [Appearance("Level2Invisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Level1) AND IsNull(Level2)")]
        [ImmediatePostData]
        public Kladr Level2
        {
            get { return level2; }
            set
            {
                SetPropertyValue("Level2", ref level2, value);
                Level2Type = (value != null) ? value.Type : null;

                CheckAndRefreshAddress(value);

                /*ReloadOkato();
                if (value != null && value.Parent != null)
                {
                    if (value.Parent.Level == 1)
                    {
                        Level1 = value.City;
                    }
                }*/
            }
        }
        
        [DataSourceCriteriaProperty("Level3Criteria")]
        [Appearance("Level3Invisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Level2) AND IsNull(Level3)")]
        [ImmediatePostData]
        public Kladr Level3
        {
            get { return level3; }
            set
            {
                SetPropertyValue("Level3", ref level3, value);
                Level3Type = (value != null) ? value.Type : null;

                CheckAndRefreshAddress(value);

                /*//ReloadOkato();
                if (value != null && value.Parent != null)
                {
                    Level1 = null;
                    Level2 = null;
                    if (value.Parent.Level == 1)
                    {
                        Level1 = value.Parent;
                    }
                    if (value.Parent.Level == 2)
                    {
                        Level2 = value.Parent;
                    }
                }*/
            }
        }

        [DataSourceCriteriaProperty("Level4Criteria")]
        [Appearance("Level4Invisible", Visibility = ViewItemVisibility.Hide, Criteria = "IsNull(Level3) AND IsNull(Level4)")]
        [ImmediatePostData]
        public Kladr Level4
        {
            get { return level4; }
            set
            {
                SetPropertyValue("Level4", ref level4, value);
                Level4Type = (value != null) ? value.Type : null;

                CheckAndRefreshAddress(value);

                /*ReloadOkato();
                if (value != null && value.Parent != null)
                {
                    Level1 = null;
                    Level2 = null;
                    Level3 = null;
                    if (value.Parent.Level == 1)
                    {
                        Level1 = value.Parent;
                    }
                    if (value.Parent.Level == 2)
                    {
                        Level2 = value.Parent;
                    }
                    if (value.Parent.Level == 3)
                    {
                        Level3 = value.Parent;
                    }
                }*/
            }
        }

        [ImmediatePostData]
        [DataSourceCriteriaProperty ("StreetCriteria")]
        [Appearance("Level5Invisible", Visibility = ViewItemVisibility.Hide, Criteria = "(Not IsNull(Level1) AND Not Level1.IsCity) AND IsNull(Level3) AND IsNull(Level4) AND IsNull(Street)")]
        public Street Street
        {
            get { return street; }
            set
            {
                SetPropertyValue("Street", ref street, value);
                StreetType = (value != null) ? value.Type : null;
                if (value != null && value.City != null)
                {
                    var city = GetFieldByLevel(value.City.Level);
                    CheckAndRefreshAddress(city);
                }

                /*ReloadOkato();
                if (value != null && value.City != null)
                {
                    Level1 = null;
                    Level2 = null;
                    Level3 = null;
                    Level4 = null;
                    if (value.City.Level == 1)
                    {
                        Level1 = value.City;
                    }
                    if (value.City.Level == 2)
                    {
                        Level2 = value.City;
                    }
                    if (value.City.Level == 3)
                    {
                        Level3 = value.City;
                    }
                    if (value.City.Level == 4)
                    {
                        Level4 = value.City;
                    }
                }*/
            }
        }

        [Size(50)]
        public string House { get; set; }
        [Size(50)]
        public string Build { get; set; }
        [Size(50)]
        public string Flat { get; set; }

        /// <summary>
        /// Стараемся получить почтовый индекс
        /// </summary>
        /// <returns>Возвращает null, если не найден</returns>
        public string GetPostCode()
        {
            if (street != null && !string.IsNullOrEmpty(street.CodePost))
            {
                return street.CodePost;
            }

            if (level4 != null && !string.IsNullOrEmpty(level4.CodePost))
            {
                return level4.CodePost;
            }

            if (level3 != null && !string.IsNullOrEmpty(level3.CodePost))
            {
                return level3.CodePost;
            }

            if (level2 != null && !string.IsNullOrEmpty(level2.CodePost))
            {
                return level2.CodePost;
            }

            if (level1 != null && !string.IsNullOrEmpty(level1.CodePost))
            {
                return level1.CodePost;
            }

            return null;
        }

        private void ReloadOkato()
        {
            if (Street != null)
            {
                OKATO = Street.CodeOkato;
                return;
            }
            if (Level4 != null)
            {
                OKATO = Level4.CodeOkato;
                return;
            }
            if (Level3 != null)
            {
                OKATO = Level3.CodeOkato;
                return;
            }
            if (Level2 != null)
            {
                OKATO = Level2.CodeOkato;
                return;
            }
            if (Level1 != null)
            {
                OKATO = Level1.CodeOkato;
                return;
            }
        }

        private void CheckAndRefreshAddress(Kladr newValue)
        {
            if (newValue == null) return;
            if (newValue.Level == 0) return;

            // определяем какое поле было задано
            int level = newValue.Level;

            // обнуляем вышестоящие поля
            for (int i = level - 1; i > 0; i--)
            {
                var field = GetFieldByLevel(i);
                field = null;
            }

            // проставляем новые значения
            var temp = newValue.Parent;
            while (temp!=null)
            {
                SetFieldByLevel(temp.Level, temp);
                temp = temp.Parent;
            }

            // проверяем, что нижестоящие поля содержатся в новом значении, если нет - обнулить значения
            var parent = newValue;
            for (int i = level + 1; i<5; i++)
            {
                var field = GetFieldByLevel(i);
                if (field!=null)
                {
                    if (field.Parent != parent)
                    {
                        SetFieldByLevel(i, null);
                    }
                    else
                        parent = field;
                }
            }

            if (Street != null && (Street.City != GetFieldByLevel(3)) && (Street.City != GetFieldByLevel(4)))
            {
                Street = null;
            }

            // обновляем критерии
            UpdateCriterias(level);
        }

        private void UpdateCriterias(int level)
        {
            switch (level)
            {
                case 1:
                    OnChanged("Level2Criteria");
                    OnChanged("Level3Criteria");
                    OnChanged("Level4Criteria");
                    OnChanged("StreetCriteria");
                    break;
                case 2:
                    OnChanged("Level3Criteria");
                    OnChanged("Level4Criteria");
                    OnChanged("StreetCriteria");
                    break;
                case 3:
                    OnChanged("Level4Criteria");
                    OnChanged("StreetCriteria");
                    break;
                case 4:
                    OnChanged("StreetCriteria");
                    break;
            }
        }

        private Kladr GetFieldByLevel(int level)
        {
            switch (level)
            {
                case 1:
                    return this.Level1;
                case 2:
                    return this.Level2;
                case 3:
                    return this.Level3;
                case 4:
                    return this.Level4;
            }
            return null;
        }

        private Kladr SetFieldByLevel(int level, Kladr newValue)
        {
            switch (level)
            {
                case 1:
                    this.level1 = newValue;
                    OnChanged("Level1");
                    break;
                case 2:
                    this.level2 = newValue;
                    OnChanged("Level2");
                    break;
                case 3:
                    this.level3 = newValue;
                    OnChanged("Level3");
                    break;
                case 4:
                    this.level4 = newValue;
                    OnChanged("Level4");
                    break;
            }
            return null;
        }

        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public CriteriaOperator Level2Criteria
        {
            get
            {
                var level = new BinaryOperator("Level", 2);
                return this.Level1 != null ? CriteriaOperator.And(new BinaryOperator("Parent", Level1), level) : level;
            }
        }

        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public CriteriaOperator Level3Criteria
        {
            get
            {
                var level = new BinaryOperator("Level", 3);
                var collection = new CriteriaOperatorCollection();
                if (this.Level1 != null) { collection.Add(new BinaryOperator("Parent", Level1)); }
                if (this.Level2 != null) { collection.Add(new BinaryOperator("Parent", Level2)); }

                return CriteriaOperator.And(level, CriteriaOperator.Or(collection));
            }
        }

        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public CriteriaOperator Level4Criteria
        {
            get
            {
                var level = new BinaryOperator("Level", 4);
                var collection = new CriteriaOperatorCollection();
                if (this.Level1 != null) { collection.Add(new BinaryOperator("Parent", Level1)); }
                if (this.Level2 != null) { collection.Add(new BinaryOperator("Parent", Level2)); }
                if (this.Level3 != null) { collection.Add(new BinaryOperator("Parent", Level3)); }

                return CriteriaOperator.And(level, CriteriaOperator.Or(collection));
            }
        }


        [Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public CriteriaOperator StreetCriteria
        {
            get
            {
                var collection = new CriteriaOperatorCollection();
                if (this.Level4 != null) { collection.Add(new BinaryOperator("City", Level4)); }
                else
                {
                    if (this.Level3 != null) { collection.Add(new BinaryOperator("City", Level3)); } 
                    else
                    {
                        if (this.Level2 != null) { collection.Add(new BinaryOperator("City", Level2)); }
                        else
                            if (this.Level1 != null) { collection.Add(new BinaryOperator("City", Level1)); }
                    }
                }

                /*if (this.Level1 != null) { collection.Add(new BinaryOperator("City", Level1)); }
                if (this.Level2 != null) { collection.Add(new BinaryOperator("City", Level2)); }
                if (this.Level3 != null) { collection.Add(new BinaryOperator("City", Level3)); } 
                if (this.Level4 != null) { collection.Add(new BinaryOperator("City", Level4)); }*/

                return CriteriaOperator.Or(collection);
            }
        }

        public override string ToString()
        {
            return String.Format("ул. {0}, д.{1}, кв.{2}", Street, House, Flat);
        }

        public static Address GetAddressByOkato(IObjectSpace objectSpace, string okato)
        {
            var address = new Address(((XPObjectSpace)objectSpace).Session);

            var kladr = Kladr.GetKladr(objectSpace, okato);
            if (kladr == null)
                return null;

            switch (kladr.Level)
            {
                case 1:
                    address.Level1 = kladr;
                    break;
                case 2:
                    address.Level2 = kladr;
                    break;
                case 3:
                    address.Level3 = kladr;
                    break;
                case 4:
                    address.Level4 = kladr;
                    break;
            }

            return address;
        }
    }

    /// <summary>
    /// Страна
    /// </summary>
    [DefaultClassOptions]
    [XafDisplayName("Страна")]
    public class Country : BaseObject
    {
        public Country() { }
        public Country(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        
        public int Code { get; set; }

        /// <summary>
        /// Код альфа 2
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [Size(2)]
        public string CodeAlfa2 { get; set; }

        /// <summary>
        /// Код альфа 3
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [Size(3)]
        public string CodeAlfа3 { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string FullName { get; set; }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("ShortName")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return ShortName;
        }

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            const string elementNameStartsWith = "value";

            const string code_attr = "code";


            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                Country obj = objSpace.FindObject<Country>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<Country>();

                    obj.Code = int.Parse(element.Attribute("code").Value);
                    obj.CodeAlfa2 = element.Attribute("a2").Value;
                    obj.CodeAlfа3 = element.Attribute("a3").Value;

                    obj.ShortName = element.Attribute("short").Value;
                    obj.FullName = element.Attribute("full").Value;
                    
                }
            }
        }
    }

    /// <summary>
    /// Субъекты внутри страны
    /// </summary>
    [DefaultClassOptions]
    public class Republic : BaseObject
    {
        public Republic() { }
        public Republic(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [Size(50)]
        public string Code { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [Size(50)]
        public string Type { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Name")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Районы субъекта
    /// </summary>
    [DefaultClassOptions]
    public class District : BaseObject
    {
        public District() { }
        public District(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [Size(50)]
        public string Code { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [Size(50)]
        public string Type { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Name")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return Name;
        }
    }


    /// <summary>
    /// Город
    /// </summary>
    [DefaultClassOptions]
    public class City : BaseObject
    {
        public City() { }
        public City(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [Size(50)]
        public string Code { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [Size(50)]
        public string Type { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Name")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return Name;
        }
    }
}
