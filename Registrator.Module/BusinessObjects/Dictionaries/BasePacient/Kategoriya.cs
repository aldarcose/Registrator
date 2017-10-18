using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.Xml.Linq;
using DevExpress.Persistent.Base;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    public class Kategoriya : BaseObject
    {
        public Kategoriya(Session session) : base(session) { }

        // Пример записи из XML
        // <ROWDATA>
        // <ROW id="1" name="работающий" kod_oms="1" code="1" lpu_id="03001" date_vnes_inf="2013-03-27&#160;07:44:37.249096+09" operator_id="null" pocket_id="null" sort="null" uchastok_id="null" reestr_code="1"/>
        // <ROW id="2" name="неработающий&#160;&#160;&#160;" kod_oms="5,A" code="2" lpu_id="03001" date_vnes_inf="2013-03-27&#160;07:44:37.249096+09" operator_id="null" pocket_id="null" sort="null" uchastok_id="null" reestr_code="2"/>

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
        /// Код ОМС
        /// </summary>
        public string CodeOMS { get; set; }

        /// <summary>
        /// Код ЛПУ
        /// </summary>
        public int? CodeLPU { get; set; }

        /// <summary>
        /// ФИО доктора
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

        /// <summary>
        /// Идентификатор участка
        /// </summary>
        public int? UchastokId { get; set; }

        /// <summary>
        /// Код реестра
        /// </summary>
        public int? CodeReestr { get; set; }

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            const string elementsContainer = "ROWDATA";
            const string elementNameStartsWith = "ROW";

            const string code_attr = "id";
            const string name_attr = "name";
            const string inner_code_attr = "code";
            const string code_oms_attr = "kod_oms";
            const string lpu_id_attr = "lpu_id";
            const string date_attr = "date_vnes_inf";
            const string operator_id_attr = "operator_id";
            const string pocket_id_attr = "pocket_id";
            const string sort_attr = "sort";
            const string uchastok_id_attr = "uchastok_id";
            const string code_reestr_attr = "reestr_code";


            foreach (var element in doc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                Kategoriya obj = objSpace.FindObject<Kategoriya>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<Kategoriya>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    var innerCode = element.Attribute(inner_code_attr);
                    obj.InnerCode = (innerCode == null || innerCode.Value == "null") ? 0 : int.Parse(innerCode.Value);

                    var codeOms2 = element.Attribute(code_oms_attr);
                    obj.CodeOMS = (codeOms2 == null || codeOms2.Value == "null") ? null : codeOms2.Value;

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

                    var uchastokId = element.Attribute(uchastok_id_attr);
                    obj.UchastokId = (uchastokId == null || uchastokId.Value == "null") ? null : (int?)int.Parse(uchastokId.Value);

                    var codeReestr = element.Attribute(code_reestr_attr);
                    obj.CodeReestr = (codeReestr == null || codeReestr.Value == "null") ? null : (int?)int.Parse(codeReestr.Value);
                }
            }
        }
    }
}
