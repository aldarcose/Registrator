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

                var totalField = "�����";
                var totalChannelSmp = "�����: ���";
                var totalChannelOnmp = "�����: ����";
                var totalDay = "8:00-20:00";
                var totalNight = "20:00-8:00";
                var totalInBaby = "������� (�����): �� 1 ����";
                var totalInChild = "������� (�����): � 1 ���� �� 17 ���";
                var totalInAdult = "������� (�����): � 18 ���";

                var totalRefuseBeforeBrigade = "����� �� ������ �� ������";
                var noanychancestounderstoodthis = "����� �� 03 �� ������";

                var totalOutBaby = "������� (������.): �� 1 ����";
                var totalOutChild = "������� (������.): � 1 ���� �� 17 ���";
                var totalOutAdult = "������� (������.): � 18 ���";

                var totalPlaceStreet = "�����: ��� ��";
                var totalPlaceOnmp = "�����: ����";

                var totalResultOnPlace = "������� ���.������ �� �����, ��� ������.";
                var totalResultTransport = "������� ���.������, ��������. � �� �� �������";
                var totalResultSendSmp = "������� � ���";
                var totalDeath = "������ � ����������� ������� ���";
                var totalNoResult = "��������������� �����";
                var totalWaitTime = "����� ��������";
					


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
                        if (visitCase.Services[0].Usluga != null && visitCase.Services[0].Usluga.Name == "���������� ���. ������")
                        {
                            if (visitCase.Services[0].EditableProtocol.Records != null &&
                                visitCase.Services[0].EditableProtocol.Records.Count > 0)
                            {
                                report[visitCase.DateIn.Day][totalField]++;
                                var time = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "06. ����� ������");
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

                                var is03 = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "03. ������� � \"03\" (���)");
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

                                var isTrans = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "11. �������� � �������������");
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

                                var result = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "16. ���������� ������");
                                if (result != null)
                                {
                                    if (!string.IsNullOrEmpty(result.Value))
                                    {
                                        if (result.Value.StartsWith("�������"))
                                        {
                                            report[visitCase.DateIn.Day][totalResultOnPlace]++;
                                        }
                                        if (result.Value.StartsWith("���������"))
                                        {
                                            report[visitCase.DateIn.Day][totalResultTransport]++;
                                        }
                                        if (result.Value.StartsWith("�������"))
                                        {
                                            report[visitCase.DateIn.Day][totalResultSendSmp]++;
                                        }
                                        if (result.Value.StartsWith("������"))
                                        {
                                            report[visitCase.DateIn.Day][totalDeath]++;
                                        }
                                    }
                                }

                                var noResult = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "17. ���������������  ����� (��. �. 35)");
                                if (noResult != null)
                                {
                                    if (!string.IsNullOrEmpty(noResult.Value))
                                    {
                                        report[visitCase.DateIn.Day][totalNoResult]++;
                                    }
                                }

                                var timeCall = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "06. ����� ������");
                                var timeArrive = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "09. �������� �� ����� ������");
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

                                /*var date = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "01. ���� �������� ���������� ���. ������");
                                var time = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "06. ����� ������");
                                var result = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "16. ���������� ������");
                                var is03 = visitCase.Services[0].EditableProtocol.Records.FirstOrDefault(t => t.Type.Name == "03. ������� � \"03\" (���)");
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
