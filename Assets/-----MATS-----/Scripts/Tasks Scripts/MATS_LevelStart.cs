using ScratchCardAsset;
using System;
using System.Collections;
using UnityEngine;
//[CreateAssetMenu(menuName = "CarWash/Tasks/LevelStart")]
public class MATS_LevelStart : MATS_LevelTask
{

    public bool isTask1 = false;

    [SerializeField] GameObject mainCarObject;
    [SerializeField] GameObject levelAvatar;
    [SerializeField] GameObject playerVehicleBody,rareTire,frontTire;

    

    [Header ("Cleaning")]
    [Space(10)]
    [SerializeField] bool isRemovingMudComplete = false;
    bool[] hasFaded;
    [Space(10)]
    public CarsParts[] carPartsToClean;
    [Header("Mud Brush")]
    public Texture mudBrushTexture;

    [Header("Guns")]
    [SerializeField] GameObject waterGun;
    [SerializeField] GameObject soapGun;

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


        
       /* if(playerVehicleBody.transform.parent.GetComponent<Animator>())
        playerVehicleBody.transform.parent.GetComponent<Animator>().enabled = false;*/
        userClicked = false;

      

        if(taskName== "MudRemoval")
        {
            MATS_LevelManager.Instance.levels[0].GetComponent<MATS_LevelData>().tasks[1].gameObject.SetActive(true);
            DelayActive(waterGun,0.6f);
            soapGun.SetActive(false);
            StartCoroutine(MATS_LevelManager.Instance.levels[0].MoveAvatar(mainCarObject.transform.parent));
        }
        else if(taskName == "Soap")
        {
            MATS_LevelManager.Instance.levels[0].GetComponent<MATS_LevelData>().tasks[2].gameObject.SetActive(true);
            DelayActive(soapGun, 0.6f);
            waterGun.SetActive(false);
        }
        else if (taskName == "WashSoap")
        {
            MATS_LevelManager.Instance.levels[0].GetComponent<MATS_LevelData>().tasks[3].gameObject.SetActive(true);
            DelayActive(waterGun, 0.6f);
            soapGun.SetActive(false);
     
        }

        

        StartCoroutine(UpdateProgress());


        // WAIT until user clicks button
          yield return new WaitUntil(() => isRemovingMudComplete);
        if(MATS_GameManager.Instance.activeGun != null)
        {
            MATS_GameManager.Instance.activeGun.OnScratchStopedDelayed();
        }
        yield return new WaitForSeconds(1f);
        mainCarObject.SetActive(false);
        MATS_UIManager.instance.taskCompleteParticles.Play();



        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(false);
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

                        //  Fade ONLY once when it becomes cleaned
                        if (!hasFaded[j])
                        {
                            hasFaded[j] = true;
                            FadeMudPart(mud.partsEraseProgress[j].gameObject);
                            if(MATS_GameManager.Instance.activeGun != null)
                            {
                                MATS_GameManager.Instance.activeGun.OnScratchStopedDelayed();
                            }
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

    void DelayActive(GameObject obj,float wait)
    {
        StartCoroutine(Delay(wait,obj));
    }

    IEnumerator Delay(float wait,GameObject obj)
    {
        yield return new WaitForSeconds(wait);
        obj.SetActive(true);
    }

    private void FadeMudPart(GameObject part)
    {
        SpriteRenderer sr = part.GetComponent<SpriteRenderer>();

        StartCoroutine(
            TweenFloat(1f, 0f, 1f, value =>
            {
                Color c = sr.color;
                c.a = value;
                sr.color = c;
            })
        );
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