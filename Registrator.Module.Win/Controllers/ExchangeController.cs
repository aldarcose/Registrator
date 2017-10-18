using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using Registrator.Module.BusinessObjects.DTO;
using DevExpress.Xpo;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class ExchangeController : ViewController
    {
        public ExchangeController()
        {
            InitializeComponent();
            RegisterActions(components);
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void LoadXMLAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK) 
            {
                using (Stream stream = dialog.OpenFile())
                {
                    UnitOfWork uow = new UnitOfWork(((DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjectSpace).Session.DataLayer);
                    XDocument doc = XDocument.Load(stream);
                    if (doc.Root.Name != "PERS_LIST")
                    {
                        throw new ArgumentException("PERS_LIST");
                    }
                    var pers_list = new PERS_LIST(uow);
                    //PERS_LIST pers_list = ObjectSpace.CreateObject<PERS_LIST>();

                    //заголовок
                    XElement xzglv = doc.Root.Element("ZGLV");
                    if (xzglv != null)
                    {
                        var zglv = new ZGLV(uow);
                        //var zglv = ObjectSpace.CreateObject<ZGLV>();
                        zglv.VERSION = GetString(xzglv.Element("VERSION"), true);
                        zglv.DATA = GetString(xzglv.Element("DATA"), true);
                        zglv.FILENAME = GetString(xzglv.Element("FILENAME"), true);
                        zglv.FILENAME1 = GetString(xzglv.Element("FILENAME1"), false);

                        pers_list.ZGLV = zglv;
                    }
                    else
                    {
                        throw new ArgumentException("ZGLV");
                    }

                    int counter = 0;
                    //Список персов
                    foreach(var xpers in doc.Root.Elements("PERS"))
                    {
                        var pers = new PERS(uow);
                        //var pers = ObjectSpace.CreateObject<PERS>();

                        pers.ID_PAC = GetString(xpers.Element("ID_PAC"), true);
                        pers.FAM = GetString(xpers.Element("FAM"), false);
                        pers.IM = GetString(xpers.Element("IM"), false);
                        pers.OT = GetString(xpers.Element("OT"), false);
                        pers.W = GetNumber(xpers.Element("W"), true);
                        pers.DR = GetString(xpers.Element("DR"), false);

                        //foreach (var xdost in xpers.Elements("DOST"))
                        //{
                        //    Dost dost = (Dost)GetNumber(xdost, true);
                        //    pers.DOST.Add(dost);
                        //}

                        pers.FAM_P = GetString(xpers.Element("FAM_P"), false);
                        pers.IM_P = GetString(xpers.Element("IM_P"), false);
                        pers.OT_P = GetString(xpers.Element("OT_P"), false);
                        pers.W_P = GetNumber(xpers.Element("W_P"), false);
                        pers.DR_P = GetString(xpers.Element("DR_P"), false);
                        
                        //foreach (var xdost in xpers.Elements("DOST_P"))
                        //{
                        //    Dost dost = (Dost)GetNumber(xdost, true);
                        //    pers.DOST_P.Add(dost);
                        //}

                        pers.MR = GetString(xpers.Element("MR"), false);
                        pers.DOCTYPE = GetString(xpers.Element("DOCTYPE"), false);
                        pers.DOCSER = GetString(xpers.Element("DOCSER"), false);
                        pers.DOCNUM = GetString(xpers.Element("DOCNUM"), false);
                        pers.SNILS = GetString(xpers.Element("SNILS"), false);
                        pers.OKATOG = GetString(xpers.Element("OKATOG"), false);
                        pers.OKATOP = GetString(xpers.Element("OKATOP"), false);
                        pers.COMENTP = GetString(xpers.Element("COMENTP"), false);
                        //pers.PERS_LIST = pers_list;
                        pers_list.People.Add(pers);

                        counter++;
                        if (counter % 1000 == 0)
                        {
                            uow.CommitChanges();
                            uow.Dispose();
                            uow = new UnitOfWork(((DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjectSpace).Session.DataLayer);
                            pers_list = uow.GetObjectByKey<PERS_LIST>(pers_list.Oid);
                        }
                    }
                    uow.CommitChanges();
                    MessageBox.Show("Загрузка завершена. Всего записей : " + counter);
                }
            }
        }

        private string GetString(XElement el, bool Required)
        {
            if (Required && el == null)
            {
                throw new ArgumentNullException("el", "Ошибка #1000: Не найден обязательный элемент");
            }
            if (Required && String.IsNullOrWhiteSpace(el.Value))
            {
                throw new ArgumentException("el", "Ошибка #1001: Обязательный элемент не содержит значений");
            }
            return el != null ? el.Value : null;
        }

        private int GetNumber(XElement el, bool Required)
        {
            if (Required && el == null)
            {
                throw new ArgumentNullException("el", "Ошибка #1000: Не найден обязательный элемент");
            }
            if (Required && String.IsNullOrWhiteSpace(el.Value))
            {
                throw new ArgumentException("el", "Ошибка #1001: Обязательный элемент не содержит значений");
            }
            int num;
            if (el != null && Int32.TryParse(el.Value, out num))
            {
                return num;
            }

            return 0;
        }

        private XElement AddXElement(XElement list, string name, string value, bool required)
        {            
            if (required && String.IsNullOrWhiteSpace(value))
            {
                list.Add(new XElement(name));
            }

            if (!String.IsNullOrWhiteSpace(value))
            {
                list.Add(new XElement(name) { Value = value });
            }

            return list;
        }

        private void SaveLAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int counter = 0;
                if (View.CurrentObject is PERS_LIST)
                {
                    PERS_LIST list = (PERS_LIST)View.CurrentObject;

                    XElement pers_list = new XElement("PERS_LIST");
                    XElement xzglv = new XElement("ZGLV");
                    xzglv.Add(new XElement("VERSION") { Value = list.ZGLV.VERSION });
                    xzglv.Add(new XElement("DATA") { Value = list.ZGLV.DATA });
                    xzglv.Add(new XElement("FILENAME") { Value = list.ZGLV.FILENAME });
                    xzglv.Add(new XElement("FILENAME1") { Value = list.ZGLV.FILENAME1 });
                    pers_list.AddFirst(xzglv);

                    foreach (PERS pers in list.People)
                    {
                        XElement xpers = new XElement("PERS");
                        xpers = AddXElement(xpers, "ID_PAC", pers.ID_PAC, true);
                        xpers = AddXElement(xpers, "FAM", pers.FAM, false);
                        xpers = AddXElement(xpers, "IM", pers.IM, false);
                        xpers = AddXElement(xpers, "OT", pers.OT, false);
                        xpers = AddXElement(xpers, "W", Convert.ToString(pers.W), false);
                        xpers = AddXElement(xpers, "DR", pers.DR, false);
                        xpers = AddXElement(xpers, "FAM_P", pers.FAM_P, false);
                        xpers = AddXElement(xpers, "IM_P", pers.IM_P, false);
                        xpers = AddXElement(xpers, "OT_P", pers.OT_P, false);
                        xpers = AddXElement(xpers, "W_P", Convert.ToString(pers.W_P), false);
                        xpers = AddXElement(xpers, "DR_P", pers.DR_P, false);
                        xpers = AddXElement(xpers, "MR", pers.MR, true);
                        xpers = AddXElement(xpers, "DOCTYPE", pers.DOCTYPE, false);
                        xpers = AddXElement(xpers, "DOCSER", pers.DOCSER, false);
                        xpers = AddXElement(xpers, "DOCNUM", pers.DOCNUM, false);
                        xpers = AddXElement(xpers, "SNILS", pers.SNILS, false);
                        xpers = AddXElement(xpers, "OKATOG", pers.OKATOG, false);
                        xpers = AddXElement(xpers, "OKATOP", pers.OKATOP, false);
                        xpers = AddXElement(xpers, "COMENTP", pers.COMENTP, false);

                        pers_list.Add(xpers);
                        counter++;
                    }

                    using (Stream stream = dialog.OpenFile())
                    {
                        XDeclaration decl = new XDeclaration("1.0", "windows-1251", "");
                        XDocument doc =  new XDocument(decl, pers_list);
                        //doc.Declaration = decl;
                        doc.Save(stream);
                    }
                }

                MessageBox.Show("Выгрузка завершена. Всего записей : " + counter);
            }
        }

        private void SaveHAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int counter = 0;
                if (View.CurrentObject is ZL_LIST)
                {
                    ZL_LIST list = (ZL_LIST)View.CurrentObject;

                    XElement zl_list = new XElement("ZL_LIST");
                    XElement xzglv = new XElement("ZGLV");
                    xzglv.Add(new XElement("VERSION") { Value = list.ZGLV.VERSION });
                    xzglv.Add(new XElement("DATA") { Value = list.ZGLV.DATA });
                    xzglv.Add(new XElement("FILENAME") { Value = list.ZGLV.FILENAME });
                    zl_list.AddFirst(xzglv);

                    XElement xschet = new XElement("SCHET");
                    xschet = AddXElement(xschet, "CODE", list.SCHET.CODE, true);
                    xschet = AddXElement(xschet, "CODE_MO", list.SCHET.CODE_MO, true);
                    xschet = AddXElement(xschet, "YEAR", list.SCHET.YEAR, false);
                    xschet = AddXElement(xschet, "MONTH", list.SCHET.MONTH, false);
                    xschet = AddXElement(xschet, "NSCHET", list.SCHET.NSCHET, false);
                    xschet = AddXElement(xschet, "DSCHET", list.SCHET.DSCHET, false);
                    xschet = AddXElement(xschet, "PLAT", list.SCHET.PLAT, false);
                    xschet = AddXElement(xschet, "SUMMAV", list.SCHET.SUMMAV, false);
                    xschet = AddXElement(xschet, "COMENTS", list.SCHET.COMENTS, false);
                    xschet = AddXElement(xschet, "SUMMAP", list.SCHET.SUMMAP, false);
                    xschet = AddXElement(xschet, "SANK_MEK", list.SCHET.SANK_MEK, false);
                    xschet = AddXElement(xschet, "SANK_MEE", list.SCHET.SANK_MEE, false);
                    xschet = AddXElement(xschet, "SANK_EKMP", list.SCHET.SANK_EKMP, false);
                    zl_list.Add(xschet);

                    foreach (ZAP zap in list.ZAPS)
                    {
                        XElement xzap = new XElement("ZAP");
                        xzap = AddXElement(xzap, "N_ZAP", zap.N_ZAP, true);
                        xzap = AddXElement(xzap, "PR_NOV", zap.PR_NOV, false);

                        var pacient = zap.PACIENT;
                        if (pacient != null)
                        {
                            XElement xpacient = new XElement("PACIENT");
                            xpacient = AddXElement(xpacient, "ID_PAC", pacient.ID_PAC, true);
                            xpacient = AddXElement(xpacient, "VPOLIS", pacient.VPOLIS, true);
                            xpacient = AddXElement(xpacient, "SPOLIS", pacient.SPOLIS, false);
                            xpacient = AddXElement(xpacient, "NPOLIS", pacient.NPOLIS, true);
                            xpacient = AddXElement(xpacient, "ST_OKATO", pacient.ST_OKATO, false);
                            xpacient = AddXElement(xpacient, "SMO", pacient.SMO, false);
                           // xpacient = AddXElement(xpacient, "SMO_OGRN", pacient.SMO_OGRN, false);
                            xpacient = AddXElement(xpacient, "SMO_OK", pacient.SMO_OK, false);
                            xpacient = AddXElement(xpacient, "SMO_NAM", pacient.SMO_NAM, false);
                            xpacient = AddXElement(xpacient, "NOVOR", pacient.NOVOR, false);
                            xpacient = AddXElement(xpacient, "VNOV_D", pacient.VNOV_D, false);

                            xzap.Add(xpacient);
                        }

                        foreach (var sluch in zap.SLUCH)
                        {
                            XElement xsluch = new XElement("SLUCH");
                            xsluch = AddXElement(xsluch, "IDCASE", sluch.IDCASE, false);
                            xsluch = AddXElement(xsluch, "USL_OK", sluch.USL_OK, false);
                            xsluch = AddXElement(xsluch, "VIDPOM", sluch.VIDPOM, false);
                            xsluch = AddXElement(xsluch, "FOR_POM", sluch.FOR_POM, false);
                            xsluch = AddXElement(xsluch, "NPR_MO", sluch.NPR_MO, false);
                            xsluch = AddXElement(xsluch, "EXTR", sluch.EXTR, false);
                            xsluch = AddXElement(xsluch, "LPU", sluch.LPU, false);
                            xsluch = AddXElement(xsluch, "LPU_1", sluch.LPU_1, false);
                            xsluch = AddXElement(xsluch, "PODR", sluch.PODR, false);
                            xsluch = AddXElement(xsluch, "PROFIL", sluch.PROFIL, false);
                            xsluch = AddXElement(xsluch, "DET", sluch.DET, false);
                            xsluch = AddXElement(xsluch, "NHISTORY", sluch.NHISTORY, false);
                            xsluch = AddXElement(xsluch, "DATE_1", sluch.DATE_1, false);
                            xsluch = AddXElement(xsluch, "DATE_2", sluch.DATE_2, false);
                            xsluch = AddXElement(xsluch, "DS0", sluch.DS0, false);
                            xsluch = AddXElement(xsluch, "DS1", sluch.DS1, false);
                            //xsluch = AddXElement(xsluch, "DS2", sluch.DS2, false);
                            //xsluch = AddXElement(xsluch, "DS3", sluch.DS3, false);
                            xsluch = AddXElement(xsluch, "VNOV_M", sluch.VNOV_M, false);
                            xsluch = AddXElement(xsluch, "CODE_MES1", sluch.CODE_MES1, false);
                            xsluch = AddXElement(xsluch, "CODE_MES1", sluch.CODE_MES2, false);
                            xsluch = AddXElement(xsluch, "RSLT", sluch.RSLT, false);
                            xsluch = AddXElement(xsluch, "ISHOD", sluch.ISHOD, false);
                            xsluch = AddXElement(xsluch, "PRVS", sluch.PRVS, false);
                            xsluch = AddXElement(xsluch, "VERS_SPEC", sluch.VERS_SPEC, false);
                            xsluch = AddXElement(xsluch, "IDDOKT", sluch.IDDOKT, true);
                            xsluch = AddXElement(xsluch, "OS_SLUCH", sluch.OS_SLUCH, false);
                            xsluch = AddXElement(xsluch, "IDSP", sluch.IDSP, true);
                            xsluch = AddXElement(xsluch, "ED_COL", sluch.ED_COL, false);
                            xsluch = AddXElement(xsluch, "TARIF", sluch.TARIF, false);
                            xsluch = AddXElement(xsluch, "SUMV", sluch.SUMV, true);
                            xsluch = AddXElement(xsluch, "OPLATA", sluch.OPLATA, false);
                            xsluch = AddXElement(xsluch, "SUMP", sluch.SUMP, false);
                            xsluch = AddXElement(xsluch, "SANK_IT", sluch.SANK_IT, false);
                            

                            foreach (var usl in sluch.USL)
                            {
                                XElement xusl = new XElement("USL");

                                xusl = AddXElement(xusl, "IDSERV", usl.IDSERV, true);
                                xusl = AddXElement(xusl, "LPU", usl.LPU, true);
                                xusl = AddXElement(xusl, "LPU_1", usl.LPU_1, false);
                                xusl = AddXElement(xusl, "PODR", usl.PODR, false);
                                xusl = AddXElement(xusl, "PROFIL", usl.PROFIL, false);
                                xusl = AddXElement(xusl, "VID_VME", usl.VID_VME, false);
                                xusl = AddXElement(xusl, "DET", usl.DET, false);
                                xusl = AddXElement(xusl, "DATE_IN", usl.DATE_IN, false);
                                xusl = AddXElement(xusl, "DATE_OUT", usl.DATE_OUT, false);
                                xusl = AddXElement(xusl, "DS", usl.DS, true);
                                xusl = AddXElement(xusl, "CODE_USL", usl.CODE_USL, false);
                                xusl = AddXElement(xusl, "KOL_USL", usl.KOL_USL, false);
                                xusl = AddXElement(xusl, "TARIF", usl.TARIF, false);
                                xusl = AddXElement(xusl, "SUMV_USL", usl.SUMV_USL, false);
                                xusl = AddXElement(xusl, "PRVS", usl.PRVS, false);
                                xusl = AddXElement(xusl, "CODE_MD", usl.CODE_MD, false);
                                xusl = AddXElement(xusl, "COMENTU", usl.COMENTU, false);

                                xsluch.Add(xusl);
                            }
                            xsluch = AddXElement(xsluch, "COMENTSL", sluch.COMENTSL, false);
                            xzap.Add(xsluch);
                        }
                        zl_list.Add(xzap);
                        counter++;
                    }

                    using (Stream stream = dialog.OpenFile())
                    {
                        XDeclaration decl = new XDeclaration("1.0", "windows-1251", "");
                        XDocument doc = new XDocument(decl, zl_list);
                        doc.Save(stream);
                    }
                }

                MessageBox.Show("Выгрузка завершена. Всего записей : " + counter);
            }
        }

        private void LoadHAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = dialog.OpenFile())
                {
                    UnitOfWork uow = new UnitOfWork(((DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjectSpace).Session.DataLayer);
                    XDocument doc = XDocument.Load(stream);
                    if (doc.Root.Name != "ZL_LIST")
                    {
                        throw new ArgumentException("ZL_LIST");
                    }
                    //ZL_LIST zl_list = ObjectSpace.CreateObject<ZL_LIST>();
                    var zl_list = new ZL_LIST(uow);

                    //заголовок
                    XElement xzglv = doc.Root.Element("ZGLV");
                    if (xzglv != null)
                    {
                        var zglv = new ZGLV(uow);
                        //var zglv = ObjectSpace.CreateObject<ZGLV>();
                        zglv.VERSION = GetString(xzglv.Element("VERSION"), true);
                        zglv.DATA = GetString(xzglv.Element("DATA"), true);
                        zglv.FILENAME = GetString(xzglv.Element("FILENAME"), true);
                        //zglv.FILENAME1 = GetString(xzglv.Element("FILENAME1"), false);

                        zl_list.ZGLV = zglv;
                    }
                    else
                    {
                        throw new ArgumentException("ZGLV");
                    }

                    //счет
                    XElement xschet = doc.Root.Element("SCHET");
                    if (xschet != null)
                    {
                        var schet = new SCHET(uow);
                        //SCHET schet = ObjectSpace.CreateObject<SCHET>();
                        schet.CODE = GetString(xschet.Element("CODE"), true);
                        schet.CODE_MO = GetString(xschet.Element("CODE_MO"), true);
                        schet.YEAR = GetString(xschet.Element("YEAR"), false);
                        schet.MONTH = GetString(xschet.Element("MONTH"), false);
                        schet.NSCHET = GetString(xschet.Element("NSCHET"), false);
                        schet.DSCHET = GetString(xschet.Element("DSCHET"), false);
                        schet.PLAT = GetString(xschet.Element("PLAT"), false);
                        schet.SUMMAV = GetString(xschet.Element("SUMMAV"), false);
                        schet.COMENTS = GetString(xschet.Element("COMENTS"), false);
                        schet.SUMMAP = GetString(xschet.Element("SUMMAP"), false);
                        schet.SANK_MEK = GetString(xschet.Element("SANK_MEK"), false);
                        schet.SANK_MEE = GetString(xschet.Element("SANK_MEE"), false);
                        schet.SANK_EKMP = GetString(xschet.Element("SANK_EKMP"), false);
                        zl_list.SCHET = schet;
                    }

                    int counter = 0;
                    //Список записей
                    foreach (var xzap in doc.Root.Elements("ZAP"))
                    {
                        //var zap = ObjectSpace.CreateObject<ZAP>();
                        var zap = new ZAP(uow);

                        zap.N_ZAP = GetString(xzap.Element("N_ZAP"), true);
                        zap.PR_NOV = GetString(xzap.Element("PR_NOV"), true);

                        var xpacient = xzap.Element("PACIENT");
                        if (xpacient != null)
                        {
                            //PACIENT pacient = ObjectSpace.CreateObject<PACIENT>();
                            var pacient = new PACIENT(uow);

                            pacient.ID_PAC = GetString(xpacient.Element("ID_PAC"), true);
                            pacient.VPOLIS = GetString(xpacient.Element("VPOLIS"), true);
                            pacient.SPOLIS = GetString(xpacient.Element("SPOLIS"), false);
                            pacient.NPOLIS = GetString(xpacient.Element("NPOLIS"), true);
                            pacient.ST_OKATO = GetString(xpacient.Element("ST_OKATO"), false);
                            pacient.SMO = GetString(xpacient.Element("SMO"), false);
                            pacient.SMO_OGRN = GetString(xpacient.Element("SMO_OGRN"), false);
                            pacient.SMO_OK = GetString(xpacient.Element("SMO_OK"), false);
                            pacient.SMO_NAM = GetString(xpacient.Element("SMO_NAM"), false);
                            pacient.NOVOR = GetString(xpacient.Element("NOVOR"), false);
                            pacient.VNOV_D = GetString(xpacient.Element("VNOV_D"), false);
                            
                            zap.PACIENT = pacient;
                        }

                        foreach(var xsluch in xzap.Elements("SLUCH"))
                        {
                            var sluch = new SLUCH(uow);
                            //SLUCH sluch = ObjectSpace.CreateObject<SLUCH>();

                            sluch.IDCASE = GetString(xsluch.Element("IDCASE"), true);
                            sluch.USL_OK = GetString(xsluch.Element("USL_OK"), false);
                            sluch.VIDPOM = GetString(xsluch.Element("VIDPOM"), false);
                            sluch.FOR_POM = GetString(xsluch.Element("FOR_POM"), false);
                            sluch.NPR_MO = GetString(xsluch.Element("NPR_MO"), false);
                            sluch.EXTR = GetString(xsluch.Element("EXTR"), false);
                            sluch.LPU = GetString(xsluch.Element("LPU"), false);
                            sluch.LPU_1 = GetString(xsluch.Element("LPU_1"), false);
                            sluch.PODR = GetString(xsluch.Element("PODR"), false);
                            sluch.PROFIL = GetString(xsluch.Element("PROFIL"), false);
                            sluch.DET = GetString(xsluch.Element("DET"), false);
                            sluch.NHISTORY = GetString(xsluch.Element("NHISTORY"), false);
                            sluch.DATE_1 = GetString(xsluch.Element("DATE_1"), false);
                            sluch.DATE_2 = GetString(xsluch.Element("DATE_2"), false);
                            sluch.DS0 = GetString(xsluch.Element("DS0"), false);
                            sluch.DS1 = GetString(xsluch.Element("DS1"), false);
                            //sluch.DS2 = GetString(xsluch.Element("DS2"), false);
                            //sluch.DS3 = GetString(xsluch.Element("DS3"), false);
                            sluch.VNOV_M = GetString(xsluch.Element("VNOV_M"), false);
                            sluch.CODE_MES1 = GetString(xsluch.Element("CODE_MES1"), false);
                            sluch.CODE_MES2 = GetString(xsluch.Element("CODE_MES2"), false);
                            sluch.RSLT = GetString(xsluch.Element("RSLT"), false);
                            sluch.ISHOD = GetString(xsluch.Element("ISHOD"), false);
                            sluch.PRVS = GetString(xsluch.Element("PRVS"), false);
                            sluch.VERS_SPEC = GetString(xsluch.Element("VERS_SPEC"), false);
                            sluch.IDDOKT = GetString(xsluch.Element("IDDOKT"), false);
                            sluch.OS_SLUCH = GetString(xsluch.Element("OS_SLUCH"), false);
                            sluch.IDSP = GetString(xsluch.Element("IDSP"), false);
                            sluch.ED_COL = GetString(xsluch.Element("ED_COL"), false);
                            sluch.TARIF = GetString(xsluch.Element("TARIF"), false);
                            sluch.SUMV = GetString(xsluch.Element("SUMV"), false);
                            sluch.OPLATA = GetString(xsluch.Element("OPLATA"), false);
                            sluch.SUMP = GetString(xsluch.Element("SUMP"), false);
                            sluch.SANK_IT = GetString(xsluch.Element("SANK_IT"), false);
                            sluch.SANK = GetString(xsluch.Element("SANK"), false);
                            sluch.COMENTSL = GetString(xsluch.Element("COMENTSL"), false);

                            foreach(var xusl in xsluch.Elements("USL"))
                            {
                                var usl = new USL(uow);
                                //USL usl = ObjectSpace.CreateObject<USL>();

                                usl.IDSERV = GetString(xusl.Element("IDSERV"), true);
                                usl.LPU = GetString(xusl.Element("LPU"), false);
                                usl.LPU_1 = GetString(xusl.Element("LPU_1"), false);
                                usl.PODR = GetString(xusl.Element("PODR"), false);
                                usl.PROFIL = GetString(xusl.Element("PROFIL"), false);
                                usl.VID_VME = GetString(xusl.Element("VID_VME"), false);
                                usl.DET = GetString(xusl.Element("DET"), false);
                                usl.DATE_IN = GetString(xusl.Element("DATE_IN"), false);
                                usl.DATE_OUT = GetString(xusl.Element("DATE_OUT"), false);
                                usl.DS = GetString(xusl.Element("DS"), false);
                                usl.CODE_USL = GetString(xusl.Element("CODE_USL"), false);
                                usl.KOL_USL = GetString(xusl.Element("KOL_USL"), false);
                                usl.TARIF = GetString(xusl.Element("TARIF"), false);
                                usl.SUMV_USL = GetString(xusl.Element("SUMV_USL"), false);
                                usl.PRVS = GetString(xusl.Element("PRVS"), false);
                                usl.CODE_MD = GetString(xusl.Element("CODE_MD"), false);
                                usl.COMENTU = GetString(xusl.Element("COMENTU"), false);

                                sluch.USL.Add(usl);
                            }

                            zap.SLUCH.Add(sluch);
                        }
                        zl_list.ZAPS.Add(zap);

                        counter++;
                        if (counter % 1000 == 0)
                        {
                            uow.CommitChanges();
                            uow.Dispose();
                            uow = new UnitOfWork(((DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjectSpace).Session.DataLayer);
                            zl_list = uow.GetObjectByKey<ZL_LIST>(zl_list.Oid);
                            //ObjectSpace.CommitChanges();
                        }
                    }
                    uow.CommitChanges();
                    //ObjectSpace.CommitChanges();
                    MessageBox.Show("Загрузка завершена. Всего записей : " + counter);
                }
            }
        }
    }
}
