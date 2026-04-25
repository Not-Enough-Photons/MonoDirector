using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Proxy;
using NEP.MonoDirector.State;

using UnityEngine;

namespace NEP.MonoDirector.UI
{
    public static class MarkerManager
    {
        private const int MaxMarkerCount = 64;

        private static GameObject m_container;
        private static Dictionary<GameObject, Marker> m_markers = new Dictionary<GameObject, Marker>();
        private static List<Marker> m_loadedMarkers = new List<Marker>();

        public static void Initialize()
        {
            m_markers = new Dictionary<GameObject, Marker>();
            m_loadedMarkers = new List<Marker>();

            m_container = new GameObject("[MonoDirector] - Marker Container");
            m_container.transform.SetParent(Bootstrap.MainContainerObject.transform);

            for (int i = 0; i < MaxMarkerCount; i++)
            {
                GameObject obj = GameObject.Instantiate(BundleLoader.PropMarkerObject);
                obj.transform.SetParent(m_container.transform);
                obj.transform.localPosition = Vector3.zero;

                Marker marker = new Marker(obj);
                marker.SetMarker(obj);
                marker.Hide();

                m_loadedMarkers.Add(marker);
            }

            Caster.OnPropAdded += AddMarkerToProp;
            Caster.OnPropRemoved += RemoveMarkerFromProp;

            Events.OnPlayStateSet += ShowMarkers;
        }

        public static void CleanUp()
        {
            Caster.OnPropAdded -= AddMarkerToProp;
            Caster.OnPropRemoved -= RemoveMarkerFromProp;

            Events.OnPlayStateSet -= ShowMarkers;

            m_markers.Clear();
            m_loadedMarkers.Clear();
        }

        public static void Update()
        {
            foreach (var marker in m_loadedMarkers)
                marker.Update();
        }

        public static void AddMarkerToProp(Prop prop)
        {
            if (m_markers.ContainsKey(prop.gameObject))
                return;

            Marker marker = m_loadedMarkers.FirstOrDefault((marker) => !marker.Active);

            // HACK: Don't show prop markers during recording, if the prop was added during recording!
            if (Director.PlayState != PlayState.Recording)
                marker.Show();
            else
                marker.Hide();

            marker.SetOffset(Vector3.up * 0.125f);
            marker.Parent(prop);
            m_markers.Add(prop.gameObject, marker);
        }

        public static void RemoveMarkerFromProp(Prop prop)
        {
            if (!m_markers.ContainsKey(prop.gameObject))
                return;

            Marker marker = m_markers[prop.gameObject];
            marker.SetOffset(Vector3.zero);
            marker.SetTarget(null);
            marker.Hide();
            m_markers.Remove(prop.gameObject);
        }

        public static void AddMarkerToActor(ActorProxy proxy)
        {
            if (m_markers.ContainsKey(proxy.gameObject))
                return;

            Marker marker = m_loadedMarkers.FirstOrDefault((marker) => !marker.Active);

            // HACK: Don't show prop markers during recording, if the prop was added during recording!
            if (Director.PlayState != PlayState.Recording)
                marker.Show();
            else
                marker.Hide();

            marker.Parent(proxy);
            m_markers.Add(proxy.gameObject, marker);
        }

        public static void RemoveMarkerFromActor(ActorProxy proxy)
        {
            if (!m_markers.ContainsKey(proxy.gameObject))
                return;

            Marker marker = m_markers[proxy.gameObject];
            marker.SetOffset(Vector3.zero);
            marker.SetTarget(null);
            marker.Hide();
            m_markers.Remove(proxy.gameObject);
        }

        private static void ShowMarkers(PlayState playState)
        {
            if (playState == PlayState.Preplaying || playState == PlayState.Prerecording)
            {
                foreach (var marker in m_loadedMarkers)
                    marker.Hide();
            }

            if (playState == PlayState.Stopped)
            {
                foreach (var marker in m_loadedMarkers)
                    if (marker.HasTarget)
                        marker.Show();
            }
        }
    }
}
