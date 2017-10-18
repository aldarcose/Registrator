using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Вид медицинского вмешательства
    /// </summary>
    public class V001: DevExpress.Persistent.BaseImpl.BaseObject
    {
        public V001(Session session)
            : base(session)
        {
        }

        [Size(50)]
        public string Code { get; set; }

        [Size(255)]
        public string Name { get; set; }

        public DateTime? DATEBEG { get; set; }

        public DateTime? DATEEND { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
