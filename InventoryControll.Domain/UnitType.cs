using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.Domain;

public enum UnitType
{

    PCE = 796, //штука
    Litr = 112, //литр
    KG = 166, //килограмм
    MTR = 6, //метр
    NMP = 778, //упаковка
    CMT = 4 //сантиметр
}
