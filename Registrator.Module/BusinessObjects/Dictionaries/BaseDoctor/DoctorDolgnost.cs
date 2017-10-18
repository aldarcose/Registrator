using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    public class DoctorDolgnost : BaseObject
    {
        public DoctorDolgnost(Session session) : base(session) { }

        // Пример записи из XML
        // <ROWDATA>
            // <ROW dolg_doctor_id="703001" dolg_doctor="Рабиолог" nagruzka="null" name_file="null" zena_oms="null" zena_platno="null" zena_inogorod="null" flag_dolg="0" can_send_from_doctor="1" max_days_can_send="null" max_date_can_send="null" eq_spec_fed_codes_id="null"/>
            // <ROW dolg_doctor_id="11" dolg_doctor="ТЕРАПЕВТ" nagruzka="449" name_file="null" zena_oms="39,59" zena_platno="0" zena_inogorod="96,05" flag_dolg="0" can_send_from_doctor="1" max_days_can_send="31" max_date_can_send="01.01.2020" eq_spec_fed_codes_id="11"/>

        /// <summary>
        /// Код должности
        /// </summary>
        public int Code {get;set;}

        public override string ToString()
        {
            return Name;
        }
        
        /// <summary>
        /// Название должности
        /// </summary>
        [Size(250)]
        public string Name {get; set;}

        /// <summary>
        /// Нагрузка
        /// </summary>
        public int? Nagruzka {get; set;}

        /// <summary>
        /// Имя файла
        /// </summary>
        [Size(255)]
        public string NameFile {get; set;}

        /// <summary>
        /// Цена ОМС
        /// </summary>
        public decimal? ZenaOms {get;set;}

        /// <summary>
        /// Цена платно
        /// </summary>
        public decimal? ZenaPlatno {get;set;}

        /// <summary>
        /// Цена иногород.
        /// </summary>
        public decimal? ZenaInogorod {get;set;}

        /// <summary>
        /// Флаг дол.
        /// </summary>
        public int FlagDolg {get;set;}

        /// <summary>
        /// Can send from doctor
        /// </summary>
        public int CanSendFromDoctor {get;set;}

        /// <summary>
        /// Макс. кол-во дней
        /// </summary>
        public int? MaxDaysCanSend {get;set;}

        /// <summary>
        /// Макс. дата для отправки
        /// </summary>
        public DateTime? MaxDateCanSend {get;set;}

        /// <summary>
        /// Что то непонятное
        /// </summary>
        public int? EqSpecFedCodesId {get;set;}

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            const string elementsContainer = "ROWDATA";
            const string elementNameStartsWith = "ROW";

            const string code_attr = "dolg_doctor_id";
            const string name_attr = "dolg_doctor";
            const string nagruzka_attr ="nagruzka";
            const string name_file_attr = "name_file";
            const string zena_oms_attr = "zena_oms";
            const string zena_platno_attr = "zena_platno";
            const string zena_inogorod_attr = "zena_inogorod";
            const string flag_dolg_attr = "flag_dolg";
            const string can_send_from_doctor_attr = "can_send_from_doctor";
            const string max_days_can_send_attr = "max_days_can_send";
            const string max_date_can_send_attr ="max_date_can_send";
            const string eq_spec_fed_codes_id_attr = "eq_spec_fed_codes_id";

            foreach (var element in doc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                DoctorDolgnost obj = objSpace.FindObject<DoctorDolgnost>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<DoctorDolgnost>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    var nagruzka = element.Attribute(nagruzka_attr);
                    obj.Nagruzka = (nagruzka == null || nagruzka.Value == "null") ? null : (int?) int.Parse(nagruzka.Value);

                    var nameFile = element.Attribute(name_file_attr);
                    obj.NameFile = (nameFile == null || nameFile.Value == "null") ? null : nameFile.Value;

                    var zenaOms = element.Attribute(zena_oms_attr);
                    obj.ZenaOms = (zenaOms == null || zenaOms.Value == "null") ? null : (decimal?) Decimal.Parse(zenaOms.Value);

                    var zenaPlatno = element.Attribute(zena_platno_attr);
                    obj.ZenaPlatno = (zenaPlatno == null || zenaPlatno.Value == "null") ? null : (decimal?)Decimal.Parse(zenaPlatno.Value);

                    var zenaInogorod = element.Attribute(zena_inogorod_attr);
                    obj.ZenaInogorod = (zenaInogorod == null || zenaInogorod.Value == "null") ? null : (decimal?)Decimal.Parse(zenaInogorod.Value);

                    var flagDolg = element.Attribute(flag_dolg_attr);
                    obj.FlagDolg = (flagDolg == null || flagDolg.Value == "null") ? 0 : int.Parse(flagDolg.Value);

                    var canSendDoctor = element.Attribute(can_send_from_doctor_attr);
                    obj.CanSendFromDoctor = (canSendDoctor == null || canSendDoctor.Value == "null") ? 0 : int.Parse(canSendDoctor.Value);

                    var maxDays = element.Attribute(max_days_can_send_attr);
                    obj.MaxDaysCanSend = (maxDays == null || maxDays.Value == "null") ? null : (int?) int.Parse(maxDays.Value);

                    var maxDate = element.Attribute(max_date_can_send_attr);
                    obj.MaxDateCanSend = (maxDate == null || maxDate.Value == "null") ? null : (DateTime?)DateTime.Parse(maxDate.Value);

                    var eqSpec = element.Attribute(eq_spec_fed_codes_id_attr);
                    obj.EqSpecFedCodesId = (eqSpec == null || eqSpec.Value == "null") ? null : (int?)int.Parse(eqSpec.Value);
                }
            }
        }
    }
}
