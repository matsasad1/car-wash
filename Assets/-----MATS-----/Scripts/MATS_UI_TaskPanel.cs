using TMPro;
using UnityEngine;

public class MATS_UI_TaskPanel : MonoBehaviour
{
    public static MATS_UI_TaskPanel Instance;
    public TextMeshProUGUI taskText, taskDesText;


    private void Awake() => Instance = this;

    public void Show(string taskName,string des)
    {
        taskText.text = taskName;
        taskText.text = des;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}