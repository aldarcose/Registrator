using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace xml2xml
{
    /// <summary>
    /// Слияние двух файлов в один
    /// </summary>
    public static class Merger
    {
        public static void Merge(string folderIn, string folderOut)
        {
            string[] files = Directory.GetFiles(folderIn);

            string fileHM = files.FirstOrDefault(X => Path.GetFileName(X).Substring(0, 2) == "HM");
            string fileLM = files.FirstOrDefault(X => Path.GetFileName(X).Substring(0, 2) == "LM");
            string fileH1 = files.FirstOrDefault(X => Path.GetFileName(X).Substring(0, 2) == "H_");
            string fileL1 = files.FirstOrDefault(X => Path.GetFileName(X).Substring(0, 2) == "L_");

            string errorMessage = "";
            if (String.IsNullOrWhiteSpace(fileHM))
            {
                errorMessage += "Отсутствует файл HM\r\n";
            }if (String.IsNullOrWhiteSpace(fileLM))
            {
                errorMessage += "Отсутствует файл LM\r\n";
            }
            if (String.IsNullOrWhiteSpace(fileH1))
            {
                errorMessage += "Отсутствует файл H_\r\n";
            }
            if (String.IsNullOrWhiteSpace(fileL1))
            {
                errorMessage += "Отсутствует файл L_\r\n";
            }
            if (!String.IsNullOrWhiteSpace(errorMessage))
            {
                Messenger.ThrowMessage(errorMessage);
                //MessageBox.Show(errorMessage);
                return;
            }

            var docHM = XDocument.Load(fileHM);
            var docLM = XDocument.Load(fileLM);
            var docH1 = XDocument.Load(fileH1);
            var docL1 = XDocument.Load(fileL1);

            MergeXML(docHM, docH1, docLM, docL1);
            Converter.CountSum(docHM);

            Messenger.WriteMessage("Чиним PRVS в HM");
            PRVS.RepairPRVSInDoc(docHM, folderIn);

            Messenger.WriteMessage("Чиним IDSP в HM");
            Converter.FixIdsp(docHM);

            Messenger.WriteMessage("Сохраняемся в папку");
            docLM.Save(folderOut + Path.GetFileName(fileLM));
            docHM.Save(folderOut + Path.GetFileName(fileHM));
            Messenger.WriteMessage("Завершено");
        }

        private static void MergeXML(XDocument docHM, XDocument docH1, XDocument docLM, XDocument docL1)
        {
            //взять последний IDCASE из НМ
            var sluchCounter = new Counter(MaxIDCaseInDoc(docHM));
            //получается будет 4 файла HM, LM главные и HM1 и LM2 которые надо влить в в главные. 
            //Алгоритм таков: берется полис из HM1 сравнивается с данными в HM 
            // и если такой полис есть, то в эту запись в HMку добавляется случай из HM1
            //который соответствует этому полису, начиная с тега SLUCH.
            //Если полиса такого нет, то  в HM добавляем всю запись с ZAP и в LM добавляем запись по этому полису из LM1. 
            //Связи  HM и LM идут по ID_PAC/
            foreach (var xzap in docH1.Root.Elements())
            {
                var xpacient = xzap.Element("PACIENT");
                if (xpacient != null)
                {
                    var npolis = xpacient.Element("NPOLIS");
                    var spolis = xpacient.Element("SPOLIS");
                    if (npolis != null)
                    {
                        string num = npolis.Value;
                        string ser = spolis!=null?spolis.Value: "";

                        XElement zap = FindZapInHM(docHM, num, ser);

                        if (zap != null)
                        {
                            //если такой полис есть, то в эту запись в HMку добавляется случай из HM1
                            MoveAllSluchToZap(xzap.Elements("SLUCH"), zap, sluchCounter);
                        }
                        else
                        {
                            //Если полиса такого нет, то  в HM добавляем всю запись с ZAP в конец
                            var newZap = new XElement(xzap);
                            var xN_Zap = newZap.Element("N_ZAP");

                            zap = docHM.Root.Elements("ZAP").LastOrDefault(); // находим крайний элемент ZAP
                            if (zap != null)
                            {
                                var N_Zap = zap.Element("N_ZAP");
                                if (xN_Zap != null && N_Zap != null)
                                {
                                    xN_Zap.Value = Convert.ToString(Convert.ToInt32(N_Zap.Value) + 1);
                                    //продолжаем нумерацию; 
                                }
                            }
                            else
                            {
                                //если нет вообще элементов ZAP, то будем первыми
                                if (xN_Zap != null)
                                {
                                    xN_Zap.Value = "1";
                                }
                            }
                            FixAllSluchsIDsInZap(newZap, sluchCounter);

                            foreach (var newSluch in newZap.Elements("SLUCH"))
                            {
                                newSluch.SetElementValue("IDCASE", sluchCounter.GetNext()); //продолжаем нумерацию случаев
                            }

                            docHM.Root.Add(newZap);//и в LM добавляем запись по этому полису из LM1
                            var id_pac = xpacient.Element("ID_PAC");
                            if (id_pac == null)
                            {
                                Messenger.ThrowMessage("Error 302: Pacient не содержит элемент ID_PAC");
                                return;
                            }
                            var xpers = FindPersInL1(docL1, id_pac.Value);

                            if (xpers == null)
                            {
                                Messenger.ThrowMessage("Error 305: Файл L_ не содержит элемент PERS c ID_PAC=" + id_pac.Value);
                            }

                            if (docLM.Root == null)
                            {
                                Messenger.ThrowMessage("Error 304: Файл LM не содержит корневого элемента");
                            }

                            docLM.Root.Add(new XElement(xpers));
                        }
                    }
                }
            }
        }

        private static void FixAllSluchsIDsInZap(XElement xzap, Counter counter)
        {
            foreach (var xsluch in xzap.Elements("SLUCH"))
            {
                var newSluch = new XElement(xsluch);
                newSluch.SetElementValue("IDCASE", counter.GetNext());
            }
        }

        private static int Max(int a, int b)
        {
            return a >= b ? a : b;
        }

        public static int MaxIDCaseInDoc(XDocument doc)
        {
            if (doc.Root == null) { throw new ArgumentNullException("doc");}
            int max = 0;
            foreach (var zap in doc.Root.Elements("ZAP"))
            {
                foreach (var sluch in zap.Elements("SLUCH"))
                {
                    var xElement = sluch.Element("IDCASE");
                    if (xElement == null) { throw new Exception("в одном из случаев отсутствует элемент IDCASE"); }
                    max = Max(max, Int32.Parse(xElement.Value));
                }
            }
            return max;
        }

        /// <summary>
        /// Возвращаем первый элемент котороый удовлетворяет условию, либо возвращаем null
        /// </summary>
        /// <returns></returns>
        private static XElement FindPersInL1(XDocument docL1, string id_pac)
        {
            if (docL1.Root == null)
            {
                Messenger.ThrowMessage("Error 303: Файл L_ не содержит корневого элемента");
            }
            
            return docL1.Root.Elements("PERS").FirstOrDefault(X => X.Element("ID_PAC") != null && X.Element("ID_PAC").Value.Trim() == id_pac.Trim());
        }

        /// <summary>
        /// Переместить все случаи в zap
        /// </summary>
        private static void MoveAllSluchToZap(IEnumerable<XElement> xsluchs, XElement zap, Counter counter)
        {
            foreach (var xsluch in xsluchs)
            {
                var newSluch = new XElement(xsluch);
                newSluch.SetElementValue("IDCASE", counter.GetNext());
                
                var sluch = zap.Elements("SLUCH").LastOrDefault();
                if (sluch != null)
                {
                    sluch.AddAfterSelf(newSluch);
                }
                else
                {
                    zap.Add(newSluch);
                }
            }
        }

        /// <summary>
        /// Возвращет первый элемент ZAP в документе docHM, в котором есть пациент с указанными series и number полиса
        /// </summary>
        /// <param name="docHM"></param>
        /// <param name="number"></param>
        /// <param name="series"></param>
        /// <returns>первый элемент ZAP</returns>
        private static XElement FindZapInHM(XDocument docHM, string number, string series = null)
        {
            return docHM.Root.Elements("ZAP").FirstOrDefault(X =>
            {
                var xpacient = X.Element("PACIENT");
                if (xpacient != null)
                {
                    var npolis = xpacient.Element("NPOLIS");
                    var spolis = xpacient.Element("SPOLIS");
                    if (npolis != null)
                    {
                        string num = npolis.Value;
                        string ser = spolis != null ? spolis.Value : null;

                        if (num == number && ser == series) { return true; }
                    }
                }
                return false;
            });
        }        
    }
}
