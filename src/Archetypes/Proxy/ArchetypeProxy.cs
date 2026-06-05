using System;
using UnityEngine;

namespace NEP.MonoDirector.Archetypes.Proxy;

[MelonLoader.RegisterTypeInIl2Cpp]
public abstract class ArchetypeProxy(IntPtr ptr) : MonoBehaviour(ptr)
{
}
