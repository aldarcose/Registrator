using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xml2xml
{
    public class PRVS
    {
        private static int counter = 0;

        public string Code { get; set; }
        public string NewCode { get; set; }
        public static void RepairPRVSInDoc(XDocument doc, string folderIn)
        {
            if (doc.Root != null)
            {
                var ListPRVS = new ListPRVS(folderIn + "PRVS.csv");

                foreach (var zap in doc.Root.Elements("ZAP"))
                {
                    foreach (var sluch in zap.Elements("SLUCH"))
                    {
                        RepairPRVS(sluch, ListPRVS);
                        foreach (var usl in sluch.Elements("USL"))
                        {
                            RepairPRVS(usl, ListPRVS);
                        }
                    }
                }
            }
            Messenger.WriteMessage(String.Format("Починили {0} элементов PRVS", counter));
        }

        private static void RepairPRVS(XElement el, ListPRVS list)
        {
            var prvs = el.Element("PRVS");
            if (prvs != null && !String.IsNullOrWhiteSpace(prvs.Value))
            {
                var newValue = list.GetNewPRVS(prvs.Value);
                if (!String.IsNullOrWhiteSpace(newValue))
                {
                    el.SetElementValue("PRVS", newValue);
                    counter++;
                }
            }
        }
    }

    public class ListPRVS
    {
        public List<PRVS> List { get; set; }
        public ListPRVS()
        {
        }

        public ListPRVS(string path)
        {
            List = GetList(path);
        }

        public string GetNewPRVS(string PRVS)
        {
            var sn = List.FirstOrDefault(X => X.Code == PRVS);
            return sn != null ? sn.NewCode : null;
        }

        public static List<PRVS> GetList(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path, Encoding.Default);

            List<PRVS> list =
                lines.Select(line => line.Split(';'))
                    .Select(words => new PRVS()
                    {
                        Code = words[0],
                        NewCode = words[1]
                    }).ToList();
            return list;
        }
    }
}

