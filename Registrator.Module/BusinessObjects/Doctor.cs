using System;
using System.Linq;
using System.Net.Mime;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.XtraPrinting.Native;
using Registrator.Module.BusinessObjects.Dictionaries;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base.General;
using System.Drawing;

namespace Registrator.Module.BusinessObjects
{
    /// <summary>
    /// Доктора
    /// </summary>
    [DefaultClassOptions]
    [DefaultProperty("FullName")]
    public class Doctor : BaseObject, ISecurityUser, IAuthenticationStandardUser, IOperationPermissionProvider
    {
        private bool changePasswordOnFirstLogon;
        private string userName = String.Empty;
        private string storedPassword;
        private bool isActive = true;
        private bool scheduling;

        public Doctor(Session session) : base(session) { }
       
        /// <summary>
        /// Фамилия
        /// </summary>
        [Size(100)]
        [XafDisplayName("Фамилия")]
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Size(100)]
        [XafDisplayName("Имя")]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [Size(100)]
        [XafDisplayName("Отчество")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        [XafDisplayName("Дата рождения")]
        public DateTime Birthdate { get; set; }

        [XafDisplayName("ФИО")]
        [VisibleInLookupListView(false)]
        [PersistentAlias("Concat(LastName, ' ', FirstName, ' ', MiddleName)")]
        public string FullName
        {
            get { return (string)EvaluateAlias("FullName"); }
        }

        /// <summary>
        /// Возможность создания расписания
        /// </summary>
        [VisibleInLookupListView(false)]
        [XafDisplayName("Возможность создания расписания")]
        public bool Scheduling
        {
            get { return scheduling; }
            set { SetPropertyValue("Scheduling", ref scheduling, value); }
        }

        /// <summary>
        /// Внутренний код (Совпадает с полным кодом врача, поэтому полный код не загружен)
        /// </summary>
        [XafDisplayName("Внутренний код врача")]
        public int? InnerCode { get; set; }
        
        /// <summary>
        /// Федеральный код
        /// </summary>
        [XafDisplayName("Федеральный код врача")]
        public string FederalCode { get; set; }

        /// <summary>
        /// Номер СНИЛС
        /// </summary>
        [XafDisplayName("Номер СНИЛС")]
        [ModelDefault("EditMask", "000-000-000 00")]
        [Size(14)]
        [RuleRequiredField("", DefaultContexts.Save, "Укажите СНИЛС", ResultType = ValidationResultType.Warning)]
        public string SNILS { get; set; }

        /// <summary>
        /// Дата приема на работу
        /// </summary>
        [XafDisplayName("Дата приема")]
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Дата увольнения
        /// </summary>
        [XafDisplayName("Дата увольнения")]        
        public DateTime DismissDate { get; set; }

        /// <summary>
        /// Занимаемая должность
        /// </summary>
        [XafDisplayName("Должность")]
        public DoctorDolgnost Position { get; set; }

        /// <summary>
        /// Отделение, в котором работает доктор
        /// </summary>
        [Association("Otdel-Doctor")]
        [XafDisplayName("Отделение")]
        public Otdel Otdelenie { get; set; }

        /// <summary>
        /// Специальность
        /// </summary>
        [Browsable(false)]
        [NonPersistent]
        public string SpecialityCode
        {
            get
            {
                if (SpecialityTree == null) return null;
                return SpecialityTree.Code;
            }
        }

        /// <summary>
        /// Специальность
        /// </summary>
        [XafDisplayName("Специальность")]
        [RuleRequiredField("", DefaultContexts.Save, "Укажите специальность", ResultType = ValidationResultType.Warning)]
        public DoctorSpecTree SpecialityTree { get; set; }

        /// <summary>
        /// Прикрепленные участки
        /// </summary>
        [XafDisplayName("Прикрепленные участки")]
        [Association("Uchastok-Doctor")]
        public XPCollection<Uchastok> Uchastki
        {
            get
            {
                return GetCollection<Uchastok>("Uchastki");
            }
        }
        
        /// <inheritdoc/>
        public override string ToString()
        {
            return this.FullName;
        }

        // пример записи из XML
        // <ROW fio="Баирова&#160;Л.В." dan_id="167" fam="Баирова" nam="Людмила" mid="Викторовна" date_born="26.08.1981" sex="ж" full_doctor_id="348" users_id="348" norma="30.12.1899&#160;0:15:00" stavka="1" inner_doctor_code="348" federal_code="348" sp_otdel_id="101" sp_spec_doctor_id="176" spec_doctor="Терапевты-1" sp_lpu_id="2301001" full_fio="348&#160;Баирова&#160;Л.В.&#160;(Терапевты-1)" full_name="348&#160;Баирова&#160;Л.В.&#160;(Терапевты-1)" spec_name="Баирова&#160;Л.В.&#160;(Терапевты-1)" expiration_date="23.02.2025" employee_type="0" dolg_name="Врач-терапевт&#160;участковый" otdel_fio="Баирова&#160;Л.В.&#160;(Теpапия&#160;1)" dolg_fio="Баирова&#160;Л.В.&#160;(Врач-терапевт&#160;участковый)" blist_dolg="ТЕРАПЕВТ" can_have_schedule="1"/>
        // <ROW fio="Очирова&#160;М.Д." dan_id="78" fam="Очирова" nam="Мария" mid="Дунгаровна" date_born="06.12.1960" sex="ж" full_doctor_id="2650" users_id="2650" norma="30.12.1899" stavka="1" inner_doctor_code="2650" federal_code="null" sp_otdel_id="202" sp_spec_doctor_id="1313" spec_doctor="Педиатры&#160;2" sp_lpu_id="2301001" full_fio="2650&#160;Очирова&#160;М.Д.&#160;(Педиатры&#160;2)" full_name="Очирова&#160;М.Д.&#160;(Педиатры&#160;2)" spec_name="Очирова&#160;М.Д.&#160;(Педиатры&#160;2)" expiration_date="18.02.2045" employee_type="0" dolg_name="Врач-педиатр&#160;участковый" otdel_fio="Очирова&#160;М.Д.&#160;(Педиатрия&#160;2)" dolg_fio="Очирова&#160;М.Д.&#160;(Врач-педиатр&#160;участковый)" blist_dolg="ПЕДИАТР" can_have_schedule="1"/>

        public static void UpdateDbFromXml(DevExpress.ExpressApp.IObjectSpace objSpace, string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            const string elementsContainer = "ROWDATA";
            const string elementNameStartsWith = "ROW";

            const string code_attr = "inner_doctor_code";


            foreach (var element in doc.Root.Element(elementsContainer).Elements())
            {
                if (element.Name.ToString().StartsWith(elementNameStartsWith) == false) continue;

                Doctor obj = objSpace.FindObject<Doctor>(DevExpress.Data.Filtering.CriteriaOperator.Parse("InnerCode=?", element.Attribute(code_attr).Value));
                if (obj == null)
                {
                    obj = objSpace.CreateObject<Doctor>();

                    obj.FirstName = element.Attribute("nam").Value;
                    obj.LastName = element.Attribute("fam").Value;
                    obj.MiddleName = element.Attribute("mid").Value;
                    DateTime birthDate = DateTime.MinValue;
                    if (DateTime.TryParse(element.Attribute("date_born").Value, out birthDate))
                        obj.Birthdate = birthDate;

                    var innerCode = element.Attribute("inner_doctor_code");
                    obj.InnerCode = (innerCode == null || innerCode.Value == "null") ? null : (int?) int.Parse(innerCode.Value);

                    var federalCode = element.Attribute("federal_code");
                    obj.FederalCode = (federalCode == null || federalCode.Value == "null") ? null : federalCode.Value;

                    var otdelElement = element.Attribute("sp_otdel_id");
                    if (otdelElement !=null && otdelElement.Value != "null")
                    {
                        obj.Otdelenie = objSpace.FindObject<Otdel>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Id=?", otdelElement.Value));
                    }
                    
                    var uchastok = objSpace.FindObject<Uchastok>(DevExpress.Data.Filtering.CriteriaOperator.Parse("DoctorId=?", obj.InnerCode));
                    obj.Uchastki.Add(uchastok);

                    /// в XML должность доктора почему-то храниться в аттрибуте sp_spec_doctor_id.
                    /// Можно спутать со специальностью.
                    var positionElement = element.Attribute("sp_spec_doctor_id");
                    if (positionElement !=null && positionElement.Value != "null")
                    {
                        obj.Position = objSpace.FindObject<DoctorDolgnost>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Code=?", positionElement.Value));
                    }
                }
            }
        }
        
        [Association("Doctors-DoctorRoles")]
        [RuleRequiredField("DoctorRoleIsRequired", DefaultContexts.Save,
            TargetCriteria = "IsActive",
            CustomMessageTemplate = "An active employee must have at least one role assigned")]
        public XPCollection<DoctorRole> DoctorRoles
        {
            get { return GetCollection<DoctorRole>("DoctorRoles"); }
        }

        #region Шаблоны
        [XafDisplayName("Анамнез")]
        public IList<AnamnezTemplate> AnamnezTemplates
        {
            get { return TextTemplates.OfType<AnamnezTemplate>().ToList(); }
        }

        [XafDisplayName("Жалобы")]
        public IList<ComplainTemplate> ComplainTemplates
        {
            get { return TextTemplates.OfType<ComplainTemplate>().ToList(); }
        }

        [XafDisplayName("Рекомендации")]
        public IList<RecomendTemplate> RecomendTemplates
        {
            get { return TextTemplates.OfType<RecomendTemplate>().ToList(); }
        }

        [XafDisplayName("Объективный статус терапевта")]
        public IList<ObjStatusTerTemplate> ObjStatusTerTemplates
        {
            get { return TextTemplates.OfType<ObjStatusTerTemplate>().ToList(); }
        }

        [Association("Doctor-TextTemplates")]
        [Browsable(false)]
        public XPCollection<TextTemplate> TextTemplates
        {
            get { return GetCollection<TextTemplate>("TextTemplates"); }
        }

        public void DeleteTemplates(Type typeToDelete)
        {
            var listToDelete = new List<TextTemplate>();
            foreach (var textTemplate in TextTemplates)
            {
                if (textTemplate.GetType() == typeToDelete)
                    listToDelete.Add(textTemplate);
            }

            Session.Delete(listToDelete);
            Session.CommitTransaction();
        }

        public void CreateTemplates(string text, Type typeToCreate)
        {
            var textValues = text.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < textValues.Count(); i++)
            {
                if (textValues[i].Trim().Length == 0) continue;
                if (textValues[i].StartsWith("\t") == false)
                {
                    var template = GenTemplate(0, i, textValues, typeToCreate);
                    if (template != null)
                    {
                        TextTemplates.Add(template);
                    }
                }
            }

            OnChanged("AnamnezTemplates");
            OnChanged("ComplainTemplates");
            OnChanged("RecomendTemplates");
            OnChanged("ObjStatusTerTemplates");
            Session.CommitTransaction();
        }

        private TextTemplate GenTemplate(int level, int start, string[] text, Type typeToCreate)
        {
            var textTemplate = CreateConcreteTemplate(typeToCreate);
            textTemplate.Name = text[start].Trim();

            for (int i = start + 1; i < text.Count(); i++)
            {
                int lvl = GetLevel(i, text);
                if (lvl == level + 1)
                {
                    textTemplate.Children.Add(GenTemplate(lvl, i, text, typeToCreate));
                }
                else
                {
                    if (lvl < level + 1)
                        return textTemplate;
                }
            }

            return textTemplate;
        }

        private int GetLevel(int line, string[] text)
        {
            if (text[line].Trim().Length == 0) return 0;
            int level = 0;
            while (text[line][level] == '\t')
                level++;
            return level;
        }

        private TextTemplate CreateConcreteTemplate(Type type)
        {
            return Activator.CreateInstance(type, Session) as TextTemplate;
        }

        #endregion

        #region ISecurityUser Members
        
        public bool IsActive
        {
            get { return isActive; }
            set { SetPropertyValue("IsActive", ref isActive, value); }
        }
        
        [RuleRequiredField("DoctorUserNameRequired", DefaultContexts.Save)]
        [RuleUniqueValue("DoctorUserNameIsUnique", DefaultContexts.Save, 
            "Такой логин уже зарегистрирован в системе. Выберите, пожалуйста, другой.")]
        public string UserName
        {
            get { return userName; }
            set { SetPropertyValue("UserName", ref userName, value); }
        }

        #endregion

        #region IAuthenticationStandardUser Members
        public bool ChangePasswordOnFirstLogon
        {
            get { return changePasswordOnFirstLogon; }
            set
            {
                SetPropertyValue("ChangePasswordOnFirstLogon", ref changePasswordOnFirstLogon, value);
            }
        }
        
        [Browsable(false), Size(SizeAttribute.Unlimited), Persistent, SecurityBrowsable]
        protected string StoredPassword
        {
            get { return storedPassword; }
            set { storedPassword = value; }
        }

        public bool ComparePassword(string password)
        {
            return SecurityUserBase.ComparePassword(this.storedPassword, password);
        }

        public void SetPassword(string password)
        {
            this.storedPassword = new PasswordCryptographer().GenerateSaltedPassword(password);
            OnChanged("StoredPassword");
        }
        #endregion

        #region IOperationPermissionProvider Members
        IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions()
        {
            return new IOperationPermission[0];
        }

        IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren()
        {
            return new EnumerableConverter<IOperationPermissionProvider, DoctorRole>(DoctorRoles);
        }
        #endregion

        /// <summary>Операнды свойств класса</summary>
        public static new readonly FieldsClass Fields = new FieldsClass();
        /// <summary>Операнды свойств класса</summary>
        public new class FieldsClass : BaseObject.FieldsClass
        {
            /// <summary>Конструктор</summary>
            public FieldsClass() { }
            /// <summary>Конструктор</summary>
            /// <param name="propertyName">Название вложенного свойства</param>
            public FieldsClass(string propertyName) : base(propertyName) { }

            /// <summary>Операнд свойства Scheduling</summary>
            public OperandProperty Scheduling { get { return new OperandProperty(GetNestedName("Scheduling")); } }

            /// <summary>Операнд свойства SpecialityTree</summary>
            public DoctorSpecTree.FieldsClass SpecialityTree { get { return new DoctorSpecTree.FieldsClass(GetNestedName("SpecialityTree")); } }
            /// <summary>Операнд свойства Oid</summary>
            public OperandProperty Oid { get { return new OperandProperty(GetNestedName("Oid")); } }
        }
    }
    
    [ImageName("BO_Role")]
    public class DoctorRole : SecuritySystemRoleBase
    {
        public DoctorRole(Session session) : base(session) { }

        [Association("Doctors-DoctorRoles")]
        public XPCollection<Doctor> Doctors
        {
            get { return GetCollection<Doctor>("Doctors"); }
        }
    }
}


