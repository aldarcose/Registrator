using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Registrator.Module.BusinessObjects
{
	/// <summary>
	/// КЛАДР - Общероссийский классификатор адресов
	/// </summary>
    [DefaultClassOptions]
    public class Kladr : BaseObject
	{
        public Kladr() { }
        public Kladr(Session session) : base(session) { }
        
		/// <summary>
		/// Тип объекта
		/// </summary>
        [DataSourceCriteria("Level='@This.Level'")]
        public KladrType Type { get; set; }

		/// <summary>
		/// Уровень в иерархии типов
		/// </summary>
        public int Level { get; set; }

		/// <summary>
		/// Родительский объект
		/// </summary>
        public Kladr Parent { get; set; }
        
		/// <summary>
		/// Название
		/// </summary>
        public string Name { get; set; }

		/// <summary>
		/// Код
		/// </summary>
        public string Code { get; set; }

		/// <summary>
		/// Значащие символы кода
		/// </summary>
		/// <remarks>Стартовые символы кода, одинаковые для текущего объекта КЛАДР и всех его дочерних объектов</remarks>
		public string CodeSignificantChars
		{
			get
			{
				switch (Level)
				{
                    case 1: return Code.Substring(0, 2);
                    case 2: return Code.Substring(0, 5);
                    case 3: return Code.Substring(0, 8);
                    case 4: return Code.Substring(0, 11);
                    case 5: return Code.Substring(0, 15);
				}
				return null;
			}
		}

		/// <summary>
		/// Статус. 0 - Запись актуальная
		/// </summary>
        public int Status { get; set; }

		/// <summary>
		/// Почтовый индекс
		/// </summary>
        public string CodePost { get; set; }

		/// <summary>
		/// Код ИФНС
		/// </summary>
        public string CodeIfns { get; set; }

		/// <summary>
		/// Код территории ИФНС
		/// </summary>
        public string CodeIfnsTerr { get; set; }

		/// <summary>
		/// Код ОКАТО
		/// </summary>
        public string CodeOkato { get; set; }

		/// <summary>
		/// Не отображать объект в справках
		/// </summary>
		public bool IsHidden
		{
			get
			{
                //TODO
                return false;
			}
		}

		/// <summary>
		/// Является городом
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
		public bool IsCity
		{
            get { return Level == 3; }
		}

		/// <summary>
		/// Город
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
		public Kladr City
		{
			get { return IsCity ? this : Parent == null ? null : Parent.City; }
		}

		/// <summary>
		/// Название, тип
		/// </summary>
		public string NameType
		{
			get
			{
                //return Name + ", " + Type.ShortName + (Parent != CommonSettings.City && Parent != null ? " (" + Parent.Name + ")" : String.Empty);
                return Name + ", " + (Type != null ? Type.ShortName : String.Empty) + (Parent != null ? " (" + Parent.Name + ")" : String.Empty);
			}
		}

		/// <summary>
		/// Тип, название
		/// </summary>
		public string TypeName
		{
			get
			{
                //return Type.ShortName + " " + Name + (Parent != CommonSettings.City && Parent != null ? " (" + Parent.Name + ")" : String.Empty);
                return (Type != null ? Type.ShortName + " " : String.Empty) + Name + (Parent != null ? " (" + Parent.Name + ")" : String.Empty);
			}
		}

		/// <summary>
		/// Полный адрес с учетом скрываемых элементов
		/// </summary>
		public string AddressLongText
		{
			get
			{
				// Собираем полный адрес на основе ссылки на родительский элемент 
				string result = "";
				Kladr kladr = this;
				while (true)
				{
					if (!kladr.IsHidden)
						result = (kladr.Type == null ? null : kladr.Type.ShortName) + " " + kladr.Name + ", " + result;
					kladr = kladr.Parent;
					if (kladr == null)
						break;
				}
				// Убираем последнюю запятую
				return result.Substring(0, result.Length - 2);
			}
		}
                
		/// <summary>
		/// Получить элемент КЛАДР-а
		/// </summary>
		/// <param name="level">Уровень</param>
		/// <returns>Элемент КЛАДР-а</returns>
		public Kladr GetElementKladr(int level)
		{
			Kladr kladr = this;
			if (kladr == null) return null;
			while (true)
			{
				if (kladr.Level == level)
					return kladr;
				kladr = kladr.Parent;
				if (kladr == null)
					return null;
			}
		}

        public static Kladr GetKladr(DevExpress.ExpressApp.IObjectSpace objSpace, string okato)
        {
            return objSpace.FindObject<Kladr>(DevExpress.Data.Filtering.CriteriaOperator.Parse("CodeOkato = ?", okato));
        }

		/// <summary>
		/// Объект КЛАДР неотображаемый?
		/// </summary>
		/// <param name="type">Тип</param>
		/// <param name="name">Название</param>
        /// <param name="parentName">Название элемента верхнего уровня</param>
		/// <returns>True - неотображаемый, false - отображаемый</returns>
		public static bool GetIsHidden(KladrType type, string name, string parentName)
        {
            throw new NotImplementedException();
            //LoadHiddens();
            //if (hiddenKladr != null && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(parentName))
            //{
            //    foreach (Kladr k in hiddenKladr.Keys)
            //        if (k.type == type && string.Equals(k.name, name, StringComparison.OrdinalIgnoreCase) && string.Equals(k.Parent.Name, parentName, StringComparison.OrdinalIgnoreCase))
            //            return true;
            //}
            //return false;
		}


		/// <summary>
		/// Строковый формат
		/// </summary>
		/// <returns>Название объекта</returns>
		public override string ToString()
		{
			// Собираем полный адрес на основе ссылки на родительский элемент 
			string result = string.Empty;
			Kladr kladr = this;
            if (kladr.Level != 5)
                while (kladr != null)
                {
                    result = (kladr.Type != null ? kladr.Type.ShortName + " " : string.Empty) + kladr.Name +
                        (kladr != this ? ", " : string.Empty) + result;
                    kladr = kladr.Parent;
                }
            else
                result = (kladr.Type != null ? kladr.Type.ShortName + " " : string.Empty) + kladr.Name;
			return result;
		}
    }
}
