using System.Text;
using System.Web.UI.Design;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Validation;
using Registrator.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ListView = DevExpress.ExpressApp.ListView;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class DnevnoyStacionarController : ViewController
    {
        const string dateTimeFormat = "{0:yyyy-MM-dd}";

        public DnevnoyStacionarController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.

            var loggedDoctor = (Doctor)SecuritySystem.CurrentUser;
            var isAdmin = loggedDoctor.DoctorRoles.Any(t => t.IsAdministrative);
            
            // если авторизованный доктор - администратор
            if (loggedDoctor != null && isAdmin)
            {
                // то даем доступ к особым экшенам
                ExportDataReestrAction.Active.SetItemValue("ReestrExport", true);
                // то даем доступ к особым экшенам
                DupsFilter.Active.SetItemValue("DupsFilter", true);
                ClearCriteria.Active.SetItemValue("ClearFilter", true);
            }
            else
            {
                // иначе - скрываем кнопки особых экшенов
                ExportDataReestrAction.Active.SetItemValue("ReestrExport", false);
                DupsFilter.Active.SetItemValue("DupsFilter", false);
                ClearCriteria.Active.SetItemValue("ClearFilter", false);
            }

            // если открыто "детальное" окно
            if (View.Id == "DnevnoyStacionar_DetailView")
            {
                // получаем текущий дневной стационар
                var ds = View.CurrentObject as DnevnoyStacionar;

                if (ds != null)
                {
                    // если дата выписки не проставлена
                    if (ds.DataVypiski != DateTime.MinValue)
                    {
                        BindDoctorAction.Active.SetItemValue("CaseIsClosed", false);
                        CloseStacionarAction.Active.SetItemValue("CaseIsClosed", false);

                    }
                    else
                    {

                        if (ds.Doctor == null)
                            BindDoctorAction.Active.SetItemValue("NoDoctor", false);
                        else
                            BindDoctorAction.Active.SetItemValue("CanBind", true);

                        CloseStacionarAction.Active.SetItemValue("CanCloseCase", true);
                    }
                }
            }
            
            // убираем ненужные экшены в детальном представлении
            if (View.Id == "DnevnoyStacionar_DetailView")
            {
                var recordNavigationController = Frame.GetController<DevExpress.ExpressApp.SystemModule.RecordsNavigationController>();
                if (recordNavigationController != null)
                    recordNavigationController.Active.SetItemValue("NoNeedInStacionar", false);

                var newObjectController = Frame.GetController<DevExpress.ExpressApp.SystemModule.NewObjectViewController>();
                if (newObjectController != null)
                    newObjectController.Active.SetItemValue("NoNeedInStacionar", false);

                var openRelatedController = Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.OpenObjectController>();
                if (openRelatedController != null)
                    openRelatedController.Active.SetItemValue("NoNeedInStacionar", false);
            }
        }

        private void BindDoctorAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var currentDS = e.CurrentObject as DnevnoyStacionar;
            var fields = e.PopupWindowViewCurrentObject as BindDoctorFields;

            var bindDoctorData = View.ObjectSpace.CreateObject<Registrator.Module.BusinessObjects.Dictionaries.StacionarBindDoctorHistory>();
            
            bindDoctorData.BindDate = DateTime.Now;
            bindDoctorData.Reason = fields.Note;
            bindDoctorData.OldDoctor = currentDS.Doctor;
            bindDoctorData.NewDoctor = View.ObjectSpace.FindObject<Doctor>(CriteriaOperator.Parse("InnerCode=?", fields.Doctor.InnerCode));
            bindDoctorData.Stacionar = currentDS;

            currentDS.Doctor = bindDoctorData.NewDoctor;
            currentDS.BindDoctorHistory.Add(bindDoctorData);

            View.ObjectSpace.SetModified(currentDS);
            
            if (View is DetailView && ((DetailView)View).ViewEditMode == ViewEditMode.View)
            {
                View.ObjectSpace.CommitChanges();
            }
        }

        private void BindDoctorAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            e.View = Application.CreateDetailView(Application.CreateObjectSpace(), new BindDoctorFields());
        }

        private void CloseStacionarAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            e.View = Application.CreateDetailView(Application.CreateObjectSpace(), new CloseStacionarFields() { CloseDateTime = DateTime.Now, ResultatHospital = ResultatGospitalizacii.Uluchshenie });
        }

        private void CloseStacionarAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var currentDS = e.CurrentObject as DnevnoyStacionar;
            var fields = e.PopupWindowViewCurrentObject as CloseStacionarFields;

            currentDS.DataVypiski = fields.CloseDateTime;
            currentDS.Resultat = fields.ResultatHospital;
            View.ObjectSpace.SetModified(currentDS);

            if (View is DetailView && ((DetailView)View).ViewEditMode == ViewEditMode.View)
            {
                View.ObjectSpace.CommitChanges();
            }
        }

        private void exportDataReestrAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            e.View = Application.CreateDetailView(Application.CreateObjectSpace(), new DayHospitalExportParameters());
        }

        private void exportDataReestrAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var fields = e.PopupWindowViewCurrentObject as DayHospitalExportParameters;
            
            GroupOperator doctorDSCriteria = new GroupOperator();
            GroupOperator doctorVCCriteria = new GroupOperator();
            if (fields.Doctor != null)
            {
                doctorDSCriteria.Operands.Add(DnevnoyStacionar.Fields.Doctor.Oid == fields.Doctor.Oid);
                doctorVCCriteria.Operands.Add(VisitCase.Fields.Doctor.Oid == fields.Doctor.Oid);
            }

            string lpuCode = fields.LpuCode;

            // перед выгрузкой следует проверить данные на валидность, при необходимости вывести список с ошибками.
            var sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Title = "Экспорт данных";
            sfd.Filter = "XML файлы (*.xml)|*.xml";
            sfd.RestoreDirectory = true;
            sfd.DefaultExt = ".xml";

            var mo = Registrator.Module.BusinessObjects.Settings.MOSettings.GetCurrentMOCode(ObjectSpace);
            var tfoms = Registrator.Module.BusinessObjects.Settings.RegionSettings.RegionCode(ObjectSpace);

            sfd.FileName = string.Format("M{0}T{1}_{2:yyMM}", mo, tfoms, DateTime.Now);

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string name = System.IO.Path.GetFileName(sfd.FileName);
                string dirpath = System.IO.Path.GetFullPath(sfd.FileName);
                dirpath = dirpath.Substring(0, dirpath.Length - name.Length);
                // Пространство хранимых объектов
                IObjectSpace objectSpace = Application.CreateObjectSpace();

                IList<DnevnoyStacionar> dayHospitals = objectSpace.GetObjects<DnevnoyStacionar>(doctorDSCriteria &
                    DnevnoyStacionar.Fields.ResultatOplati != new OperandValue(Oplata.Polnaya) &
                    DnevnoyStacionar.Fields.DataVypiski >= fields.FromDate &
                    DnevnoyStacionar.Fields.DataVypiski <= fields.ToDate);

                // проверить стационары на полноту данных
                
                // опеределить имя рута
                XElement rootHM = new XElement("ZL_LIST");
                XElement rootLM = new XElement("PERS_LIST");
                rootHM.Add(CreateZGLV(false, name));

                // получить номер счета!!!!
                var getSchet = 1;
                var schet = CreateSchet(getSchet);
                rootHM.Add(schet);

                rootLM.Add(CreateZGLV(true, name));
                int recordNum = 0; decimal summAv = 0; int count = 0;

                foreach (var dayHospital in dayHospitals)
                {
                    // пропускаем невыписанных пациентов
                    if ((dayHospital.DataVypiski == DateTime.MinValue) || (dayHospital.DataVypiski < fields.FromDate) || (dayHospital.DataVypiski > fields.ToDate))
                        continue;

                    // если установлен флаг поиска иногородних и пациент не иногородний, то пропускаем
                    if (dayHospital.Pacient != null)
                    {
                        Polis polis = dayHospital.Pacient.CurrentPolis;
                        if (polis != null && polis.IsFromAnotherRegion != fields.IsInogorodniy)
                            continue;
                    }

                    // пропускаем оплаченные стационары
                    if (dayHospital.ResultatOplati == Oplata.Polnaya)
                        continue;
                    try
                    {
                        // получаем запись LM реестра
                        rootLM.Add(dayHospital.Pacient.GetReestrElement());
                        // получаем запись HM реестра
                        XElement xZap = dayHospital.GetReesterElement(++recordNum, lpuCode);
                        rootHM.Add(xZap);

                        var decimalValue = Utils.GetDecimalFromString(xZap.Element("SLUCH").Element("SUMV").Value);
                        summAv += decimalValue;
                    }
                    catch (Exception error)
                    {
                        string er = error.Message;
                        var erds = dayHospital;
                    }
                    
                    count++;
                }

                // Посещения
                IList<VisitCase> visitCases = objectSpace.GetObjects<VisitCase>(doctorVCCriteria &
                    VisitCase.Fields.DateIn >= fields.FromDate &
                    VisitCase.Fields.DateOut <= fields.ToDate);

                foreach (var visitCase in visitCases)
                {
                    // если установлен флаг поиска иногородних и пациент не иногородний, то пропускаем
                    if (visitCase.Pacient != null)
                    {
                        Polis polis = visitCase.Pacient.CurrentPolis;
                        if (polis != null && polis.IsFromAnotherRegion != fields.IsInogorodniy)
                            continue;
                    }

                    try
                    {
                        // получаем запись LM реестра
                        rootLM.Add(visitCase.Pacient.GetReestrElement());
                        // получаем запись HM реестра
                        XElement xZap = visitCase.GetReestrElement(++recordNum, lpuCode);
                        rootHM.Add(xZap);

                        var decimalValue = Utils.GetDecimalFromString(xZap.Element("SLUCH").Element("SUMV").Value);
                        summAv += decimalValue;
                    }
                    catch (Exception error)
                    {
                        string er = error.Message;
                    }
                }

                // Диспансеризации
                IList<DispanserizaionCase> dispCases = objectSpace.GetObjects<DispanserizaionCase>(
                    VisitCase.Fields.DateIn >= fields.FromDate &
                    VisitCase.Fields.DateOut <= fields.ToDate);

                foreach (var dispCase in dispCases)
                {
                    // если установлен флаг поиска иногородних и пациент не иногородний, то пропускаем
                    if (dispCase.Pacient != null)
                    {
                        Polis polis = dispCase.Pacient.CurrentPolis;
                        if (polis != null && polis.IsFromAnotherRegion != fields.IsInogorodniy)
                            continue;
                    }

                    try
                    {
                        // получаем запись LM реестра
                        rootLM.Add(dispCase.Pacient.GetReestrElement());
                        // получаем запись HM реестра
                        XElement xZap = dispCase.GetReestrElement(++recordNum, lpuCode);
                        rootHM.Add(xZap);

                        var decimalValue = Utils.GetDecimalFromString(xZap.Element("SLUCH").Element("SUMV").Value);
                        summAv += decimalValue;
                    }
                    catch (Exception error)
                    {
                        string er = error.Message;
                    }
                }

                if (count != 0)
                {
                    schet.Element("SUMMAV").Value = summAv.ToString("0.00").Replace(",", ".");
                }

                XDocument reestrHM = new XDocument(rootHM);
                reestrHM.Declaration = new XDeclaration("1.0", "windows-1251", "yes");
                XDocument reestrLM = new XDocument(rootLM);
                reestrLM.Declaration = new XDeclaration("1.0", "windows-1251", "yes");

                reestrLM.Save(System.IO.Path.Combine(dirpath, "L" + name));
                reestrHM.Save(System.IO.Path.Combine(dirpath, "H" + name));
            }
        }

        public DSValidationErrors CheckDs(DnevnoyStacionar ds)
        {
            var validateErrors = new DSValidationErrors() { DS = ds };
            validateErrors.Errors = new Dictionary<string, string>();

            if (ds.Doctor == null)
                validateErrors.Errors.Add("Врач", "Поле врач должно быть установлено!");

            if (ds.Doctor.Otdelenie == null)
                validateErrors.Errors.Add("Отделение", "Не указано отделение, в котором работает врач!");

            if (ds.Doctor.SpecialityCode == null)
                validateErrors.Errors.Add("Специальность", "Не указана специальность врача!");

            if (ds.Diagnose == null)
                validateErrors.Errors.Add("Диагноз", "Поле диагноз должно быть установлено!");

            if (string.IsNullOrEmpty(ds.Diagnose.MKB))
                validateErrors.Errors.Add("Значение диагноза", "Значение диагноза не должно быть пустым!");

            return validateErrors;
        }

        private XElement CreateSchet(int nSchet)
        {
            var schet = new XElement("SCHET");
            schet.Add(new XElement("CODE", 1));
            var codeMO = BusinessObjects.Settings.MOSettings.GetCurrentMOCode(ObjectSpace);
            schet.Add(new XElement("CODE_MO", codeMO));
            schet.Add(new XElement("YEAR", DateTime.Now.Year));
            schet.Add(new XElement("MONTH", DateTime.Now.Month));
            schet.Add(new XElement("DSCHET", string.Format(dateTimeFormat, DateTime.Now)));
            schet.Add(new XElement("NSCHET", nSchet));
            schet.Add(new XElement("SUMMAV", ""));

            return schet;
        }

        private XElement CreateZGLV(bool isLFile, string fileName)
        {
            // версия взаимодействия текущей редакции
            const string version = "2.1";

            var zglv = new XElement("ZGLV");
            zglv.Add(new XElement("VERSION", version));
            zglv.Add(new XElement("DATA", string.Format(dateTimeFormat, DateTime.Now)));

            if (isLFile)
            {
                zglv.Add(new XElement("FILENAME", "L" + fileName));
                zglv.Add(new XElement("FILENAME1", "H" + fileName));
            }
            else
                zglv.Add(new XElement("FILENAME", "H" + fileName));
            return zglv;
        }

        private void FilterForDoctorAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            e.View = Application.CreateDetailView(Application.CreateObjectSpace(), new FilterForDoctorDnevnoyStacionarFields() { });
        }

        private void FilterForDoctorAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var listView = this.View as ListView;

            var filterFields = e.PopupWindowViewCurrentObject as FilterForDoctorDnevnoyStacionarFields;

            if (filterFields.ToDate == DateTime.MinValue)
            {
                filterFields.ToDate = DateTime.MaxValue;
            }

            listView.CollectionSource.Criteria.Clear();

            // если задан диапазон дат
            if ((filterFields.FromDate  != DateTime.MinValue) || (filterFields.ToDate != DateTime.MaxValue))
                listView.CollectionSource.Criteria.Add("DateFilter",
                    CriteriaOperator.And(
                        new BinaryOperator("DataVypiski", filterFields.FromDate, BinaryOperatorType.GreaterOrEqual),
                        new BinaryOperator("DataVypiski", filterFields.ToDate, BinaryOperatorType.LessOrEqual)));
            
            string currentRegionOKATO = BusinessObjects.Settings.RegionSettings.GetCurrentRegionOKATO(View.ObjectSpace);
            
            if (filterFields.ByDoctor!=null)
                listView.CollectionSource.Criteria.Add("DoctorFilter",
                    CriteriaOperator.Parse("Doctor.InnerCode=?", filterFields.ByDoctor.InnerCode));

            /*listView.CollectionSource.Criteria.Add("InogorodniyFilter",
                    CriteriaOperator.Parse("Pacient.IsInogorodniy=?", filterFields.IsInogorodniy)
                );*/
        }

        private void SetOplata_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Title = "Укажите файл";
            ofd.Filter = "Файлы реестра (*.xml)|*.xml";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder log = new StringBuilder();
                var changedCount = 0;
                var os = Application.CreateObjectSpace();
                try
                {
                    var loadedCases = LoadCases(ofd.FileName);
                    var hosps = os.GetObjects<DnevnoyStacionar>();
                    
                    
                    foreach (var dnevnoyStacionar in hosps)
                    {
                        XElement reestrElement = null;
                        try
                        {
                            reestrElement = dnevnoyStacionar.GetReesterElement(0);
                        }
                        catch (Exception)
                        {
                            log.AppendLine("Reestr error: " + dnevnoyStacionar.Oid.ToString());
                            continue;
                        }
                        
                        var pacientInfo = reestrElement.Element("PACIENT");
                        var v = pacientInfo.Element("VPOLIS");
                        var type = v == null ? "" : v.Value;
                        var s = pacientInfo.Element("SPOLIS");
                        var serial = s == null ? "" : s.Value;
                        var n = pacientInfo.Element("NPOLIS");
                        var number = n == null ? "" : n.Value;
                        var polisData = string.Format("{0}:s{1}n{2}", type, serial, number);

                        var sluchInfo = reestrElement.Element("SLUCH");
                        var sluchNhistory = sluchInfo.Element("NHISTORY").Value;
                        var sluchSum = sluchInfo.Element("SUMV").Value;
                        var finded =
                            loadedCases.Where(
                                t =>
                                    t.NHistory.Equals(sluchNhistory) &&
                                    t.PacientPolistData.Equals(polisData));
                        var findedCount = finded.Count();
                        if (findedCount > 0)
                        {
                            if (findedCount == 1 && dnevnoyStacionar.ResultatOplati == Oplata.NetResheniya)
                            {
                                dnevnoyStacionar.ResultatOplati = Oplata.Polnaya;
                                dnevnoyStacionar.Save();
                                os.CommitChanges();
                                changedCount++;
                            }
                            else
                            {
                                log.AppendLine("Найдено много совпадений для этого пациента и случая (код): " + dnevnoyStacionar.Oid.ToString());
                            }
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                os.CommitChanges();
                MessageBox.Show(changedCount.ToString());
                if (log.Length > 0)
                    MessageBox.Show(log.ToString());

            }
        }

        public List<CaseFields> LoadCases(string AHMPath)
        {
            XDocument xml = XDocument.Load(AHMPath);
            var cases = xml.Descendants("SLUCH").Where(t => (t.Elements("USL").Count() == 1) && t.Element("OPLATA").Value == "1" && t.Element("IDSP").Value == "43");
            var list = new List<CaseFields>();
            foreach (var xElement in cases)
            {
                list.Add(new CaseFields(xElement));
            }
            return list;
        }

        private void DupsFilter_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var lv = View as ListView;
            if (lv!=null)
            {
                var DSs = ObjectSpace.GetObjects<DnevnoyStacionar>();

                var dups = new List<int>();

                var count = DSs.Count;

                for (int i = 0; i < count; i++)
                {
                    var ds = DSs[i];
                    if (dups.Contains(ds.Oid))
                        continue;

                    var timeBeg = ds.DataPriema;
                    var timeEnd = ds.DataVypiski;
                    var FIO = ds.Pacient.FullName.ToLower();
                    int dupCount = 0;
                    for (int j = i + 1; j < count; j++)
                    {
                        if (DSs[j].DataPriema.Date == timeBeg.Date && DSs[j].DataVypiski.Date == timeEnd.Date &&
                            DSs[j].Pacient.FullName.ToLower().Equals(FIO))
                        {
                            dupCount++;
                            dups.Add(DSs[j].Oid);
                        }
                    }
                    if (dupCount > 0)
                        dups.Add(ds.Oid);
                }

                lv.CollectionSource.Criteria.Clear();

                lv.CollectionSource.Criteria.Add("DupsFilter", new InOperator("Oid", dups));
            }
        }

        private void ClearCriteria_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var lv = View as ListView;
            if (lv != null)
            {
                lv.CollectionSource.Criteria.Clear();
            }
        }
    }

    [DomainComponent]
    public class CaseFields
    {
        public CaseFields(XElement element)
        {
            NHistory = element.Element("NHISTORY").Value;
            Sum = element.Element("SUMV").Value;

            var pacient = element.Parent.Element("PACIENT");
            var v = pacient.Element("VPOLIS");
            var type = v==null ? "" : v.Value;
            var s = pacient.Element("SPOLIS");
            var serial = s == null ? "" : s.Value;
            var n = pacient.Element("NPOLIS");
            var number = n == null ? "" : n.Value;

            PacientPolistData = string.Format("{0}:s{1}n{2}", type, serial, number);
        }

        public string NHistory { get; set; }
        public string PacientPolistData { get; set; }
        public string Sum { get; set; }
    }

    [DomainComponent]
    [XafDisplayName("Фильтр стационаров")]
    public class FilterForDoctorDnevnoyStacionarFields
    {
        /// <summary>
        /// Фильтруем по дате выписки
        /// </summary>
        [XafDisplayName("Дата выписки от:")]
        [RuleRequiredField]
        public DateTime FromDate { get; set; }

        [XafDisplayName("До:")]
        [RuleRequiredField]
        public DateTime ToDate { get; set; }

        //[XafDisplayName("Искать иногородних пациентов")]
        //public bool IsInogorodniy { get; set; }

        [XafDisplayName("Выписанных врачом")]
        public Doctor ByDoctor { get; set; }
    }

    [DomainComponent]
    [XafDisplayName("Параметры выгрузки дневного стационара")]
    public class DayHospitalExportParameters
    {
        public DayHospitalExportParameters()
        {
            this.FromDate = DateTime.Today;
            this.ToDate = DateTime.Today;
        }

        /// <summary>
        /// Фильтруем по дате выписки
        /// </summary>
        [XafDisplayName("От")]
        [RuleRequiredField]
        public DateTime FromDate { get; set; }

        [XafDisplayName("До")]
        [RuleRequiredField]
        public DateTime ToDate { get; set; }

        [XafDisplayName("Выгрузить иногородних")]
        public bool IsInogorodniy { get; set; }

        [XafDisplayName("Доктор")]
        public Doctor Doctor { get; set; }

        [XafDisplayName("Код ЛПУ")]
        public string LpuCode { get; set; }
    }

    [DomainComponent]
    [XafDisplayName("Передать пациента")]
    public class BindDoctorFields
    {
        [XafDisplayName("Передать пациента доктору")]
        [RuleRequiredField]
        public Doctor Doctor { get; set; }

        [XafDisplayName("Основание")]
        [RuleRequiredField]
        public string Note { get; set; }
    }

    [DomainComponent]
    [XafDisplayName("Выписка")]
    public class CloseStacionarFields
    {
        [XafDisplayName("Дата выписки")]
        [RuleRequiredField]
        public DateTime CloseDateTime { get; set; }

        [XafDisplayName("Результат госпитализации")]
        [RuleRequiredField]
        public ResultatGospitalizacii ResultatHospital { get; set; }
    }


    [DomainComponent]
    public class DSValidationErrors
    {
        public DnevnoyStacionar DS { get; set; }
        public Dictionary<string, string> Errors { get; set; }
    }
}
