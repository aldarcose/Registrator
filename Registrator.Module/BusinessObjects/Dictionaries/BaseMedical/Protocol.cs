using System;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Abstract;
using System.Collections.Generic;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    public class CommonProtocol : AbstractProtocol
    {
        public CommonProtocol(Session session) : base(session) { }

        /// <summary>
        /// Анамнез
        /// </summary>
        [Size(1000)]
        [XafDisplayName("Анамнез")]
        public string Anamnez { get; set; }

        /// <summary>
        /// Шаблоны анамнеза
        /// </summary>
        [XafDisplayName("Шаблоны анамнеза")]
        public IList<AnamnezTemplate> AnamnezTemplates
        {
            get { return CurrentDoctor.AnamnezTemplates; }
        }

        /// <summary>
        /// Жалобы
        /// </summary>
        [Size(1000)]
        [XafDisplayName("Жалобы")]
        public string Complain { get; set; }

        /// <summary>
        /// Шаблоны жалоб
        /// </summary>
        [XafDisplayName("Шаблоны жалоб")]
        public IList<ComplainTemplate> ComplainTemplates
        {
            get { return CurrentDoctor.ComplainTemplates; }
        }

        /// <summary>
        /// Рекомендации
        /// </summary>
        [Size(1000)]
        [XafDisplayName("Рекомендации")]
        public string Recommendation { get; set; }

        /// <summary>
        /// Шаблоны рекомендаций
        /// </summary>
        [XafDisplayName("Шаблоны рекомендаций")]
        public IList<RecomendTemplate> RecomendTemplates
        {
            get { return CurrentDoctor.RecomendTemplates; }
        }

        /// <summary>
        /// Объективный статус терапевта
        /// </summary>
        [Size(1000)]
        [XafDisplayName("Объективный статус терапевта")]
        public string ObjectiveStatus { get; set; }

        /// <summary>
        /// Шаблоны объективного статуса
        /// </summary>
        [XafDisplayName("Шаблоны объективного статуса")]
        public IList<ObjStatusTerTemplate> ObjStatusTerTemplates
        {
            get { return CurrentDoctor.ObjStatusTerTemplates; }
        }

        [Association("Protocol-Medicaments"), DevExpress.Xpo.Aggregated]
        public XPCollection<VipiskaMedicamenta> Medicaments
        {
            get { return GetCollection<VipiskaMedicamenta>("Medicaments"); }
        }
    }

    [DefaultClassOptions]
    public class EditableProtocol : AbstractProtocol
    {
        public EditableProtocol(Session session) : base(session) { }

        /// <summary>
        /// Протокол
        /// </summary>
        [Association("Protocol-Records")]
        [XafDisplayName("Протокол")]
        public XPCollection<ProtocolRecord> Records
        {
            get { return GetCollection<ProtocolRecord>("Records"); }
        }

        public void ApplyPacientFilter(Pacient pacient, DateTime? createDate = null)
        {
            var listToRemove = new List<ProtocolRecord>();
            var today= DateTime.Now;

            if (createDate.HasValue && (pacient.Birthdate < createDate))
                today = createDate.Value;
            var age = pacient.GetAge(today);
            DateTime bd = pacient.Birthdate.Value;
            
            // смотрим по году исполнения
            if (age >= 3)
            {
                if (today.Month < bd.Month)
                    age++;
                if (today.Month == bd.Month)
                    if (today.Day < bd.Day)
                        age++;
            }
            int month = pacient.GetMonthWithNoAge(today);

            foreach (var protocolRecord in Records)
            {
                var type = protocolRecord.Type;
                // проверяем пол
                switch (type.Gender)
                {
                    case GenderFor.All:
                        break;
                    case GenderFor.F:
                        if (pacient.Gender == Gender.Male)
                        {
                            listToRemove.Add(protocolRecord);
                            continue;
                        }
                        break;
                    case GenderFor.M:
                        if (pacient.Gender == Gender.Female)
                        {
                            listToRemove.Add(protocolRecord);
                            continue;
                        }
                        break;
                    default:
                        break;
                }
                // проверяем возраст
                switch (type.TimeType)
                {
                        // если фильтр ОТ возраста
                    case TimeTypes.FromLimit:
                        // если тек. год меньше или (если года совпадают, тек. месяц меньше)
                        if (age < type.TimeFrom.Year || (age == type.TimeFrom.Year && month < type.TimeFrom.Month))
                        {
                            // тогда удаляем
                            listToRemove.Add(protocolRecord);
                        }
                        break;
                        // если фильтр ДО возраста
                    case TimeTypes.ToLimit:
                        // если тек. год больше или (если года совпадают, то тек. месяц больше)
                        if (age > type.TimeTo.Year || (age == type.TimeTo.Year && month > type.TimeTo.Month))
                        {
                            listToRemove.Add(protocolRecord);
                        }
                        break;
                        // если диапазон
                    case TimeTypes.Range:
                        if (
                            (age < type.TimeFrom.Year || age > type.TimeTo.Year)
                            ||
                            (age == type.TimeFrom.Year && month < type.TimeFrom.Month)
                            ||
                            (age == type.TimeTo.Year && month > type.TimeTo.Month)
                            )
                        {
                            listToRemove.Add(protocolRecord);
                        }
                        break;
                    case TimeTypes.All:
                        break;
                    default:
                        break;
                }
            }

            foreach (var protocolRecord in listToRemove)
            {
                Records.Remove(protocolRecord);
            }
        }
    }
}
