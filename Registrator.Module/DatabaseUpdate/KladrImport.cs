using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformationSystems.Atlas;
using EAS.Common.Dictionaries;

namespace EAS.Common.Imports
{
    /// <summary>
    /// Импорт КЛАДР
    /// </summary>
    public class KladrImport : ProgramImport
    {
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="file">Файл</param>
        public void Init(
            [FileName(FileNameMode.Open, Filter = "Файлы DBF (*.dbf)|*.dbf|Все файлы (*.*)|*.*")]
            FileName file)
        {
            // Существующие записи КЛАДР
            IEnumerable<Kladr> kladr = QueryBase.Get("Query.View(cmn.vw_kladr)").Select<Kladr>();
            Dictionary<string, Kladr> codes = new Dictionary<string, Kladr>(kladr.Count());
            foreach (Kladr k in kladr) codes[k.Code] = k;

            // Обработка записей
            DbfImporter import = new DbfImporter(new DbfImportParameters(file));
            Files.Add(import);
            import.ProcessRecordFunc = delegate(DbfRecord r)
            {
                string code = (string)r.Record["CODE"];
                if (!codes.ContainsKey(code))
                {
                    // Родительская запись
                    int level = Convert.ToInt32((decimal)r.Record["LEVEL"]);
                    Kladr parent = null;
                    if (level > 1)
                    {
                        int l = level;
                        while (l > 1 && parent == null)
                        {
                            int count = l == 5 ? 11 : l == 4 ? 8 : l == 3 ? 5 : 2;
                            string parentCode = code.Substring(0, count) +
                                "0000000000000".Substring(0, code.Length - count - 2) + code.Substring(code.Length - 2, 2);
                            codes.TryGetValue(parentCode, out parent);
                            l--;
                        }
                        if (parent == null)
                            throw new KeyNotFoundException("Не найдена родительская запись: " + code);
                    }

                    // Импорт записи
                    KladrType type = KladrType.GetType((decimal)r.Record["TYPE"]);
                    string name = (string)r.Record["NAME"];
                    string codePost = (string)r.Record["CODE_POST"];
                    string codeIfns = (string)r.Record["CODE_IFNS"];
                    string codeIfnsT = (string)r.Record["CODE_IFNST"];
                    string codeOkato = (string)r.Record["CODE_OKATO"];
                    codes[code] = new Kladr(type, parent, name, code, codePost, codeIfns, codeIfnsT, codeOkato);
                }
            };
        }
    }
}
