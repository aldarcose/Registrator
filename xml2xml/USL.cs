using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2xml
{
    public class USL
    {
        public USL()
        {
        }

        public string Code { get; set; }
        public List<string> CodeList { get; set; }
        public bool Replace { get; set; }
    }
    public class ListUSL
    {
        public List<USL> List { get; set; } 
        public ListUSL()
        {
        }

        public ListUSL(string path)
        {
            List = GetList(path);
        }

        public List<string> GetCodeList(string Code)
        {
            var list = List.FirstOrDefault(X => X.Code == Code);
            if (list != null)
            {
                return list.CodeList;
            }
            else
            {
                return null;
            }
        }

        public bool IsReplace(string Code)
        {
            var list = List.FirstOrDefault(X => X.Code == Code);
            
                                        //ТЫ УДАЛЯЕШЬ ТОЛЬКО УСЛУГИ 063411-06314 И 063421-063424 ОСТАЛЬНЫЕ ОСТАВЛЯЕШЬ
            if (list != null && (list.Code == "063411" || list.Code == "063412" || list.Code == "063413" || list.Code == "063414" ||
                                 list.Code == "063421" || list.Code == "063422" || list.Code == "063423" || list.Code == "063424"))
            {
                return true;
            }
            return false;
        }

        public bool HasCode(string Code)
        {
            return List.FirstOrDefault(X => X.Code == Code) != null;
        }

        public static List<USL> GetList(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path, Encoding.Default);
            List<USL> uslList = new List<USL>();
            int counter = 0;
            foreach (string line in lines)
            {
                if (counter > 0)
                {
                    USL usl = new USL();
                    usl.CodeList = new List<string>();
                    string[] words = line.Split(';');

                    int i = 0;
                    foreach (string word in words)
                    {
                        if (i == 0)
                        {
                            usl.Code = word.PadLeft(6, '0'); // только 6 символов
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(word))
                            {
                                if (word == "1")
                                {
                                    usl.Replace = true;
                                }
                                else
                                {
                                    usl.CodeList.Add(word);
                                }
                            }
                        }
                        i++;
                    }
                    uslList.Add(usl);
                }
                counter++;
            }
            return uslList;
        }
    }
}
