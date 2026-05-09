using System.Collections;

using UnityEngine;

using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Pool;
using NEP.MonoDirector.Core;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow.Warehouse;

namespace NEP.MonoDirector.Yielding;

public sealed class WaitForAssetSpawn<T> : IEnumerator
{
    public WaitForAssetSpawn(Spawnable spawnable, Vector3 position, Quaternion rotation)
    {
        m_spawnTask = AssetSpawner.SpawnAsync(
            spawnable,
            position,
            rotation, 
            new Il2CppSystem.Nullable<Vector3>(Vector3.one), 
            null, 
            false, 
            new Il2CppSystem.Nullable<int>(0));
    }

    public object Current => null;

    private T m_result;
    private Action<T> m_callback;
    private UniTask<Poolee> m_spawnTask;

    public IEnumerator Then(Action<T> callback)
    {
        m_callback = callback;
        return this;
    }

    public bool MoveNext()
    {
        if (!m_spawnTask.GetAwaiter().IsCompleted)
            return true;

        m_result = m_spawnTask.GetAwaiter().GetResult().GetComponent<T>();

        if (m_callback != null)
            m_callback?.Invoke(m_result);

        return false;
    }

    public void Reset()
    {

    }
}
