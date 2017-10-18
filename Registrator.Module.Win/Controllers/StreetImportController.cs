using System;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using Registrator.Module.BusinessObjects;
using System.Windows.Forms;
using System.IO;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Dictionaries;

namespace Registrator.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class StreetImportController : ViewController
    {
        public StreetImportController()
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

        private void LoadStreetAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var dialog = new OpenFileDialog();
            string streetFile = "STREET.dbf";
            
            if (dialog.ShowDialog() == DialogResult.OK && dialog.SafeFileName.ToUpper()== streetFile.ToUpper())
            {
                LoadingStreets(dialog.FileName);
            }
        }

        private void LoadingStreets(string path)
        {
            if (path != null)
            {
                int counter = 1;
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
                            //if (code.Substring(0, 2) != "03")
                            //{
                            //    continue;
                            //} // only Buryatia

                            bool isModified = false;

                            var street = uow.FindObject<Street>(new BinaryOperator("Code", code));
                            if (street == null)
                            {
                                //если нет элемента с таким кодом, то создать

                                street = new Street(uow);
                                street.Code = code;
                                // Импорт записи
                                street.Name = reader.GetString("NAME"); ;
                                street.CodePost = reader.GetString("INDEX"); ;
                                street.CodeOkato = reader.GetString("OCATD"); ;
                                //street.Status = status;
                                street.CodeIfns = reader.GetString("GNINMB");
                                street.CodeIfnsTerr = reader.GetString("UNO");
                                
                                if (street.Type == null)
                                {
                                    street.Type = uow.FindObject<KladrType>(new BinaryOperator("ShortName", reader.GetString("SOCR")));
                                    isModified = true;
                                }

                                // Ищем родителя
                                if (street.City == null)
                                //if (kladr.Level > 1)
                                {
                                    street.City = GetParent(code, uow);
                                    isModified = true;
                                }
                                isModified = true;
                            }
                            counterKladrRows++;
                            
                            if (counter % 1000 == 0)
                            {
                                uow.CommitChanges();
                                uow.Dispose();
                                uow = new UnitOfWork(((DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjectSpace).Session.DataLayer);
                                Console.WriteLine(String.Format("\r\n{0}\r\n{1}\r\n", counter, counterKladrRows));
                            }
                            if (isModified)
                            {
                                counter++;
                            //    uow.Save(kladr);
                            //}
                            }
                        } 
                        uow.CommitChanges();
                    }
                    uow.Dispose();                 
                }
                MessageBox.Show(String.Format(@"Всего записей в КЛАДР:{0} / Loaded/changed: {1}", counterKladrRows, counter));
            }
        }
        
        private Kladr GetParent(string code, UnitOfWork uow)
        {
            Kladr parent = null;
            int l = 5;
            while (l > 1 && parent == null)
            {
                int count = l == 5 ? 11 : l == 4 ? 8 : l == 3 ? 5 : 2;
                string parentCode = code.Substring(0, count) +
                    "0000000000000".Substring(0, code.Length - count - 4);
                parent = uow.FindObject<Kladr>(new BinaryOperator("Code", parentCode));
                l--;
            }
            //if (parent == null)
            //    throw new KeyNotFoundException("Не найдена родительская запись: " + code);
            return parent;
        }
    }
}
