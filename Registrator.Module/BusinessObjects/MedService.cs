using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Registrator.Module.BusinessObjects.Dictionaries;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Конкретная реализация общей услуги
    /// </summary>
    [Persistent]
    public class MedService : CommonService
    {
        public MedService(Session session) : base(session) { }

        public VisitCase VisitCase { get { return (VisitCase)Case; } }

        /*
        /// <summary>Цель посещения</summary>
        public CelPosescheniya CelPosescheniya { get { return ((VisitCase)Case).Cel; } }

        /// <summary>Место обслуживания</summary>
        public MestoObsluzhivaniya MestoObsluzhivaniya { get { return ((VisitCase)Case).Mesto; } }
        */

        #region Reestr Methods
        public override bool IsValidForReestr()
        {
            throw new NotImplementedException();
        }

        public override System.Xml.Linq.XElement GetReestrElement()
        {
            throw new NotImplementedException();

            // подумать как лучше сделать!
            var services = Case.Services.ToList<CommonService>();

            // сортируем по дате
            services.Sort((c1, c2) => c1.DateIn.CompareTo(c2.DateIn));

            int zap = services.IndexOf(this);

            return GetReestrElement(zap);
        }

        public override System.Xml.Linq.XElement GetReestrElement(int zapNumber)
        {
            // проверяем поля услуги
            if (IsValidForReestr() == false)
                return null;

            const string dateTimeFormat = "{0:yyyy-M-d}";

            XElement element = new XElement("USL");

            // идентификатор услуги
            element.Add(new XElement("IDSERV", zapNumber));

            // код МО
            element.Add(new XElement("LPU"), this.LPU.Code);

            // код подразделения МО
            if (!string.IsNullOrEmpty(this.LPU_1))
                element.Add(new XElement("LPU_1"), this.LPU_1);

            // код отделения
            if (this.Otdelenie != null)
                element.Add(new XElement("PODR"), this.LPU_1);

            // профиль мед. услуги
            element.Add(new XElement("PROFIL", this.Profil.Code));

            // вид мед. вмешательства
            if (this.VidVme != null)
                element.Add(new XElement("VID_VME", this.VidVme.Code));

            // признак детского профиля
            element.Add(new XElement("DET", (int)this.DetProfil));

            // даты лечения
            element.Add(new XElement("DATE_IN", string.Format(dateTimeFormat, this.DateIn)));
            element.Add(new XElement("DATE_OUT", string.Format(dateTimeFormat, this.DateOut)));

            // Диагноз, падает основной диагноз
            element.Add(new XElement("DS", this.CaseDiagnose.CODE));

            // Код оказанной услуги
            element.Add(new XElement("CODE_USL", Usluga.Code));

            // кол-во оказанных услуг
            element.Add(new XElement("KOL_USL", this.KolUslug.ToString("0.00")));

            // тариф услуги (для госпитализации брать КЗ из КСГ)
            if (this.Usluga != null && this.Usluga.Tarif.HasValue)
            {
                var value = this.Usluga.Tarif.Value;
                element.Add(new XElement("TARIF", value.ToString("0.00")));
            }

            // Сумма услуги
            element.Add(new XElement("SUMV_USL", this.Sum));

            // код специльаности врача
            element.Add(new XElement("PRVS", DoctorSpec.Code));

            // Код врача
            element.Add(new XElement("CODE_MD", this.Doctor.InnerCode));

            if (!string.IsNullOrEmpty(this.Comment))
                element.Add(new XElement("COMENTU", this.Comment));

            return element;
        }
        #endregion
    }
}
