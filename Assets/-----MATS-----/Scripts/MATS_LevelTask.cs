using System;
using System.Collections;
using UnityEngine;
using static MATS_GameManager;

public abstract class MATS_LevelTask : MonoBehaviour
{
    [Header("Task Info")]
    public string taskName;
    public bool canFail;
    public bool isOptional;


    [Header("Runtime State (Read Only)")]
    [SerializeField] private TaskState _state = TaskState.Idle;
    [SerializeField] private bool _isCurrentTask;

    public TaskState state => _state;
    public bool IsCurrentTask => _isCurrentTask;

    public event Action<MATS_LevelTask> OnTaskCompleted;

    // ---------------- TASK CONTROL ----------------

    public IEnumerator StartTask()
    {
        SetAsCurrent(true);
        SetState(TaskState.Running);

        MATS_Debug.Log($"TASK STARTED → {taskName}");

        yield return ExecuteTask();

        if (_state == TaskState.Running)
        {
            CompleteTask();
        }
    }

    public void Skip()
    {
        if (_state == TaskState.Running)
        {
            MATS_Debug.Log("TASK SKIPPED → " + taskName);
            SetState(TaskState.Skipped);
            FinishTask();
        }
    }

    public void Fail()
    {
        if (_state == TaskState.Running && canFail)
        {
            MATS_Debug.Log("TASK FAILED → " + taskName);
            SetState(TaskState.Failed);
            FinishTask();
        }
    }

    private void CompleteTask()
    {
        SetState(TaskState.Completed);
        FinishTask();
    }

    private void FinishTask()
    {
        SetAsCurrent(false);
        MATS_Debug.Log($"TASK FINISHED → {taskName} | STATE = {_state}");
        OnTaskCompleted?.Invoke(this);
    }

    // ---------------- HELPERS ----------------

    private void SetState(TaskState newState)
    {
        _state = newState;
    }

    private void SetAsCurrent(bool value)
    {
        _isCurrentTask = value;
    }

    public void ResetRuntimeState()
    {
        _state = TaskState.Idle;
        _isCurrentTask = false;
    }

    protected abstract IEnumerator ExecuteTask();
    public static IEnumerator TweenFloat(
        float startValue,
        float endValue,
        float duration,
        Action<float> onUpdate,
        Action onComplete = null)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float value = Mathf.Lerp(startValue, endValue, t);

            onUpdate?.Invoke(value);
            yield return null;
        }

        // Ensure final value
        onUpdate?.Invoke(endValue);
        onComplete?.Invoke();
    }
    public IEnumerator FadeInFadeOut()
    {
        MATS_UIManager.instance.fadeOutScreenMat.SetFloat("_FadeAmount", -1f);
        MATS_UIManager.instance.fadeInScreenMat.SetFloat("_FadeAmount", -1f);

            MATS_UIManager.instance.fadeInScreen.SetActive(true);

            StartCoroutine(
                TweenFloat(-1f, 1f, 1.5f, value =>
                {

                    MATS_UIManager.instance.fadeInScreenMat.SetFloat("_FadeAmount", value);

                })
            );
            yield return new WaitForSeconds(1.8f);

            MATS_UIManager.instance.fadeInScreen.SetActive(false);
        
        MATS_UIManager.instance.fadeOutScreen.SetActive(true);
        StartCoroutine(
           TweenFloat(-1f, 1f, 1.5f, value =>
           {
               MATS_UIManager.instance.fadeOutScreenMat.SetFloat("_FadeAmount", value);

           })
       );
        yield return new WaitForSeconds(1.8f);
        MATS_UIManager.instance.fadeOutScreen.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        MATS_UIManager.instance.fadeOutScreenMat.SetFloat("_FadeAmount", -1f);
       

        MATS_UIManager.instance.fadeOutScreen.SetActive(true);
        StartCoroutine(
           TweenFloat(-1f, 1f, 1.5f, value =>
           {
               MATS_UIManager.instance.fadeOutScreenMat.SetFloat("_FadeAmount", value);

           })
       );
        yield return new WaitForSeconds(1.8f);
        MATS_UIManager.instance.fadeOutScreen.SetActive(false);
    }
}


