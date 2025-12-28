using System.Collections;
using UnityEngine;

public class MATS_UIManager : MonoBehaviour
{
    public static MATS_UIManager instance;
    [Header("Faders")]
    public GameObject fadeInScreen;
    public GameObject fadeOutScreen;
    public Material fadeInScreenMat;
    public Material fadeOutScreenMat;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
          
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        
    }

    IEnumerator FadeIn()
    {
        fadeInScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        fadeInScreen.SetActive(false);
    }

}
