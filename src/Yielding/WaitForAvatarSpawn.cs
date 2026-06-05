using System.Collections;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using UnityEngine;

namespace NEP.MonoDirector.Yielding;

public sealed class WaitForAvatarSpawn<T> : IEnumerator
{
    public WaitForAvatarSpawn(AvatarCrate crate)
    {
        m_spawnTask = crate.LoadAssetAsync();
    }
    
    public object Current => null;

    private T m_result;
    private Action<T> m_callback;
    private UniTask<GameObject> m_spawnTask;

    public static WaitForAvatarSpawn<T> Run(AvatarCrate crate)
    {
        var instance = new WaitForAvatarSpawn<T>(crate);
        MelonCoroutines.Start(instance);
        return instance;
    }
    
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