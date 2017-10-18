using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace xml2xml
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Messenger.WriteMessage("Конвертирую...");
            string folderIn = Directory.GetCurrentDirectory()+ @"\";
            string folderOut = Directory.GetCurrentDirectory()+ @"\Out\";
            Directory.CreateDirectory(folderOut);
            Converter.ConvertXML(folderIn, folderOut, radioButtonChilds.Checked, radioButtonAll.Checked);
            string message = "Конвертировано в папку " + folderOut;
            Messenger.WriteMessage(message);
            MessageBox.Show(message);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Messenger.WriteMessage("Объединяю...");
            string folderIn = Directory.GetCurrentDirectory() + @"\";
            string folderOut = Directory.GetCurrentDirectory() + @"\Out\";
            Directory.CreateDirectory(folderOut);
            try
            {
                Merger.Merge(folderIn, folderOut);
                string message = "Конвертировано в папку " + folderOut;
                Messenger.WriteMessage(message);
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                string message = String.Format("Не выполнено из-за ошибок: \r\n{0}\r\n. Подробности в логах", ex.Message);
                Messenger.WriteMessage(message);
                MessageBox.Show(message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string folderOut = Directory.GetCurrentDirectory() + @"\Out\";

            try
            {
                string[] files = Directory.GetFiles(folderOut);
                foreach (var file in files)
                {
                    XDocument doc = XDocument.Load(file,LoadOptions.None);

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.NewLineChars = "";
                    settings.NewLineHandling = NewLineHandling.Replace;
                    settings.NewLineOnAttributes = true;
                    settings.Indent = true;
                    settings.IndentChars = "";
                    settings.WriteEndDocumentOnClose = true;
                    settings.Encoding = Encoding.GetEncoding("windows-1251");
                    XmlWriter writer = XmlWriter.Create(file, settings);

                    doc.Save(writer);
                    string message = "Файл " + file + " обработан";
                    Messenger.WriteMessage(message);
                    writer.Close();
                }
                MessageBox.Show("Обработано " + files.Count() + " файлов");
            }
            catch (Exception ex)
            {
                string message = String.Format("Не выполнено из-за ошибок: \r\n{0}\r\n. Подробности в логах", ex.Message);
                Messenger.WriteMessage(message);
                MessageBox.Show(message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string folderOut = Directory.GetCurrentDirectory() + @"\Out\";

            try
            {
                string[] files = Directory.GetFiles(folderOut);
                var counter = 0;
                foreach (var file in files)
                {
                    var fi = new FileInfo(file);
                    if (fi.Name.ToLower().StartsWith("hm"))
                    {
                        XDocument xml = XDocument.Load(file);
                        foreach (var sluch in xml.Descendants("SLUCH"))
                        {
                            if (sluch.Element("COMENTSL") == null)
                            {
                                sluch.Add(new XElement("COMENTSL"));
                            }

                            var commSl = sluch.Element("COMENTSL");

                            var uslOk = sluch.Element("USL_OK");
                            if (uslOk != null && uslOk.Value == "3")
                            {
                                var idsp = sluch.Element("IDSP");
                                if (idsp != null)
                                {
                                    if (idsp.Value.Equals("23"))
                                    {
                                        commSl.Value = "F62:6;";
                                    }
                                    if (idsp.Value.Equals("28"))
                                    {
                                        commSl.Value = "F62:9;";
                                    }
                                    if (idsp.Value.Equals("29"))
                                    {
                                        commSl.Value = "F62:4;";
                                    }

                                    if (idsp.Value.Equals("30"))
                                    {
                                        if (sluch.Elements("USL").Count() > 1)
                                        {
                                            commSl.Value = "F62:1;";
                                        }
                                        else
                                        {
                                            var homeUsl =
                                                "021612,061002,061006,061015,061018,061021,061027,061032,061043,061046,061050,061054,061058,061079,061103,061107,061137,061141,161002,161007,161011,161015,161021,161026,161028,161036,161037,161040,161044,161045,161079,161083,161141,161151";
                                            if (homeUsl.Contains(sluch.Element("USL").Element("CODE_USL").Value))
                                            {
                                                commSl.Value = "F62:3;";
                                            }
                                            else
                                            {
                                                commSl.Value = "F62:2;";
                                            }
                                        }
                                    }
                                }
                            }

                            var commSl2 = sluch.Element("COMENTSL");
                            var idsp2 = sluch.Element("IDSP");
                            if (idsp2 != null && idsp2.Value.Equals("41"))
                            {
                                sluch.Element("USL_OK").Value = "3";
                                sluch.Element("FOR_POM").Value = "2";
                                sluch.Element("EXTR").Value = "2";
                                sluch.Element("PROFIL").Value = "160";
                                sluch.Element("TARIF").Value = "613.59";
                                sluch.Element("SUMV").Value = "613.59";
                                if (sluch.Element("DET").Value.Equals(0))
                                {
                                    if (commSl2.Value.Equals("П"))
                                    {
                                        switch (sluch.Element("USL").Element("CODE_USL").Value)
                                        {
                                            case "001006":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061042";
                                                break;
                                            case "001026":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061027";
                                                break;
                                            case "001027":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061014";
                                                break;
                                            case "001034":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061018";
                                                break;
                                            case "001039":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061089";
                                                break;
                                            case "001043":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061078";
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (sluch.Element("USL").Element("CODE_USL").Value)
                                        {
                                            case "001006":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061043";
                                                break;
                                            case "001026":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061027";
                                                break;
                                            case "001027":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061015";
                                                break;
                                            case "001034":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061018";
                                                break;
                                            case "001043":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061079";
                                                break;
                                            case "001039":
                                                sluch.Element("USL").Element("CODE_USL").Value = "061137";
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (commSl2.Value.Equals("П"))
                                    {
                                        switch (sluch.Element("USL").Element("CODE_USL").Value)
                                        {
                                            case "001006":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161039";
                                                break;
                                            case "001026":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161042";
                                                break;
                                            case "001027":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161047";
                                                break;
                                            case "001034":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161028";
                                                break;
                                            case "001039":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161083";
                                                break;
                                            case "001043":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161078";
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (sluch.Element("USL").Element("CODE_USL").Value)
                                        {
                                            case "001006":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161037";
                                                break;
                                            case "001026":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161040";
                                                break;
                                            case "001027":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161045";
                                                break;
                                            case "001034":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161028";
                                                break;
                                            case "001043":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161079";
                                                break;
                                            case "001039":
                                                sluch.Element("USL").Element("CODE_USL").Value = "161083";
                                                break;
                                        }
                                    }
                                }
                                if (commSl2.Value.Equals("П"))
                                {
                                    switch (sluch.Element("USL").Element("CODE_USL").Value)
                                    {
                                        case "001005":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161031";
                                            break;
                                        case "001007":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061045";
                                            break;
                                        case "001008":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161023";
                                            break;
                                        case "001010":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061053";
                                            break;
                                        case "001012":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161001";
                                            break;
                                        case "001013":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061031";
                                            break;
                                        case "001015":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161010";
                                            break;
                                        case "001016":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061061";
                                            break;
                                        case "001019":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161006";
                                            break;
                                        case "001020":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161014";
                                            break;
                                        case "001023":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061020";
                                            break;
                                        case "001025":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061057";
                                            break;
                                        case "001029":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061005";
                                            break;
                                        case "001031":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061011";
                                            break;
                                        case "001032":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161036";
                                            break;
                                        case "001033":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161044";
                                            break;
                                        case "001037":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061001";
                                            break;
                                        case "001038":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161026";
                                            break;
                                        case "001044":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061140";
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (sluch.Element("USL").Element("CODE_USL").Value)
                                    {
                                        case "001005":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161029";
                                            break;
                                        case "001007":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061046";
                                            break;
                                        case "001008":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161021";
                                            break;
                                        case "001010":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061054";
                                            break;
                                        case "001012":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161002";
                                            break;
                                        case "001013":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061032";
                                            break;
                                        case "001015":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161011";
                                            break;
                                        case "001016":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061107";
                                            break;
                                        case "001019":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161007";
                                            break;
                                        case "001020":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161015";
                                            break;
                                        case "001023":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061021";
                                            break;
                                        case "001025":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061058";
                                            break;
                                        case "001029":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061006";
                                            break;
                                        case "001031":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061103";
                                            break;
                                        case "001032":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161036";
                                            break;
                                        case "001033":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161044";
                                            break;
                                        case "001037":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061002";
                                            break;
                                        case "001038":
                                            sluch.Element("USL").Element("CODE_USL").Value = "161026";
                                            break;
                                        case "001044":
                                            sluch.Element("USL").Element("CODE_USL").Value = "061141";
                                            break;
                                    }
                                }
                                var homeUsl =
                                                "161029, 161079, 161045, 161040, 161037, 161021, 161015, 161011, 161007, 161002, 061137, 061107, 061103, 061079, 061058, 061054, 061046, 061050, 061043, 061032, 061021, 061015, 061006, 061002";
                                            if (homeUsl.Contains(sluch.Element("USL").Element("CODE_USL").Value))
                                            {
                                                commSl2.Value = "F62:8;";
                                            }
                                            else
                                            {
                                                commSl2.Value = "F62:7;";
                                            }
                                //sluch.Element("COMENTSL").Value = "F62:8;";

                                var usl = sluch.Element("USL");
                                if (usl != null)
                                {
                                    usl.Element("PROFIL").Value = "160";
                                    usl.Element("TARIF").Value = "613.59";
                                    usl.Element("SUMV_USL").Value = "613.59";
                                }
                            }
                        }
                        xml.Save(file);

                        counter++;
                    }
                }
                MessageBox.Show("Обработано " + counter + " файлов");

            }
            catch (Exception ex)
            {
                string message = String.Format("Не выполнено из-за ошибок: \r\n{0}\r\n. Подробности в логах", ex.Message);
                Messenger.WriteMessage(message);
                MessageBox.Show(message);
            }
        }
    }
}
