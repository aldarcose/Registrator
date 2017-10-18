using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Uchastok : BaseObject
    {
        public Uchastok(Session session) : base(session) { }

        public override string ToString()
        {
            return Name;
        }

        // Пример записи из XML
        // <ROWDATA>
        // <ROW uchactok_id="1860010" uchactok="37-й&#160;ВСП" doctor_id="1906" fio="Цыбикжапова&#160;А.Б." otdel="Амбулатория&#160;п.Сокол" full_doctor_id="1906" otdel_id="9004"/>
        // <ROW uchactok_id="1870010" uchactok="38-й&#160;ВСП" doctor_id="459" fio="Цыренжапова&#160;Ж.И." otdel="Амбулатория&#160;п.Сокол" full_doctor_id="459" otdel_id="9004"/>

        /// <summary>
        /// Код участка
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Имя участка
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Полный код доктора, привязанного к участку
        /// </summary>
        public int? DoctorFullId { get; set; }

        /// <summary>
        /// Код доктора, привязанного к участку
        /// </summary>
        protected int? DoctorId { get; set; }

        /// <summary>
        /// ФИО доктора
        /// </summary>
        [Size(255)]
        protected string DoctorFio { get; set; }

        /// <summary>
        /// Код отдела участка
        /// </summary>
        public int? OtdelId { get; set; }

        /// <summary>
        /// Название отдела
        /// </summary>
        [Size(255)]
        public string OtdelName { get; set; }

        /// <summary>
        /// Доктора, прикрепленные к участку
        /// </summary>
        [Association("Uchastok-Doctor")]
        public XPCollection<Doctor> Doctors
        {
            get
            {
                return GetCollection<Doctor>("Doctors");
            }
        }

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            const string elementsContainer = "ROWDATA";
            const string elementNameStartsWith = "ROW";

            const string code_attr = "uchactok_id";
            const string name_attr = "uchactok";
            const string doctor_full_id_attr = "full_doctor_id";
            const string doctor_id_attr = "doctor_id";
            const string doctor_fio_attr = "fio";
            const string otdel_id_attr = "zena_oms";
            const string otdel_name_attr = "otdel_id";
            

            foreach (var element in doc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                Uchastok obj = objSpace.FindObject<Uchastok>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<Uchastok>();
                    obj.Code = int.Parse(element.Attribute(code_attr).Value);
                    obj.Name = element.Attribute(name_attr).Value;

                    var doctorFullId = element.Attribute(doctor_full_id_attr);
                    obj.DoctorFullId = (doctorFullId == null || doctorFullId.Value == "null") ? null : (int?)int.Parse(doctorFullId.Value);
                    
                    var doctorId = element.Attribute(doctor_id_attr);
                    obj.DoctorId = (doctorId == null || doctorId.Value == "null") ? null : (int?)int.Parse(doctorId.Value);

                    var otdelId = element.Attribute(otdel_id_attr);
                    obj.OtdelId = (otdelId == null || otdelId.Value == "null") ? null : (int?)int.Parse(otdelId.Value);

                    var doctorFio = element.Attribute(doctor_fio_attr);
                    obj.DoctorFio = (doctorFio == null || doctorFio.Value == "null") ? null : doctorFio.Value;

                    var otdelName = element.Attribute(otdel_name_attr);
                    obj.OtdelName = (otdelName == null || otdelName.Value == "null") ? null : otdelName.Value;
                }
                else
                {
                    if (obj.DoctorFullId != null)
                    {
                        var doctor = objSpace.FindObject<Doctor>(DevExpress.Data.Filtering.CriteriaOperator.Parse("InnerCode=?", obj.DoctorFullId));
                        if (doctor != null)
                            obj.Doctors.Add(doctor);
                    }
                }
            }
        }
    }
}
