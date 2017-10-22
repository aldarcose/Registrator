using System;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Представляет запись протокола: комбинация поля протокола и его значения
    /// Реализует Сущность шаблона EAV
    /// </summary>
    [XafDisplayName("Запись протокола")]
    [DefaultClassOptions]
    public class ProtocolRecord : BaseObject, ITreeNode
    {
        public ProtocolRecord(Session session) : base(session) { }

        /// <summary>
        /// Инстанцирует запись для протокола на основе типа
        /// </summary>
        /// <returns></returns>
        public static ProtocolRecord GetProtocolRecord(Session session, ProtocolRecordType type)
        {
            var protocolRecord = new ProtocolRecord(session);

            protocolRecord.Type = type;
            protocolRecord.Value = type.GetDefaultValue();

            // если комплекс, создаем комплекс для значений
            if (type.Type == TypeEnum.Complex)
                CreateComplex(protocolRecord, type);

            return protocolRecord;
        }

        private static void CreateComplex(ProtocolRecord parent, ProtocolRecordType type)
        {
            foreach (var protocolRecordType in type._children)
            {
                var protocolRecord = new ProtocolRecord(parent.Session) { Type = protocolRecordType };
                protocolRecord.Value = protocolRecordType.GetDefaultValue();
                parent._children.Add(protocolRecord);

                if (protocolRecordType.Type == TypeEnum.Complex)
                    CreateComplex(protocolRecord, protocolRecordType);
            }
        }

        [XafDisplayName("Поле")]
        [VisibleInListView(true)]
        public ProtocolRecordType Type { get; set; }

        [XafDisplayName("Значение записи")]
        [Size(256)]
        [VisibleInListView(true)]
        public string Value { get; set; }

        [Association("Protocol-Records")]
        [Browsable(false)]
        public EditableProtocol Protocol { get; set; }

        #region ITreeNode
        [Browsable(false)]
        public string Name { get { return Type.Name; } }

        [Association("RecordParent - RecordChildren")]
        [Browsable(false)]
        public ProtocolRecord _parent { get; set; }

        [Association("RecordParent - RecordChildren")]
        [Browsable(false)]
        public XPCollection<ProtocolRecord> _children 
        { 
            get { return GetCollection<ProtocolRecord>("_children"); }
        }

        [Browsable(false)]
        public ITreeNode Parent 
        { 
            get { return _parent as ITreeNode; } 
        }

        [Browsable(false)]
        public IBindingList Children 
        { 
            get { return _children as IBindingList; } 
        }
        #endregion
    }
}
