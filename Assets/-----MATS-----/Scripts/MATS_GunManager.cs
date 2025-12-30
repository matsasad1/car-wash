using Lean.Touch;
using ScratchCardAsset;
using UnityEngine;

public class MATS_GunManager : MonoBehaviour
{
    [Header("Laser Scratch")]
    public Transform gunTip;
     public float laserDistance = 15f;
     public LayerMask scratchLayer;
    [SerializeField] public bool debugLaser = true;

    [Header("Scratch Card")]
    public ScratchCard scratchCard;
    public bool IsActuallyScratching;
    private bool isShooting = false;
    private Vector2? getScratchPosition = null;

    public Vector2? GetScratchPosition { get => getScratchPosition; set => getScratchPosition = value; }


    [Header("Laser FX")]
    [SerializeField] private ParticleSystem gunParticle;
    [SerializeField] private ParticleSystem scratchParticle;


    void OnEnable()
    {
        MATS_GameManager.Instance.activeGun = this;

        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
    }

    void OnDisable()
    {
        MATS_GameManager.Instance.activeGun = null;

        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUp -= HandleFingerUp;
    }

   

    private void HandleFingerDown(LeanFinger finger)
    {
        isShooting = true;
            gunParticle?.Play();
        MATS_Debug.Log("[Laser] Shooting started");
    }

    private void HandleFingerUp(LeanFinger finger)
    {
        isShooting = false;
        gunParticle?.Stop();
        
        MATS_Debug.Log("[Laser] Shooting stopped");
    }


    bool onScratchPlaying = false;

   public void OnScratch()
    {
        MATS_Debug.Log("[Laser] OnScratch started");

        if (!onScratchPlaying)
        {
            scratchParticle.Play();
            onScratchPlaying = true;
        }
    }

    public void OnScratchStopedDelayed()
    {
        scratchParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        MATS_Debug.Log("[Laser] OnScratch Stoped");
        onScratchPlaying = false;
    }


    /* public void OnScratchStoped()
     {
         Invoke(nameof(OnScratchStopedDelayed), 0.2f);
     }*/


}