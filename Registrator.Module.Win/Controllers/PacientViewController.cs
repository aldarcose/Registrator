using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.BaseImpl;
using Registrator.Module.BusinessObjects;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Editors;
using Registrator.Module.BusinessObjects.Dictionaries;
using TFOmsServices;
using Address = Registrator.Module.BusinessObjects.Address;
using ListView = DevExpress.ExpressApp.ListView;
using Task = System.Threading.Tasks.Task;
using Timer = System.Timers.Timer;
using DevExpress.Persistent.Validation;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class PacientViewController : ViewController
    {
        private MainRibbonFormV2 _templateMain;

        private static LongOperationThread _longOperationThread;
        public PacientViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
            
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (Application.MainWindow == null)
                return;
            _templateMain = Application.MainWindow.Template as MainRibbonFormV2;
            if (_templateMain != null)
            {
                if (View.Id == "Pacient_DetailView")
                {
                    /*
                    _templateMain.Ribbon.SelectedPage = _templateMain.Ribbon.Pages[0];
                    var pacient = View.CurrentObject as Pacient;
                    if (pacient != null)
                    {
                        DisabilitySetView(false);
                    }

                    PropertyEditor disabilityEditor = ((DetailView)View).FindItem("Disability.Invalidnost") as PropertyEditor;
                    if (disabilityEditor != null)
                    {
                        disabilityEditor.ControlValueChanged += propertyEditorDisability_ControlValueChanged;
                    }*/
                }

                if (View is ListView && View.Id == "Pacient_ListView")
                {
                    ListView l = View as ListView;
                    l.ControlsCreated += ShowActionPopup;
                }
            }

            /*
            var loggedDoctor = (Doctor)SecuritySystem.CurrentUser;
            var isAdmin = loggedDoctor.DoctorRoles.Any(t => t.IsAdministrative);

            // если авторизованный доктор - администратор
            if (loggedDoctor != null && isAdmin)
            {
                // то даем доступ к особым экшенам
                LoadNewSRZAction .Active.SetItemValue("ReestrImport", true);
                Frame.GetController<DeleteObjectsViewController>().Active.SetItemValue("AdminCanDelete", true);
            }
            else
            {
                // иначе - скрываем кнопки особых экшенов
                LoadNewSRZAction.Active.SetItemValue("ReestrImport", false);
                Frame.GetController<DeleteObjectsViewController>().Active.SetItemValue("AdminCanDelete", false);
            }
            */
        }

        private void propertyEditorDisability_ControlValueChanged(object sender, EventArgs e)
        {
            var disabilityEditor = sender as PropertyEditor;
            if (disabilityEditor != null)
            {
                var value = (Invalidnost)disabilityEditor.ControlValue;
                DisabilitySetView(value != Invalidnost.Net);
            }
        }

        private void DisabilitySetView(bool show)
        {
            PropertyEditor disabilityGroupEditor = ((DetailView)View).FindItem("Disability.InvalidnostGroup") as PropertyEditor;
            PropertyEditor disabilityChildhoodEditor = ((DetailView)View).FindItem("Disability.InvalidDetstva") as PropertyEditor;

            if (disabilityChildhoodEditor == null || disabilityGroupEditor == null)
                return;

            if (!show)
            {
                disabilityChildhoodEditor.AllowEdit.SetItemValue("DisabilityChildReadOnly", false);
                disabilityGroupEditor.AllowEdit.SetItemValue("DisabilityGroupReadOnly", false);
            }
            else
            {
                disabilityChildhoodEditor.AllowEdit.SetItemValue("DisabilityChildReadOnly", true);
                disabilityGroupEditor.AllowEdit.SetItemValue("DisabilityGroupReadOnly", true);
            }

            disabilityChildhoodEditor.Refresh();
            disabilityGroupEditor.Refresh();
        }

        protected override void OnDeactivated()
        {
            var view = View as DetailView;
            if (view != null)
            {
                /*
                var pacient = view.CurrentObject as Pacient;
                if (pacient != null)
                {
                    pacient.Cases.CollectionChanged -= pacient.CasesCollectionChanged;
                    pacient.Polises.CollectionChanged -= pacient.PolisesCollectionChanged;
                }*/
            }
            // Unsubscribe from previously subscribed events and release other references and resources.s
            base.OnDeactivated();
        }

        void ShowActionPopup(object sender, EventArgs e)
        {
            if (_longOperationThread != null && _longOperationThread.IsWorking == false)
            {
                MessageBox.Show(string.Format("Выполняется операция: {0}!\nПожалуйста, подождите", _longOperationThread.OperationName));
                return;
            }

            if (View is ListView && View.Id == "Pacient_ListView")
            {
                PopupWindowShowActionHelper helper = new PopupWindowShowActionHelper(pacientFilterAction);
                helper.ShowPopupWindow();
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.

            var view = View as DetailView;
            if (view != null)
            {
                /*
                var pacient = view.CurrentObject as Pacient;
                if (pacient != null)
                {
                    // реагируем на изменения в случаях
                    pacient.Cases.CollectionChanged += pacient.CasesCollectionChanged;

                    // реагируем на изменения в полисах
                    pacient.Polises.CollectionChanged += pacient.PolisesCollectionChanged;
                }*/
            }
        }

        private void pacientFilterAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var listView = this.View as ListView;

            var filterFieldsParameters = e.PopupWindowViewCurrentObject as PacientFilterFieldsParameters;
            Validator.RuleSet.Validate(ObjectSpace, filterFieldsParameters, "Action");

            if (listView != null)
            {
                listView.CollectionSource.Criteria.Clear();

                var fioCriteriaStringBuilder = new StringBuilder();
                if (filterFieldsParameters != null)
                {
                    bool needAnd = false;
                    if (!string.IsNullOrEmpty(filterFieldsParameters.LastName))
                    {
                        fioCriteriaStringBuilder.AppendFormat("Lower(LastName) like '{0}%'", filterFieldsParameters.LastName.ToLower());
                        needAnd = true;
                    }

                    if (!string.IsNullOrEmpty(filterFieldsParameters.FirstName))
                    {
                        if (needAnd)
                            fioCriteriaStringBuilder.Append(" AND ");
                        fioCriteriaStringBuilder.AppendFormat("Lower(FirstName) like '{0}%'", filterFieldsParameters.FirstName.ToLower());
                        needAnd = true;
                    }

                    if (!string.IsNullOrEmpty(filterFieldsParameters.MiddleName))
                    {
                        if (needAnd)
                            fioCriteriaStringBuilder.Append(" AND ");
                        fioCriteriaStringBuilder.AppendFormat("Lower(MiddleName) like '{0}%'", filterFieldsParameters.MiddleName.ToLower());
                    }

                    listView.CollectionSource.Criteria.Add("FIOFilter",
                        CriteriaOperator.Parse(fioCriteriaStringBuilder.ToString()));

                    var polisCriteriaString = string.Format("Number like '{0}%'", filterFieldsParameters.PolisNum);

                    listView.CollectionSource.Criteria.Add("PolisFilter",
                        new ContainsOperator("Polises",
                            CriteriaOperator.Parse(polisCriteriaString)));
                }
            }
        }

        private void pacientFilterAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            e.View = Application.CreateDetailView(Application.CreateObjectSpace(), new PacientFilterFieldsParameters());
        }

        private void LoadNewSRZAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            // загружаем сводный регистр застрахованных
            // регистр содержит ВСЕХ пациентов, прикрепленных к этому ЛПУ
            // пациенты, которые присутствуют в базе, но отсутствуют в СРЗ - открепленные

            var ofd = new OpenFileDialog();
            ofd.Filter = "Xml files (*.xml)|*.xml|Dbf files (*.dbf)|*.dbf";
            ofd.Title = "Выберите файл СРЗ с актуальными данными";
            ofd.DefaultExt = ".xml";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var path = ofd.FileName;

                if (path.EndsWith(".xml"))
                {
                    try
                    {
                        _longOperationThread = new LongOperationThread(LoadFromXml);
                        _longOperationThread.OperationName = "Загрузка СРЗ";
                        _longOperationThread.Done += (o, args) => { MessageBox.Show("Done"); };
                        _longOperationThread.Start(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }

                if (path.EndsWith(".dbf"))
                {
                    
                }
            }
        }

        // однопоточная загрузка срз
        private void LoadFromXml(object oPath)
        {
            var path = oPath as String;
            var timeStart = DateTime.Now;
            Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Загрузка XML"));
            XDocument xDoc = XDocument.Load(path);

            const string elementsContainer = "ROWDATA";
            const string elementNameStartsWith = "ROW";

            var objSpace = Application.CreateObjectSpace();

            int loaded = 0;
            int counter = 0;
            int countToLoadPart = 200;
            int countToLoad = 10000;

            foreach (var element in xDoc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                // получаем ФИО пациента
                var lastName = element.Attribute("FAM").Value;
                var firstName = element.Attribute("IM").Value;
                var middleName = element.Attribute("OT").Value;

                // по требованию местного ТФОМС, если нет фамилии явно указывать НЕТ
                if (string.IsNullOrEmpty(lastName))
                    lastName = "НЕТ";
                if (string.IsNullOrEmpty(firstName))
                    firstName = "НЕТ";
                if (string.IsNullOrEmpty(middleName))
                    middleName = "НЕТ";

                DateTime birthDateTemp = DateTime.MinValue;
                DateTime.TryParse(element.Attribute("DR").Value, out birthDateTemp);

                // дата рождения и пол
                var birthdate = birthDateTemp == DateTime.MinValue ? null : (DateTime?)birthDateTemp;
                var gender = element.Attribute("W").Value == "1" ? Gender.Male : Gender.Female;

                // ищем пациента по этим данным
                Pacient pacient = objSpace.FindObject<Pacient>(CriteriaOperator.Parse("LastName=? And FirstName=? And MiddleName=? And Birthdate=?", lastName, firstName, middleName, birthdate));

                // получаем данные документа, удостоверяющего личность
                var docType = objSpace.FindObject<VidDocumenta>(CriteriaOperator.Parse("Code=?", element.Attribute("DOCTP").Value));
                var docSerial = element.Attribute("DOCS").Value;
                var docNumber = element.Attribute("DOCN").Value;
                var document = objSpace.FindObject<Document>(CriteriaOperator.Parse("Type=? AND Serial=? AND Number=?", docType, docSerial, docNumber));

                var newDocument = false;
                // если документ не был найден
                if (document == null)
                {
                    newDocument = true;
                    // создаем документ
                    document = objSpace.CreateObject<Document>();
                    document.Type = docType;
                    document.Serial = docSerial;
                    document.Number = docNumber;
                }

                // получаем страховую мед. организацию
                // если не найдено. попросить обновить реестр СМО
                var SMO = objSpace.FindObject<StrahMedOrg>(CriteriaOperator.Parse("Code=?", element.Attribute("Q").Value));

                if (SMO == null)
                {
                    // выводим сообщение
                }

                // получаем мед. организацию, к которой прикрепляется пациент
                // если не найдено. попросить обновить реестр МО
                var MO = objSpace.FindObject<MedOrg>(CriteriaOperator.Parse("Code=?", element.Attribute("LPU").Value));
                var prikreplenieDate = (element.Attribute("LPUDT") == null || element.Attribute("LPUDT").Value == "") ? null : (DateTime?)DateTime.Parse(element.Attribute("LPUDT").Value);

                if (MO == null)
                {
                    // выводим сообщение
                }

                // получаем данные полиса
                var polisType = objSpace.FindObject<VidPolisa>(CriteriaOperator.Parse("Code=?", element.Attribute("OPDOC").Value));
                var polisSerial = element.Attribute("SPOL").Value;
                var polisNumber = element.Attribute("NPOL").Value;
                var polisPRZ = element.Attribute("PRZ").Value;
                var polisDateBegin = (element.Attribute("DBEG") == null || element.Attribute("DBEG").Value == "") ? null : (DateTime?)DateTime.Parse(element.Attribute("DBEG").Value);

                // получаем полис
                var polis =
                    objSpace.FindObject<Polis>(CriteriaOperator.Parse("Type=? AND Serial=? AND Number=?",
                        polisType, polisSerial, polisNumber));

                var newPolis = false;
                // если полис не был найден
                if (polis == null)
                {
                    newPolis = true;

                    // создаем новый полис в базе
                    polis = objSpace.CreateObject<Polis>();
                    polis.Type = polisType;
                    polis.Serial = polisSerial;
                    polis.PRZ = polisPRZ;
                    polis.DateBegin = polisDateBegin;
                    polis.SMO = SMO;
                }

                // если пациент найден
                if (pacient != null)
                {
                    // меняем все данные //

                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Пациента найден, устанавливаем при необходимости документ и полис"));

                    // устанавливаем документ пациента
                    if (pacient.Document == null || pacient.Document.Oid != document.Oid)
                        pacient.Document = document;
                    pacient.PrikreplenieDate = prikreplenieDate;
                    // добавляем полис
                    pacient.AddPolis(polis);
                }
                // если пациент НЕ найден
                else
                {
                    // документ И полис не найдены, создаем их и добавляем к новому пациенту
                    if (newPolis && newDocument)
                    {
                        Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Создаем пациента, задаем его данные"));
                        // создаем пациента
                        pacient = objSpace.CreateObject<Pacient>();
                        pacient.IsNewBorn = false;

                        pacient.LastName = lastName;
                        pacient.FirstName = firstName;
                        pacient.MiddleName = middleName;
                        pacient.Birthdate = birthdate;
                        pacient.PrikreplenieDate = prikreplenieDate;

                        // устанавливаем документ пациента
                        pacient.Document = document;

                        // добавляем полис
                        pacient.AddPolis(polis);
                    }
                    // но найден либо документ, либо полис, которые указывают на пациента
                    else
                    {
                        Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Пациент не найден, зато найден либо документ, либо полис"));
                        // пациент, к которому привязан полис
                        Pacient polisPacient = null;

                        // пациент, к которому привязан документ
                        Pacient documentPacient = null;

                        if (!newPolis)
                        {
                            polisPacient = polis.Pacient;
                        }

                        if (!newDocument)
                        {
                            documentPacient = objSpace.FindObject<Pacient>(CriteriaOperator.Parse("Document.Oid=?", document.Oid));
                        }

                        // если ничего не нашли
                        if (polisPacient == null && documentPacient == null)
                        {
                            // то это забитые документы, но не приаттаченные к пациенту, которого не создали.
                            Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Отбой, это были 'забытые' документы"));

                            // исправляем косяк
                            pacient = objSpace.CreateObject<Pacient>();
                            pacient.IsNewBorn = false;
                            pacient.LastName = lastName;
                            pacient.FirstName = firstName;
                            pacient.MiddleName = middleName;
                            pacient.Birthdate = birthdate;
                            pacient.PrikreplenieDate = prikreplenieDate;

                            pacient.Document = document;

                            pacient.AddPolis(polis);
                        }
                        else
                        {
                            loaded--;
                            // если что-то нашли в обоих случаях
                            if (polisPacient != null && documentPacient != null)
                            {
                                // проверяем, что указывают на одного
                                if (polisPacient.Oid == documentPacient.Oid)
                                {
                                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Пациент у документов найден и он совпадает, данные в СРЗ сменились"));
                                    // более менее норм, возможно:
                                    // 1) пациент поменял ФИО/ДР 
                                    // 2) были неправильно забиты данные пациента

                                    // сообщаем что есть полис и документ в базе с такими же номерами,
                                    // у которых есть ОДИН пациент-хозяин, но данные которого отличаются от тех что в СРЗ
                                }
                                else
                                {
                                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Пациент у документов найден и он НЕ! совпадает, данные в СРЗ сменились"));
                                    // ваще беда:
                                    // для документа создавали пациента
                                    // для полиса создавали пациента
                                    // в СРЗ еще один пациент

                                    // сообщаем что есть полис и документ в базе с такими же номерами,
                                    // у которых есть СВОИ пациент-хозяин и данные которых отличаются от тех что в СРЗ
                                }
                            }
                            else
                            {

                                if (polisPacient != null)
                                {
                                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Пациент у полиса найден, данные в СРЗ сменились"));
                                    // сообщаем что есть полис в базе с таким же номером,
                                    // у которого есть пациент-хозяин, но данные которого отличаются от тех что в СРЗ
                                }

                                if (documentPacient != null)
                                {
                                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Пациент у документа найден, данные в СРЗ сменились"));
                                    // сообщаем что есть документ в базе с таким же номером,
                                    // у которого есть пациент-хозяин, но данные которого отличаются от тех что в СРЗ
                                }
                            }
                        }
                    }
                }

                // просто меняем данные по месту жительства
                // не важно для какого: вновь созданного, либо найденного
                if (pacient != null)
                {
                    SetAddress(objSpace, element, pacient);
                }

                if (counter > countToLoadPart)
                {
                    objSpace.CommitChanges();
                    counter = 0;
                }
                counter++;

                if (loaded > countToLoad)
                {   
                    break;
                }
                loaded++;
            }

            objSpace.CommitChanges();

            var endTime = DateTime.Now;

            TimeSpan span = endTime - timeStart;

            Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Добавление заняло: " + span.TotalMinutes + " м."));

            MessageBox.Show(string.Format("{0}\n{1}", "Загружено: " + loaded,
                "Добавление заняло: " + span.TotalMinutes + " м."));
        }

        private void SetAddress(IObjectSpace objSpace, XElement element, Pacient pacient)
        {
            var okatoAttr = element.Attribute("RN");
            if (okatoAttr != null)
            {
                var okato = okatoAttr.Value;
                Console.WriteLine(string.Format("{0}\t{1}: {2}", DateTime.Now, "Задаем адрес по окато", okato));
                pacient.Address = Address.GetAddressByOkato(objSpace, okato);
            }
            if (pacient.Address != null)
            {
                var collection = new CriteriaOperatorCollection();
                if (pacient.Address.Level1 != null)
                {
                    collection.Add(new BinaryOperator("City", pacient.Address.Level1));
                }
                if (pacient.Address.Level2 != null)
                {
                    collection.Add(new BinaryOperator("City", pacient.Address.Level2));
                }
                if (pacient.Address.Level3 != null)
                {
                    collection.Add(new BinaryOperator("City", pacient.Address.Level3));
                }
                if (pacient.Address.Level4 != null)
                {
                    collection.Add(new BinaryOperator("City", pacient.Address.Level4));
                }

                var streetValue = element.Attribute("UL").Value;
                Console.WriteLine(string.Format("{0}\t{1}: {2}", DateTime.Now, "Ищем улицу", streetValue));
                var street = objSpace.FindObject<Street>(CriteriaOperator.And(
                    CriteriaOperator.Or(collection),
                    CriteriaOperator.Parse(string.Format("Lower(Name) like '{0}%'", streetValue.ToLower())))
                    );
                if (street == null)
                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Не нашли"));
                else
                {
                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Нашли"));
                }

                pacient.Address.Street = street;

                pacient.Address.House = element.Attribute("DOM").Value;
                pacient.Address.Build = element.Attribute("KOR").Value;
                pacient.Address.Flat = element.Attribute("KV").Value;
            }
        }

        private string GetString(XElement el, bool Required)
        {
            if (Required && el == null)
            {
                throw new ArgumentNullException("el", "Ошибка #1000: Не найден обязательный элемент");
            }
            if (Required && String.IsNullOrWhiteSpace(el.Value))
            {
                throw new ArgumentException("el", "Ошибка #1001: Обязательный элемент не содержит значений");
            }
            return el != null ? el.Value : null;
        }

        private int GetNumber(XElement el, bool Required)
        {
            if (Required && el == null)
            {
                throw new ArgumentNullException("el", "Ошибка #1000: Не найден обязательный элемент");
            }
            if (Required && String.IsNullOrWhiteSpace(el.Value))
            {
                throw new ArgumentException("el", "Ошибка #1001: Обязательный элемент не содержит значений");
            }
            int num;
            if (el != null && Int32.TryParse(el.Value, out num))
            {
                return num;
            }

            return 0;
        }

        private DateTime GetDate(XElement el, bool Required)
        {
            if (Required && el == null)
            {
                throw new ArgumentNullException("el", "Ошибка #1000: Не найден обязательный элемент");
            }
            if (Required && String.IsNullOrWhiteSpace(el.Value))
            {
                throw new ArgumentException("el", "Ошибка #1001: Обязательный элемент не содержит значений");
            }
            DateTime date;
            if (el != null && DateTime.TryParse(el.Value, out date))
            {
                return date;
            }

            return DateTime.MinValue;
        }

        private void ParallelLoadFromXml(object oPath)
        {
            var path = oPath as String;
            Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Загрузка XML"));
            XDocument xDoc = XDocument.Load(path);
            var timeStart = DateTime.Now;
            const string elementsContainer = "ROWDATA";

            
            var elementsList = xDoc.Root.Element(elementsContainer).Elements().Where(t => t.Attribute("FAM").Value.ToUpper().StartsWith("А"));

            // макс. кол-во одновременных задач
            int maxTaskCountWorking = 10;
            // Create a scheduler that uses two threads. 
            var lcts = new LimitedConcurrencyLevelTaskScheduler(maxTaskCountWorking);
            var factory = new TaskFactory(lcts);
            var tasks = new List<Task>();
            var cts = new CancellationTokenSource();

            // сколько элементов обрабатывает одна задача
            int partCountToHandle = 1000;

            var taskCount = (int) (elementsList.Count() / partCountToHandle);
            if (elementsList.Count()%partCountToHandle != 0)
            {
                taskCount ++;
            }

            for (int i = 0; i < taskCount; i++)
            {
                var partList = (i == taskCount - 1) ? elementsList.Skip(partCountToHandle * i) : elementsList.Skip(partCountToHandle * i).Take(partCountToHandle);
                tasks.Add(factory.StartNew(ImportPartCollection, partList, cts.Token));
            }

            Task.WaitAll(tasks.ToArray());

            var endTime = DateTime.Now;

            TimeSpan span = endTime - timeStart;

            Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now,
                "Добавление заняло: " + span.TotalMinutes + " м."));
        }

        private void ImportPartCollection(object o)
        {
            var part = o as IEnumerable<XElement>;

            var startLetter = part.First().Attribute("FAM").Value[0];

            var objSpace = Application.CreateObjectSpace();

            var memPacients = objSpace.GetObjects<Pacient>(CriteriaOperator.Parse("Substring(LastName, 0 ,1)=?", startLetter));

            int loaded = 0;

            int count = part.Count();

            int loadCount = 200;

            foreach (var element in part)
            {
                // получаем ФИО пациента
                var lastName = element.Attribute("FAM").Value;
                var firstName = element.Attribute("IM").Value;
                var middleName = element.Attribute("OT").Value;

                // по требованию местного ТФОМС, если нет фамилии явно указывать НЕТ
                if (string.IsNullOrEmpty(lastName))
                    lastName = "НЕТ";
                if (string.IsNullOrEmpty(firstName))
                    firstName = "НЕТ";
                if (string.IsNullOrEmpty(middleName))
                    middleName = "НЕТ";

                DateTime birthDateTemp = DateTime.MinValue;
                DateTime.TryParse(element.Attribute("DR").Value, out birthDateTemp);

                // дата рождения и пол
                var birthdate = birthDateTemp == DateTime.MinValue ? null : (DateTime?)birthDateTemp;
                var gender = element.Attribute("W").Value == "1" ? Gender.Male : Gender.Female;

                // ищем пациента по этим данным
                Pacient pacient = memPacients.FirstOrDefault(t => t.LastName.ToUpper().Equals(lastName.ToUpper()) && t.FirstName.ToUpper().Equals(firstName.ToUpper()) && t.MiddleName.ToUpper().Equals(middleName.ToUpper()) && t.Birthdate == birthdate);
                    /*objSpace.FindObject<Pacient>(
                        CriteriaOperator.Parse("LastName=? And FirstName=? And MiddleName=? And Birthdate=?",
                            lastName, firstName, middleName, birthdate));*/

                // получаем данные документа, удостоверяющего личность
                var docType =
                    objSpace.FindObject<VidDocumenta>(CriteriaOperator.Parse("Code=?",
                        element.Attribute("DOCTP").Value));
                var docSerial = element.Attribute("DOCS").Value;
                var docNumber = element.Attribute("DOCN").Value;
                var document =
                    objSpace.FindObject<Document>(CriteriaOperator.Parse("Type=? AND Serial=? AND Number=?", docType,
                        docSerial, docNumber));

                var newDocument = false;
                // если документ не был найден
                if (document == null)
                {
                    newDocument = true;

                    // создаем документ
                    document = objSpace.CreateObject<Document>();

                    document.Type = docType;
                    document.Serial = docSerial;
                    document.Number = docNumber;
                }

                // получаем страховую мед. организацию
                // если не найдено. попросить обновить реестр СМО
                var SMO =
                    objSpace.FindObject<StrahMedOrg>(CriteriaOperator.Parse("Code=?", element.Attribute("Q").Value));

                if (SMO == null)
                {
                    // выводим сообщение
                }

                // получаем мед. организацию, к которой прикрепляется пациент
                // если не найдено. попросить обновить реестр МО
                var MO =
                    objSpace.FindObject<MedOrg>(CriteriaOperator.Parse("Code=?", element.Attribute("LPU").Value));
                var prikreplenieDate = (element.Attribute("LPUDT") == null || element.Attribute("LPUDT").Value == "")
                    ? null
                    : (DateTime?)DateTime.Parse(element.Attribute("LPUDT").Value);

                if (MO == null)
                {
                    // выводим сообщение
                }

                // получаем данные полиса
                var polisType =
                    objSpace.FindObject<VidPolisa>(CriteriaOperator.Parse("Code=?", element.Attribute("OPDOC").Value));
                var polisSerial = element.Attribute("SPOL").Value;
                var polisNumber = element.Attribute("NPOL").Value;
                var polisPRZ = element.Attribute("PRZ").Value;
                var polisDateBegin = (element.Attribute("DBEG") == null || element.Attribute("DBEG").Value == "")
                    ? null
                    : (DateTime?)DateTime.Parse(element.Attribute("DBEG").Value);

                // получаем полис
                var polis =
                    objSpace.FindObject<Polis>(CriteriaOperator.Parse("Type=? AND Serial=? AND Number=?",
                        polisType, polisSerial, polisNumber));

                var newPolis = false;
                // если полис не был найден
                if (polis == null)
                {
                    newPolis = true;

                    // создаем новый полис в базе
                    polis = objSpace.CreateObject<Polis>();
                    polis.Type = polisType;
                    polis.Serial = polisSerial;
                    polis.PRZ = polisPRZ;
                    polis.DateBegin = polisDateBegin;
                    polis.SMO = SMO;
                }

                // если пациент найден
                if (pacient != null)
                {
                    // меняем все данные //

                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now,
                        "Пациента найден, устанавливаем при необходимости документ и полис"));

                    // устанавливаем документ пациента
                    if (pacient.Document == null || pacient.Document.Oid != document.Oid)
                        pacient.Document = document;

                    // добавляем полис
                    pacient.AddPolis(polis);

                }
                // если пациент НЕ найден
                else
                {
                    // документ И полис не найдены, создаем их и добавляем к новому пациенту
                    if (newPolis && newDocument)
                    {
                        Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now,
                            "Создаем пациента, задаем его данные"));
                        // создаем пациента
                        pacient = objSpace.CreateObject<Pacient>();
                        pacient.IsNewBorn = false;

                        pacient.LastName = lastName;
                        pacient.FirstName = firstName;
                        pacient.MiddleName = middleName;
                        pacient.Birthdate = birthdate;

                        // устанавливаем документ пациента
                        pacient.Document = document;

                        // добавляем полис
                        pacient.AddPolis(polis);
                    }
                    // но найден либо документ, либо полис, которые указывают на пациента
                    else
                    {
                        Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now,
                            "Пациент не найден, зато найден либо документ, либо полис"));
                        // пациент, к которому привязан полис
                        Pacient polisPacient = null;

                        // пациент, к которому привязан документ
                        Pacient documentPacient = null;

                        if (!newPolis)
                        {
                            polisPacient = polis.Pacient;
                        }

                        if (!newDocument)
                        {
                            documentPacient =
                                objSpace.FindObject<Pacient>(CriteriaOperator.Parse("Document.Oid=?", document.Oid));
                        }

                        // если ничего не нашли
                        if (polisPacient == null && documentPacient == null)
                        {
                            // то это забитые документы, но не приаттаченные к пациенту, которого не создали.
                            Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now,
                                "Отбой, это были 'забытые' документы"));

                            // исправляем косяк
                            pacient = objSpace.CreateObject<Pacient>();
                            pacient.IsNewBorn = false;
                            pacient.LastName = lastName;
                            pacient.FirstName = firstName;
                            pacient.MiddleName = middleName;
                            pacient.Birthdate = birthdate;

                            pacient.Document = document;

                            pacient.AddPolis(polis);
                        }
                        else
                        {
                            loaded--;
                            // если что-то нашли в обоих случаях
                            if (polisPacient != null && documentPacient != null)
                            {
                                // проверяем, что указывают на одного
                                if (polisPacient.Oid == documentPacient.Oid)
                                {
                                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now,
                                        "Пациент у документов найден и он совпадает, данные в СРЗ сменились"));
                                    // более менее норм, возможно:
                                    // 1) пациент поменял ФИО/ДР 
                                    // 2) были неправильно забиты данные пациента

                                    // сообщаем что есть полис и документ в базе с такими же номерами,
                                    // у которых есть ОДИН пациент-хозяин, но данные которого отличаются от тех что в СРЗ
                                }
                                else
                                {
                                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now,
                                        "Пациент у документов найден и он НЕ! совпадает, данные в СРЗ сменились"));
                                    // ваще беда:
                                    // для документа создавали пациента
                                    // для полиса создавали пациента
                                    // в СРЗ еще один пациент

                                    // сообщаем что есть полис и документ в базе с такими же номерами,
                                    // у которых есть СВОИ пациент-хозяин и данные которых отличаются от тех что в СРЗ
                                }
                            }
                            else
                            {

                                if (polisPacient != null)
                                {
                                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now,
                                        "Пациент у полиса найден, данные в СРЗ сменились"));
                                    // сообщаем что есть полис в базе с таким же номером,
                                    // у которого есть пациент-хозяин, но данные которого отличаются от тех что в СРЗ
                                }

                                if (documentPacient != null)
                                {
                                    Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now,
                                        "Пациент у документа найден, данные в СРЗ сменились"));
                                    // сообщаем что есть документ в базе с таким же номером,
                                    // у которого есть пациент-хозяин, но данные которого отличаются от тех что в СРЗ
                                }
                            }
                        }
                    }
                }

                // просто меняем данные по месту жительства
                // не важно для какого: вновь созданного, либо найденного
                if (pacient != null)
                {
                    var okatoAttr = element.Attribute("RN");
                    if (okatoAttr != null)
                    {
                        var okato = okatoAttr.Value;
                        Console.WriteLine(string.Format("{0}\t{1}: {2}", DateTime.Now, "Задаем адрес по окато",
                            okato));
                        pacient.Address = Address.GetAddressByOkato(objSpace, okato);
                    }
                    if (pacient.Address != null)
                    {
                        var collection = new CriteriaOperatorCollection();
                        if (pacient.Address.Level1 != null)
                        {
                            collection.Add(new BinaryOperator("City", pacient.Address.Level1));
                        }
                        if (pacient.Address.Level2 != null)
                        {
                            collection.Add(new BinaryOperator("City", pacient.Address.Level2));
                        }
                        if (pacient.Address.Level3 != null)
                        {
                            collection.Add(new BinaryOperator("City", pacient.Address.Level3));
                        }
                        if (pacient.Address.Level4 != null)
                        {
                            collection.Add(new BinaryOperator("City", pacient.Address.Level4));
                        }

                        var streetValue = element.Attribute("UL").Value;
                        Console.WriteLine(string.Format("{0}\t{1}: {2}", DateTime.Now, "Ищем улицу", streetValue));
                        var street = objSpace.FindObject<Street>(CriteriaOperator.And(
                            CriteriaOperator.Or(collection),
                            CriteriaOperator.Parse(string.Format("Lower(Name) like '{0}%'", streetValue.ToLower())))
                            );
                        if (street == null)
                            Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Не нашли"));
                        else
                        {
                            Console.WriteLine(string.Format("{0}\t{1}", DateTime.Now, "Нашли"));
                        }

                        pacient.Address.Street = street;

                        pacient.Address.House = element.Attribute("DOM").Value;
                        pacient.Address.Build = element.Attribute("KOR").Value;
                        pacient.Address.Flat = element.Attribute("KV").Value;
                    }
                }

                loaded++;

                if (loaded > loadCount)
                {
                    objSpace.CommitChanges();
                    objSpace = Application.CreateObjectSpace();
                    loaded = 0;
                }

            }
            objSpace.CommitChanges();
        }

        private void GetPacientPoliciesInfo_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var dv = View as DetailView;
            if (dv != null)
            {
                var pacient = dv.CurrentObject as Pacient;
                if (pacient != null)
                {
                    TFOmsServices.IService client = new Service();
                    var TfomsInfo = new TfomsPolicesInfo {PoliciesInfo = new List<PolicyInfo>()};
                    if (pacient.Birthdate != null)
                    {
                        var pacientInfo = new PacientInfo()
                        {
                            LastName = pacient.LastName,
                            FirstName = pacient.FirstName,
                            MiddleName = pacient.MiddleName,
                            BirthDate = pacient.Birthdate.Value
                        };
                        TfomsInfo.PoliciesInfo = (List<PolicyInfo>)client.GetPolicyInfos(pacientInfo);
                        if (TfomsInfo.PoliciesInfo.Count == 0)
                        {
                            MessageBox.Show(client.ErrorMessage, "Ошибка получения данных от ТФОМС", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            var current = TfomsInfo.PoliciesInfo.FirstOrDefault(t => t.DateEnd == null);
                            if (current != null)
                                TfomsInfo.MoAttached = client.GetMoAttached(current);

                            if (string.IsNullOrEmpty(TfomsInfo.MoAttached))
                            {
                                MessageBox.Show(client.ErrorMessage, "Ошибка получения данных от ТФОМС", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                var os = Application.CreateObjectSpace();

                                DetailView dvPolicies = Application.CreateDetailView(os, TfomsInfo);
                                ShowViewParameters svp = new ShowViewParameters();
                                svp.CreatedView = dvPolicies;
                                svp.TargetWindow = TargetWindow.NewModalWindow;

                                //отображаем окно
                                Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, GetPacientPoliciesInfo));
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Укажите дату рождения пациента", "Неполные данные", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void CopyAddressFromFactAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var dv = View as DetailView;
            if (dv != null)
            {
                var pacient = dv.CurrentObject as Pacient;
                if (pacient != null)
                {
                    //pacient.Address = pacient.AddressFact;
                    pacient.Address.Copy(pacient.AddressFact);
                    ObjectSpace.SetModified(pacient);
                    dv.Refresh();
                }
            }
        }

        private void CopyAddressToFactAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var dv = View as DetailView;
            if (dv != null)
            {
                var pacient = dv.CurrentObject as Pacient;
                if (pacient != null)
                {
                    //pacient.AddressFact = pacient.Address;
                    pacient.AddressFact.Copy(pacient.Address);
                    ObjectSpace.SetModified(pacient);
                    dv.Refresh();
                }
            }
        }
    }

    public class LongOperationThread
    {
        private Thread _thread;
        private Timer _timer;

        public string OperationName { get; set; }

        public event EventHandler Done;
        public LongOperationThread(ThreadStart threadStart)
        {
            _thread = new Thread(threadStart);
            InitTimer();
        }

        public LongOperationThread(ParameterizedThreadStart paramThreadStart)
        {
            _thread = new Thread(paramThreadStart);
            InitTimer();
        }

        private void InitTimer()
        {
            _timer = new Timer();
            _timer.Interval = 500;
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_thread.IsAlive == false)
            {
                _timer.Stop();
                if (Done != null)
                    Done(_thread, null);
            }
        }

        public bool IsWorking { get { return _thread.IsAlive; } }

        public void Start()
        {
            _thread.Start();
            _timer.Start();
        }

        public void Start(object param)
        {   
            _thread.Start(param);
            _timer.Start();
            
        }
    }

    /// <summary>
    /// Provides a task scheduler that ensures a maximum concurrency level while
    /// running on top of the ThreadPool.
    /// </summary>
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        /// <summary>Whether the current thread is processing work items.</summary>
        [ThreadStatic]
        private static bool _currentThreadIsProcessingItems;
        /// <summary>The list of tasks to be executed.</summary>
        private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)
        /// <summary>The maximum concurrency level allowed by this scheduler.</summary>
        private readonly int _maxDegreeOfParallelism;
        /// <summary>Whether the scheduler is currently processing work items.</summary>
        private int _delegatesQueuedOrRunning = 0; // protected by lock(_tasks)

        /// <summary>
        /// Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the
        /// specified degree of parallelism.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        /// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be queued.</param>
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough
            // delegates currently queued or running to process tasks, schedule another.
            lock (_tasks)
            {
                _tasks.AddLast(task);
                if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                {
                    ++_delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        /// <summary>
        /// Informs the ThreadPool that there's work to be executed for this scheduler.
        /// </summary>
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                // Note that the current thread is now processing work items.
                // This is necessary to enable inlining of tasks into this thread.
                _currentThreadIsProcessingItems = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        Task item;
                        lock (_tasks)
                        {
                            // When there are no more items to be processed,
                            // note that we're done processing, and get out.
                            if (_tasks.Count == 0)
                            {
                                --_delegatesQueuedOrRunning;
                                break;
                            }

                            // Get the next item from the queue
                            item = _tasks.First.Value;
                            _tasks.RemoveFirst();
                        }

                        // Execute the task we pulled out of the queue
                        base.TryExecuteTask(item);
                    }
                }
                // We're done processing items on the current thread
                finally { _currentThreadIsProcessingItems = false; }
            }, null);
        }

        /// <summary>Attempts to execute the specified task on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued"></param>
        /// <returns>Whether the task could be executed on the current thread.</returns>
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!_currentThreadIsProcessingItems) return false;

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued) TryDequeue(task);

            // Try to run the task.
            return base.TryExecuteTask(task);
        }

        /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
        /// <param name="task">The task to be removed.</param>
        /// <returns>Whether the task could be found and removed.</returns>
        protected sealed override bool TryDequeue(Task task)
        {
            lock (_tasks) return _tasks.Remove(task);
        }

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

        /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
        /// <returns>An enumerable of the tasks currently scheduled.</returns>
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_tasks, ref lockTaken);
                if (lockTaken) return _tasks.ToArray();
                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_tasks);
            }
        }
    }

    [DomainComponent]
    [XafDisplayName("Данные полисов пациента из ТФОМС")]
    public class TfomsPolicesInfo
    {
        [XafDisplayName("Полисы пациента")]
        public List<PolicyInfo> PoliciesInfo { get; set; }
        [XafDisplayName("Прикреплен к МО")]
        public string MoAttached { get; set; }
    }

    [DomainComponent]
    [XafDisplayName("Поиск пациента")]
    [RuleCriteria("Iris.Jurist.Lawsuit.ImagesSumLength", "Action",
        "not IsNullOrEmpty(LastName) or not IsNullOrEmpty(FirstName) or not IsNullOrEmpty(MiddleName) or not IsNullOrEmpty(PolisNum)",
        "Необходимо указать хотя бы один параметр поиска", SkipNullOrEmptyValues = false)]
    public class PacientFilterFieldsParameters
    {
        private static string lastName;
        private static string firstName;
        private static string middleName;
        private static string polisNum;

        public PacientFilterFieldsParameters()
        {
            LastName = LastName ?? string.Empty;
            FirstName = FirstName ?? string.Empty;
            MiddleName = MiddleName ?? string.Empty;
            PolisNum = PolisNum ?? string.Empty;
        }

        [XafDisplayName("Фамилия")]
        public string LastName
        {
            get { return lastName; }
            set
            {
                if (lastName == value)
                    return;

                lastName = value;
            }
        }
        [XafDisplayName("Имя")]
        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (firstName == value)
                    return;

                firstName = value;
            }
        }
        [XafDisplayName("Отчество")]
        public string MiddleName
        {
            get { return middleName; }
            set
            {
                if (middleName == value)
                    return;

                middleName = value;
            }
        }

        [XafDisplayName("Номер полиса")]
        public string PolisNum
        {
            get { return polisNum; }
            set
            {
                if (polisNum == value)
                    return;

                polisNum = value;
            }
        }
    }

    [DomainComponent]
    [XafDisplayName("Конфликты")]
    public class PacientLoadConflicts
    {
        
    }

}
