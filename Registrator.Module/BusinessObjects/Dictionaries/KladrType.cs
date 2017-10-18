using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// ��� ������� �����-�
    /// </summary>
    [DefaultClassOptions]
    public class KladrType : BaseObject
    {
        public KladrType() { }
        public KladrType(Session session) : base(session) { }

        /// <summary>
        /// ���
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// ������� � ��������
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// ������� ��������
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <returns>�������� �������</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
