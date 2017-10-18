using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace xml2xml
{
    public class SNILS
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Snils { get; set; }
    }

    public class ListSNILS
    {
        public List<SNILS> List { get; set; } 
        public ListSNILS()
        {
        }

        public ListSNILS(string path)
        {
            List = GetList(path);
        }

        public string GetSNILS(string Code)
        {
            var sn = List.FirstOrDefault(X => X.Code == Code && !string.IsNullOrEmpty(X.Snils));
            return sn != null ? sn.Snils : null;
        }

        public static List<SNILS> GetList(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path, Encoding.Default);

            List<SNILS> list = 
                lines.Select(line => line.Split(';'))
                    .Select(words => new SNILS()
                    {
                        Name = words[0], Code = words[1], Snils = words[2]
                    }).ToList();
            return list;
        }
    }
}
