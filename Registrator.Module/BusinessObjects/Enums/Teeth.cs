using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.Module.BusinessObjects.Enums
{
    /// <summary>
    /// Зубы
    /// </summary>
    [Flags]
    public enum Teeth
    {
        None = 0,
        Tooth11 = 1 << 0,        
        Tooth12 = 1 << 1,
        Tooth13 = 1 << 2,        
        Tooth14 = 1 << 3,
        Tooth15 = 1 << 4,
        Tooth16 = 1 << 5,
        Tooth17 = 1 << 6,
        Tooth18 = 1 << 7,
        Tooth21 = 1 << 8,
        Tooth22 = 1 << 9, 
        Tooth23 = 1 << 10,
        Tooth24 = 1 << 11,
        Tooth25 = 1 << 12,
        Tooth26 = 1 << 13,
        Tooth27 = 1 << 14,
        Tooth28 = 1 << 15,
        Tooth31 = 1 << 16,
        Tooth32 = 1 << 17,
        Tooth33 = 1 << 18,
        Tooth34 = 1 << 19,
        Tooth35 = 1 << 20,
        Tooth37 = 1 << 21,
        Tooth38 = 1 << 22,
        Tooth41 = 1 << 23,
        Tooth42 = 1 << 24,
        Tooth43 = 1 << 25,
        Tooth44 = 1 << 26,
        Tooth45 = 1 << 27,
        Tooth46 = 1 << 28,
        Tooth47 = 1 << 29, 
        Tooth48 = 1 << 30, 
    }

    /// <summary>
    /// Зубы (молочный прикус)
    /// </summary>
    [Flags]
    public enum MilkByteTeeth
    {
        None = 0,
        Tooth51 = 1 << 1,
        Tooth52 = 1 << 2,
        Tooth53 = 1 << 3,
        Tooth54 = 1 << 4,
        Tooth55 = 1 << 5,
        Tooth61 = 1 << 6,
        Tooth62 = 1 << 7,
        Tooth63 = 1 << 8,
        Tooth64 = 1 << 9,
        Tooth65 = 1 << 10,
        Tooth71 = 1 << 11,
        Tooth72 = 1 << 12,
        Tooth73 = 1 << 13,
        Tooth74 = 1 << 14,
        Tooth75 = 1 << 15,
        Tooth81 = 1 << 16,
        Tooth82 = 1 << 17,
        Tooth83 = 1 << 18,
        Tooth84 = 1 << 19,
        Tooth85 = 1 << 20
    }
}
