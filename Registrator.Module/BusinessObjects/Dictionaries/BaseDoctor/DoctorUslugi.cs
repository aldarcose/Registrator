using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using System.Xml.Linq;
using System.Linq;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    /// <summary>
    /// Справочник услуг, оказываемых врачом
    /// </summary>
    [DefaultClassOptions]
    public class DoctorUslugi : BaseObject
    {
        public DoctorUslugi(Session session) : base(session) { }

        /// <summary>
        /// Код услуги
        /// </summary>
        [Size(20)]
        [XafDisplayName("Код услуги")]
        public string Code { get; set; }

        /// <summary>
        /// Наименование услуги
        /// </summary>
        [Size(255)]
        [XafDisplayName("Наименование")]
        public string Name { get; set; }

        /// <summary>
        /// Тариф услуги
        /// </summary>
        [XafDisplayName("Тариф")]
        public double Tarif { get; set; }

        public override string ToString()
        {
            return Name;
        }

        // Пример записи из XML
        // <WorkBook>
        //  <WorkSheet>
        //   <Table>
        //    <Row>
        //      <Cell><Data ss:Type="String">CODE_USL</Data></Cell>
        //      <Cell><Data ss:Type="String">NAME</Data></Cell>
        //      <Cell><Data ss:Type="String">TARIF</Data></Cell>
        //    </Row>
        //    <Row>
        //      <Cell ss:StyleID="s16"><Data ss:Type="String">009101</Data></Cell>
        //      <Cell ss:StyleID="s16"><Data ss:Type="String">кариес в ст. белого пятна 1-ое посещение</Data></Cell>
        //      <Cell ss:StyleID="s18"><Data ss:Type="Number">1.5</Data></Cell>
        //    </Row>

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);

            var ns = doc.Root.Name.Namespace;

            var rows = doc.Root.Descendants(ns + "Row");
            
            foreach (var element in rows)
            {
                var data = element.Descendants(ns + "Data").ToList();

                var code = data[0].Value;
                var name = data[1].Value;
                var rate = "0";
                if (data.Count == 3)
                  rate = (data[2] == null || data[2].Value=="") ? "0" : data[2].Value;

                
                // если текущий элемент строка с заголовком, то пропускаем
                if (code == "CODE_USL") continue;

                DoctorUslugi obj = objSpace.FindObject<DoctorUslugi>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", code));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<DoctorUslugi>();
                    obj.Code = code;
                    obj.Name = name;

                    double parsedTarif = 0.0;
                    double.TryParse(rate.Replace(".", ","), out parsedTarif);
                    obj.Tarif = parsedTarif;
                }
            }
        }
    }
}
