using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Dictionaries;


namespace Registrator.Module.BusinessObjects
{
    //[DefaultClassOptions]
    [XafDisplayName("Карта вызова отделения неотложной медицинской помощи")]
    [Appearance("CallUrgentDocumentVisible", Context = "DetailView", Criteria = "!HasDocument", Visibility = ViewItemVisibility.Hide, TargetItems = "Document", AppearanceItemType = "LayoutItem")]
    [Appearance("CallUrgentPolicyVisible", Context = "DetailView", Criteria = "!HasPolicy", Visibility = ViewItemVisibility.Hide, TargetItems = "Policy", AppearanceItemType = "LayoutItem")]
    public class CallCardUrgent : XPObject
    {
        public CallCardUrgent(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            CallPerson = new CallPerson(Session);
            TestAddress = new Address(Session);}

        [XafDisplayName("Дата оказания неотложной мед. помощи")]
        public DateTime Date { get; set; }

        [XafDisplayName("Кто вызвал, № телефона")]
        [DevExpress.Xpo.Aggregated]
        public CallPerson CallPerson { get; set; }

        [XafDisplayName("Передал с \"03\" (код)")]
        public string From03 { get; set; }
        
        [XafDisplayName("Номер бригады")]
        public string BrigadeNum { get; set; }

        [XafDisplayName("Врач (Фельдшер)")]
        public Doctor Doctor { get; set; }
        
        [XafDisplayName("Время принятия вызова")]
        public TimeSpan AcceptTime { get; set; }

        [XafDisplayName("Время передачи вызова бригаде НМП")]
        public TimeSpan BrigadeAcceptTime { get; set; }

        [XafDisplayName("Время выезда на вызов")]
        public TimeSpan BrigadeGoTime { get; set; }
        
        [XafDisplayName("Время прибытия на место вызова")]
        public TimeSpan BrigadeArriveTime { get; set; }

        [XafDisplayName("Время начала транспортировки")]
        public TimeSpan StartTransportTime { get; set; }

        [XafDisplayName("Время прибытия в медучреждение")]
        public TimeSpan ArriveTransportTime { get; set; }

        [XafDisplayName("Время освобождения бригады")]
        public TimeSpan BrigadeReleaseTime { get; set; }

        [XafDisplayName("Следующий вызов или возвращение в пункт")]
        public TimeSpan NextCallOrReturnTime { get; set; }

        [XafDisplayName("Время, затр. на выполнение вызова")]
        public TimeSpan SpentTime
        {
            get
            {
                return BrigadeReleaseTime - AcceptTime;
            }
        }

        public Address TestAddress { get; set; }

        [XafDisplayName("Адрес вызова")]
        public string CallAddress { get; private set; }

        [XafDisplayName("Адрес прописки")]
        public string RegAddress { get; private set; }

        public void SetRegAddress(string address)
        {
            RegAddress = address;
        }

        [XafDisplayName("Прикреплен к поликлинике")]
        public string LPU { get; set; }

        [XafDisplayName("Место работы/учебы")]
        public string WorkOrStudyPlace { get; set; }

        [XafDisplayName("Причина вызова")]
        public CallReason Reason { get; set; }

        [XafDisplayName("Повод вызова")]
        public string Motive { get; set; }

        [XafDisplayName("Имеет документ, удостоверяющий личность")]
        [ImmediatePostData]
        public bool HasDocument { get; set; }

        [XafDisplayName("Вид документа, удостоверяющий личность")]
        public VidDocumenta DocumentType { get; set; }
        [XafDisplayName("Серия документа, удостоверяющий личность")]
        public string DocumentSerial { get; set; }
        [XafDisplayName("Номер документа, удостоверяющий личность")]
        public string DocumentNumber { get; set; }

        [XafDisplayName("Имеет медицинский полис")]
        [ImmediatePostData]
        public bool HasPolicy { get; set; }

        [XafDisplayName("Тип медицинского полиса")]
        public VidPolisa PolicyType { get; set; }

        [XafDisplayName("Серия полиса")]
        public string PolicySerial { get; set; }

        [XafDisplayName("Номер полиса")]
        public string PolicyNumber { get; set; }

        [XafDisplayName("Страховая компания")]
        public string SMO { get; set; }

        [XafDisplayName("Фамилия")]
        public string Fam { get; set; }
        [XafDisplayName("Имя")]
        public string Nam { get; set; }

        [XafDisplayName("Отчество")]
        public string Ot { get; set; }

        [XafDisplayName("Дата рождения")]
        public DateTime BirthDate { get; set; }

        [XafDisplayName("Пол")]
        public Gender Gender { get; set; }

        /*01. Дата оказания неотложной мед. помощи		Отмечено
02. Кто вызвал, № телефона		Отмечено
03. Передал с "03" (код)		Неотмечено
04. Номер бригады		Отмечено
05. Причина вызова		Отмечено
06. Вызов принят		Отмечено
07. Вызов передан бригаде НМП		Отмечено
08. Выезд на вызов		Отмечено
09. Прибытие на место вызова		Отмечено
10. Начало транспортировки		Неотмечено
11. Прибытие в медучреждение		Неотмечено
12. Освобождение бригады		Отмечено
13. Время доезда		Отмечено
14. Повод вызова		Отмечено
15. Адрес оказания неотложной мед. помощи		Отмечено
16. Результаты выезда		Неотмечено
17. Безрезультатный  выезд (см. п. 35)		Неотмечено
18. Доставлен в медучреждение		Неотмечено
	Время	Неотмечено
	Код ЛПУ	Неотмечено
	ФИО дежурного врача	Неотмечено
19. Дисп. учет по данному заболеванию		Отмечено
20. Присутствие алкоголя		Отмечено
21. Передано во ФГУЗ ЦГиЭ в РБ		Неотмечено
	Время	Неотмечено
	ФИО	Неотмечено
	Экстр. извещ. №	Неотмечено
22. Передано в ОВД, ГИБДД, УФСКН		Неотмечено
	№	Неотмечено
	Время	Неотмечено
	ФИО	Неотмечено
23. Подлежит активному посещению		Отмечено*/

    }

    public class CallPerson : BaseObject
    {
        public CallPerson(Session session) : base(session) { }

        [XafDisplayName("Кто вызвал")]
        public string FIO { get; set; }

        [XafDisplayName("№ телефона")]
        public string PhoneNumber { get; set; }
    }

    public enum CallReason
    {
        [XafDisplayName("Несчастный случай")]
        Accident = 1,
        [XafDisplayName("Внезапное заболевание")]
        SuddenIllness = 2,
        [XafDisplayName("Экстранная перевозка")]
        EmergencyTransportation = 3,
        [XafDisplayName("Плановая перевозка")]
        PlannedTransportation = 4,
        [XafDisplayName("Обострение хронического заболевания")]
        ChronicIllnessWorsening = 5,
        [XafDisplayName("Травма")]
        Trauma = 6,
        [XafDisplayName("Другое")]
        Other = 7
    }
}
