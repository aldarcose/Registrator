using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Registrator.Module.BusinessObjects.Dictionaries;
using DevExpress.Persistent.Base;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Конкретная реализация общей услуги
    /// </summary>
    [Persistent]
    [VisibleInReports(true)]
    public class MedService : CommonService
    {
        public MedService(Session session) : base(session) { }

        public VisitCase VisitCase { get { return (VisitCase)Case; } }

        #region Reestr Methods
        public override bool IsValidForReestr()
        {
            throw new NotImplementedException();
        }

        public override System.Xml.Linq.XElement GetReestrElement()
        {
            throw new NotImplementedException();

            // подумать как лучше сделать!
            //var services = Case.Services.ToList<CommonService>();
            //// сортируем по дате
            //services.Sort((c1, c2) => c1.DateIn.CompareTo(c2.DateIn));
            //int zap = services.IndexOf(this);
            //return GetReestrElement(zap);
        }

        public override System.Xml.Linq.XElement GetReestrElement(int zapNumber)
        {
            const string decimalFormat = "n2";

            // проверяем поля услуги
            //if (IsValidForReestr() == false)
            //    return null;

            const string dateTimeFormat = "{0:yyyy-M-d}";

            XElement element = new XElement("USL");

            // идентификатор услуги
            element.Add(new XElement("IDSERV", zapNumber));

            // код МО
            element.Add(new XElement("LPU", this.LPU.Code));

            // код подразделения МО
            if (!string.IsNullOrEmpty(this.LPU_1))
                element.Add(new XElement("LPU_1", this.LPU_1));

            // код отделения
            if (this.Otdelenie != null)
                element.Add(new XElement("PODR", this.LPU_1));

            // профиль мед. услуги
            if (Profil != null)
                element.Add(new XElement("PROFIL", Profil.Code));

            // вид мед. вмешательства
            if (this.VidVme != null)
                element.Add(new XElement("VID_VME", this.VidVme.Code));

            // признак детского профиля
            element.Add(new XElement("DET", (int)this.DetProfil));

            // даты лечения
            element.Add(new XElement("DATE_IN", string.Format(dateTimeFormat, this.DateIn)));
            element.Add(new XElement("DATE_OUT", string.Format(dateTimeFormat, this.DateOut)));

            // Диагноз, падает основной диагноз
            if (VisitCase.MainDiagnose != null && VisitCase.MainDiagnose.Diagnose != null)
                element.Add(new XElement("DS", VisitCase.MainDiagnose.Diagnose.CODE));

            // Код оказанной услуги
            element.Add(new XElement("CODE_USL", Usluga.Code));

            // кол-во оказанных услуг
            element.Add(new XElement("KOL_USL", this.KolUslug.ToString("0.00")));

            // тариф услуги (для госпитализации брать КЗ из КСГ)
            if (this.Usluga != null && this.Usluga.Tarif.HasValue)
            {
                decimal value = this.Usluga.Tarif.Value;
                element.Add(new XElement("TARIF", value.ToString(decimalFormat).Replace(",", ".")));
            }

            // Сумма услуги
            element.Add(new XElement("SUMV_USL", this.Sum.ToString(decimalFormat).Replace(",", ".")));

            // код специльаности врача
            element.Add(new XElement("PRVS", DoctorSpec.Code));

            // Код врача
            element.Add(new XElement("CODE_MD", this.Doctor.SNILS));

            if (!string.IsNullOrEmpty(this.Comment))
                element.Add(new XElement("COMENTU", this.Comment));

            return element;
        }
        #endregion
    }
}
