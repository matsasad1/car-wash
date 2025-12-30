using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class MATS_LevelInitials : MATS_LevelTask
{
    [SerializeField] GameObject mainCarObj;

    [SerializeField] GameObject levelAvatar;
    [SerializeField] GameObject initialCar,playerVehicleBody, rareTire, frontTire;
    [Header("Scene Objects")]

    [SerializeField] GameObject bg1;
    [SerializeField] GameObject bg2;
    [SerializeField] GameObject ramp;




    [Header("Movement")]
    [SerializeField] private float minY = 1.64f;
    [SerializeField] private float maxY = 1.65f;
    [SerializeField] private float moveSpeed = 1.5f;

    public Button washItButton;

    bool startTask1;
    Coroutine tempCoroutine;

    private void Start()
    {
        bg1.SetActive(true);
    }
    protected override IEnumerator ExecuteTask()
    {
        washItButton.onClick.RemoveAllListeners();
        washItButton.onClick.AddListener(IsStartInitialTask);
        levelAvatar.SetActive(false);
        initialCar.SetActive(false);
        MATS_Debug.Log("Starting Initials of level Task: " + taskName);
     
        if (tempCoroutine != null)
        {
            StopCoroutine(tempCoroutine);
            tempCoroutine = null;
        }
        tempCoroutine = StartCoroutine(FadeOut());
        yield return new WaitForSeconds(1.8f);
        initialCar.SetActive(true);
        levelAvatar.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        if (playerVehicleBody.transform.parent.GetComponent<Animator>())
            playerVehicleBody.transform.parent.GetComponent<Animator>().enabled = false;

        Coroutine moveRoutine = StartCoroutine(MoveBody());
        washItButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => startTask1);
        if (tempCoroutine != null)
        {
            StopCoroutine(tempCoroutine);
            tempCoroutine = null;
        }
        tempCoroutine = StartCoroutine(FadeInFadeOut());
        yield return new WaitForSeconds(1.8f);
        levelAvatar.SetActive(false);
        initialCar.SetActive(false);

        mainCarObj.SetActive(true);
        bg1.SetActive(false);
        bg2.SetActive(true);
        ramp.SetActive(true);
        yield return new WaitForSeconds(1.8f);
    /*    for(int i=1;i< MATS_LevelManager.Instance.levels[0].GetComponent<MATS_LevelData>().tasks.Length;i++)
        {
        }*/
        MATS_LevelManager.Instance.levels[0].GetComponent<MATS_LevelData>().tasks[0].gameObject.SetActive(false);
            MATS_LevelManager.Instance.levels[0].GetComponent<MATS_LevelData>().tasks[1].gameObject.SetActive(true);
        StopCoroutine(moveRoutine);
     

    }

    public void IsStartInitialTask()
    {
        startTask1 = true;
        washItButton.gameObject.SetActive(false);
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
