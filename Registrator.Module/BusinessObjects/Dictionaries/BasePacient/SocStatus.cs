using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    public class SocStatus : BaseObject
    {
        public SocStatus(Session session) : base(session) { }

        // Пример записи из XML
        // <ROWDATA>
        // <ROW id="1" name="Работающий,&#160;служащий" code="1" kod_oms2="null" lpu_id="03001" date_vnes_inf="2011-06-09&#160;13:57:57.371729+09" operator_id="null" pocket_id="null" sort="null"/>
        // <ROW id="2" name="Неработающий" code="2" kod_oms2="null" lpu_id="03001" date_vnes_inf="2011-06-09&#160;13:57:57.371729+09" operator_id="null" pocket_id="null" sort="null"/>

        /// <summary>
        /// Код статуса (Id)
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Имя статуса
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Код (Code) ??
        /// </summary>
        public int InnerCode { get; set; }

        /// <summary>
        /// Код ОМС2
        /// </summary>
        public int? CodeOMS2 { get; set; }

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

            const string code_attr = "id";
            const string name_attr = "name";
            const string inner_code_attr = "code";
            const string code_oms2_attr = "kod_oms2";
            const string lpu_id_attr = "lpu_id";
            const string date_attr = "date_vnes_inf";
            const string operator_id_attr = "operator_id";
            const string pocket_id_attr = "pocket_id";
            const string sort_attr = "sort";


            foreach (var element in doc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                SocStatus obj = objSpace.FindObject<SocStatus>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<SocStatus>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    var innerCode = element.Attribute(inner_code_attr);
                    obj.InnerCode = (innerCode == null || innerCode.Value == "null") ? 0 : int.Parse(innerCode.Value);

                    var codeOms2 = element.Attribute(code_oms2_attr);
                    obj.CodeOMS2 = (codeOms2 == null || codeOms2.Value == "null") ? null : (int?)int.Parse(codeOms2.Value);

                    var lpu = element.Attribute(lpu_id_attr);
                    obj.CodeLPU = (lpu == null || lpu.Value == "null") ? null : (int?)int.Parse(lpu.Value);

                    var dateBegin = element.Attribute(date_attr);
                    obj.DateBegin = (dateBegin == null || dateBegin.Value == "null") ? null : (DateTime?) DateTime.Parse(dateBegin.Value);

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
