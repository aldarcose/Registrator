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
    [XafDisplayName("������ �� ������������")]
    public class DisablityData : BaseObject
    {
        public DisablityData(Session session) : base(session) { }
        
        [XafDisplayName("������������")]
        [ImmediatePostData(true)]
        public Invalidnost Invalidnost { get; set; }

        [XafDisplayName("������ ������������")]
        [Appearance("GroupDisabilityAppearence")]
        public GrupaInvalidnosti InvalidnostGroup { get; set; }

        [XafDisplayName("������� �������")]
        [Appearance("ChildDisabilityAppearence")]
        public bool InvalidDetstva { get; set; }

        [XafDisplayName("��������� �����")]
        public KategoriyaLgot Kategoriya { get; set; }

        [XafDisplayName("������")]
        public Lgota Lgota { get; set; }

        [XafDisplayName("������")]
        [VisibleInListView(true)]
        [PersistentAlias(@"Iif([Invalidnost]=0,'���',
                                Concat(
                                        Iif([InvalidnostGroup]=0,
                                              '�������� �� 18 ���' ,
                                               Iif([InvalidnostGroup]=1,
                                                       '1 ������' ,
                                                        Iif([InvalidnostGroup]=2,
                                                             '2 ������' ,
                                                             Iif([InvalidnostGroup]=3,
                                                                   '3 ������', '')
                                                           )
                                                  )
                                            ),
                                        Iif([InvalidDetstva],' ������� �������','')
                                    )
                            )")]
        public string Text
        {
            get { return (string)EvaluateAlias("Text"); }
        }

        public override string ToString()
        {
            if (Invalidnost == BusinessObjects.Invalidnost.Net)
                return "���";

            string group = "";
            string brokenChildhood = "";

            switch (InvalidnostGroup)
            {
                case GrupaInvalidnosti.Deti:
                    group = "�������� �� 18 ���";
                    break;
                case GrupaInvalidnosti.Pervaya:
                    group = "1 ������";
                    break;
                case GrupaInvalidnosti.Vtoraya:
                    group = "2 ������";
                    break;
                case GrupaInvalidnosti.Tretiya:
                    group = "3 ������";
                    break;
            }

            if (InvalidDetstva)
                brokenChildhood = "������� �������";

            return string.Format("{0}; {1}", group, brokenChildhood);
        }
    }
}
