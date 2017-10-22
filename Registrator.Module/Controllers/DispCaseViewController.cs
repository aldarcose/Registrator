using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.PivotGrid.OLAP.AdoWrappers;
using DevExpress.XtraEditors;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Abstract;
using Registrator.Module.BusinessObjects.Dictionaries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Registrator.Module.BusinessObjects.HelpUtils;
using ListView = DevExpress.ExpressApp.ListView;

namespace Registrator.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class DispCaseViewController : ViewController
    {
        public DispCaseViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            var listView = View as ListView;

            if (listView != null)
            {
                Frame.GetController<NewObjectViewController>().Active.SetItemValue("EnabledNewAction", false);
                Frame.GetController<DeleteObjectsViewController>().Active.SetItemValue("EnabledDeleteAction", false);
                Frame.GetController<LinkUnlinkController>().Active.SetItemValue("EnabledLinkAction", false);
                Frame.GetController<StateMachineController>().Active.SetItemValue("EnabledStateAction", false);
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.

            var detailView = View as DetailView;

            if (detailView != null)
            {
                var dispCase = View.CurrentObject as DispCase;

                if (dispCase != null)
                {
                    
                }
            }
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private DispTypeCreateInfo GetDispTypesCanCreated(Pacient pacient, DateTime now)
        {
            if (pacient == null)
                return null;
            var age = pacient.Age;
            DispTypeCreateInfo dispTypeSelector = new DispTypeCreateInfo();

            

            // ���� ������� �������� (18 <= age <= 99)
            if (age >= 18)
            {
                
                // ���� ��� ������� ��������������� � ���� ����
                if (pacient.DispanserizaionCases != null && pacient.DispanserizaionCases.Any(t => t.DateIn.Year == now.Year))
                    // ��� �������� ��������� ���
                    return dispTypeSelector;

                // ����� �������� ����������
                dispTypeSelector.AddType(DispType.ProfOsmotrAdult);

                // � ���� ������� ������� �� ��� (21, 24, 27) �������� �����
                if (age > 18 & age % 3 == 0)
                {
                    // � �� ���� � ������� ���� �����������
                    if (pacient.DispanserizaionCases==null || !pacient.DispanserizaionCases.Any(t => t.Type == DispType.ProfOsmotrAdult && t.DateIn.Year == (now.Year - 1)))
                    {
                        // ��: ����� �������� ���������������.
                        dispTypeSelector.AddType(DispType.DOGVN1);
                    }

                }
            }
            // ���� ������� ������ 18 ���
            else
            {
                // �������� ������ ���� ����� ���������������, ������� ����� ��������
                var services = ObjectSpace.GetObjects<DispsServiceList>();

                // �� ��� �������� ������ ��, ������� ����� �������� ��������
                var servicesCanAdd = services.Where(t => t.CheckPacient(pacient, now)).ToList();

                if (servicesCanAdd.Count > 0)
                {
                    if (pacient.DispanserizaionCases != null && pacient.DispanserizaionCases.Count > 0)
                    {
                        // ��� ������ ����������� �������� ��������������� ���������� �� ���
                        // � ������� ��� �� ������ ������� ����� ��������
                        foreach (var dispanserizaionCase in pacient.DispanserizaionCases)
                        {
                            var addedService =
                                services.FirstOrDefault(
                                    t =>
                                        t.Type == dispanserizaionCase.Type &&
                                        t.CheckPacient(pacient, dispanserizaionCase.DateIn));
                            if (addedService != null)
                            {
                                servicesCanAdd.Remove(addedService);
                            }
                        }

                        // ����� ���������� ��������� ������ ��� ���������������
                        var typesCanAdd = new List<DispType>();
                        foreach (var dispsServiceList in servicesCanAdd)
                        {
                            if (!typesCanAdd.Contains(dispsServiceList.Type))
                                typesCanAdd.Add(dispsServiceList.Type);
                        }

                        // ��������� �� � ������ ������

                        foreach (var dispType in typesCanAdd)
                        {
                            dispTypeSelector.AddType(dispType);
                            /*dispTypeSelector.AddType(DispType.ProfOsmotrChild);
                            dispTypeSelector.AddType(DispType.PreProfOsmotrChild);
                            dispTypeSelector.AddType(DispType.PeriodProfOsmotrChild);
                            dispTypeSelector.AddType(DispType.DispChildOrphan1);
                            dispTypeSelector.AddType(DispType.DispChildOrphan12);
                            dispTypeSelector.AddType(DispType.DispStacionarChildOrphan1);
                            dispTypeSelector.AddType(DispType.DispStacionarChildOrphan12);*/
                        }
                    }
                    else{
                        dispTypeSelector.AddType(DispType.ProfOsmotrChild);
                        dispTypeSelector.AddType(DispType.PreProfOsmotrChild);
                        dispTypeSelector.AddType(DispType.PeriodProfOsmotrChild);
                        dispTypeSelector.AddType(DispType.DispChildOrphan1);
                        dispTypeSelector.AddType(DispType.DispChildOrphan12);
                        dispTypeSelector.AddType(DispType.DispStacionarChildOrphan1);
                        dispTypeSelector.AddType(DispType.DispStacionarChildOrphan12);
                    }
                }
                else
                {
                    return dispTypeSelector;
                }

                
            }

            return dispTypeSelector;
        }


        private void NewDispCaseAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var listView = View as ListView;
            var propertyCollSource = (PropertyCollectionSource) listView.CollectionSource;
            if (listView != null && propertyCollSource != null)
            {
                var pacient = propertyCollSource.MasterObject as Pacient;

                // ����������� ���� �������� (������ ������)
                InputBox input = new InputBox();
                input.Caption = "������� ���� �������� ������";

                var now = DateTime.Now;
                if (IsAdmin())
                    if (input.ShowDialog() == DialogResult.OK)
                    {
                        if (!DateTime.TryParse(input.Value, out now))
                        {
                            MessageBox.Show("�� ����");
                        }
                    }

                var dispTypeSelector = GetDispTypesCanCreated(pacient, now);

                if (dispTypeSelector != null && dispTypeSelector.AllowedTypes.Count > 0)
                {

                    // ���������� ���� ������ ����
                    ShowViewParameters svp = new ShowViewParameters();
                    var os = Application.CreateObjectSpace();
                    DetailView dv = Application.CreateDetailView(os, dispTypeSelector);
                    dv.Tag = now;
                    svp.CreatedView = dv;
                    svp.TargetWindow = TargetWindow.NewModalWindow;

                    var dc = Application.CreateController<DialogController>();
                    dc.CancelAction.Caption = "������";
                    dc.Accepting += dc_Accepting;
                    svp.Controllers.Add(dc);
                    dv.Closing += dv_Closing;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, (ActionBase) sender));
                }
                else
                {
                    XtraMessageBox.Show("������ ��������������� ��� ����������� ��� �������� ��� ������");
                }
            }
        }

        void dc_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {
            (e.AcceptActionArgs.CurrentObject as DispTypeCreateInfo).IsAccepted = true;
        }

        void dv_Closing(object sender, EventArgs e)
        {
            var dv = sender as DetailView;
            if ((dv.CurrentObject as DispTypeCreateInfo).IsAccepted)
            {
                var lb = dv.FindItem("AllowedTypes");
                if (lb != null)
                {
                    var navi = lb.Control as INavigatableControl;

                    if (navi != null)
                    {
                        var curObject = dv.CurrentObject as DispTypeCreateInfo;
                        CreateDispCase(curObject.GetType(navi.Position), (DateTime)dv.Tag);
                    }
                }
            }
        }

        public bool IsAdmin()
        {
            var loggedDoctor = (Doctor)SecuritySystem.CurrentUser;
            return loggedDoctor.DoctorRoles.Any(t => t.IsAdministrative);
        }

        void CreateDispCase(DispType type, DateTime date)
        {
            var listView = View as ListView;

            var dispCaseType = typeof (DispanserizaionCase);

            if (listView != null)
            {
                var pacient = ((PropertyCollectionSource)listView.CollectionSource).MasterObject as Pacient;

                if (pacient != null)
                {
                    // ������� ObjectSpace
                    var os = Application.CreateObjectSpace();

                    // ������� ������ ������ � ���� ������������
                    var dispCase = os.CreateObject(dispCaseType) as DispanserizaionCase;
                    // ����������� ������ � ��������
                    ((AbstractCase)dispCase).Pacient = os.GetObject(pacient);

                    dispCase.DateIn = date;

                    dispCase.Type = type;

                    // ��������� ������. ��� ���������� ������������ �������! ������ ���� ����� ��� ������
                    dispCase.AddDefaultServices(pacient, date);

                    // ������� ��������� ���
                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, dispCase);
                    dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                    svp.CreatedView = dv;

                    
                    //svp.TargetWindow = TargetWindow.NewModalWindow;
                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, (ActionBase)NewDispCaseAction));
                }
            }
        }

        private void simpleAction1_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var lv = View as ListView;
            if (lv != null)
            {
                if (lv.SelectedObjects.Count > 0)
                {
                    var disp = lv.SelectedObjects[0] as DispanserizaionCase;

                    var schemaPath = @"c:\Users\tsb\Desktop\��+��.+������.+�����+�+�����������.us556bf8393df20\orph-card-schema.xsd";

                    XmlSchemaSet schemas = new XmlSchemaSet();
                    schemas.Add("", schemaPath);

                    var report = new XDocument();
                    report.Declaration = new XDeclaration("1.0", "utf-8", "yes");

                    var root = new XElement("children");

                    try
                    {
                        var card = disp.GetChildBlock();
                        root.Add(card);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine(ex.Source);
                        MessageBox.Show("error. details in out");
                    }

                    report.Add(root);

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "xml files (*.xml)|*.xml";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        report.Save(sfd.FileName);

                        report.Validate(schemas, (o, eArg) =>
                        {
                            MessageBox.Show("�������� �� ��������!\n" + eArg.Message);
                        });
                            
                    }
                    
                }
            }
        }
    }

    [DomainComponent]
    [XafDisplayName("��� ���������������")]
    public class DispTypeCreateInfo
    {
        private Dictionary<DispType, string> dispTypeDictionary;

        public DispTypeCreateInfo()
        {
            AllowedTypes = new List<string>();

            dispTypeDictionary = new Dictionary<DispType, string>();
            dispTypeDictionary.Add(DispType.DOGVN1, "�����, 1-� ����");
            dispTypeDictionary.Add(DispType.DOGVN2, "�����, 2-� ����");
            dispTypeDictionary.Add(DispType.ProfOsmotrAdult, "���������� ��������� ���������");
            dispTypeDictionary.Add(DispType.DispChildOrphan1,
                "��������������� �����-����� � �����, ���������� ��� ��������� ���������, 1-� ����");
            dispTypeDictionary.Add(DispType.DispChildOrphan12,
                "��������������� �����-����� � �����, ���������� ��� ��������� ���������, 1-� � 2-� ����");
            dispTypeDictionary.Add(DispType.DispStacionarChildOrphan1,
                "��������������� ����������� � ������������ ����������� �����-����� � ����� � ������� ��������� ��������, 1-� ����");
            dispTypeDictionary.Add(DispType.DispStacionarChildOrphan12,
                "��������������� ����������� � ������������ ����������� �����-����� � ����� � ������� ��������� ��������, 1-� � 2-� ����");
            dispTypeDictionary.Add(DispType.ProfOsmotrChild, "���������� ������������������");
            dispTypeDictionary.Add(DispType.PreProfOsmotrChild, "��������������� ���������� ������������������");
            dispTypeDictionary.Add(DispType.PeriodProfOsmotrChild, "������������� ���������� ������������������");

            IsAccepted = false;
        }

        [Browsable(false)]
        public bool IsAccepted { get; set; }

        [XafDisplayName("�������� ���:")]
        public List<string> AllowedTypes { get; set; }

        public void AddType(DispType type)
        {
            AllowedTypes.Add(dispTypeDictionary[type]);
        }

        public DispType GetType(int position)
        {
            if (position < 0 || position > AllowedTypes.Count - 1)
                throw new IndexOutOfRangeException();

            return dispTypeDictionary.First(t => t.Value.Equals(AllowedTypes[position])).Key;
        }
    }
}
