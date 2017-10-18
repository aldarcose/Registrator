using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;

namespace xml2xml
{
    public static class Converter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        internal static void ConvertXML(string folderIn, string folderOut, bool moveChilds = false, bool moveAll=false)
        {
            string[] files = Directory.GetFiles(folderIn);

            string fileD = files.FirstOrDefault(X => Path.GetFileName(X).Substring(0, 2) == "DM");
            string fileH = files.FirstOrDefault(X => Path.GetFileName(X).Substring(0, 2) == "HM");

            var docD = XDocument.Load(fileD);
            var docH = XDocument.Load(fileH);
            Messenger.WriteMessage("Исправляем Диспансеризацию");
            FixDispanser(docD, folderIn);

            Messenger.WriteMessage("Исправляем Диспансеризацию для тех, кто прошел после 01.04.2015");
            ConverterApril.FixDispanser01042015(docD, folderIn);

            Messenger.WriteMessage("Исправляем СНИЛСы врачей в D-файле");
            FixSnilsInDFile(docD, folderIn);
            Messenger.WriteMessage("Исправляем СНИЛСы врачей в H-файле");
            FixSinlsInHFile(docH, folderIn);
            Messenger.WriteMessage("Чиним IDSP в HM");
            Converter.FixIdsp(docH);
            if (moveChilds)
            {
                Messenger.WriteMessage("Переносим диспансеризацию детей");
                MoveChildsFromDToH(docD, docH);//, folderOut + Path.GetFileName(fileD), folderOut + Path.GetFileName(fileH));
            }
            if (moveAll)
            {
                Messenger.WriteMessage("Переносим диспансеризацию всех");
                MoveAllFromDToH(docD, docH);//, folderOut + Path.GetFileName(fileD), folderOut + Path.GetFileName(fileH));
            }

            if (moveAll || moveChilds)
            {
                Messenger.WriteMessage("Исправляем все перенесенные записи в H-файле");
                FixHFile3(docH);
            }

            if (!moveAll && !moveChilds)
            {
                Messenger.WriteMessage("Исправляем в DM файле (когда не переносим в HM):");
                FixDispanserIfNotMove(docD);
            }
            Messenger.WriteMessage("Исправляем h-файл");
            FixhFile2(docH, folderIn);

            Messenger.WriteMessage("Пересчитываем цены услуг");
            FixUslPrices(docD, folderIn);
            FixUslPrices(docH, folderIn);

            Messenger.WriteMessage("Пересчитываем убщую сумму");
            CountSum(docD);
            CountSum(docH);

            Messenger.WriteMessage("Чиним PRVS в DM");
            PRVS.RepairPRVSInDoc(docD, folderIn);
            Messenger.WriteMessage("Чиним PRVS в HM");
            PRVS.RepairPRVSInDoc(docH, folderIn);

            Messenger.WriteMessage("Чиним IDSERV в HM");
            FixIdServ(docH);
            Messenger.WriteMessage("Чиним IDSERV в DM");
            FixIdServ(docD);

            Messenger.WriteMessage("Чиним CODE_USL в HM");
            FixCodeUsl(docH);
            Messenger.WriteMessage("Чиним CODE_USL в DM");
            FixCodeUsl(docD);

            Messenger.WriteMessage("Сохраняемся в папку");

            var dOut = folderOut + Path.GetFileName(fileD);
            var hOut = folderOut + Path.GetFileName(fileH);

            docD.Save(dOut, SaveOptions.None);
            docH.Save(hOut, SaveOptions.None);

            Messenger.WriteMessage("Завершено");
        }

        private static void FixIdServ(XDocument xml)
        {
            foreach (var sluch in xml.Descendants("SLUCH").Where(t => t.Elements("USL").Count() > 1))
            {
                var index = 1;
                var dictionary = new Dictionary<string, int>();
                foreach (var usl in sluch.Elements("USL"))
                {
                    usl.Element("IDSERV").Value = (index++).ToString();
                }
            }
        }

        private static void FixCodeUsl(XDocument xml)
        {
            foreach (var sluch in xml.Descendants("SLUCH"))
            {
                foreach (var usl in sluch.Elements("USL"))
                {
                    var code = usl.Element("CODE_USL").Value;
                    if (code.Length == 5)
                    {
                        usl.Element("CODE_USL").Value = string.Format("0{0}", code);
                    }
                }
            }
        }

        public static string GetNumPol(XElement zap)
        {
            string numpol="";
            if (zap != null)
            {
                var pacient = zap.Element("PACIENT");
                if (pacient != null)
                {
                    var npolis = pacient.Element("NPOLIS");
                    if (npolis != null) numpol = npolis.Value;
                }
            }
            return numpol;
        }

        /// <summary>
        /// Преобразует строку реестра в формат даты
        /// </summary>
        /// <param name="date">Строка даты в формате 2015-02-04</param>
        /// <returns></returns>
        public static DateTime GetDate(string date)
        {
            if (date.Length == 10)
            {
                int year, month, day;
                                                //{
                                                //    date = String.Format("{0}-{1}-{2}",
                                                //        date.Substring(6, 4),
                                                //        date.Substring(3, 2),
                                                //        date.Substring(0, 2)); //формат гггг-мм-дд
                                                //    date_in = date.Replace(".", "-");
                                                //    date_out = date.Replace(".", "-");
                                                //}
                if (Int32.TryParse(date.Substring(0, 4), out year) &&
                    Int32.TryParse(date.Substring(5, 2), out month) &&
                    Int32.TryParse(date.Substring(8, 2), out day))
                {
                    return new DateTime(year, month, day);
                }
            }
            throw new Exception("Неверный формат даты: " + date);
        }

        private static void FixhFile2(XDocument docH, string folderIn)
        {
            if (docH.Root != null)
            {
                foreach (var zap in docH.Root.Elements("ZAP"))
                {
                    foreach (var sluch in zap.Elements("SLUCH"))
                    {
                        var nrp_mo = sluch.Element("NPR_MO");
                        if (nrp_mo != null && String.IsNullOrWhiteSpace(nrp_mo.Value))
                        {
                            //            1) убрать <NPR_MO></NPR_MO> (пустые)
                            nrp_mo.Remove();
                        }
                        var CODE_MES1 = sluch.Element("CODE_MES1");
                        if (CODE_MES1 != null && String.IsNullOrWhiteSpace(CODE_MES1.Value))
                        {
                            //2) убрать <CODE_MES1></CODE_MES1> (пустые)
                            CODE_MES1.Remove();
                        }
                        var VIDPOM = sluch.Element("VIDPOM");
                        if (VIDPOM != null)
                        {
                            if (VIDPOM.Value == "2")
                            {
                                VIDPOM.Value = "3";
                            }
                            //3) заменить <VIDPOM>2</VIDPOM> на <VIDPOM>3</VIDPOM>
                        }
                        var FOR_POM = sluch.Element("FOR_POM");
                        if (FOR_POM != null)
                        {
                            if (FOR_POM.Value == "1")
                            {
                                FOR_POM.Value = "3";
                            }
                            //4) заменить <FOR_POM>1</FOR_POM> на <FOR_POM>3</FOR_POM>
                        }
                        var VERS_SPEC = sluch.Element("VERS_SPEC");
                        if (VERS_SPEC == null)
                        {
                            var PRVS = sluch.Element("PRVS");
                            if (PRVS != null)
                            {
                                VERS_SPEC = new XElement("VERS_SPEC", "V015");
                                PRVS.AddAfterSelf(VERS_SPEC);
                            }
                        }
                        //5) После </PRVS> перед <IDDOKT> поставить <VERS_SPEC>V015</VERS_SPEC>
                        foreach (var usl in sluch.Elements("USL"))
                        {
                            var code_usl = usl.Element("CODE_USL");
                            if (code_usl != null)
                            {
                                if (code_usl.Value == "020025") { code_usl.Value = "003015"; }
                                if (code_usl.Value == "020026") { code_usl.Value = "003014"; }
                                if (code_usl.Value == "020024") { code_usl.Value = "003013"; }
                                if (code_usl.Value == "020034") { code_usl.Value = "003033"; }
                                if (code_usl.Value == "020030") { code_usl.Value = "003024"; }
                                if (code_usl.Value == "020012") { code_usl.Value = "003020"; }
                                if (code_usl.Value == "020033") { code_usl.Value = "003023"; }

                                //6) заменить <CODE_USL>020025</CODE_USL> на <CODE_USL>003015</CODE_USL>
                                //<CODE_USL>020026</CODE_USL> на <CODE_USL>003014</CODE_USL>
                                //<CODE_USL>020024</CODE_USL> на <CODE_USL>003013</CODE_USL>
                                //<CODE_USL>020034</CODE_USL> на <CODE_USL>003033</CODE_USL>
                                //<CODE_USL>020030</CODE_USL> на <CODE_USL>003024</CODE_USL>
                                //<CODE_USL>020012</CODE_USL> на <CODE_USL>003020</CODE_USL>
                                //<CODE_USL>020033</CODE_USL> на <CODE_USL>003023</CODE_USL>
                            }
                        }
                    }
                }
            }
        }

        private static void FixDispanserIfNotMove(XDocument docH)
        {
            if (docH.Root != null)
            {
                var schet = docH.Root.Element("SCHET");

                if (schet.Element("DISP") != null)
                {
                    schet.Element("DISP").Remove();
                    //убрать <DISP></DISP>
                }

                foreach (var zap in docH.Root.Elements("ZAP"))
                {
                    foreach (var sluch in zap.Elements("SLUCH"))
                    {
                        var vidpom = sluch.Element("VIDPOM");
                        if (vidpom != null)
                        {
                            if (vidpom.Value == "2")
                            {
                                vidpom.Value = "13";
                            }
                            if (vidpom.Value == "1")
                            {
                                vidpom.Value = "12";
                            }
                            //заменить <VIDPOM>2</VIDPOM> на <VIDPOM>13</VIDPOM> <VIDPOM>1</VIDPOM> на <VIDPOM>12</VIDPOM>
                        }
                        foreach (var usl in sluch.Elements("USL"))
                        {
                            if (usl.Element("KOL_USL") != null)
                            {
                                usl.Element("KOL_USL").Remove();
                                //убрать <KOL_USL>1</KOL_USL>
                            }
                        }
                    }
                }
            }
        }

        private static void FixHFile3(XDocument docH)
        {
            if (docH.Root != null)
            {
                foreach (var zap in docH.Root.Elements("ZAP"))
                {
                    var pacient = zap.Element("PACIENT");
                    AddNovorInPacient(pacient);
                    foreach (var sluch in zap.Elements("SLUCH"))
                    {
                        //foreach (var usl in sluch.Elements("USL"))
                        //{

                        //}
                        AddUslokInSluch(sluch);
                        AddForPomAfterVidpom(sluch);
                        AddElementAfterRsltD(sluch);
                    }
                }
            }
        }
        /// <summary>
        /// Пересчитываем услуги и тарифы
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="folderIn"></param>
        private static void FixUslPrices(XDocument doc, string folderIn)
        {
            var ListPrice = new ListPrice(folderIn + "PRICE.csv");
            int counter = 0;
            if (doc.Root != null)
            {
                foreach (var zap in doc.Root.Elements("ZAP"))
                {
                    foreach (var sluch in zap.Elements("SLUCH"))
                    {
                        decimal tarif = 0;
                        decimal sumv = 0;
                        foreach (var usl in sluch.Elements("USL"))
                        {
                            var uslTarif = Convert.ToDecimal(usl.Element("TARIF").Value.Replace(".", ","));
                            var uslSumv = Convert.ToDecimal(usl.Element("SUMV_USL").Value.Replace(".", ","));
                            var xCodeUsl = usl.Element("CODE_USL");
                            if (xCodeUsl != null && ListPrice.HasUsl(xCodeUsl.Value))
                            {
                                // надо в тег TARIF, SUMV_USL в блоке USL указать стоимость которая прописана в файле
                                uslTarif = Convert.ToDecimal(ListPrice.GetPrice(xCodeUsl.Value.Replace(".", ",")));
                                uslSumv = Convert.ToDecimal(ListPrice.GetPrice(xCodeUsl.Value.Replace(".", ",")));
                                
                                counter++;
                            }
                            usl.Element("TARIF").Value = uslTarif.ToString("#0.00").Replace(",", ".");
                            usl.Element("SUMV_USL").Value = uslSumv.ToString("#0.00").Replace(",", ".");

                            tarif += uslTarif;
                            sumv += uslSumv;
                        }
                        sluch.Element("TARIF").Value = tarif.ToString("#0.00").Replace(",", ".");
                        sluch.Element("SUMV").Value = sumv.ToString("#0.00").Replace(",", ".");
                        //также потом в блоке SLUCH теги TARIF, SUMV будут пересчитываться в тариф упадет сумма всех тарифов услуг данного случая, а сумма - сумма всех сумм услуг данного случая.
                    }
                }
            }
            Messenger.WriteMessage(String.Format("Задействовано {0} элементов", counter));
        }
        
        public static XDocument FixDispanser(XDocument doc, string folderIn)
        {
            ListUSL ListUsl = new ListUSL(folderIn + "USL.csv");
            ListProf listProf1 = new ListProf(folderIn + "prof.xml");
            ListProf listProf2 = new ListProf(folderIn + "bez_prof.xml");
            int counter1 = 0;
            int countDel = 0;


            if (doc.Root != null)
            {
                foreach (var zap in doc.Root.Elements("ZAP"))
                {
                    string numpol = GetNumPol(zap);

                    foreach (var sluch in zap.Elements("SLUCH"))
                    {
                        //Развернуть все услуги по диспансеризации
                        if (sluch.Elements("USL").Any())
                        {
                            var usl =
                                sluch.Elements("USL")
                                    .FirstOrDefault(
                                        X =>
                                            X.Element("CODE_USL") != null
                                                ? ListUsl.HasCode(X.Element("CODE_USL").Value)
                                                : false); //Если какая либо услуга в списке

                            if (usl != null)
                            {
                                var xCodeUsl = usl.Element("CODE_USL");
                                if (xCodeUsl != null)
                                {
                                    var list = ListUsl.GetCodeList(xCodeUsl.Value);
                                    if (list != null)
                                    {
                                        int counter = 0;
                                        foreach (var newCode in list)
                                        {
                                            var newUsl = new XElement(usl);
                                            var id_serv = newUsl.Element("IDSERV");
                                            if (id_serv != null)
                                            {
                                                id_serv.Value += (++counter).ToString();//просто прибавить счетчик
                                                //<IDSERV> получается в случаях одинаковые, а должны быть разными, в одном случае повторяться не должны
                                            }
                                            var sumv_usl = newUsl.Element("SUMV_USL");
                                            if (sumv_usl != null)
                                            {
                                                sumv_usl.Value = "0";  //1) <SUMV_USL> надо в дополнительных услугах приравнять к нулю
                                            }
                                            var code_usl = newUsl.Element("CODE_USL");
                                            if (code_usl != null)
                                            {
                                                code_usl.Value = newCode;
                                            }
                                            var tarif = newUsl.Element("TARIF");
                                            if (tarif != null)
                                            {
                                                tarif.Value = "0";
                                            }


                                            //расставляем дополнительным услугам врачей, даты и xprvs
                                            var newCodeUsl = newUsl.Element("CODE_USL");
                                            if (newCodeUsl != null)
                                            {
                                                string prvs = newUsl.Element("PRVS").Value;
                                                string code_md = newUsl.Element("CODE_MD").Value;
                                                string date_in = newUsl.Element("DATE_IN").Value;
                                                string date_out = newUsl.Element("DATE_OUT").Value;

                                                string date = null;

                                                var prof = listProf1.GetProf(numpol, xCodeUsl.Value, newCodeUsl.Value);
                                                if (prof != null)
                                                {
                                                    prvs = prof.Xprvs;
                                                    code_md = prof.Doctor_Id;
                                                    date = prof.MeasureDate;
                                                }

                                                var prof1 = listProf2.GetProf(numpol, xCodeUsl.Value, newCodeUsl.Value);
                                                if (prof1 != null)
                                                {
                                                    prvs = prof1.Xprvs;
                                                    code_md = prof1.Doctor_Id;
                                                    date = prof1.MeasureDate;
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

                                            counter1++;
                                            sluch.LastNode.AddBeforeSelf(newUsl);
                                        }

                                        bool replace = ListUsl.IsReplace(xCodeUsl.Value);
                                        if (replace)
                                        {
                                            usl.Remove();
                                            countDel++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Messenger.WriteMessage(String.Format("Удалено {0} элементов", countDel));
            Messenger.WriteMessage(String.Format("Добавлено {0} элементов", counter1));
            return doc;
        }

        public static XDocument FixSinlsInHFile(XDocument doc, string folderIn)
        {
            //ЗАменить в IDDOKT и CODE_MD код на СНИЛС
            ListSNILS ListSn = new ListSNILS(folderIn + "SNILS.csv");
            int counter1 = 0;
            int counter2 = 0;
            var notFindedList = new List<String>();
            if (doc.Root != null)
            {
                foreach (var sluch in doc.Descendants("SLUCH"))
                {
                    foreach (var usl in sluch.Elements("USL"))
                    {
                        //ЗАменить в CODE_MD код на СНИЛС
                        var code_md = usl.Element("CODE_MD");
                        if (code_md != null)
                        {
                            string snils = ListSn.GetSNILS(code_md.Value);
                            if (!String.IsNullOrWhiteSpace(snils))
                            {
                                code_md.Value = snils;
                                counter1++;
                            }
                            else
                            {
                                var value = string.Format("<CODE_MD>{0}</CODE_MD>", code_md.Value);
                                if (!notFindedList.Contains(value))
                                    notFindedList.Add(value);
                            }
                        }
                    }
                    //ЗАменить в IDDOKT код на СНИЛС
                    var iddokt = sluch.Element("IDDOKT");
                    if (iddokt != null)
                    {
                        string snils = ListSn.GetSNILS(iddokt.Value);
                        if (!String.IsNullOrWhiteSpace(snils))
                        {
                            iddokt.Value = snils;
                            counter2++;
                        }
                        else
                        {
                            var value = string.Format("<IDDOKT>{0}</IDDOKT>", iddokt.Value);
                            if (!notFindedList.Contains(value))
                                notFindedList.Add(value);
                        }
                    }
                }
            }
            var path = Path.Combine(folderIn, "notFindedHCodes.txt");
            if (File.Exists(path))
                File.Delete(path);

            File.WriteAllLines(path, notFindedList);
            Messenger.WriteMessage(String.Format("Исправлено {0} элементов", counter1));
            Messenger.WriteMessage(String.Format("Исправлено {0} элементов", counter2));
            return doc;
        }

        public static XDocument FixSnilsInDFile(XDocument doc, string folderIn)
        {
            //ЗАменить в CODE_MD код на СНИЛС
            ListSNILS ListSn = new ListSNILS(folderIn + "SNILS.csv");
            int counter1 = 0;

            var notFindedList = new List<String>();

            if (doc.Root != null)
            {
                foreach (var codeMd in doc.Descendants("CODE_MD"))
                {
                    string snils = ListSn.GetSNILS(codeMd.Value);
                    if (!String.IsNullOrWhiteSpace(snils))
                    {
                        codeMd.Value = snils;
                        counter1++;
                    }
                    else
                    {
                        if (!notFindedList.Contains(codeMd.Value))
                            notFindedList.Add(codeMd.Value);
                    }

                }

                var path = Path.Combine(folderIn, "notFindedDCodes.txt");
                if (File.Exists(path))
                    File.Delete(path);

                File.WriteAllLines(path, notFindedList);
            }
            Messenger.WriteMessage(String.Format("Исправлено {0} элементов", counter1));

            return doc;
        }

        public static void MoveChildsFromDToH(XDocument docD, XDocument docH)
        {
            int counter = 0;
            if (docH.Root != null && docD.Root != null)
            {
                var zaps = docD.Root.Elements("ZAP");
                var childZaps =
                    zaps.Where(
                        zap =>
                            zap.Elements("SLUCH")
                                .Any(
                                    sluch =>
                                        sluch.Elements("USL")
                                            .Any(
                                                usl =>
                                                    (usl.Element("CODE_USL") != null && usl.Element("CODE_USL").Value.Length >= 3)
                                                        ? usl.Element("CODE_USL").Value.Substring(0, 3) == "163"
                                                        : false)));

                counter = MoveElements(docH, childZaps, counter);
            }
            Messenger.WriteMessage(String.Format("Перенесено {0} элементов", counter));
        }
        public static void MoveAllFromDToH(XDocument docD, XDocument docH)
        {
            int counter = 0;
            if (docH.Root != null && docD.Root != null)
            {
                var zaps = docD.Root.Elements("ZAP");
                var childZaps = zaps.Where(zap => true);

                counter = MoveElements(docH, childZaps, counter);
            }
            Messenger.WriteMessage(String.Format("Перенесено {0} элементов", counter));
        }

        private static int MoveElements(XDocument docH, IEnumerable<XElement> childZaps, int counter)
        {
            //взять последний IDCASE из НМ
            var sluchCounter = new Counter(Merger.MaxIDCaseInDoc(docH));
            int n_zap = 0;
            var lastEl = docH.Root.Elements("ZAP").LastOrDefault();
            if (lastEl != null)
            {
                var xN_Zap = lastEl.Element("N_ZAP");
                if (xN_Zap != null)
                {
                    n_zap = Convert.ToInt32(xN_Zap.Value);
                }
            }
            var zapCounter = new Counter(n_zap);

            foreach (var zap in childZaps)
            {
                var newZap = new XElement(zap);
                newZap.SetElementValue("N_ZAP", zapCounter.GetNext()); //3) <N_ZAP> тоже дублируется надо исправить, которые из DM идут
                docH.Root.Add(newZap);
                counter++;

                foreach (var sluch in newZap.Elements("SLUCH"))
                {
                    AddElementsAfterLpu1(sluch);//добавляем недостающие элементы для случая
                    sluch.SetElementValue("IDCASE", sluchCounter.GetNext()); //продолжаем нумерацию случаев
                }
            }

            childZaps.Remove();
            return counter;
        }

        private static void AddNovorInPacient(XElement pacient)
        {
            //у тех кого ты перекинул из DMки в HMку нужно добавить теги: В блок PACIENT: после тега SMO добавить <NOVOR>0</NOVOR>
            if (pacient != null && pacient.Element("SMO")!=null && pacient.Element("NOVOR") == null)
            {
                pacient.Element("SMO").AddAfterSelf(new XElement("NOVOR", 0));
            }
        }

        private static void AddUslokInSluch(XElement sluch)
        {
            //В блок SLUCH: после тега IDCASE добавить <USL_OK>3</USL_OK>,     
            if (sluch != null && sluch.Element("IDCASE") != null && sluch.Element("USL_OK") == null)
            {
                sluch.Element("IDCASE").AddAfterSelf(new XElement("USL_OK", 3));
            }
        }

        private static void AddForPomAfterVidpom(XElement sluch)
        {
            //после тега VIDPOM добавить <FOR_POM>3</FOR_POM><EXTR>1</EXTR>, 
            if (sluch != null)
            {
                var vidpom = sluch.Element("VIDPOM");
                if (vidpom!=null && sluch.Element("FOR_POM") == null && sluch.Element("EXTR") == null)
                {
                    vidpom.AddAfterSelf(
                        new XElement("FOR_POM", 3), 
                        new XElement("EXTR", 1));
                }
            }
        }

        private static void AddElementsAfterLpu1(XElement usl, bool isDispanser=false)
        {
            //после тега LPU_1 добавить <PODR>100</PODR><PROFIL>68</PROFIL><DET>1</DET>  если услуга 163....., а если услуга 063... то добавить <PODR>001</PODR><PROFIL>97</PROFIL><DET>0</DET>, 
            if (usl != null)
            {
                var lpu1 = usl.Element("LPU_1");
                var code_usl = usl.Element("CODE_USL");
                if (lpu1 != null && code_usl != null)
                {
                    if (usl.Element("PODR") == null && usl.Element("PROFIL") == null && usl.Element("DET") == null)
                    {
                        if (code_usl.Value.StartsWith("163") || (isDispanser && code_usl.Value.StartsWith("16")))
                        {
                            lpu1.AddAfterSelf(
                                new XElement("PODR", 100), 
                                new XElement("PROFIL", 68),
                                new XElement("DET", 1));
                        }
                        if (code_usl.Value.StartsWith("063") || (isDispanser && code_usl.Value.StartsWith("06")))
                        {
                            lpu1.AddAfterSelf(
                                new XElement("PODR", "001"), 
                                new XElement("PROFIL", 97),
                                new XElement("DET", 0));
                        }
                    }
                }
            }
        }

        private static void AddElementsAfterLpu1(XElement sluch)
        {
            //после тега LPU_1 добавить <PODR>100</PODR><PROFIL>68</PROFIL><DET>1</DET>  если услуга 163....., а если услуга 063... то добавить <PODR>001</PODR><PROFIL>97</PROFIL><DET>0</DET>, 
            if (sluch != null)
            {
                var lpu1 = sluch.Element("LPU_1");
                bool is163 = sluch.Elements("USL").Where(X => X.Element("CODE_USL") != null && (X.Element("CODE_USL").Value.StartsWith("163"))).Count() > 0;
                bool is063 = sluch.Elements("USL").Where(X => X.Element("CODE_USL") != null && (X.Element("CODE_USL").Value.StartsWith("063"))).Count() > 0;

                if (lpu1 != null)
                {
                    if (sluch.Element("PODR") == null && sluch.Element("PROFIL") == null && sluch.Element("DET") == null)
                    {
                        if (is163)
                        {
                            lpu1.AddAfterSelf(
                                new XElement("PODR", 100),
                                new XElement("PROFIL", 68),
                                new XElement("DET", 1));
                        }
                        else if (is063)
                        {
                            lpu1.AddAfterSelf(
                                new XElement("PODR", "001"),
                                new XElement("PROFIL", 97),
                                new XElement("DET", 0));
                        }
                    }
                }
            }
        }

        private static void AddElementAfterRsltD(XElement sluch)
        {
            //после тегов RSLT_D добавить <ISHOD>306</ISHOD><PRVS>1109</PRVS> = PRVS из услуги <IDDOKT>2642</IDDOKT>= CODE_MD из услуги,
            if (sluch != null)
            {
                int count = sluch.Elements("USL").Count();
                bool isDispancer = sluch.Elements("USL")
                    .Where(
                        X => X.Element("CODE_USL")!=null && (
                            X.Element("CODE_USL").Value.StartsWith("063") ||                              
                            X.Element("CODE_USL").Value.StartsWith("163"))
                        ).Count() > 0;

                foreach (var usl in sluch.Elements("USL"))
                {
                    string prvs = "";
                    string iddokt = "";
                    string ds1 = sluch.Element("DS1") != null ? sluch.Element("DS1").Value : "";
                    if (usl != null)
                    {
                        if (usl.Element("PRVS") != null)
                        {
                            prvs = usl.Element("PRVS").Value;
                            iddokt = usl.Element("CODE_MD").Value;
                        }
                        AddElementsAfterLpu1(usl, isDispancer);

                        //после тега DATE_OUT добавить <DS>Z00.2</DS> = DS1 из случая   
                        if (usl.Element("DATE_OUT") != null && usl.Element("DS") == null)
                        {
                            usl.Element("DATE_OUT").AddAfterSelf(new XElement("DS", ds1));
                        }
                    }

                    var rslt_d = sluch.Element("RSLT_D");
                    if (rslt_d != null && rslt_d.Element("ISHOD") == null && rslt_d.Element("PRVS") == null && rslt_d.Element("IDDOKT") == null)
                    {
                        rslt_d.AddAfterSelf(
                            new XElement("ISHOD", "306"),
                            new XElement("PRVS", prvs),
                            new XElement("IDDOKT", iddokt)
                            );

                        //Переименовать тег RSLT_D на RSLT
                        rslt_d.Name = "RSLT";
                    }
                }
            }
        }

        public static void FixIdsp(XDocument doc)
        {
            //в HMке проверить если есть случаи с несколькими услугами, то IDSP этого случая менять на 30, после слития файлов
            int counter = 0;
            if (doc.Root != null)
            {
                foreach (var zap in doc.Root.Elements("ZAP"))
                {
                    foreach (var sluch in zap.Elements("SLUCH"))
                    {
                        if (FixIdsp(sluch))
                        {
                            counter++;
                        }
                    }
                }
            }
            Messenger.WriteMessage(String.Format("Исправлено {0} записей", counter));
        }

        public static bool FixIdsp(XElement sluch)
        {
            if (sluch.Elements("USL").Count() > 1)
            {
                sluch.SetElementValue("IDSP", 30);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Пересчет всех сумм
        /// </summary>
        /// <param name="doc"></param>
        public static void CountSum(XDocument doc)
        {
            if (doc.Root != null)
            {
                decimal summav = 0;
                foreach (var zap in doc.Root.Elements("ZAP"))
                {
                    foreach (var sluch in zap.Elements("SLUCH"))
                    {
                        decimal sumv = 0;
                        foreach (var usl in sluch.Elements("USL"))
                        {
                            var sumv_usl = usl.Element("SUMV_USL");
                            if (sumv_usl == null) { continue; }
                            sumv += Convert.ToDecimal(sumv_usl.Value.Replace(".", ","));;
                        }
                        sluch.SetElementValue("SUMV", sumv.ToString("#0.00").Replace(",", "."));
                        //Пересчет суммы услуг в случае
                        summav += sumv;
                    }
                }

                var schet = doc.Root.Element("SCHET");
                if (schet != null)
                {
                    if (schet.Element("SUMMAV") != null)
                    {
                        //decimal sum = (from zap in doc.Root.Elements("ZAP")
                        //    from sluch in zap.Elements("SLUCH")
                        //    where sluch.Element("SUMV") != null
                        //    select Convert.ToDecimal(sluch.Element("SUMV").Value.Replace(".", ","))).Sum();

                        //foreach (var zap in doc.Root.Elements("ZAP"))
                        //{
                        //    foreach (var sluch in zap.Elements("SLUCH"))
                        //    {
                        //        if (sluch.Element("SUMV") != null)
                        //        {//            sum += Convert.ToDecimal(sluch.Element("SUMV").Value.Replace(".",","));
                        //        }
                        //    }
                        //}
                        var xSummav = schet.Element("SUMMAV");
                        if (xSummav != null)
                        {
                            var oldsum = xSummav.Value;
                            xSummav.Value = summav.ToString("#0.00").Replace(",", ".");
                            Messenger.WriteMessage(String.Format("Сумма изменена: {0}=>{1}", oldsum, xSummav.Value));
                        }
                    }
                }
            }
        }
        }
}
