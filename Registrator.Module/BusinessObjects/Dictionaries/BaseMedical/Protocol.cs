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
        /// �������
        /// </summary>
        [Size(1000)]
        [XafDisplayName("�������")]
        public string Anamnez { get; set; }

        /// <summary>
        /// ������� ��������
        /// </summary>
        [XafDisplayName("������� ��������")]
        public IList<AnamnezTemplate> AnamnezTemplates
        {
            get { return CurrentDoctor.AnamnezTemplates; }
        }

        /// <summary>
        /// ������
        /// </summary>
        [Size(1000)]
        [XafDisplayName("������")]
        public string Complain { get; set; }

        /// <summary>
        /// ������� �����
        /// </summary>
        [XafDisplayName("������� �����")]
        public IList<ComplainTemplate> ComplainTemplates
        {
            get { return CurrentDoctor.ComplainTemplates; }
        }

        /// <summary>
        /// ������������
        /// </summary>
        [Size(1000)]
        [XafDisplayName("������������")]
        public string Recommendation { get; set; }

        /// <summary>
        /// ������� ������������
        /// </summary>
        [XafDisplayName("������� ������������")]
        public IList<RecomendTemplate> RecomendTemplates
        {
            get { return CurrentDoctor.RecomendTemplates; }
        }

        /// <summary>
        /// ����������� ������ ���������
        /// </summary>
        [Size(1000)]
        [XafDisplayName("����������� ������ ���������")]
        public string ObjectiveStatus { get; set; }

        /// <summary>
        /// ������� ������������ �������
        /// </summary>
        [XafDisplayName("������� ������������ �������")]
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
        /// ��������
        /// </summary>
        [Association("Protocol-Records")]
        [XafDisplayName("��������")]
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
            
            // ������� �� ���� ����������
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
                // ��������� ���
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
                // ��������� �������
                switch (type.TimeType)
                {
                        // ���� ������ �� ��������
                    case TimeTypes.FromLimit:
                        // ���� ���. ��� ������ ��� (���� ���� ���������, ���. ����� ������)
                        if (age < type.TimeFrom.Year || (age == type.TimeFrom.Year && month < type.TimeFrom.Month))
                        {
                            // ����� �������
                            listToRemove.Add(protocolRecord);
                        }
                        break;
                        // ���� ������ �� ��������
                    case TimeTypes.ToLimit:
                        // ���� ���. ��� ������ ��� (���� ���� ���������, �� ���. ����� ������)
                        if (age > type.TimeTo.Year || (age == type.TimeTo.Year && month > type.TimeTo.Month))
                        {
                            listToRemove.Add(protocolRecord);
                        }
                        break;
                        // ���� ��������
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
