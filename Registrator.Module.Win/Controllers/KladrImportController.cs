using System;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using Registrator.Module.BusinessObjects;
using System.Windows.Forms;
using System.IO;
using DevExpress.Xpo;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class KladrImportController : ViewController
    {
        public KladrImportController()
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

        private int GetLevelFromCode(string Code)
        {
            if (!String.IsNullOrWhiteSpace(Code) && Code.Length == 13)
            {
                //int level4Count = 0;// 13;
                int level3Count = 5;// 8;
                int level2Count = 8;//5;
                int level1Count = 11;// 2;

                int count0 = 0; //количество нулей в окончании
                for (int i = 0; i < Code.Length; i++)
                {
                    if (Code[Code.Length - i - 1] == '0')
                    {
                        count0++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (count0 < level3Count) { return 4; }
                if (count0 >= level3Count && count0 < level2Count) { return 3; }
                if (count0 >= level2Count && count0 < level1Count) { return 2; }
                if (count0 >= level1Count) { return 1; }
            }
            return 0;
        }

        private void LoadKladrAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            string socrbaseFile = "SOCRBASE.dbf";
            string kladrFile = "KLADR.dbf";
            string streetFile = "STREET.dbf";


            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileNames = Directory.GetFiles(dialog.SelectedPath);
                var safeFileNames = fileNames.Select(X => Path.GetFileName(X).ToUpper());

                if (safeFileNames.Contains(socrbaseFile.ToUpper()) && safeFileNames.Contains(kladrFile.ToUpper()) && safeFileNames.Contains(streetFile.ToUpper()))
                {
                    //loading shortnames
                    var path = fileNames.FirstOrDefault(X => Path.GetFileName(X).ToUpper() == socrbaseFile.ToUpper());
                    LoadingShortNames(path);

                    //loading KLADR
                    path = fileNames.FirstOrDefault(X => Path.GetFileName(X).ToUpper() == kladrFile.ToUpper());

                    LoadingKladr(path);
                }
            }
        }

        private void LoadingKladr(string path)
        {
            if (path != null)
            {
                int counter = 0;
                int counterKladrRows = 0;
                using (Stream stream = File.OpenRead(path))
                {
                    NDbfReader.Table table = NDbfReader.Table.Open(stream);
                    var uow = new UnitOfWork(((DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjectSpace).Session.DataLayer);
                    var reader = table.OpenReader(Encoding.GetEncoding(866));
                    if (reader != null && uow != null)
                    {
                        while (reader.Read())
                        {
                            string code = reader.GetString("CODE");
                            bool isModified = false;

                            var kladr = uow.FindObject<Kladr>(new BinaryOperator("Code", code));
                            if (kladr == null)
                            {
                                //если нет элемента с таким кодом, то создать
                                int status = -1;
                                Int32.TryParse(reader.GetString("STATUS"), out status);

                                kladr = new Kladr(uow);
                                kladr.Code = code;
                                // Импорт записи
                                kladr.Name = reader.GetString("NAME"); ;
                                kladr.CodePost = reader.GetString("INDEX"); ;
                                kladr.CodeOkato = reader.GetString("OCATD"); ;
                                kladr.Status = status;
                                kladr.CodeIfns = reader.GetString("GNINMB");
                                kladr.CodeIfnsTerr = reader.GetString("UNO");

                                isModified = true;
                            }
                            if (kladr.Type == null)
                            {
                                kladr.Type = uow.FindObject<KladrType>(new BinaryOperator("ShortName", reader.GetString("SOCR")));
                                isModified = true;
                            }

                            if (kladr.Level == 0)
                            {
                                kladr.Level = GetLevelFromCode(code);
                                isModified = true;
                            }
                            // Ищем родителя
                            if (kladr.Parent == null && kladr.Level > 1)
                            //if (kladr.Level > 1)
                            {
                                kladr.Parent = GetParent(code, kladr.Level, uow);
                                isModified = true;
                            }
                            counterKladrRows++;

                            if (isModified)
                            {
                                counter++;
                            //    uow.Save(kladr);
                            //}

                            //if (counter % 100 == 0)
                            //{
                                uow.CommitChanges();
                                uow.Dispose();
                                uow = new UnitOfWork(((DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjectSpace).Session.DataLayer);
                                Console.WriteLine(String.Format("\r\n{0}\r\n{1}\r\n", counter, counterKladrRows));
                            }
                        } 
                        uow.CommitChanges();
                    }
                    uow.Dispose();                 
                }
                MessageBox.Show(String.Format(@"Всего записей в КЛАДР:{0} / Loaded/changed: {1}", counterKladrRows, counter));
            }
        }

        private void LoadingShortNames(string path)
        {
            if (path != null)
            {
                using (Stream stream = File.OpenRead(path))
                {
                    NDbfReader.Table table = NDbfReader.Table.Open(stream);
                    var reader = table.OpenReader(Encoding.GetEncoding(866));
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var type = ObjectSpace.FindObject<KladrType>(new BinaryOperator("Code", reader.GetString("KOD_T_ST")));
                            if (type == null)
                            {
                                type = ObjectSpace.CreateObject<KladrType>();
                                int level;
                                Int32.TryParse(reader.GetString("LEVEL"), out level);
                                type.Level = level;
                                type.ShortName = reader.GetString("SCNAME");
                                type.Name = reader.GetString("SOCRNAME");
                                type.Code = reader.GetString("KOD_T_ST");
                            }
                        }
                        ObjectSpace.CommitChanges();
                    }
                }
            }
        }

        private Kladr GetParent(string code, int level, UnitOfWork uow)
        {
            Kladr parent = null;
            int l = level;
            while (l > 1 && parent == null)
            {
                int count = l == 5 ? 11 : l == 4 ? 8 : l == 3 ? 5 : 2;
                string parentCode = code.Substring(0, count) +
                    "0000000000000".Substring(0, code.Length - count);// +code.Substring(code.Length - 2, 2);
                parent = uow.FindObject<Kladr>(new BinaryOperator("Code", parentCode));
                l--;
            }
            //if (parent == null)
            //    throw new KeyNotFoundException("Не найдена родительская запись: " + code);
            return parent;
        }
    }
}
