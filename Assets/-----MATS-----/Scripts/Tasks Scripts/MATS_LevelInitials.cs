using System.Collections;
using UnityEngine;

public class MATS_LevelInitials : MATS_LevelTask
{
    [SerializeField] GameObject levelAvatar;
    [SerializeField] GameObject playerVehicleBody, rareTire, frontTire;

    [Header("Movement")]
    [SerializeField] private float minY = 1.64f;
    [SerializeField] private float maxY = 1.65f;
    [SerializeField] private float moveSpeed = 1.5f;

    bool startTask1;
    protected override IEnumerator ExecuteTask()
    {
        MATS_Debug.Log("Starting Initials of level Task: " + taskName);
        yield return new WaitForSeconds(2f);
        if (playerVehicleBody.transform.parent.GetComponent<Animator>())
            playerVehicleBody.transform.parent.GetComponent<Animator>().enabled = false;

        Coroutine moveRoutine = StartCoroutine(MoveBody());
        yield return new WaitUntil(() => startTask1);

        StopCoroutine(moveRoutine);
    }

    private IEnumerator MoveBody()
    {


        MATS_Debug.Log("Moving the avatar   xxxxx");
        Transform t = playerVehicleBody.transform;
        float midY = (minY + maxY) / 2f;
        float amplitude = (maxY - minY) / 2f;
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime * moveSpeed;
            float newY = midY + Mathf.Sin(time) * amplitude;
            Vector3 pos = t.position;
            pos.y = newY;
            t.position = pos;

            yield return null;
        }
    }
}
