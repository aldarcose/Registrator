using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using Registrator.Module.BusinessObjects;
using Registrator.Module.Win.Rias;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using ListView = DevExpress.ExpressApp.ListView;
using Message = System.ServiceModel.Channels.Message;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class TestRiasViewController : ViewController
    {
        public TestRiasViewController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.

            if ((SecuritySystem.CurrentUser as Doctor).DoctorRoles.Any(t => t.IsAdministrative))
            {
                simpleAction1.Active.SetItemValue("Dis", true);
                simpleAction2.Active.SetItemValue("Dis2", true);
            }
            else
            {
                simpleAction1.Active.SetItemValue("Dis", false);
                simpleAction2.Active.SetItemValue("Dis2", false);
            }
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void simpleAction1_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var lv = View as ListView;
            if (lv == null)
                return;

            var tasks = new List<Task>();
            var tf = new TaskFactory();

            var list = lv.SelectedObjects.Cast<Pacient>().ToList();

            tasks.Add(tf.StartNew(()=>SendMessage(0, list)));
            tasks.Add(tf.StartNew(() => SendMessage(1, list)));

        }

        private void SendMessage(int i, List<Pacient> patients)
        {
            var myBinding = new WSHttpBinding(SecurityMode.None);
            myBinding.MessageEncoding = WSMessageEncoding.Text;

            var myEndpoint = new EndpointAddress("http://medframe.burmiac.ru/rias/service.svc");

            Rias.StatServiceClient client = new StatServiceClient(myBinding, myEndpoint);
            var myEndPointBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(myEndPointBehavior);


            //токен доступа к сервису
            Guid guid = Guid.Parse("4926c49b-dd05-4ffc-a281-5f134d14a671");

            //идентиификатор организации
            string idOrg = "1.2.643.5.1.13.3.25.3.32";

            
            var maxCount = 500;

            client.Open();
            Console.WriteLine("Task: " + i);

            var takenPatients = patients.Take((i+1)*maxCount).Skip(i*maxCount);

            var sb = new StringBuilder();
            int count = 0;

            foreach (Pacient p in takenPatients)
            {
                Console.WriteLine("Пациент: " + p.FullName);
                Console.WriteLine("Преобразуем нашего пациента к их пациенту.");
                Rias.Patient patient = null;try
                {
                    patient = GetRiasPatient(p);
                    if (patient == null)
                    {
                        Console.WriteLine("Что-то пошло не так!");
                        continue;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
                
                
                Console.WriteLine("Добавляем его:");
                try
                {
                    var answer = client.AddPatient(guid, idOrg, patient);

                    if (answer.Success)
                    {
                        Console.WriteLine("Успешно добавлено!");
                    }
                    else
                    {
                        Console.WriteLine("Код ошибки:" + answer.Code);
                        Console.WriteLine("Выполнено с ошибками:");
                        foreach (var entityValidationResult in answer.Errors)
                        {

                            Console.WriteLine(entityValidationResult.ErrorMessage);
                        }

                        var existMsg = "Пациент с такими данными существует";

                        if (answer.Errors.Count() == 1 && answer.Errors[0].ErrorMessage.Equals(existMsg))
                        {
                            Console.WriteLine("Ах так, тогда меняем данные пациента!");

                            // Поменяем фамилию
                            patient.Familyname += "_ADDED_FUST_FOR_TEST!_";

                            var policies = patient.InsurancePolicy.ToList();
                            // Добавим полис с неправильным номером
                            policies.Add(new InsurancePolicy() { CompanyCode = "22001", Isvalid = false, Number = "1234567890123456!" });

                            patient.InsurancePolicy = policies.ToArray();

                            client.UpdatePatient(guid, idOrg, patient);
                        }

                    }
                    count++;
                }
                catch (Exception ex)
                {
                    
                    sb.AppendLine(ex.Message);
                }
                

                
            }

            File.WriteAllText(string.Format("log_thread {0}.txt",i), sb.ToString());

            client.Close();

            MessageBox.Show(count.ToString());
        }

        private Rias.Patient GetRiasPatient(Pacient p)
        {
            var patient = new Rias.Patient();

            patient.Familyname = p.LastName;
            patient.Firstname = p.FirstName;
            patient.Middlename = p.MiddleName;

            patient.BirthDate = p.Birthdate;

            patient.BirthPlaceAddress = new Rias.Address() { StringAddress = p.BirthPlace };

            patient.LocationAddress = new Rias.Address();
            if (p.Address != null)
            {
                patient.LocationAddress.OKATO = p.Address.OKATO;
                var city = new[] { p.Address.Level1, p.Address.Level2, p.Address.Level3, p.Address.Level4 }.FirstOrDefault(t => t!=null && t.IsCity == true);
                if (city != null)
                    patient.LocationAddress.City = city.CodeOkato;

                if (p.Address.Street != null)
                    patient.LocationAddress.Street = p.Address.Street.CodeOkato;

                patient.LocationAddress.AddressType = 1;

                patient.LocationAddress.StringAddress = p.Address.ToString();
            }

            patient.IdSex = (int) p.Gender;

            patient.SNILS = p.SNILS;

            var policies = new List<Rias.InsurancePolicy>();
            foreach (var policy in p.Polises)
            {
                policies.Add(new InsurancePolicy()
                {
                    CompanyCode = policy.SMO.Code,
                    Number = policy.Number,
                    Serial = policy.Serial,
                    Isvalid = (policy.DateEnd == null) || (policy.DateEnd > DateTime.Now)
                });
            }

            patient.InsurancePolicy = policies.ToArray();

            var misInfos = new List<Rias.PatientMISInfo>();
            misInfos.Add(new PatientMISInfo()
            {
                IdPatientMIS = p.Oid.ToString()
            });

            patient.MISInfo = misInfos.ToArray();

            return patient;
        }

        private void simpleAction2_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var lv = View as ListView;
            if (lv == null)
                return;

            var tasks = new List<Task>();
            var tf = new TaskFactory();

            var list = lv.SelectedObjects.Cast<Pacient>().ToList().Select(t=>GetRiasPatient(t)).ToList();

            tasks.Add(tf.StartNew(() => SendMessage2(0, list)));
            tasks.Add(tf.StartNew(() => SendMessage2(1, list)));
            tasks.Add(tf.StartNew(() => SendMessage2(2, list)));
            tasks.Add(tf.StartNew(() => SendMessage2(3, list)));
            tasks.Add(tf.StartNew(() => SendMessage2(4, list)));
            tasks.Add(tf.StartNew(() => SendMessage2(5, list)));
        }

        private void SendMessage2(int i, List<Patient> patients)
        {
            var myBinding = new WSHttpBinding(SecurityMode.None);
            myBinding.MessageEncoding = WSMessageEncoding.Text;

            var myEndpoint = new EndpointAddress("http://medframe.burmiac.ru/rias/service.svc");

            Rias.StatServiceClient client = new StatServiceClient(myBinding, myEndpoint);
            var myEndPointBehavior = new MyEndpointBehavior();
            client.Endpoint.Behaviors.Add(myEndPointBehavior);


            //токен доступа к сервису
            Guid guid = Guid.Parse("1d8ea6e0-fc6c-45e8-bfb2-50e7aae255c2");

            //идентиификатор организации
            string idOrg = "1.2.643.5.1.13.3.25.3.32";


            var maxCount = 100;

            client.Open();
            Console.WriteLine("Task: " + i);

            var takenPatients = patients.Take((i + 1) * maxCount).Skip(i * maxCount);

            var sb = new StringBuilder();
            int count = 0;

            foreach (Patient p in takenPatients)
            {
                Console.WriteLine("Добавляем его:");
                try
                {
                    var answer = client.AddPatient(guid, idOrg, p);

                    if (answer.Success)
                    {
                        Console.WriteLine("Успешно добавлено!");
                    }
                    else
                    {
                        Console.WriteLine("Код ошибки:" + answer.Code);
                        Console.WriteLine("Выполнено с ошибками:");
                        foreach (var entityValidationResult in answer.Errors)
                        {

                            Console.WriteLine(entityValidationResult.ErrorMessage);
                        }

                        var existMsg = "Пациент с такими данными существует";

                        if (answer.Errors.Count() == 1 && answer.Errors[0].ErrorMessage.Equals(existMsg))
                        {
                            Console.WriteLine("Ах так, тогда меняем данные пациента!");

                            // Поменяем фамилию
                            p.Familyname += "_ADDED_FUST_FOR_TEST!_";

                            var policies = p.InsurancePolicy.ToList();
                            // Добавим полис с неправильным номером
                            policies.Add(new InsurancePolicy() { CompanyCode = "22001", Isvalid = false, Number = "1234567890123456!" });

                            p.InsurancePolicy = policies.ToArray();

                            client.UpdatePatient(guid, idOrg, p);
                        }

                    }
                    count++;
                }
                catch (Exception ex)
                {

                    sb.AppendLine(ex.Message);
                }



            }

            File.WriteAllText(string.Format("log_thread {0}.txt", i), sb.ToString());

            client.Close();

            MessageBox.Show(count.ToString());
        }
    }

    public class MyEndpointBehavior : IEndpointBehavior
    {
        public void Validate(
            ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(
            ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(
            ServiceEndpoint endpoint,
            EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(
            ServiceEndpoint endpoint,
            ClientRuntime clientRuntime)
        {
            var myClientMessageInspector = new ClientMessageInspector();

            clientRuntime.MessageInspectors.Add(myClientMessageInspector);
        }
    }

    public class ClientMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(
            ref Message request,
            IClientChannel channel)
        {

            using (StreamWriter sw = new StreamWriter("soaplog.txt", true))
            {
                sw.WriteLine(DateTime.Now + " client request");
                sw.WriteLine(request);
                sw.WriteLine("-------------------------------------------------------");

            }

            // If you return something here, it will be available in the 
            // correlationState parameter when AfterReceiveReply is called.
            return null;
        }

        public void AfterReceiveReply(
            ref Message reply,
            object correlationState)
        {
            using (StreamWriter sw = new StreamWriter("soaplog.txt", true))
            {
                sw.WriteLine(DateTime.Now + " server reply");
                sw.WriteLine(reply);
                sw.WriteLine("-------------------------------------------------------");
            }

            // If you returned something in BeforeSendRequest
            // it will be available in the correlationState parameter.
        }
    }
}
