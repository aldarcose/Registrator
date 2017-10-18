using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.Interfaces
{
    public interface IWorkflowCaseStatus
    {
        WorkflowCaseStatus WorkflowCaseStatus { get; set; }
    }

    public enum WorkflowCaseStatus
    {
        // создан - только только создан
        Created = 1,
        // редактируется - услуги заполняются
        InProgress = 2,
        // закрыт - закрыт
        Closed = 3,
        // отказано - отказано в оказании 
        Rejected = 4,
        // отказ пациента - отказ пациента
        Refused = 5,
        // ожидает оплаты - при формировании реестра
        WaitingForPayment = 10,
        // оплачен - ответ от тфомс
        Paid = 11,
        // частично оплачено - ответ от тфомс
        PartlyPaid = 12,
        // отказано в оплате - ответ от тфомс
        PaymentRejected = 13
    }
}
