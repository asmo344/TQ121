using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos.OpticalSensor
{
    public interface IPageSelect
    {
        byte GetPage();

        void SetPage(byte page);
    }
}