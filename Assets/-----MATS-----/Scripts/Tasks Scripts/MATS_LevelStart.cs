using ScratchCardAsset;
using System;
using System.Collections;
using UnityEngine;
//[CreateAssetMenu(menuName = "CarWash/Tasks/LevelStart")]
public class MATS_LevelStart : MATS_LevelTask
{


    

    [SerializeField] GameObject levelAvatar;
    [SerializeField] GameObject playerVehicleBody,rareTire,frontTire;

    [Header("Movement")]
    [SerializeField] private float minY = 1.64f;
    [SerializeField] private float maxY = 1.65f;
    [SerializeField] private float moveSpeed = 1.5f;

    [Header ("Cleaning")]
    [Space(10)]
    [SerializeField] bool isRemovingMudComplete = false;
    [Space(10)]
    public CarsParts[] carPartsToClean;
    [Header("Mud Brush")]
    public Texture mudBrushTexture;


    private void Start()
    {
        for (int i = 0; i < carPartsToClean.Length; i++)
        {
            for (int j = 0; j < carPartsToClean[i].mudParts.parts.Length; j++)
            {
                carPartsToClean[i].mudParts.parts[j].GetComponent<ScratchCardManager>().InputEnabled = false;
            }

        }
    }

    private bool userClicked;
    protected override IEnumerator ExecuteTask()
    {
        MATS_Debug.Log("Starting Start Task: " + taskName);
        //  levelAvatar.SetActive(true);


        yield return new WaitForSeconds(2f);
        playerVehicleBody.transform.parent.GetComponent<Animator>().enabled = false;
        userClicked = false;

        // Start movement coroutine
        Coroutine moveRoutine = StartCoroutine(MoveAvatar());
      
        Coroutine moveRoutine1 = StartCoroutine(UpdateProgress());


        // WAIT until user clicks button
        yield return new WaitUntil(() => userClicked);

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
                carPartsToClean[i].mudParts.parts[j].GetComponent<ScratchCardManager>().InputEnabled = true;
            }

        }

        while (isRemovingMudComplete==false)
        {
            for (int i = 0; i < carPartsToClean.Length; i++)
            {
                for (int j = 0; j < carPartsToClean[i].mudParts.partsEraseProgress.Length; j++)
                {
                    carPartsToClean[i].mudParts.currentProgress[j] = carPartsToClean[i].mudParts.partsEraseProgress[j].GetProgress();

                    if(carPartsToClean[i].mudParts.currentProgress[j] >= carPartsToClean[i].mudParts.requiredCleanProgress[j])
                    {
                        carPartsToClean[i].mudParts.isCleaned[j] = true;
                       // carPartsToClean[i].mudParts.parts[j].InputEnabled = false;
                    }
                    

                   
                   
                   
                }

            }

            if (isRemovingMudComplete == false) {
                if (carPartsToClean[0].mudParts.isCleaned[0]==true&& carPartsToClean[0].mudParts.isCleaned[1]==true&& carPartsToClean[0].mudParts.isCleaned[2])
                    {
                    isRemovingMudComplete = true;


                    for (int i = 0; i < carPartsToClean.Length; i++)
                    {
                        for (int j = 0; j < carPartsToClean[i].mudParts.parts. Length; i++)
                        {

                    StartCoroutine( TweenFloat(0f, 1f, 1.5f,value => carPartsToClean[i].mudParts.parts[j].GetComponent<SpriteRenderer>().color. = value));
                        }
                    }

                }
            }

            yield return null;
        }
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