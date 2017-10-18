using System;
using System.Linq;
using System.Text;
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

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    public class Street : BaseObject
    {
        public Street() { }
        public Street(Session session)
            : base(session)
        {
        }

        /// <summary>
        /// ���
        /// </summary>
        [Size(50)]
        public string Code { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [RuleRequiredField]
        [DataSourceCriteria("Level=5")]
        public KladrType Type { get; set; }

        public Kladr City { get; set; }

        [Size(255)]
        [RuleRequiredField]
        public string Name { get; set; }
        
        /// <summary>
        /// �������� ������
        /// </summary>
        [Size(6)]
        public string CodePost { get; set; }

        /// <summary>
        /// ��� ����
        /// </summary>
        [Size(4)]
        public string CodeIfns { get; set; }

        /// <summary>
        /// ��� ���������� ����
        /// </summary>
        [Size(4)]
        public string CodeIfnsTerr { get; set; }

        /// <summary>
        /// ��� �����
        /// </summary>
        [Size(11)]
        public string CodeOkato { get; set; }

        /// <summary>
        /// ��������� �������������
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Concat(Iif(!IsNull(Type), Concat(Type.ShortName, '. '),''), Name)")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return String.Format("{1}{0}", Name, Type != null ? Type.ShortName + ". " : String.Empty);
        }
    }
}
