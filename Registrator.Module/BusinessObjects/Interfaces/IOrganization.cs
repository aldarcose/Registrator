using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.Interfaces
{
    /// <summary>
    /// Интерфейс организации
    /// </summary>
    public interface IOrganization
    {
        /// <summary>
        /// Получение кода ТФ ОКАТО
        /// </summary>
        string GetTfOkato();
    }
}
