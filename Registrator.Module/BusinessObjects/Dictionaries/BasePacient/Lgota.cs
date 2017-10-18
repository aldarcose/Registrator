using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    public class Lgota : BaseObject
    {
        public Lgota(Session session) : base(session) { }

        // Пример записи из XML
        // <ROWDATA>
        // <ROW id="113" name="Дети до 18 перв. и втор. поколения граждан,получивших дозу&gt;5бэр" diagn_id="null" code="113" vid_preparat_id="1" lpu_id="null" date_vnes_inf="null" operator_id="null" pocket_id="null" sort="null"/>
        // <ROW id="112" name="Граждане, получившие дозу 5-25 бэр" diagn_id="null" code="112" vid_preparat_id="1" lpu_id="null" date_vnes_inf="null" operator_id="null" pocket_id="null" sort="null"/>

        /// <summary>
        /// Идентификатор льготы (Id)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор диагноза (?)
        /// </summary>
        [Size(255)]
        public string DiagnoseId { get; set; }

        /// <summary>
        /// Код льготы
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Идентификатор вида препарата
        /// </summary>
        public int? VidPreparatId { get; set; }

        /// <summary>
        /// Код ЛПУ
        /// </summary>
        public int? CodeLPU { get; set; }

        /// <summary>
        /// Дата внесения записи
        /// </summary>
        public DateTime? DateBegin { get; set; }

        /// <summary>
        /// Идентификатор оператора
        /// </summary>
        public int? OperatorId { get; set; }

        /// <summary>
        /// Идентификатор пакета
        /// </summary>
        public int? PocketId { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public string Sort { get; set; }

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            const string elementsContainer = "ROWDATA";
            const string elementNameStartsWith = "ROW";

            const string id_attr = "id";
            const string name_attr = "name";
            const string inner_code_attr = "code";
            const string diagn_id_attr = "diagn_id";
            const string vid_preparat_id_attr = "vid_preparat_id";
            const string lpu_id_attr = "lpu_id";
            const string date_attr = "date_vnes_inf";
            const string operator_id_attr = "operator_id";
            const string pocket_id_attr = "pocket_id";
            const string sort_attr = "sort";


            foreach (var element in doc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                Lgota obj = objSpace.FindObject<Lgota>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Id=?", element.Attribute(id_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<Lgota>();
                    obj.Id = int.Parse(element.Attribute(id_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    var innerCode = element.Attribute(inner_code_attr);
                    obj.Code = (innerCode == null || innerCode.Value == "null") ? "" : innerCode.Value;

                    var diagnoseId = element.Attribute(diagn_id_attr);
                    obj.DiagnoseId = (diagnoseId == null || diagnoseId.Value == "null") ? null : diagnoseId.Value;

                    var vidPreparat = element.Attribute(vid_preparat_id_attr);
                    obj.VidPreparatId = (vidPreparat == null || vidPreparat.Value == "null") ? null : (int?)int.Parse(vidPreparat.Value);

                    var lpu = element.Attribute(lpu_id_attr);
                    obj.CodeLPU = (lpu == null || lpu.Value == "null") ? null : (int?)int.Parse(lpu.Value);

                    var dateBegin = element.Attribute(date_attr);
                    obj.DateBegin = (dateBegin == null || dateBegin.Value == "null") ? null : (DateTime?)DateTime.Parse(dateBegin.Value);

                    var operatorId = element.Attribute(operator_id_attr);
                    obj.OperatorId = (operatorId == null || operatorId.Value == "null") ? null : (int?)int.Parse(operatorId.Value);

                    var pocketId = element.Attribute(pocket_id_attr);
                    obj.PocketId = (pocketId == null || pocketId.Value == "null") ? null : (int?)int.Parse(pocketId.Value);

                    var sort = element.Attribute(sort_attr);
                    obj.Sort = (sort == null || sort.Value == "null") ? null : sort.Value;
                }
            }
        }
    }
}
