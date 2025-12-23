using ScratchCardAsset;
using System;
using System.Collections;
using UnityEngine;
//[CreateAssetMenu(menuName = "CarWash/Tasks/LevelStart")]
public class MATS_LevelStart : MATS_LevelTask
{

    public bool isTask1 = false;
    

    [SerializeField] GameObject levelAvatar;
    [SerializeField] GameObject playerVehicleBody,rareTire,frontTire;

    [Header("Movement")]
    [SerializeField] private float minY = 1.64f;
    [SerializeField] private float maxY = 1.65f;
    [SerializeField] private float moveSpeed = 1.5f;

    [Header ("Cleaning")]
    [Space(10)]
    [SerializeField] bool isRemovingMudComplete = false;
    bool[] hasFaded;
    [Space(10)]
    public CarsParts[] carPartsToClean;
    [Header("Mud Brush")]
    public Texture mudBrushTexture;


    private void Start()
    {
        hasFaded = new bool[carPartsToClean[0].mudParts.parts.Length];
        for (int i = 0; i < carPartsToClean.Length; i++)
        {
            for (int j = 0; j < carPartsToClean[i].mudParts.parts.Length; j++)
            {
                carPartsToClean[i].mudParts.parts[j].GetComponent<ScratchCard>().InputEnabled = false;
            }

        }
    }

    private bool userClicked;
    protected override IEnumerator ExecuteTask()
    {
        MATS_Debug.Log("Starting Start Task: " + taskName);
        //  levelAvatar.SetActive(true);


        yield return new WaitForSeconds(2f);
        if(playerVehicleBody.transform.parent.GetComponent<Animator>())
        playerVehicleBody.transform.parent.GetComponent<Animator>().enabled = false;
        userClicked = false;

        // Start movement coroutine
        Coroutine moveRoutine = StartCoroutine(MoveAvatar());
        if(taskName== "MudRemoval")
        {
            MATS_LevelManager.Instance.levels[0].GetComponent<MATS_LevelData>().tasks[1].gameObject.SetActive(true);
        }
        else if(taskName == "Soap")
        {
            MATS_LevelManager.Instance.levels[0].GetComponent<MATS_LevelData>().tasks[2].gameObject.SetActive(true);
        }

        Coroutine moveRoutine1 = StartCoroutine(UpdateProgress());


        // WAIT until user clicks button
          yield return new WaitUntil(() => isRemovingMudComplete);
       this.gameObject.SetActive(false);
        
        // Stop movement
        StopCoroutine(moveRoutine);


        yield return new WaitForSeconds(2f);
        MATS_Debug.Log("Wash Task Finished: " + taskName);
    }
    private IEnumerator UpdateProgress()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < carPartsToClean.Length; i++)
        {
            for (int j = 0; j < carPartsToClean[i].mudParts.parts.Length; j++)
            {
                carPartsToClean[i].mudParts.parts[j].GetComponent<ScratchCard>().InputEnabled = true;
            }

        }

        while (!isRemovingMudComplete)
        {
            bool allCleaned = true;

            for (int i = 0; i < carPartsToClean.Length; i++)
            {
                var mud = carPartsToClean[i].mudParts;

                for (int j = 0; j < mud.partsEraseProgress.Length; j++)
                {
                    mud.currentProgress[j] = mud.partsEraseProgress[j].GetProgress();

                    // Mark cleaned
                    if (mud.currentProgress[j] >= mud.requiredCleanProgress[j])
                    {
                        mud.isCleaned[j] = true;

                        // 🔥 Fade ONLY once when it becomes cleaned
                        if (!hasFaded[j])
                        {
                            hasFaded[j] = true;
                            FadeMudPart(mud.partsEraseProgress[j].gameObject);
                        }
                    }

                    if (!mud.isCleaned[j])
                        allCleaned = false;
                }
            }

            isRemovingMudComplete = allCleaned;
            yield return null;
        }
    }

    private void FadeMudPart(GameObject part)
    {
        SpriteRenderer sr = part.GetComponent<SpriteRenderer>();

        StartCoroutine(
            TweenFloat(1f, 0f, 1.5f, value =>
            {
                Color c = sr.color;
                c.a = value;
                sr.color = c;
            })
        );
    }

    private IEnumerator MoveAvatar()
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

    // 🔘 CALL THIS FROM UI BUTTON
    public void OnUserClick()
    {
        userClicked = true;
    }


       [Serializable]
    public class CarsParts {
        
    public PartsToClean mudParts;
    }
    [Serializable]
    public class PartsToClean
    {
        public ScratchCardManager[] parts;
        public EraseProgress[] partsEraseProgress;
        public float[] requiredCleanProgress,currentProgress;
        
        public bool[] isCleaned;
    }
}