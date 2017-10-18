using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using System.Xml.Linq;
namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Классификатор причин отказа в оплате медицинской помощи
    /// </summary>
    public class F014 : BaseObject
    {
        // пример записи в XML:
        // <rec1 Kod="1" IDVID="" Naim="Нарушение прав застрахованных лиц на получение медицинской помощи в медицинской организации на выбор медицинской организации из медицинских организаций, участвующих в реализации территориальной программы обязательного медицинского страхования." Osn="1.1.1." Komment="Нарушение прав ЗЛ на выбор МО" KodPG="" DATEBEG="01.01.2011" DATEEND="" />
        // <rec2 Kod="2" IDVID="" Naim="Нарушение прав застрахованных лиц на получение медицинской помощи в медицинской организации на выбор врача путем подачи заявления лично или через своего представителя на имя руководителя медицинской организации." Osn="1.1.2." Komment="Нарушение прав ЗЛ на выбор врача" KodPG="" DATEBEG="01.01.2011" DATEEND="" />
        // <rec3 Kod="3" IDVID="" Naim="Нарушение прав застрахованных лиц на получение медицинской помощи в медицинской организации: нарушение условий оказания медицинской помощи, в том числе сроки ожидания медицинской помощи, предоставляемой в плановом порядке." Osn="1.1.3." Komment="Нарушение прав ЗЛ (условия оказания МП, сроки ожидания)" KodPG="" DATEBEG="01.01.2011" DATEEND="" />

        public F014() : base() { }
        public F014(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(650)]
        public string Name { get; set; }

        /// <summary>
        /// Номер Osn
        /// todo: что это?
        /// </summary>
        [Size(50)]
        public string Osn {get;set;}
        
        /// <summary>
        /// Комментарий
        /// </summary>
        [Size(255)]
        public string Comment {get;set;}

        public string IDVID { get; set; }
        public string KodPG {get;set;}

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public DateTime? DateBeg { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Добавляет в базу классификаторы из файла XML
        /// </summary>
        /// <param name="updater">Пространство объектов ObjectSpace</param>
        /// <param name="xmlPath">Путь до файла классификатора</param>
        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            const string elementNameStartsWith = "rec";

            const string code_attr = "Kod";
            const string name_attr = "Naim";
            const string osn_attr = "Osn";
            const string comment_attr = "Komment";
            const string idvid_attr = "IDVID";
            const string kodPg_attr = "KodPG";
            const string dateBeg_attr = "DATEBEG";
            const string dateEnd_attr = "DATEEND";

            foreach (var element in doc.Root.Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                F014 obj = objSpace.FindObject<F014>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<F014>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    obj.Osn = element.Attribute(osn_attr).Value;
                    obj.Comment = element.Attribute(comment_attr).Value;
                    obj.IDVID = element.Attribute(idvid_attr).Value;
                    obj.KodPG = element.Attribute(kodPg_attr).Value;

                    obj.DateBeg = element.Attribute(dateBeg_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateBeg_attr).Value);
                    obj.DateEnd = element.Attribute(dateEnd_attr).Value == "" ? null : (DateTime?)Convert.ToDateTime(element.Attribute(dateEnd_attr).Value);
                }
            }
        }
    }
}