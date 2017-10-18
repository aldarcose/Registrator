using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;

using System.Xml.Linq;

using Registrator.Module.BusinessObjects.Dictionaries;
using System.ComponentModel;
using DevExpress.Data.Filtering;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Клинико-статистические группы
    /// </summary>
    [DefaultClassOptions]
    [XafDisplayName("КСГ")]
    public class ClinicStatGroups : BaseObject
    {
        public ClinicStatGroups(Session session) : base(session) { }

        /// <summary>
        /// Профиль доктора
        /// </summary>
        [DataSourceCriteriaProperty("MedProfilCriteria")]
        public MedProfil Profil { get; set; }
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [NonPersistent]
        [Browsable(false)]
        private CriteriaOperator MedProfilCriteria
        {
            get
            {
                // Не разрешаем указывать в КСГ "закрытые" профили
                return CriteriaOperator.Parse("DateEnd is null");
            }
        }

        /// <summary>
        /// Номер КСГ
        /// </summary>
        [XafDisplayName("Номер КСГ")]
        public int Number { get; set; }

        /// <summary>
        /// Имя КСГ
        /// </summary>
        [XafDisplayName("КСГ")]
        [Size(650)]
        public string Name { get; set; }

        /// <summary>
        /// Диагноз
        /// </summary>
        // уникальный для каждого КСГ. Использовать при поиске дубликатов
        public MKB10 Diagnose { get; set; }

        /// <summary>
        /// Коэффициент затрат для КСГ
        /// </summary>
        [XafDisplayName("Коэффициент затрат")]
        public double KoeffZatrat { get; set; }

        public override string ToString()
        {
            return Name;
        }

        // пример записи из XML
        // <ROWDATA>
        //  <ROW id="100000" lpu_id="03001" date_vnes_inf="2015-02-11&#160;09:44:05.098924+08" operator_id="2469" pocket_id="null" code="136" sort="null" diagn="O12.0" ksg="1" name_ksg="Отеки,&#160;протеинурия,&#160;гипертензивные&#160;расстройства&#160;в&#160;период&#160;беременности,&#160;в&#160;родах&#160;и&#160;после&#160;родов" kz="0,82" name_profile="Акушерство&#160;и&#160;гинекология"/>
        //  <ROW id="203001" lpu_id="03001" date_vnes_inf="2015-02-11&#160;09:43:56.323753+08" operator_id="2469" pocket_id="null" code="136" sort="null" diagn="O12.2" ksg="1" name_ksg="Отеки,&#160;протеинурия,&#160;гипертензивные&#160;расстройства&#160;в&#160;период&#160;беременности,&#160;в&#160;родах&#160;и&#160;после&#160;родов" kz="0,82" name_profile="Акушерство&#160;и&#160;гинекология"/>

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            const string elementsContainer = "ROWDATA";
            const string elementNameStartsWith = "ROW";

            // диагноз в КСГ является уникальным, сверяем по нему.
            const string code_attr = "diagn";

            StringBuilder log = new StringBuilder();

            foreach (var element in doc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                var diagnoseCode = element.Attribute(code_attr).Value;

                // ищем в справочнике диагнозов диагноз по коду.
                MKB10 diagnose = objSpace.FindObject<MKB10>(DevExpress.Data.Filtering.CriteriaOperator.Parse("MKB=?", diagnoseCode));

                // если диагноз не найден
                if (diagnose == null)
                {
                    log.AppendLine(string.Format("id: {1}. Не найден диагноз: {0}", diagnoseCode, element.Attribute("id").Value));
                    log.AppendLine("\tУдаляем точку в конце и ищем заново");
                    if (diagnoseCode[diagnoseCode.Length - 1].Equals('.'))
                    {
                        diagnose = objSpace.FindObject<MKB10>(DevExpress.Data.Filtering.CriteriaOperator.Parse("MKB=?", diagnoseCode.Substring(0, diagnoseCode.Length - 1)));
                        if (diagnose == null)
                        {
                            log.AppendLine("\tНе найдено");
                        }
                        else
                            log.AppendLine("\tНайдено");
                    }
                    continue;
                }

                ClinicStatGroups obj = objSpace.FindObject<ClinicStatGroups>(DevExpress.Data.Filtering.CriteriaOperator.Parse("MainDiagnose=?", diagnose));

                if (obj == null)
                {
                    obj = objSpace.CreateObject<ClinicStatGroups>();

                    obj.Name = element.Attribute("name_ksg").Value;
                    obj.Number = int.Parse(element.Attribute("ksg").Value);
                    obj.KoeffZatrat = double.Parse(element.Attribute("kz").Value);

                    obj.Diagnose = diagnose;

                    var codeProfil = element.Attribute("code").Value;
                    obj.Profil = objSpace.FindObject<MedProfil>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", codeProfil));
                }
            }

            if (log.Length > 0)
                System.IO.File.AppendAllText(xmlPath + ".log", log.ToString(), Encoding.UTF8);
        }
    }
}
