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
    /// Тип объекта КЛАДР-а
    /// </summary>
    [DefaultClassOptions]
    public class KladrType : BaseObject
    {
        public KladrType() { }
        public KladrType(Session session) : base(session) { }

        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Уровень в иерархии
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Краткое название
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Строковый формат
        /// </summary>
        /// <returns>Название объекта</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
