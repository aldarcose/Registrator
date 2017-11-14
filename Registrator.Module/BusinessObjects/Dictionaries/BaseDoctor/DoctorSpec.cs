using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{   
    /// <summary>
    /// Виды специальностей (PRVS)
    /// </summary>
    [DefaultClassOptions]
    public class DoctorSpec : BaseObject
    {
        public DoctorSpec() { }
        public DoctorSpec(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [Size(10)]
        public string Recid { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        [Size(10)]
        public string Code { get; set; }

        
        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name { get; set; }
        
        /// <summary>
        /// Категория
        /// </summary>
        [Size(10)]
        public string High { get; set; }

        /// <summary>
        /// OKSO
        /// </summary>
        [Size(10)]
        public string Okso { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public DateTime? DateBeg { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public DateTime? DateEnd { get; set; }


        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Concat('(', Code, ') ', Name)")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return String.Format("({0}) {1}", Code, Name);
        }

        // пример записи в XML
        // <zap RECID="0" CODE="0" NAME="Врачебные специальности" HIGH="" OKSO="1" DATEBEG="26.12.2013" DATEEND="" />
        // <zap RECID="1" CODE="1" NAME="Лечебное дело. Педиатрия" HIGH="0" OKSO="2" DATEBEG="26.12.2013" DATEEND="" />
        // <zap RECID="2" CODE="2" NAME="Медико-профилактическое дело" HIGH="0" OKSO="138" DATEBEG="26.12.2013" DATEEND="" />
        // <zap RECID="3" CODE="3" NAME="Стоматология" HIGH="0" OKSO="154" DATEBEG="26.12.2013" DATEEND="" />

        /// <summary>
        /// Добавляет в базу классификаторы из файла XML
        /// </summary>
        /// <param name="updater">Пространство объектов ObjectSpace</param>
        /// <param name="xmlPath">Путь до файла классификатора</param>
        public static void UpdateDbFromXml(IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            foreach (XElement el in doc.Root.Elements("zap"))
            {
                string id = el.Attribute("CODE").Value;
                DoctorSpec obj = objSpace.FindObject<DoctorSpec>(CriteriaOperator.Parse("Code=?", id));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<DoctorSpec>();
                    obj.Recid = el.Attribute("RECID").Value;
                    obj.Code = el.Attribute("CODE").Value;
                    obj.Name = el.Attribute("NAME").Value;
                    obj.High = el.Attribute("HIGH").Value == "" ? null : el.Attribute("HIGH").Value;
                    obj.Okso = el.Attribute("OKSO").Value;
                    obj.DateBeg = el.Attribute("DATEBEG").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEBEG").Value);
                    obj.DateEnd = el.Attribute("DATEEND").Value == "" ? null : (DateTime?)Convert.ToDateTime(el.Attribute("DATEEND").Value);
                }
            }
        }
    }
    
    [DefaultClassOptions]
    public class DoctorSpecTree : BaseObject, ITreeNode
    {
        private bool scheduling;

        public DoctorSpecTree(Session session) : base(session) { }

        [Browsable(false)]
        [Association("SpecParent-SpecChildren")]
        public DoctorSpecTree parent { get; set; }
        
        [XafDisplayName("Категория")]
        public ITreeNode Parent { get { return parent as ITreeNode; } }

        [Association("SpecParent-SpecChildren"), DevExpress.Xpo.Aggregated]
        [Browsable(false)]
        public XPCollection<DoctorSpecTree> children 
        {
            get { return GetCollection<DoctorSpecTree>("children"); }
        }
        
        [XafDisplayName("Вложенные специальности")]
        public System.ComponentModel.IBindingList Children
        {
            get { return children as System.ComponentModel.IBindingList; }
        }

        [XafDisplayName("Имя специальности")]
        public string Name { get; set; }

        [XafDisplayName("Код специальности")]
        public string Code { get; set; }

        /// <summary>
        /// Профиль специальности
        /// </summary>
        [XafDisplayName("Мед. профиль специальность")]
        [Association("MedProfil-DoctorSpecs")]
        public MedProfil MedProfil { get; set; }

        [DataSourceProperty("TerrUslugi")]
        [XafDisplayName("Услуга, оказываемая на дому")]
        public TerritorialUsluga UslugaNaDomy { get; set; }

        [DataSourceProperty("TerrUslugi")]
        [XafDisplayName("Услуга, оказываемая в ЛПУ")]
        public TerritorialUsluga UslugaLPU { get; set; }

        [DataSourceProperty("TerrUslugi")]
        [XafDisplayName("Услуга, оказываемая по МУР")]
        public TerritorialUsluga UslugaMUR { get; set; }

        /// <summary>
        /// Оказываемые специалистом услуги
        /// </summary>
        [XafDisplayName("Оказываемые специальностью услуги")]
        [Association("DoctorSpec-TerrServices")]
        public XPCollection<TerritorialUsluga> TerrUslugi
        {
            get { return GetCollection<TerritorialUsluga>("TerrUslugi"); }
        }

        /// <summary>
        /// Возможность создания расписания
        /// </summary>
        [VisibleInLookupListView(false)]
        [XafDisplayName("Возможность создания расписания")]
        public bool Scheduling
        {
            get { return scheduling; }
            set { SetPropertyValue("Scheduling", ref scheduling, value); }
        }

        public static void GetTree(IObjectSpace objectSpace, List<DoctorSpec> specs, ITreeNode parentNode)
        {
            var parent = parentNode as DoctorSpecTree;
            if (parent != null)
            {
                foreach (var spec in specs.Where(t => t.High == parent.Code))
                {
                    var node = objectSpace.CreateObject<DoctorSpecTree>();
                    node.Code = spec.Code;
                    node.Name = spec.Name;
                    node.parent = parent;
                    parent.children.Add(node);
                    GetTree(objectSpace, specs, node);
                }
            }
            else
            {
                foreach (var spec in specs.Where(t => t.High == null || t.High == string.Empty))
                {
                    var firstLevelNode = objectSpace.CreateObject<DoctorSpecTree>();
                    firstLevelNode.Code = spec.Code;
                    firstLevelNode.Name = spec.Name;
                    GetTree(objectSpace, specs, firstLevelNode);
                }
            }
        }
    }
}

