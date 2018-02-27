using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Registrator.Module.BusinessObjects;

namespace Registrator.Module.BusinessObjects.Interfaces
{
    /// <summary>
    /// ИНтерфейс объекта, данные которого должны попадать в реестр.
    /// </summary>
    interface IReestrTFoms
    {
        /// <summary>
        /// Проверяет валидны ли поля, которые должны быть выгружены в реестр
        /// </summary>
        /// <returns>True если валидно</returns>
        bool IsValidForReestr();

        /// <summary>
        /// Возвращает элемент записи реестра
        /// </summary>
        /// <returns>XElement записи</returns>
        XElement GetReestrElement();
    }
}
