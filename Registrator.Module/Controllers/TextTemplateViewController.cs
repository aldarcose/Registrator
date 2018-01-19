using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.DC;
using Registrator.Module.BusinessObjects;
using Registrator.Module.BusinessObjects.Dictionaries;
using ListView = DevExpress.ExpressApp.ListView;
using ViewController = DevExpress.ExpressApp.ViewController;

namespace Registrator.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
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
                // скрываем дефолтные экшены
                Frame.GetController<DevExpress.ExpressApp.SystemModule.NewObjectViewController>().Active.SetItemValue("EnabledNewAction", false);
                Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>().Active.SetItemValue("EnabledDeleteAction", false);
                Frame.GetController<DevExpress.ExpressApp.SystemModule.LinkUnlinkController>().Active.SetItemValue("EnabledLinkAction", false);
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            
            // в нем находим контроллер обработчика текущего объекта списка
            var listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
            // добавляем обработчик для выбранного элемента
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
                    // получаем тип объектов списка
                    var type = listView.ObjectTypeInfo.Type;

                    // динамическая выборка по типу объекта
                    MethodInfo method = typeof (Enumerable).GetMethod("OfType");
                    MethodInfo generic = method.MakeGenericMethod(new Type[] { type });
                    var templates = (IEnumerable<TextTemplate>)generic.Invoke(null, new object[] { doctor.TextTemplates });

                    // создаем объект для отображения в окне редактирования
                    var templateField = new TextTemplateField() { EditTemplates = templates.ToList(), TemplateType = type };

                    // отображаем окно
                    ShowView((ActionBase) sender, templateField);
                }
            }
        }

        private void ShowView(ActionBase action, TextTemplateField templateField)
        {
            ShowViewParameters svp = new ShowViewParameters();

            var os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, templateField);
         
            svp.CreatedView = dv;
            svp.TargetWindow = TargetWindow.NewModalWindow;
            DialogController dc = new DialogController();
            dc.Accepting += DcOnAccepting;
            dc.CancelAction.Caption = "Отмена";
            svp.Controllers.Add(dc);
            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, action));
        }

        private void DcOnAccepting(object sender, DialogControllerAcceptingEventArgs eventArgs)
        {
            var templateField = eventArgs.AcceptActionArgs.CurrentObject as TextTemplateField;
            if (templateField != null && templateField.EditTemplates != null)
            {
                var doctor = ObjectSpace.GetObject(SecuritySystem.CurrentUser as Doctor);
                if (doctor != null)
                {
                    // удаляем все шаблоны этого типа
                    doctor.DeleteTemplates(templateField.TemplateType);
                    // создаем новые
                    doctor.CreateTemplates(templateField.Text, templateField.TemplateType);
                    ObjectSpace.SetModified(templateField);
                }
            }
        }
    }

    [DomainComponent]
    [XafDisplayName("Редактирование шаблона")]
    public class TextTemplateField
    {
        public TextTemplateField()
        {
            EditTemplates = null;
            TemplateType = typeof(TextTemplate);
        }

        [Browsable(false)]
        public Type TemplateType { get; set; }

        private List<TextTemplate> _editTemplates;

        [Browsable(false)]
        public List<TextTemplate> EditTemplates
        {
            get { return _editTemplates; }
            set
            {
                _editTemplates = value;
                if (_editTemplates != null)
                {
                    GenerateTemplate(_editTemplates);
                }
            }
        }

        private void GenerateTemplate(IEnumerable<TextTemplate> _editTemplates)
        {
            var sb = new StringBuilder();
            foreach (var editTemplate in _editTemplates)
            {
                if (editTemplate.Parent==null)
                    GenerateTemplate(0, sb, editTemplate);
            }
            Text = sb.ToString();
        }

        private void GenerateTemplate(int level, StringBuilder sb, TextTemplate template)
        {
            var tab = string.Empty;
            for (int i = 0; i < level; i++)
            {
                tab += "\t";
            }

            sb.AppendLine(string.Format("{0}{1}", tab, template.Name));

            foreach (var child in template.TextTemplateChildren)
            {
                GenerateTemplate(level + 1, sb, child);
            }
        }

        [XafDisplayName("Шаблон")]
        [EditorAlias("RTF")]
        public string Text { get; set; }

    }
}
