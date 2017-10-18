using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Registrator.Module.BusinessObjects.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.Settings
{
    /// <summary>
    /// Общие настройки
    /// </summary>
    [NonPersistent]
    public class RegionSettings
    {
        static Kladr _kladr;

        public static string RegionCode (Session session)
        {
            var regionConstant = "CurrentRegion";

            Constants constant = session.FindObject<Constants>(new BinaryOperator("Name", regionConstant));

            if (constant == null)
                return "03";

            return constant.Value;
        }

        public static string RegionCode(IObjectSpace objectSpace)
        {
            var regionConstant = "CurrentRegion";

            Constants constant = objectSpace.FindObject<Constants>(new BinaryOperator("Name", regionConstant));

            if (constant == null)
                return "03";

            return constant.Value;
        }

        public static Kladr GetCurrentKladr(Session session)
        {
            if (_kladr == null)
                _kladr = session.FindObject<Kladr>(DevExpress.Data.Filtering.CriteriaOperator.Parse("CodeSignificantChars=?", RegionCode(session)));

            return _kladr;
        }

        public static Kladr GetCurrentKladr(IObjectSpace objectSpace)
        {
            if (_kladr == null)
                _kladr = objectSpace.FindObject<Kladr>(DevExpress.Data.Filtering.CriteriaOperator.Parse("CodeSignificantChars=?", RegionCode(objectSpace)));

            return _kladr;
        }

        public static string GetCurrentRegionOKATO(Session session)
        {
            if (_kladr == null)
                _kladr = session.FindObject<Kladr>(DevExpress.Data.Filtering.CriteriaOperator.Parse("CodeSignificantChars=?", RegionCode(session)));

            var okato = _kladr.CodeOkato;

            if (okato.Length > 5)
                // код ТФ ОКАТО - это 5 первых цифр ОКАТО
                okato = okato.Substring(0, 5);

            return (_kladr == null) ? string.Empty : okato;
        }

        public static string GetCurrentRegionOKATO(IObjectSpace objectSpace)
        {
            if (_kladr == null)
                _kladr = objectSpace.FindObject<Kladr>(DevExpress.Data.Filtering.CriteriaOperator.Parse("CodeSignificantChars=?", RegionCode(objectSpace)));

            var okato = _kladr.CodeOkato;

            if (okato.Length > 5)
                // код ТФ ОКАТО - это 5 первых цифр ОКАТО
                okato = okato.Substring(0, 5);

            return (_kladr == null) ? string.Empty : okato;
        }
    }

    [NonPersistent]
    public class TarifSettings
    {
        public static decimal GetDnevnoyStacionarTarif(Session session)
        {
            var tarif = session.FindObject<Constants>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Name=?", "DnevnoyStacionarTarif"));
            if (tarif != null)
            {
                return Utils.GetDecimalFromString(tarif.Value);
            }
            else
            {
                // сообщить об ошибке
                throw new NotImplementedException("Обработать ошибку: не получилось считать тариф дневного стационара");
            }
        }

        public static decimal GetDnevnoyStacionarTarif(IObjectSpace objectSpace)
        {
            var tarif = objectSpace.FindObject<Constants>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Name=?", "DnevnoyStacionarTarif"));
            if (tarif != null)
            {
                return Utils.GetDecimalFromString(tarif.Value);
            }
            else
            {
                // сообщить об ошибке
                throw new NotImplementedException("Обработать ошибку: не получилось считать тариф дневного стационара");
            }
        }
    }

    public class MOSettings
    {
        public static string GetCurrentMOCode(Session session)
        {
            var mo = session.FindObject<Constants>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Name=?", "CurrentMOCode"));
            if (mo != null)
            {
               return mo.Value;
            }
            else
            {
                // сообщить об ошибке
                throw new NotImplementedException("Обработать ошибку: не получилось считать тариф дневного стационара");
            }
        }

        public static string GetCurrentMOCode(IObjectSpace objectSpace)
        {
            var mo = objectSpace.FindObject<Constants>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Name=?", "CurrentMOCode"));
            if (mo != null)
            {
                return mo.Value;
            }
            else
            {
                // сообщить об ошибке
                throw new NotImplementedException("Обработать ошибку: не получилось считать тариф дневного стационара");
            }
        }
    }
}
