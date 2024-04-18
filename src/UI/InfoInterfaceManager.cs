using NEP.MonoDirector.Data;

namespace NEP.MonoDirector.UI
{
    public static class InfoInterfaceManager
    {
        public static void Initialize()
        {
            WarehouseLoader.SpawnFromBarcode(WarehouseLoader.infoInterfaceBarcode, false);
        }
    }
}
