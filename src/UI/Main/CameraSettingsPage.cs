using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.State;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraSettingsPage : MonoBehaviour
    {
        public CameraSettingsPage(System.IntPtr ptr) : base(ptr) { }

        private Transform grid => transform.Find("Grid");

        private TMP_Dropdown cameraModeDropdown => grid.Find("CameraMode/Dropdown").GetComponent<TMP_Dropdown>();

        private Toggle xAxisLock => grid.Find("LockRotation/ToggleX").GetComponent<Toggle>();
        private Toggle yAxisLock => grid.Find("LockRotation/ToggleY").GetComponent<Toggle>();
        private Toggle zAxisLock => grid.Find("LockRotation/ToggleZ").GetComponent<Toggle>();

        private Toggle kinematicLock => grid.Find("KinematicOnRelease/Toggle").GetComponent<Toggle>();

        private void Start()
        {
            Il2CppSystem.Collections.Generic.List<string> list = new Il2CppSystem.Collections.Generic.List<string>();

            string[] enumNames = System.Enum.GetNames(typeof(CameraMode));

            for (int i = 0; i < enumNames.Length; i++)
            {
                list.Add(enumNames[i]);
            }

            cameraModeDropdown.ClearOptions();

            cameraModeDropdown.AddOptions(list);

            cameraModeDropdown.onValueChanged.AddListener(new System.Action<int>((value) => OnValueChanged((CameraMode)value)));
        }

        private void OnValueChanged(CameraMode mode)
        {

        }
    }
}