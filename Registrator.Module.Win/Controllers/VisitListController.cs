using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects;
using ListView = DevExpress.ExpressApp.ListView;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class VisitListController : ViewController
    {
        public VisitListController()
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
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void action_get_emergency_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var lv = View as ListView;

            if (lv == null)
                return;
            var visits = ObjectSpace.GetObjects<VisitCase>();

            var sb = new StringBuilder();

            var f = new MonthSelectorForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                var month = f.SelectedDateTime.Month;
                var year = f.SelectedDateTime.Year;

                var report = new Dictionary<int, Dictionary<string, int>>();

                var totalField = "Всего";
                var totalChannelSmp = "Канал: СМП";
                var totalChannelOnmp = "Канал: ОНМП";
                var totalDay = "8:00-20:00";
                var totalNight = "20:00-8:00";
                var totalInBaby = "Возраст (вызов): до 1 года";
                var totalInChild = "Возраст (вызов): с 1 года до 17 лет";
                var totalInAdult = "Возраст (вызов): с 18 лет";

                var totalRefuseBeforeBrigade = "Отказ от вызова до выезда";
                var noanychancestounderstoodthis = "перед на 03 до выезда";

                var totalOutBaby = "Возраст (обслуж.): до 1 года";
                var totalOutChild = "Возраст (обслуж.): с 1 года до 17 лет";
                var totalOutAdult = "Возраст (обслуж.): с 18 лет";

                var totalPlaceStreet = "Место: вне МО";
                var totalPlaceOnmp = "Место: ОНМП";

                var totalResultOnPlace = "Оказана мед.помощь на месте, без трансп.";
                var totalResultTransport = "Оказана мед.помощь, транспор. в МО по профилю";
                var totalResultSendSmp = "Передан в СМП";
                var totalDeath = "Смерть в присутствии бригады НМП";
                var totalNoResult = "Безрезультатный выезд";
                var totalWaitTime = "Время ожидания";
					


                foreach (var visitCase in visits.Where(t => t.DateIn.Month == month && t.DateIn.Year == year))
                {
                    if (!report.ContainsKey(visitCase.DateIn.Day))
                    {
                        report.Add(visitCase.DateIn.Day, new Dictionary<string, int>());

                        report[visitCase.DateIn.Day].Add(totalField, 0);

                        report[visitCase.DateIn.Day].Add(totalChannelSmp, 0);
                        report[visitCase.DateIn.Day].Add(totalChannelOnmp, 0);

                        report[visitCase.DateIn.Day].Add(totalDay, 0);
                        report[visitCase.DateIn.Day].Add(totalNight, 0);

                        report[visitCase.DateIn.Day].Add(totalInBaby, 0);
                        report[visitCase.DateIn.Day].Add(totalInChild, 0);
                        report[visitCase.DateIn.Day].Add(totalInAdult, 0);

                        report[visitCase.DateIn.Day].Add(totalRefuseBeforeBrigade, 0);
                        report[visitCase.DateIn.Day].Add(noanychancestounderstoodthis, 0);

                        report[visitCase.DateIn.Day].Add(totalOutBaby, 0);
                        report[visitCase.DateIn.Day].Add(totalOutChild, 0);
                        report[visitCase.DateIn.Day].Add(totalOutAdult, 0);

                        report[visitCase.DateIn.Day].Add(totalPlaceStreet, 0);
                        report[visitCase.DateIn.Day].Add(totalPlaceOnmp, 0);

                        report[visitCase.DateIn.Day].Add(totalResultOnPlace, 0);
                        report[visitCase.DateIn.Day].Add(totalResultTransport, 0);
                        report[visitCase.DateIn.Day].Add(totalResultSendSmp, 0);
                        report[visitCase.DateIn.Day].Add(totalDeath, 0);
                        report[visitCase.DateIn.Day].Add(totalNoResult, 0);
                        report[visitCase.DateIn.Day].Add(totalWaitTime, 0);
                    }

                    

                    if (visitCase.Services != null && visitCase.Services.Count != 0)
                    {
                        if (visitCase.Services[0].Usluga != null && visitCase.Services[0].Usluga.Name == "Неотложная мед. помощь")
                        {
                            if (visitCase.Services[0].EditableProtocol.Records != null &&
                                visitCase.Services[0].EditableProtocol.Records.Count > 0)
                            {
                                report[visitCase.DateIn.Day][totalField]++;
                                var time = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "06. Вызов принят");
                                if (time != null)
                                {
                                    var m = Regex.Match(time.Value, @"^(\d+):\d+");
                                    if (m.Success)
                                    {
                                        var hour = int.Parse(m.Groups[1].Value);
                                        if (hour > 7 && hour < 20)
                                        {
                                            report[visitCase.DateIn.Day][totalDay]++;
                                        }
                                        else
                                        {
                                            report[visitCase.DateIn.Day][totalNight]++;
                                        }
                                    }
                                }

                                var is03 = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "03. Передал с \"03\" (код)");
                                if (is03 != null)
                                {
                                    if (!string.IsNullOrEmpty(is03.Value))
                                    {
                                        report[visitCase.DateIn.Day][totalChannelSmp]++;
                                    }
                                    else
                                    {
                                        report[visitCase.DateIn.Day][totalChannelOnmp]++;
                                    }
                                }

                                var age = visitCase.Pacient.GetAge(visitCase.DateIn);
                                if (age < 1)
                                {
                                    report[visitCase.DateIn.Day][totalInBaby]++;
                                    report[visitCase.DateIn.Day][totalOutBaby]++;
                                }
                                else
                                {
                                    if (age < 18)
                                    {
                                        report[visitCase.DateIn.Day][totalInChild]++;
                                        report[visitCase.DateIn.Day][totalOutChild]++;
                                    }
                                    else
                                    {
                                        report[visitCase.DateIn.Day][totalInAdult]++;
                                        report[visitCase.DateIn.Day][totalOutAdult]++;
                                    }
                                }

                                var isTrans = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "11. Прибытие в медучреждение");
                                if (isTrans != null)
                                {
                                    if (!string.IsNullOrEmpty(isTrans.Value))
                                    {
                                        
                                        report[visitCase.DateIn.Day][totalPlaceOnmp]++;
                                    }
                                    else
                                    {
                                        report[visitCase.DateIn.Day][totalPlaceStreet]++;
                                    }
                                }

                                var result = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "16. Результаты выезда");
                                if (result != null)
                                {
                                    if (!string.IsNullOrEmpty(result.Value))
                                    {
                                        if (result.Value.StartsWith("Оказана"))
                                        {
                                            report[visitCase.DateIn.Day][totalResultOnPlace]++;
                                        }
                                        if (result.Value.StartsWith("Доставлен"))
                                        {
                                            report[visitCase.DateIn.Day][totalResultTransport]++;
                                        }
                                        if (result.Value.StartsWith("Передан"))
                                        {
                                            report[visitCase.DateIn.Day][totalResultSendSmp]++;
                                        }
                                        if (result.Value.StartsWith("Смерть"))
                                        {
                                            report[visitCase.DateIn.Day][totalDeath]++;
                                        }
                                    }
                                }

                                var noResult = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "17. Безрезультатный  выезд (см. п. 35)");
                                if (noResult != null)
                                {
                                    if (!string.IsNullOrEmpty(noResult.Value))
                                    {
                                        report[visitCase.DateIn.Day][totalNoResult]++;
                                    }
                                }

                                var timeCall = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "06. Вызов принят");
                                var timeArrive = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "09. Прибытие на место вызова");
                                if (timeCall != null && timeArrive != null)
                                {
                                    try
                                    {
                                        var t1 = DateTime.Parse(timeCall.Value);
                                        var t2 = DateTime.Parse(timeArrive.Value);
                                        var waitMinutes = (t2 - t1).TotalMinutes;
                                        if (waitMinutes > 0)
                                            report[visitCase.DateIn.Day][totalWaitTime] += (int)waitMinutes;

                                    }
                                    catch (Exception)
                                    {
                                        
                                    }
                                    
                                }


                                /*var records = visitCase.Services[0].EditableProtocol.Records;
                                records.Sorting.Add(new SortProperty("Type.Name", DevExpress.Xpo.DB.SortingDirection.Ascending));
                                foreach (var protocolRecord in records)
                                {
                                    sb.Append(string.Format("{0}:{1}\t", protocolRecord.Type.Name, string.IsNullOrEmpty(protocolRecord.Value) ? string.Empty : protocolRecord.Value.Trim()));
                                }
                                sb.Append(Environment.NewLine);*/

                                /*var date = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "01. Дата оказания неотложной мед. помощи");
                                var time = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "06. Вызов принят");
                                var result = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "16. Результаты выезда");
                                var is03 = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "03. Передал с \"03\" (код)");
                                if (date != null && time != null && result != null)
                                {
                                    sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", visitCase.Oid, date.Value, time.Value, result.Value, is03.Value));
                                }*/
                            }
                        }
                    }
                }

                sb.AppendFormat("{0}\t", "Day");
                foreach (var pair2 in report[1])
                {
                    sb.AppendFormat("{0}\t", pair2.Key);
                }
                sb.AppendLine();

                foreach (var pair in report)
                {
                    sb.AppendFormat("{0}\t", pair.Key);
                    foreach (var pair2 in pair.Value)
                    {
                        sb.AppendFormat("{0}\t", pair2.Value);
                    }
                    sb.AppendLine();
                }

                File.WriteAllText("report.csv", sb.ToString());
            }
        }
    }
}
