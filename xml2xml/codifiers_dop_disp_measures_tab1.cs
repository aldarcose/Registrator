using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xml2xml
{
    public class codifiers_dop_disp_measures_tab1
    {
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string ages { get; set; }
        public string tarif { get; set; }

        public static codifiers_dop_disp_measures_tab1 Get(List<codifiers_dop_disp_measures_tab1> list, string id)
        {
            return list.FirstOrDefault(X => X.id.Equals(id));
        }

        public static string GetTarif(List<codifiers_dop_disp_measures_tab1> list, string code)
        {
            if (String.IsNullOrWhiteSpace(code))
            {
                throw new FormatException("code is null");
            }
            var res = list.FirstOrDefault(X => X.code.Equals(code));
            return res != null ? res.tarif : null;
        }

        public static List<codifiers_dop_disp_measures_tab1> GetList(string path)
        {
            var doc = XDocument.Load(path);

            if (doc.Root != null)
            {
                var rowdata = doc.Root.Element("ROWDATA");
                if (rowdata != null)
                {
                    var list = rowdata.Elements("ROW").Select(x => new codifiers_dop_disp_measures_tab1()
                    {
                        id = x.Attribute("id").Value,
                        code = x.Attribute("USL").Value.PadLeft(6, '0'),
                        name = x.Attribute("name").Value,
                        ages = x.Attribute("ages").Value,tarif = x.Attribute("TARIF").Value
                    }).ToList();
                    return list;
                }
            }
            return null;
        }
    }
}
