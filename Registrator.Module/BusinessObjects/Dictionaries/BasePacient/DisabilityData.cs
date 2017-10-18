using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    [DefaultClassOptions]
    [DefaultProperty("Text")]
    [XafDisplayName("Данные по инвалидности")]
    public class DisablityData : BaseObject
    {
        public DisablityData(Session session) : base(session) { }
        
        [XafDisplayName("Инвалидность")]
        [ImmediatePostData(true)]
        public Invalidnost Invalidnost { get; set; }

        [XafDisplayName("Группа инвалидности")]
        [Appearance("GroupDisabilityAppearence")]
        public GrupaInvalidnosti InvalidnostGroup { get; set; }

        [XafDisplayName("Инвалид детства")]
        [Appearance("ChildDisabilityAppearence")]
        public bool InvalidDetstva { get; set; }

        [XafDisplayName("Категория льгот")]
        public KategoriyaLgot Kategoriya { get; set; }

        [XafDisplayName("Льгота")]
        public Lgota Lgota { get; set; }

        [XafDisplayName("Сводка")]
        [VisibleInListView(true)]
        [PersistentAlias(@"Iif([Invalidnost]=0,'Нет',
                                Concat(
                                        Iif([InvalidnostGroup]=0,
                                              'Инвалиды до 18 лет' ,
                                               Iif([InvalidnostGroup]=1,
                                                       '1 группы' ,
                                                        Iif([InvalidnostGroup]=2,
                                                             '2 группы' ,
                                                             Iif([InvalidnostGroup]=3,
                                                                   '3 группы', '')
                                                           )
                                                  )
                                            ),
                                        Iif([InvalidDetstva],' Инвалид детства','')
                                    )
                            )")]
        public string Text
        {
            get { return (string)EvaluateAlias("Text"); }
        }

        public override string ToString()
        {
            if (Invalidnost == BusinessObjects.Invalidnost.Net)
                return "Нет";

            string group = "";
            string brokenChildhood = "";

            switch (InvalidnostGroup)
            {
                case GrupaInvalidnosti.Deti:
                    group = "Инвалиды до 18 лет";
                    break;
                case GrupaInvalidnosti.Pervaya:
                    group = "1 группы";
                    break;
                case GrupaInvalidnosti.Vtoraya:
                    group = "2 группы";
                    break;
                case GrupaInvalidnosti.Tretiya:
                    group = "3 группы";
                    break;
            }

            if (InvalidDetstva)
                brokenChildhood = "Инвалид детства";

            return string.Format("{0}; {1}", group, brokenChildhood);
        }
    }
}
