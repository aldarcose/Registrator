using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TFOmsServices
{
    public class PolicyInfo
    {
        /// <summary>
        /// Серия полиса
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// Номер полиса
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public DateTime DateBeg { get; set; }

        /// <summary>
        /// Дата окончания действия полиса
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// СМО, выдавшая полис
        /// </summary>
        public string SMO { get; set; }
    }

    public class PacientInfo
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime BirthDate { get; set; }
    }
}
