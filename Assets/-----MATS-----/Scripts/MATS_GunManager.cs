using Lean.Touch;
using ScratchCardAsset;
using UnityEngine;

public class MATS_GunManager : MonoBehaviour
{
    [Header("Laser Settings")]
    public Transform laserTip;
   
    public float laserLength = 100f;
    public float minDistance = 2f;

    [Header("Scratch Card")]
    public ScratchCard scratchCard;

    private bool isShooting = false;
    private Vector2? previousScratchPos = null;

    void OnEnable()
    {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUp -= HandleFingerUp;
    }

    void Update()
    {
        if (!isShooting)
        {
           

            previousScratchPos = null;
            return;
        }

        FireLaser();
    }

    private void HandleFingerDown(LeanFinger finger)
    {
        isShooting = true;
        Debug.Log("[Laser] Shooting started");
    }

    private void HandleFingerUp(LeanFinger finger)
    {
        isShooting = false;
        Debug.Log("[Laser] Shooting stopped");
    }

    private void FireLaser()
    {
        
    }
}