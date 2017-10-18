using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    class DocSerialNumberTemplate
    {
        // R - на месте одного символа R располагается целиком римское число, заданное символами "I", "V", "X", "L", "С", набранными на верхнем регистре латинской клавиатуры;
        // возможно представление римских чисел с помощью символов "1", "У", "X", "Л", "С" соответственно, набранных на верхнем регистре русской клавиатуры;
        const string R = "[IVXLC1YXLC]+";

        // Б - любая русская заглавная буква;
        const string B = "[А-Я]";

        // 9 - любая десятичная цифра (обязательная);
        const string d9 = @"\d";

        // 0 - любая десятичная цифра (необязательная, может отсутствовать);
        const string d0 = @"\d?";

        // S- символ не контролируется (может содержать любую букву, цифру или вообще отсутствовать);
        const string S = @"\w?";

        /// <summary>
        /// Возвращает шаблон регулярного выражения для проверки правильности введенных данных
        /// </summary>
        /// <param name="template">Шаблон реквизитов</param>
        /// <returns>Возвращает шаблон регулярного выражения</returns>
        public static string GetRegexTemplate(string template)
        {
            var result = template.Replace(" ", @"\s?");

            result = result.Replace("R", R);

            result = result.Replace("Б", B);

            result = result.Replace("9", d9);

            result = result.Replace("0", d0);

            result = result.Replace("S1", S);

            result = result.Replace("S", S);

            return string.Format("^{0}$", result);
        }
    }
}
