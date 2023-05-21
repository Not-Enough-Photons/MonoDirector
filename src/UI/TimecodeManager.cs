using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEP.MonoDirector.UI
{
    public static class TimecodeManager
    {
        public static void Initialize()
        {
            UIManager.Warmup(UIManager.timecodeBarcode, 1, true);
        }
    }
}
