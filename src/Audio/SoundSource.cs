using NEP.MonoDirector.State;
using SLZ.Interaction;
using UnityEngine;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SoundSource : MonoBehaviour
    {
        public SoundSource(System.IntPtr ptr) : base(ptr) { }

        private AudioSource source;
        private GameObject sprite;
        private GameObject frame;
        private Grip grip;
        private Rigidbody rb;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            sprite = transform.Find("Sprite").gameObject;
            frame = transform.Find("Frame").gameObject;
            grip = transform.Find("Grip").GetComponent<Grip>();
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            Events.OnPlayStateSet += OnPlayStateSet;
            Events.OnStartRecording += OnStartRecording;
            Events.OnPlay += OnPlay;

            grip.attachedHandDelegate += new System.Action<Hand>(AttachedHand);
            grip.detachedHandDelegate += new System.Action<Hand>(DetachedHand);
        }

        private void OnDisable()
        {
            Events.OnPlayStateSet -= OnPlayStateSet;
            Events.OnStartRecording -= OnStartRecording;
            Events.OnPlay -= OnPlay;

            grip.attachedHandDelegate -= new System.Action<Hand>(AttachedHand);
            grip.detachedHandDelegate -= new System.Action<Hand>(DetachedHand);
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void AttachedHand(Hand hand)
        {
            rb.isKinematic = false;
        }

        private void DetachedHand(Hand hand)
        {
            rb.isKinematic = true;
        }

        private void OnPlayStateSet(PlayState playState)
        {
            if (playState == PlayState.Preplaying
            || playState == PlayState.Playing)
            {
                HideVisuals();
            }
            else
            {
                ShowVisuals();
            }
        }

        private void OnStartRecording()
        {
            source.Play();
        }

        private void OnPlay()
        {
            source.Play();
        }

        private void ShowVisuals()
        {
            sprite.SetActive(true);
            frame.SetActive(true);
        }

        private void HideVisuals()
        {
            sprite.SetActive(false);
            frame.SetActive(false);
        }
    }
}