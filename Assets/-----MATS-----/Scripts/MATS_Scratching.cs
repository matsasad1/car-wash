using ScratchCardAsset;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class ScratchPart
{
    public SpriteRenderer spriteRenderer;
    [HideInInspector] public Material scratchMaterial;
    [HideInInspector] public Material eraserMaterial;
    [HideInInspector] public Material progressMaterial;
    [HideInInspector] public float progress; // 0-1
    [HideInInspector] public bool isComplete;
}

public class MATS_Scratching : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScratchCard card; // Single ScratchCard reused
    [SerializeField] private Camera mainCamera;

    [Header("Parts")]
    public List<ScratchPart> scratchParts = new List<ScratchPart>();

    [Header("Brush")]
    [SerializeField] private Texture eraseTexture;
    [SerializeField] private Vector2 eraseTextureScale = Vector2.one;

    [Header("Shaders")]
    [SerializeField] private Shader maskShader;
    [SerializeField] private Shader brushShader;
    [SerializeField] private Shader progressShader;
    [SerializeField] private Shader progressCutOffShader;
    [SerializeField] private bool spriteHasAlpha = true;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip scratchLoop;
    [SerializeField] private AudioClip resetClip;

    [Header("Public Progress")]
    [Range(0f, 1f)]
    public float overallProgress; // 0-1 across all parts

    private bool isScratching;
    private ScratchPart currentPart;

    private void Awake()
    {
        if (!card || scratchParts.Count == 0)
        {
            Debug.LogError("MATS_Scratching: Missing references!");
            enabled = false;
            return;
        }

        card.MainCamera = mainCamera != null ? mainCamera : Camera.main;

        // Initialize each part
        foreach (var part in scratchParts)
        {
            if (part.spriteRenderer == null)
            {
                Debug.LogError("MATS_Scratching: Missing SpriteRenderer in part!");
                continue;
            }

            part.scratchMaterial = new Material(maskShader);
            part.scratchMaterial.mainTexture = part.spriteRenderer.sprite.texture;

            part.eraserMaterial = new Material(brushShader);
            part.eraserMaterial.mainTexture = eraseTexture;

            part.progressMaterial = new Material(spriteHasAlpha ? progressCutOffShader : progressShader);

            part.spriteRenderer.material = part.scratchMaterial;

            part.progress = 0f;
            part.isComplete = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentPart = GetPartUnderMouse();
            if (currentPart != null && !currentPart.isComplete)
            {
                isScratching = true;
                OnScratchStart();
                SetupScratchCardForPart(currentPart);
            }
        }

        if (Input.GetMouseButtonUp(0) && isScratching)
        {
            isScratching = false;
            OnScratchEnd();
        }

        if (isScratching && currentPart != null)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            card.ScratchHole(mousePos);
            UpdatePartProgress(currentPart);
            UpdateOverallProgress();
        }
    }

    private ScratchPart GetPartUnderMouse()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        foreach (var part in scratchParts)
        {
            var collider = part.spriteRenderer.GetComponent<Collider2D>();
            if (collider != null && collider.OverlapPoint(mousePos))
                return part;
        }
        return null;
    }

    private void SetupScratchCardForPart(ScratchPart part)
    {
        card.Surface = part.spriteRenderer.transform;
        card.ScratchSurface = part.scratchMaterial;
        card.Eraser = part.eraserMaterial;
        card.Progress = part.progressMaterial;
        card.BrushScale = eraseTextureScale;
        card.ResetRenderTexture();
    }

    private void OnScratchStart()
    {
        if (audioSource != null && scratchLoop != null)
        {
            audioSource.clip = scratchLoop;
            audioSource.Play();
        }
    }

    private void OnScratchEnd()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void UpdatePartProgress(ScratchPart part)
    {
        Texture2D tex = card.GetScratchTexture();
        if (tex == null) return;

        Color32[] pixels = tex.GetPixels32();
        int total = pixels.Length;
        int clearPixels = 0;

        for (int i = 0; i < total; i++)
        {
            if (pixels[i].a < 128) clearPixels++;
        }

        part.progress = (float)clearPixels / total;
        part.isComplete = part.progress >= 1f;

        Destroy(tex);
    }

    private void UpdateOverallProgress()
    {
        float total = scratchParts.Count;
        float sum = 0f;

        foreach (var part in scratchParts)
        {
            sum += part.progress;
        }

        overallProgress = Mathf.Clamp01(sum / total);
    }

    public void ResetScratch()
    {
        foreach (var part in scratchParts)
        {
            part.progress = 0f;
            part.isComplete = false;
            part.spriteRenderer.material = part.scratchMaterial;
        }

        overallProgress = 0f;

        if (audioSource != null && resetClip != null)
            audioSource.PlayOneShot(resetClip);
    }
}