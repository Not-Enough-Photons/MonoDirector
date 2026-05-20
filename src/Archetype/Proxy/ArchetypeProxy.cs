using System;
using UnityEngine;

namespace NEP.MonoDirector.Proxy;

[MelonLoader.RegisterTypeInIl2Cpp]
public abstract class ArchetypeProxy(IntPtr ptr) : MonoBehaviour(ptr)
{
}
