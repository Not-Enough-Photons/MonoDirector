using System.Collections;

using UnityEngine;

using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Pool;

using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;

namespace NEP.MonoDirector.Yielding;

public sealed class WaitForLevelLoad : IEnumerator
{
    public WaitForLevelLoad(string barcode)
    {
        LevelCrateReference crateRef = new LevelCrateReference(new Barcode(barcode));
        m_spawnTask = SceneStreamer.LoadAsync(crateRef);
    }
    
    public object Current => null;
    
    private Action m_callback;
    private UniTask m_spawnTask;

    public IEnumerator Then(Action callback)
    {
        m_callback = callback;
        return this;
    }

    public bool MoveNext()
    {
        if (!m_spawnTask.GetAwaiter().IsCompleted)
            return true;

        if (m_callback != null)
            m_callback?.Invoke();

        return false;
    }

    public void Reset()
    {

    }
}