using System.Threading.Tasks;
using UnityEngine;

public class MATS_LevelManager : MonoBehaviour
{
   
    public static MATS_LevelManager Instance;
    public MATS_LevelData[] levels;

    private int currentLevelIndex;
    private int currentTaskIndex;

    private void Awake() => Instance = this;

    private void Start()
    {
        MATS_Debug.Log(" levels start Call!");
        StartGame();
    }


    public void StartGame(int startLevel = 0)
    {
        currentLevelIndex = startLevel;
        MATS_Debug.Log($"  Start Level number: {startLevel}");
        if(currentTaskIndex == 0)
        StartLevel();
    }

    private void StartLevel()
    {
        MATS_Debug.Log($"  Current Task Index: {currentTaskIndex}");
        currentTaskIndex = 0;
        foreach (var task in levels[currentLevelIndex].tasks)
        {
            task.ResetRuntimeState();
        }
        ExecuteNextTask();
    }

    private void ExecuteNextTask()
    {
        if (currentLevelIndex >= levels.Length)
        {
            MATS_Debug.Log("All levels completed!");
            return;
        }

        MATS_LevelData level = levels[currentLevelIndex];
        if (currentTaskIndex >= level.tasks.Length)
        {
            LevelCompleted();
            return;
        }

        MATS_LevelTask task = level.tasks[currentTaskIndex];
        task.gameObject.SetActive(true);
        task.OnTaskCompleted += TaskCompleted;
        StartCoroutine(task.StartTask());
    }

    private void TaskCompleted(MATS_LevelTask task)
    {
        task.OnTaskCompleted -= TaskCompleted;

        // Optional: handle task states
        if (task.state == MATS_GameManager.TaskState.Failed)
        {
            Debug.Log("Level failed on task: " + task.taskName);
            return;
        }

        currentTaskIndex++;
        ExecuteNextTask();  // automatically start next task
    }

    private void LevelCompleted()
    {
        MATS_Debug.Log("Level " + levels[currentLevelIndex].levelNumber + " completed!");
        currentLevelIndex++;
        StartLevel();  // move to next level
    }
}