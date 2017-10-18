using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Document : BaseObject
    {
        public Document() { }
        public Document(Session session) : base(session) { }

        public DocumentType Type { get; set; }

        /// <summary>
        /// Серия
        /// </summary>
        [Size(20)]
        public string Serie { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        [Size(50)]
        public string Number { get; set; }

        /// <summary>
        /// Дата начала действия документа
        /// </summary>
        public DateTime? DateBegin { get; set; }

        /// <summary>
        /// Дата окончания действия документа
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Место рождения
        /// </summary>
        [Size(256)]
        public string MR { get; set; }
        
        public override string ToString()
        {
            return String.Format("{0} серия {1} №{2} от {3}", Type, Serie, Number, DateBegin);
        }
    }

    /// <summary>
    /// Тип документа удостоверяющего личность физического лица
    /// </summary>
    [DefaultClassOptions]
    public class DocumentType : BaseObject
    {
        public DocumentType() { }
        public DocumentType(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        [Size(5)]
        public string Code { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Size(255)]
        public string Name { get; set; }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Name")]
        public string Text { get { return (string)EvaluateAlias("Text"); } }

        public override string ToString()
        {
            return String.Format("{0}", Name);
        }
    }
}
