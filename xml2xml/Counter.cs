using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2xml
{
    public class Counter
    {
        public Counter() { }
        public Counter(int startValue)
        {
            counter = startValue;}

        private int counter = 0;

        public int GetNext()
        {
            return ++counter;
        }

        public int GetCurrent()
        {
            return counter;
        }

        public void AddNext()
        {
            counter++;
        }

        public void Add(int number)
        {
            counter += number;
        }
    }
}
