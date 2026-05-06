using MelonLoader;
using NEP.MonoDirector.Data;

namespace NEP.MonoDirector.UI;

public static class InfoInterfaceManager
{
    public static void Initialize()
    {
        MelonCoroutines.Start(WarehouseLoader.SpawnFromBarcode(WarehouseLoader.infoInterfaceBarcode, true));
    }
}
