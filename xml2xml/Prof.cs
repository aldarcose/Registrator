using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xml2xml
{
    public class Prof
    {
        public string NumPol { get; set; }
        public string CodeUsl { get; set; }
        public string Doctor_Id { get; set; }
        public string MeasureDate { get; set; }
        public string Xprvs { get; set; }
        public string DopCodeUsl { get; set; }
        public string dop_disp_measure_id { get; set; }
    }

    public class ListProf
    {
        public List<Prof> List { get; set; }

        public ListProf()
        {
        }

        public ListProf(string path)
        {
            List = GetList(path);
        }

        public Prof GetProf(string numpol, string codeusl, string newcode)
        {
            var item = List.FirstOrDefault(X => (X.NumPol == numpol && X.CodeUsl == codeusl && X.DopCodeUsl == newcode));
            return item;
        }

        public IEnumerable<Prof> GetProfs(string numpol, string codeusl)
        {
            return List.FindAll(X => X.NumPol == numpol && X.CodeUsl == codeusl);
        }

        public static List<Prof> GetList(string path)
        {
            var doc = XDocument.Load(path);

            if (doc.Root != null)
            {
                var rowdata = doc.Root.Element("ROWDATA");
                if (rowdata != null)
                {
                    var list = rowdata.Elements("ROW").Select(x => new Prof()
                    {                       
                        NumPol = x.Attribute("num_pol").Value,
                        CodeUsl = x.Attribute("code").Value.PadLeft(6, '0'), // только 6 символов,
                        Doctor_Id = x.Attribute("doctor_id").Value,
                        MeasureDate = x.Attribute("measure_date").Value,
                        Xprvs = x.Attribute("xprvs").Value,
                        DopCodeUsl = x.Attribute("reestr_uslug_code").Value.PadLeft(6, '0'),
                        dop_disp_measure_id =
                            x.Attribute("dop_disp_measure_id") != null ? x.Attribute("dop_disp_measure_id").Value : null
                    }).ToList();
                    return list;
                }
            }
            return null;
        }
    }
}
