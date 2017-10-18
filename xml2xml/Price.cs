using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2xml
{
    public class Price
    {
        public Price()
        {
        }

        public string Usl { get; set; }
        public string Sum { get; set; }
    }
    public class ListPrice
    {
        public List<Price> List { get; set; }
        public ListPrice()
        {
        }

        public ListPrice(string path)
        {
            List = GetList(path);
        }

        public string GetPrice(string Usl)
        {
            var list = List.FirstOrDefault(X => X.Usl == Usl);
            if (list != null)
            {
                return list.Sum;
            }
            return null;
        }

        public bool HasUsl(string Usl)
        {
            return List.FirstOrDefault(X => X.Usl == Usl) != null;
        }

        public static List<Price> GetList(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path, Encoding.Default);
            var priceList = new List<Price>();
            int counter = 0;
            foreach (string line in lines)
            {
                if (counter > 0)
                {
                    var price = new Price();
                    string[] words = line.Split(';');

                    price.Usl = words[0].PadLeft(6, '0'); // до 6 символов
                    price.Sum = words[1];
                    priceList.Add(price);
                }
                counter++;
            }
            return priceList;
        }
    }
}
