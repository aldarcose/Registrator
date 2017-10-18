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
	/// ����� - �������������� ������������� �������
	/// </summary>
    [DefaultClassOptions]
    public class Kladr : BaseObject
	{
        public Kladr() { }
        public Kladr(Session session) : base(session) { }
        
		/// <summary>
		/// ��� �������
		/// </summary>
        [DataSourceCriteria("Level='@This.Level'")]
        public KladrType Type { get; set; }

		/// <summary>
		/// ������� � �������� �����
		/// </summary>
        public int Level { get; set; }

		/// <summary>
		/// ������������ ������
		/// </summary>
        public Kladr Parent { get; set; }
        
		/// <summary>
		/// ��������
		/// </summary>
        public string Name { get; set; }

		/// <summary>
		/// ���
		/// </summary>
        public string Code { get; set; }

		/// <summary>
		/// �������� ������� ����
		/// </summary>
		/// <remarks>��������� ������� ����, ���������� ��� �������� ������� ����� � ���� ��� �������� ��������</remarks>
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
		/// ������. 0 - ������ ����������
		/// </summary>
        public int Status { get; set; }

		/// <summary>
		/// �������� ������
		/// </summary>
        public string CodePost { get; set; }

		/// <summary>
		/// ��� ����
		/// </summary>
        public string CodeIfns { get; set; }

		/// <summary>
		/// ��� ���������� ����
		/// </summary>
        public string CodeIfnsTerr { get; set; }

		/// <summary>
		/// ��� �����
		/// </summary>
        public string CodeOkato { get; set; }

		/// <summary>
		/// �� ���������� ������ � ��������
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
		/// �������� �������
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
		public bool IsCity
		{
            get { return Level == 3; }
		}

		/// <summary>
		/// �����
        /// </summary>
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
		public Kladr City
		{
			get { return IsCity ? this : Parent == null ? null : Parent.City; }
		}

		/// <summary>
		/// ��������, ���
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
		/// ���, ��������
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
		/// ������ ����� � ������ ���������� ���������
		/// </summary>
		public string AddressLongText
		{
			get
			{
				// �������� ������ ����� �� ������ ������ �� ������������ ������� 
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
				// ������� ��������� �������
				return result.Substring(0, result.Length - 2);
			}
		}
                
		/// <summary>
		/// �������� ������� �����-�
		/// </summary>
		/// <param name="level">�������</param>
		/// <returns>������� �����-�</returns>
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
		/// ������ ����� ��������������?
		/// </summary>
		/// <param name="type">���</param>
		/// <param name="name">��������</param>
        /// <param name="parentName">�������� �������� �������� ������</param>
		/// <returns>True - ��������������, false - ������������</returns>
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
		/// ��������� ������
		/// </summary>
		/// <returns>�������� �������</returns>
		public override string ToString()
		{
			// �������� ������ ����� �� ������ ������ �� ������������ ������� 
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
