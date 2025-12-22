using System.Collections;
using UnityEngine;
using static MATS_GameManager;


//[CreateAssetMenu(menuName = "CarWash/Tasks/Wash")]
public class MATS_WashTask : MATS_LevelTask

{

    public float requiredClean = 1f;

    protected override IEnumerator ExecuteTask()
    {
        MATS_Debug.Log("Starting Wash Task: " + taskName);


        // Wait until cleaning is done or skipped
        yield return new WaitForSeconds(2f);


        MATS_Debug.Log("Wash Task Finished: " + taskName);
    }
}