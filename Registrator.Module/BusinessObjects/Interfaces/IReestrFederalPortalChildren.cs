using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Registrator.Module.BusinessObjects.Interfaces
{
    public interface IReestrFederalPortalChildren
    {
        XElement GetCardBlock();

        XElement GetChildBlock();
    }
}
