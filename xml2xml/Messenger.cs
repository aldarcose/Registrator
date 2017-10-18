using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2xml
{
    public static class Messenger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Записать сообщение в лог, написать в консоль и выбросить ошибку
        /// </summary>
        /// <param name="message"></param>
        public static void ThrowMessage(string message)
        {
            WriteMessage(message);
            throw new Exception(message);
        }
        /// <summary>
        ///  Записать сообщение в лог, написать в консоль
        /// </summary>
        /// <param name="message"></param>
        public static void WriteMessage(string message)
        {
            logger.Trace(message);
            Console.WriteLine(message);
        }
    }
}
