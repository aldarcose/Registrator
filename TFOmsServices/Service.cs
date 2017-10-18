using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


namespace TFOmsServices
{
    public interface IService
    {
        string ErrorMessage { get; set; }
        IEnumerable<PolicyInfo> GetPolicyInfos(PacientInfo pacient);
        string GetMoAttached(PolicyInfo policy);
    }
    
    public class Service : IService
    {
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Получает информацию по полисам
        /// </summary>
        /// <param name="pacient">Пациент, для которого ищем полиса</param>
        /// <returns></returns>
        public IEnumerable<PolicyInfo> GetPolicyInfos(PacientInfo pacient)
        {
            ErrorMessage = string.Empty;

            var result = FormWebBrowser.Start(pacient);
            if (result == null || result.Count == 0)
                ErrorMessage = FormWebBrowser.GetErrorMessage();
            return result;

        }
        /// <summary>
        /// По полису определяет к какому МО приписан пациент
        /// </summary>
        /// <param name="policy">Действующий полис пациента</param>
        /// <returns></returns>
        public string GetMoAttached(PolicyInfo policy)
        {
            if (policy.DateEnd.HasValue && policy.DateEnd.Value < DateTime.Now)
            {
                ErrorMessage = "Указанный полис недействителен";
                return string.Empty;
            }

            try
            {
                string serial = policy.Serial;
                string number = policy.Number;
                string url = string.Format("http://www.tfomsrb.ru/pp/?srv=p&ser={0}&num={1}", serial, number);

                string result = string.Empty;
                using (WebClient client = new WebClient())
                {
                    byte[] response = client.DownloadData(url);

                    result = Encoding.GetEncoding("windows-1251").GetString(response);
                }

                Regex r = new Regex("<td>Прикрепление:</td>\n<td>(.*?)</td>");
                var match = r.Match(result);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return string.Empty;
        }
    }
}
