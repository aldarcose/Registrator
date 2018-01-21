using System;
using System.Linq;
using System.Text;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.XtraPrinting.Native;
using DevExpress.ExpressApp.Xpo;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Базовый класс для текстовых шаблонов
    /// </summary>
    public abstract class TextTemplate : BaseObject, ITreeNode
    {
        private TextTemplate textTemplateParent;
        private Doctor doctor;

        public TextTemplate(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            IObjectSpace os = XPObjectSpace.FindObjectSpaceByObject(this);
            Doctor = os.GetObject(SecuritySystem.CurrentUser as Doctor);
        }

        [XafDisplayName("Шаблон")]
        [Size(400)]
        public string Name { get; set; }

        [Browsable(false)]
        public ITreeNode Parent 
        {
            get { return TextTemplateParent as ITreeNode; } 
        }

        [VisibleInListView(true)]
        [VisibleInDetailView(false)]
        public IBindingList Children 
        { 
            get { return TextTemplateChildren as IBindingList; } 
        }

        [Browsable(false)]
        [Association("TextTemplateParent-TextTemplateChildren")]
        public TextTemplate TextTemplateParent
        {
            get { return textTemplateParent; }
            set { SetPropertyValue("TextTemplateParent", ref textTemplateParent, value); } 
        }

        [Browsable(false)]
        [Association("TextTemplateParent-TextTemplateChildren"), DevExpress.Xpo.Aggregated]
        public XPCollection<TextTemplate> TextTemplateChildren
        {
            get { return GetCollection<TextTemplate>("TextTemplateChildren"); }
        }

        [Browsable(false)]
        [Association("Doctor-TextTemplates")]
        public Doctor Doctor
        {
            get { return doctor; }
            set { SetPropertyValue("Doctor", ref doctor, value); } 
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.GetTextTemplate();
        }

        public string GetTextTemplate()
        {
            if (TextTemplateParent == null) return Name;
            return TextTemplateParent.GetTextTemplate() + " : " + this.Name;
        }
    }

    /// <summary>
    /// Шаблон анамнеза
    /// </summary>
    public class AnamnezTemplate : TextTemplate
    {
        public AnamnezTemplate(Session session) : base(session) { }
    }

    /// <summary>
    /// Шаблон жалобы
    /// </summary>
    public class ComplainTemplate : TextTemplate
    {
        public ComplainTemplate(Session session) : base(session) { }
    }

    /// <summary>
    /// Шаблон рекомендации
    /// </summary>
    public class RecomendTemplate : TextTemplate    
    {
        public RecomendTemplate(Session session) : base(session) { }
    }

    /// <summary>
    /// Шаблон объективного статуса терапевта
    /// </summary>
    public class ObjStatusTerTemplate : TextTemplate
    {
        public ObjStatusTerTemplate(Session session) : base(session) { }
    }
}
