using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace TFOmsServices
{
    public class FormWebBrowser
    {
        private static FormWebBrowser _instance;
        private PacientInfo _pacient;
        private List<PolicyInfo> _polices;
        private Form _form;
        private WebBrowser _webBrowser;
        private Timer _timer;
        private static volatile bool Completed;
        public string ErrorMessage { get; set; }
        public static List<PolicyInfo> Start(PacientInfo pacient)
        {
            Completed = false;
            _instance = _instance ?? new FormWebBrowser();
            _instance.Initialiaze(pacient);
            _instance.Navigate();
            do
            {
                Thread.Sleep(100);
            } while (!Completed);
            return _instance._polices;
        }

        public static string GetErrorMessage()
        {
            return _instance == null ? string.Empty : _instance.ErrorMessage;
        }

        protected FormWebBrowser()
        {
            
        }

        public void Initialiaze(PacientInfo patient)
        {
            _pacient = patient;
            _polices = new List<PolicyInfo>();
            ErrorMessage = string.Empty;

            _timer = new Timer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = 200;
        }
        
        public void Navigate()
        {
            Thread thread = new System.Threading.Thread(ThreadStart);
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
        }

        private void ThreadStart()
        {
            _webBrowser = new WebBrowser();
            _webBrowser.Dock = DockStyle.Fill;
            _webBrowser.Name = "webBrowser";
            _webBrowser.ScrollBarsEnabled = false;
            _webBrowser.TabIndex = 0;
            _webBrowser.DocumentCompleted += WebBrowserOnDocumentCompleted;
            _webBrowser.ScriptErrorsSuppressed = false;
            _webBrowser.Url = new Uri("http://192.168.16.203:81/checkpolicy/");

            _form = new Form();
            _form.WindowState = FormWindowState.Minimized;
            _form.Controls.Add(_webBrowser);
            _form.Name = "Browser";
            _form.Visible = false;
            _form.Hide();
            _form.Closing += _form_Closing;
            Application.Run(_form);
        }

        void _form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Completed = true;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (_webBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                try
                {
                    GetResults();
                }
                catch (Exception)
                {
                    
                }

                _timer.Stop();
                
                _form.Close();
            }
        }

        private void GetResults()
        {
            var idResult = "ResultLabel";
            var result = _webBrowser.Document.GetElementById(idResult);
            var trResults = result.GetElementsByTagName("tr");
            if (trResults.Count > 0)
            {
                foreach (HtmlElement trResult in trResults)
                {
                    var tds = trResult.GetElementsByTagName("td");
                    if (tds.Count > 0)
                    {
                        var serialNnumber = tds[0].InnerText;
                        var datebeg = tds[1].InnerText;
                        var dateend = tds[2].InnerText;
                        var smo = tds[3].InnerText;

                        if (serialNnumber != null)
                            serialNnumber = serialNnumber.Trim();
                        if (datebeg != null)
                            datebeg = datebeg.Trim();
                        if (dateend != null)
                            dateend = dateend.Trim();
                        if (smo != null)
                            smo = smo.Trim();

                        var spaceIndex = serialNnumber.IndexOf(" ", StringComparison.Ordinal);

                        var serial = string.Empty;
                        var number = serialNnumber;
                        if (spaceIndex != -1)
                        {
                            serial = serialNnumber.Substring(0, spaceIndex);
                            number = serialNnumber.Substring(spaceIndex + 1);
                        }

                        var format = "dd.MM.yyyy";
                        var polis = new PolicyInfo()
                        {
                            Serial = serial,
                            Number = number,
                            DateBeg = DateTime.ParseExact(datebeg, format, CultureInfo.InvariantCulture),
                            DateEnd =
                                string.IsNullOrEmpty(dateend)
                                    ? null
                                    : (DateTime?) DateTime.ParseExact(dateend, format, CultureInfo.InvariantCulture),
                            SMO = smo
                        };
                        _polices.Add(polis);
                    }
                }
            }
            else
            {
                var serviceError = result.InnerText;
                if (string.IsNullOrEmpty(serviceError))
                    ErrorMessage = "Не получен ответ от сервиса.";
                else
                {
                    ErrorMessage = serviceError;
                }
            }
        }

        private void WebBrowserOnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs webBrowserDocumentCompletedEventArgs)
        {
            var webBrowser = sender as WebBrowser;
            var idFirstName = "FirstNameTextBox";
            var idLastName = "LastNameTextBox";
            var idMiddleName = "FatherNameTextBox";
            var idBirthDate = "BirthdayTextBox";

            try
            {
                webBrowser.Document.GetElementById(idLastName).InnerText = _pacient.LastName;
                webBrowser.Document.GetElementById(idFirstName).InnerText = _pacient.FirstName;
                webBrowser.Document.GetElementById(idMiddleName).InnerText = _pacient.MiddleName;
                webBrowser.Document.GetElementById(idBirthDate).InnerText = _pacient.BirthDate.ToString("dd.MM.yyyy");

                var btn =
                    webBrowser.Document.GetElementsByTagName("input")
                        .Cast<HtmlElement>()
                        .FirstOrDefault(t => t.GetAttribute("id").Equals("CheckPolicyButton"));

                if (btn != null)
                {
                    btn.InvokeMember("Click");
                    _timer.Start();
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Сервис не доступен! Проверьте имеется ли подключение к интернету.";
                _form.Close();
            }
            
            
        }
    }
}
