using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;

namespace Registrator.Module.BusinessObjects.Dictionaries
{
    public enum EducationInstituteType
    {
        [XafDisplayName("Дошкольное образовательное учреждение")]
        PreSchool = 0,
        [XafDisplayName("Общее образовательное учреждение")]
        School = 1,
        [XafDisplayName("Профессиональное и специальное образовательное учреждение")]
        HighSchool = 2,
        [XafDisplayName("Профессиональное и специальное образовательное учреждение до 15 лет")]
        HighSchoolTill15 = 3,
        [XafDisplayName("Профессиональное и специальное образовательное учреждение с 15 лет")]
        HighSchoolAfter15 = 4
    }
}
