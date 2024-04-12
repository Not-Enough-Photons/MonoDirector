using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEP.MonoDirector.Data;

namespace NEP.MonoDirector.UI
{
    public static class InfoInterfaceManager
    {
        public static void Initialize()
        {
            WarehouseLoader.Warmup(WarehouseLoader.infoInterfaceBarcode, 1, true);
        }
    }
}
