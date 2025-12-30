using UnityEngine;

public class MATS_GameManager : MonoBehaviour
{
    public static MATS_GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public enum TaskState
    {
        Idle,
        Running,
        Completed,
        Failed,
        Skipped
    }

    public MATS_GunManager activeGun;
}
