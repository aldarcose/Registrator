using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.DC;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Dictionaries;

namespace Registrator.Module.Controllers
{
    public partial class TextTemplateViewController : ViewController
    {
        public event EventHandler TextTemplateItemProcess;

        public TextTemplateViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            var listView = View as ListView;
            if (listView != null)
            {
                // �������� ��������� ������
                Frame.GetController<NewObjectViewController>().Active.SetItemValue("EnabledNewAction", false);
                Frame.GetController<DeleteObjectsViewController>().Active.SetItemValue("EnabledDeleteAction", false);
                Frame.GetController<LinkUnlinkController>().Active.SetItemValue("EnabledLinkAction", false);
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // � ��� ������� ���������� ����������� �������� ������� ������
            var listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
            // ��������� ���������� ��� ���������� ��������
            listViewProcessCurrentObjectController.CustomProcessSelectedItem += (o, e) =>
            {
                if (TextTemplateItemProcess != null)
                    TextTemplateItemProcess(this, e);
            };
        }
        
        private void EditTemplateAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Doctor doctor = ObjectSpace.GetObject(SecuritySystem.CurrentUser as Doctor);
            if (doctor != null)
            {
                var listView = View as ListView;
                if (listView != null)
                {
                    // �������� ��� �������� ������
                    var type = listView.ObjectTypeInfo.Type;

                    // ������������ ������� �� ���� �������
                    MethodInfo method = typeof (Enumerable).GetMethod("OfType");
                    MethodInfo generic = method.MakeGenericMethod(new Type[] { type });
                    var templates = (IEnumerable<TextTemplate>)generic.Invoke(null, new object[] { doctor.TextTemplates });

                    // ������� ������ ��� ����������� � ���� ��������������
                    var templateField = new TextTemplateEditParameters() { EditTemplates = templates.ToList(), TemplateType = type };

                    // ���������� ����
                    ShowView((ActionBase) sender, templateField);
                }
            }
        }

        private void ShowView(ActionBase action, TextTemplateEditParameters templateField)
        {
            ShowViewParameters svp = new ShowViewParameters();

            var os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, templateField);
         
            svp.CreatedView = dv;
            svp.TargetWindow = TargetWindow.NewModalWindow;
            DialogController dc = new DialogController();
            dc.Accepting += DcOnAccepting;
            dc.CancelAction.Caption = "������";
            svp.Controllers.Add(dc);
            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, action));
        }

        private void DcOnAccepting(object sender, DialogControllerAcceptingEventArgs eventArgs)
        {
            var templateField = eventArgs.AcceptActionArgs.CurrentObject as TextTemplateEditParameters;
            if (templateField != null && templateField.EditTemplates != null)
            {
                var doctor = ObjectSpace.GetObject(SecuritySystem.CurrentUser as Doctor);
                if (doctor != null)
                {
                    // ������� ��� ������� ����� ����
                    doctor.DeleteTemplates(templateField.TemplateType);
                    // ������� �����
                    doctor.CreateTemplates(templateField.Text, templateField.TemplateType);
                    ObjectSpace.SetModified(templateField);
                }
            }
        }
    }

    [DomainComponent]
    [XafDisplayName("�������������� �������")]
    public class TextTemplateEditParameters
    {
        public TextTemplateEditParameters()
        {
            EditTemplates = null;
            TemplateType = typeof(TextTemplate);
        }

        [Browsable(false)]
        public Type TemplateType { get; set; }

        private List<TextTemplate> editTemplates;

        [Browsable(false)]
        public List<TextTemplate> EditTemplates
        {
            get { return editTemplates; }
            set
            {
                editTemplates = value;
                if (editTemplates != null)
                {
                    GenerateTemplate(editTemplates);
                }
            }
        }

        private void GenerateTemplate(IEnumerable<TextTemplate> _editTemplates)
        {
            var sb = new StringBuilder();
            foreach (var editTemplate in _editTemplates)
                if (editTemplate.Parent == null)
                    GenerateTemplate(0, sb, editTemplate);
            Text = sb.ToString();
        }

        private void GenerateTemplate(int level, StringBuilder sb, TextTemplate template)
        {
            string tabs = string.Empty;
            for (int i = 0; i < level; i++)
            {
                tabs += "\t";
            }

            sb.AppendLine(string.Format("{0}{1}", tabs, template.Name));

            foreach (var child in template.TextTemplateChildren)
                GenerateTemplate(level + 1, sb, child);
        }

        [XafDisplayName("������")]
        [EditorAlias("RTF")]
        public string Text { get; set; }
    }
}
