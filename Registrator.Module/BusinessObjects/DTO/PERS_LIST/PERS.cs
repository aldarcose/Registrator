using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.DTO
{
    [DefaultClassOptions]
    public partial class PERS : BaseObject
    {
        public PERS()
        {
        }
        public PERS(Session session)
            : base(session)
        {
        }

        [Association("PERS_PERSLIST")]
        public PERS_LIST PERS_LIST { get; set; }

        private string iD_PACField;

        private string fAMField;

        private string imField;

        private string otField;

        private int wField;

        private string drField;

        private string mrField;

        private string dOCTYPEField;

        private string dOCSERField;

        private string dOCNUMField;

        private string oKATOGField;

        private string oKATOPField;

        /// <summary>
        /// Код записи о пациенте
        /// Соответствует анало-гичному номеру в файле со сведениями счетов об оказанной медицинской помо-щи.
        /// </summary>
        [Size(36)]
        public string ID_PAC
        {
            get { return this.iD_PACField; }
            set { SetPropertyValue("ID_PAC", ref iD_PACField, value); }
        }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Size(40)]
        public string FAM
        {
            get { return this.fAMField; }
            set { SetPropertyValue("FAM", ref this.fAMField, value); }
        }

        /// <remarks/>
        [Size(40)]
        public string IM
        {
            get { return this.imField; }
            set { SetPropertyValue("IM", ref this.imField, value); }
        }

        /// <remarks/>
        [Size(40)]
        public string OT
        {
            get { return this.otField; }
            set { SetPropertyValue("OT", ref this.otField, value); }
        }
                
        /// <summary>
        /// Пол
        /// </summary>
        public int W
        {
            get { return this.wField; }
            set { SetPropertyValue("W", ref this.wField, value); }
        }

        /// <summary>
        /// Дата рождения пациента
        /// </summary>        
        public string DR
        {
            get { return this.drField; }
            set { SetPropertyValue("DR", ref this.drField, value); }
        }

        //private List<Dost> dost;
        ///// <summary>
        ///// Код надёжности идентификации пациента
        ///// Поле повторяется столько раз, сколько особых случаев имеет место.
        ///// </summary>
        //public List<Dost> DOST
        //{
        //    get { return dost; }
        //    set { SetPropertyValue("DOST", ref dost, value); }
        //}

        private string fam_p;
        /// <summary>
        /// Фамилия
        /// </summary>
        [Size(40)]
        public string FAM_P
        {
            get { return this.fam_p; }
            set { SetPropertyValue("FAM_P", ref fam_p, value); }
        }

        private string im_p;
        /// <remarks/>
        [Size(40)]
        public string IM_P
        {
            get { return this.im_p; }
            set { SetPropertyValue("IM_P", ref im_p, value); }
        }

        private string ot_p;
        /// <remarks/>
        [Size(40)]
        public string OT_P
        {
            get { return this.ot_p; }
            set { SetPropertyValue("OT_P", ref ot_p, value); }
        }

        private int w_p;
        /// <summary>
        /// Пол
        /// </summary>
        public int W_P
        {
            get { return this.w_p; }
            set { SetPropertyValue("W_P", ref this.w_p, value); }
        }

        private string dr_p;
        /// <summary>
        /// Дата рождения пациента
        /// </summary>        
        public string DR_P
        {
            get { return this.drField; }
            set { SetPropertyValue("DR_P", ref this.dr_p, value); }
        }

        //private XPCollection<Dost> dost_p;
        ///// <summary>
        ///// Код надёжности идентификации пациента
        ///// Поле повторяется столько раз, сколько особых случаев имеет место.
        ///// </summary>
        //public XPCollection<Dost> DOST_P
        //{
        //    get { return dost_p; }
        //    set { SetPropertyValue("DOST_P", ref dost_p, value); }
        //}
        /// <summary>
        /// Место рождения пациента или представителя
        /// </summary>
        [Size(100)]
        public string MR
        {
            get { return this.mrField; }
            set { SetPropertyValue("MR", ref this.mrField, value); }
        }

        /// <summary>
        /// Тип документа, удостоверяющего личность пациента или представителя
        /// F011 «Классификатор типов документов, удостоверяющих личность».
        /// При указании ЕНП в соответствующем ос-новном файле, поле может не заполнять-ся.
        /// </summary>
        [Size(2)]
        public string DOCTYPE
        {
            get { return this.dOCTYPEField; }
            set { SetPropertyValue("DOCTYPE", ref this.dOCTYPEField, value); }
        }

        /// <summary>
        /// Серия документа, удостоверяющего личность пациента или представителя
        /// При указании ЕНП в соответствующем ос-новном файле, поле может не заполнять-ся.
        /// </summary>
        [Size(10)]     
        public string DOCSER
        {
            get { return this.dOCSERField; }
            set { SetPropertyValue("DOCSER", ref this.dOCSERField, value); }
        }

        /// <summary>
        /// Серия документа, удостоверяющего личность пациента или представителя
        /// При указании ЕНП в соответствующем ос-новном файле, поле может не заполнять-ся.
        /// </summary>
        [Size(20)]
        /// <remarks/>
        public string DOCNUM
        {
            get { return this.dOCNUMField; }
            set { SetPropertyValue("DOCNUM", ref this.dOCNUMField, value); }
        }

        private string snils;
        /// <summary>
        /// СНИЛСпациента или представителя.
        /// Указывается при наличии с разделителями.
        /// </summary>
        [Size(14)]
        public string SNILS
        {
            get { return snils; }
            set { SetPropertyValue("SNILS", ref snils, value); }

        }

        /// <summary>
        /// Код места жительства по ОКАТО
        /// Заполняется при наличии сведений
        /// </summary>
        [Size(11)]
        public string OKATOG
        {
            get { return this.oKATOGField; }
            set { SetPropertyValue("OKATOG", ref this.oKATOGField, value); }
        }


        /// <summary>
        /// Код места пребывания по ОКАТО
        /// Заполняется при наличии сведений
        /// </summary>
        [Size(11)]
        public string OKATOP
        {
            get { return this.oKATOPField; }
            set { SetPropertyValue("OKATOP", ref this.oKATOPField, value); }
        }
        
        private string comentp;       
        /// <summary>
        /// Служебное поле
        /// </summary>
        [Size(250)]
        public string COMENTP
        {
            get { return this.oKATOPField; }
            set { SetPropertyValue("COMENTP", ref this.comentp, value); }
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}, {3}", FAM, IM, OT, DR);
        }
    }

    public enum Dost
    {
        ОтсутствуетОтчество = 1,
        ОттутствуетФамилия = 2,
        ОтсутствуетИмя = 3,
        ИзвестенТолькоМесяцИГодДатыРождения = 4,
        ИзвестенТолькоГодДатыРождения = 5,
        ДатаРожденияНеСоответствуетКалендарю = 6
    }

}
