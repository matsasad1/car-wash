using System.Collections;
using UnityEngine;

public class MATS_FinalTask : MATS_LevelTask
{
    public GameObject finalCelebrationParticles;
    protected override IEnumerator ExecuteTask()
    {
        MATS_Debug.Log("Starting Final Task: " + taskName);
        //  levelAvatar.SetActive(true);

        finalCelebrationParticles.SetActive(true);
        yield return new WaitForSeconds(2f);
    }

}
