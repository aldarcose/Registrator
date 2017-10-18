using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xml2xml
{
    public static class ConverterApril
    {
        private static Counter counterNewUsl = new Counter(0);
        private static Counter counterDelUsl = new Counter(0);

        public static void FixDispanser01042015(XDocument docD, string folderIn)
        {
            if (docD.Root != null)
            {
                var listProf2 = new ListProf(folderIn + "bez_prof.xml");
                var listDop = codifiers_dop_disp_measures_tab1.GetList(folderIn + "codifiers_dop_disp_measures_tab1.xml");

                foreach (var zap in docD.Root.Elements("ZAP"))
                {
                    string numpol = Converter.GetNumPol(zap);

                    foreach (var sluch in zap.Elements("SLUCH"))
                    {
                        //в файле DM услуги с кодом 0631.. 0632.. у них если начало позже или = 01.04.2015 Только для них мы делаем:
                        var xdate1 = sluch.Element("DATE_1");
                        if (xdate1 != null)
                        {
                            DateTime date = Converter.GetDate(xdate1.Value);
                            if (date >= new DateTime(2015, 02, 01))
                            {
                                RemoveDopUsl061(sluch);
                                // 1) убираем все доп услуги 061

                                var usl = sluch.Elements("USL")
                                    .FirstOrDefault(X => X.Element("CODE_USL") != null &&
                                                         (X.Element("CODE_USL").Value.StartsWith("0631") ||
                                                          (X.Element("CODE_USL").Value.StartsWith("0632"))));
                                //main usl
                                if (usl != null)
                                {
                                    bool isRemoved = false;
                                    var xCodeUsl = usl.Element("CODE_USL");
                                    if (xCodeUsl != null)
                                    {
                                        var profs = listProf2.GetProfs(numpol, xCodeUsl.Value);

                                        if (!profs.Any())
                                        {
                                            Messenger.WriteMessage(
                                                String.Format(
                                                    "Совокупность полиса '{0}' и услуги '{1}' не нашлась в списке безпроф.",
                                                    numpol, xCodeUsl.Value));
                                        }

                                        //add dop uslugi na osnove bezprof.xml
                                        AddDopUslFromBezProf(usl, profs, folderIn);


                                        DateTime dateIn = Converter.GetDate(usl.Element("DATE_IN").Value);
                                        DateTime dateOut = Converter.GetDate(usl.Element("DATE_OUT").Value);
                                        decimal percent = CalculatePercentOfProfsInPeriod(profs, dateIn, dateOut);

                                        //3) ДЛЯ случаев где главная услуга 0631... и 0632... если % выполненных мероприятий в течении срока прохождения дд составляет < 85% то убирать главную услугу 0631.... или 0632...
                                        if (percent*100 < 85)
                                        {
                                            usl.Remove();
                                            isRemoved = true;
                                        }
                                    }
                                    //.для всех услуг, которые начинаются с 0630:
                                    //- ставим тариф 0,00 - если НЕ убираем главную услугу 0631 или 0632
                                    ////- ставим тариф из Tarif - если Убираем главную услугу 0631 или 0632
                                    //string tarif = "0.00";
                                    //if (!isRemoved)
                                    //{
                                    //    tarif = usl.Element("TARIF").Value;
                                    //}

                                    foreach (var usl1 in sluch.Elements("USL").Where(X => X.Element("CODE_USL") != null
                                                                                          &&
                                                                                          X.Element("CODE_USL")
                                                                                              .Value.StartsWith("0630"))
                                        )
                                    {
                                        if (!isRemoved)
                                        {
                                            usl1.SetElementValue("TARIF", "0.00");
                                        }
                                        else
                                        {
                                            var tarif = codifiers_dop_disp_measures_tab1.GetTarif(listDop,
                                                usl1.Element("CODE_USL") != null
                                                    ? usl1.Element("CODE_USL").Value
                                                    : null);
                                            if (tarif == null) { throw new Exception("codeusl is null");}
                                            usl1.SetElementValue("TARIF", tarif);
                                            usl1.SetElementValue("SUMV_USL", tarif);
                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Messenger.WriteMessage(String.Format("Добавлено {0} новых доп.услуг", counterNewUsl.GetCurrent()));
                Messenger.WriteMessage(String.Format("Удалено {0} услуг с кодом 061ххх", counterDelUsl.GetCurrent()));
            }
        }


        private static void AddDopUslFromBezProf(XElement usl, IEnumerable<Prof> profs, string folderIn) {
            if (usl == null)
            {
                return;
            }
            var sluch = usl.Parent;
            if (sluch == null)
            {
                return;
            }

            int counter = 0;

            var listDop = codifiers_dop_disp_measures_tab1.GetList(folderIn + "codifiers_dop_disp_measures_tab1.xml");
            foreach (var prof in profs)
            {
                var xCodeUsl = usl.Element("CODE_USL");
                if (xCodeUsl != null)
                {
                    var newUsl = new XElement(usl);
                    var id_serv = newUsl.Element("IDSERV");
                    if (id_serv != null)
                    {
                        id_serv.Value += (++counter).ToString(); //просто прибавить счетчик
                        //<IDSERV> получается в случаях одинаковые, а должны быть разными, в одном случае повторяться не должны
                    }
                    newUsl.SetElementValue("SUMV_USL", "0");
                    //1) <SUMV_USL> надо в дополнительных услугах приравнять к нулю

                    var t = codifiers_dop_disp_measures_tab1.Get(listDop, prof.dop_disp_measure_id);
                    if (t == null)
                    {
                        //Messenger.WriteMessage(String.Format("Id {0} не найден в codifiers_dop_disp_measures_tab1. Либо доп.услуга не добавлена, либо не проставлены коды", prof.dop_disp_measure_id));
                        continue;
                    }

                    newUsl.SetElementValue("CODE_USL", t.code);

                    newUsl.SetElementValue("TARIF", "0.00");


                    //расставляем дополнительным услугам врачей, даты и xprvs
                    var newCodeUsl = newUsl.Element("CODE_USL");
                    if (newCodeUsl != null)
                    {
                        string prvs = newUsl.Element("PRVS").Value;
                        string code_md = newUsl.Element("CODE_MD").Value;
                        string date_in = newUsl.Element("DATE_IN").Value;
                        string date_out = newUsl.Element("DATE_OUT").Value;

                        string date = null;

                        if (prof != null)
                        {
                            if (prof.Xprvs != "null")
                            {
                                prvs = prof.Xprvs;
                            }
                            if (prof.Doctor_Id != "null")
                            {
                                code_md = prof.Doctor_Id;
                            }
                            if (prof.MeasureDate != "null")
                            {
                                date = prof.MeasureDate;
                            }
                        }
                        if (!String.IsNullOrWhiteSpace(date) && date.Length == 10)
                        {
                            date = String.Format("{0}-{1}-{2}",
                                date.Substring(6, 4),
                                date.Substring(3, 2),
                                date.Substring(0, 2)); //формат гггг-мм-дд
                            date_in = date.Replace(".", "-");
                            date_out = date.Replace(".", "-");
                        }

                        newUsl.Element("PRVS").Value = prvs;
                        newUsl.Element("CODE_MD").Value = code_md;
                        newUsl.Element("DATE_IN").Value = date_in;
                        newUsl.Element("DATE_OUT").Value = date_out;
                    }
                    counterNewUsl.AddNext();

                    sluch.LastNode.AddBeforeSelf(newUsl);
                }
            }
        }
        
        private static decimal CalculatePercentOfProfsInPeriod(IEnumerable<Prof> profs, DateTime dateIn, DateTime dateOut)
        {
            decimal sum = profs.Count();

            if (sum == 0)
            {
                return 1;
            }

            decimal counter = 0;
            foreach (var prof in profs)
            {
                DateTime date;
                if (DateTime.TryParse(prof.MeasureDate, out date))
                {
                    if (date >= dateIn.Date && date < dateOut.AddDays(1).Date)
                    {
                        counter++;
                    }
                }
                else
                {
                    Messenger.WriteMessage(String.Format("Неверный формат даты {0}. Номер полиса: {1}", prof.MeasureDate, prof.NumPol));
                }
            }
            return counter / sum;
        }
        
        private static void RemoveDopUsl061(XElement sluch)
        {
            if (sluch.Elements("USL")
                .Any(X =>
                    (X.Element("CODE_USL") != null &&
                     (X.Element("CODE_USL").Value.StartsWith("0631") ||
                      (X.Element("CODE_USL").Value.StartsWith("0632") ||
                      (X.Element("CODE_USL").Value.StartsWith("0634")))))))
            {

                var elements = sluch.Elements("USL")
                    .Where(
                        X => X.Element("CODE_USL") != null &&
                             X.Element("CODE_USL").Value.StartsWith("061")
                    );

                counterDelUsl.Add(elements.Count()); // add counter for deleted objects
                elements.Remove(); // 1) убираем все доп услуги 061
            }
        }


    }
}
