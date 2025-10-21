using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CueSystem : MonoBehaviour
{
    [Header("Cue Settings")]
    public float baseCueDisplayTime = 0.6f;
    public float baseTimingWindow = 0.2f;
    public float baseCueCooldown = 0.5f;
    public float randomVariation = 0.15f;
    
    private float currentCueDisplayTime;
    private float currentTimingWindow;
    private float currentCueCooldown;
    
    [Header("UI References")]
    public TextMeshProUGUI currentCueText;
    public GameObject[] cueButtons;
    
    [Header("Cue Types")]
    public string[] cueTypes = {"Jab", "Cross", "Hook", "Uppercut", "Block"};
    public KeyCode[] expectedKeys = {KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.I, KeyCode.Space};
    
    private string currentCue = "";
    private bool cueActive = false;
    private float cueStartTime;
    private GameManager gameManager;
    private TimingSystem timingSystem;
    private CoachAnimator coachAnimator;
    private Coroutine cueLoopCoroutine;
    
    private Color[] originalColors;
    private Vector3[] originalScales;
    
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        timingSystem = FindAnyObjectByType<TimingSystem>();
        coachAnimator = FindAnyObjectByType<CoachAnimator>();
        
        originalColors = new Color[4];
        originalScales = new Vector3[4];
        
        if (cueButtons != null)
        {
            for (int i = 0; i < cueButtons.Length && i < 4; i++)
            {
                if (cueButtons[i] != null)
                {
                    Image buttonImage = cueButtons[i].GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        originalColors[i] = buttonImage.color;
                    }
                    originalScales[i] = cueButtons[i].transform.localScale;
                }
            }
        }
        
        cueLoopCoroutine = StartCoroutine(CueLoop());
        Debug.Log("CueSystem ready!");
    }
    
    IEnumerator CueLoop()
    {
        yield return new WaitForSeconds(0.3f);
        
        while (gameManager != null)
        {
            while (gameManager != null && !gameManager.gameActive)
            {
                yield return null;
            }
            
            while (gameManager != null && gameManager.gameActive)
            {
                UpdateTimingSettings();
                ShowRandomCue();
                
                float randomWait = Random.Range(currentCueCooldown * 0.2f, currentCueCooldown * 0.8f);
                yield return new WaitForSeconds(currentCueDisplayTime + randomWait);
            }
            
            cueActive = false;
            if (currentCueText != null)
            {
                currentCueText.text = "";
            }
            
            if (coachAnimator != null)
            {
                coachAnimator.ReturnToOriginal();
            }
        }
    }
    
    void UpdateTimingSettings()
    {
        float speedMultiplier = gameManager.speedMultiplier;
        
        currentCueDisplayTime = baseCueDisplayTime / speedMultiplier;
        currentTimingWindow = baseTimingWindow / speedMultiplier;
        currentCueCooldown = baseCueCooldown / speedMultiplier;
        
        currentCueDisplayTime = Mathf.Max(currentCueDisplayTime, 0.3f);
        currentTimingWindow = Mathf.Max(currentTimingWindow, 0.15f);
        currentCueCooldown = Mathf.Max(currentCueCooldown, 0.4f);
    }
    
    void ShowRandomCue()
    {
        if (!gameManager.gameActive) return;
        
        int randomIndex = Random.Range(0, cueTypes.Length);
        currentCue = cueTypes[randomIndex];
        cueActive = true;
        cueStartTime = Time.time;
        
        if (currentCueText != null)
        {
            currentCueText.text = "PRESS: " + GetKeyForCue(currentCue);
            currentCueText.color = Color.white;
        }
        
        if (coachAnimator != null)
        {
            coachAnimator.MoveToCuePosition(currentCue);
        }
        
        // Highlight corresponding button with particle effects
        HighlightButton(randomIndex);
        
        Debug.Log($"Cue shown: {currentCue}");
        
        // Start timing window for cue response
        StartCoroutine(CueTimeout());
    }
    
    public bool IsCueActive()
    {
        return cueActive;
    }
    
    IEnumerator CueTimeout()
    {
        yield return new WaitForSeconds(currentCueDisplayTime);
        
        if (cueActive)
        {
            // Player missed the cue - deduct a life!
            cueActive = false;
            gameManager.ResetStreak();
            gameManager.TakeDamage();
            gameManager.ShowFeedback("Miss");
            
            if (currentCueText != null)
            {
                currentCueText.text = "";
            }
            
            if (coachAnimator != null)
            {
                coachAnimator.OnCueComplete();
            }
            
            ResetButtonHighlights();
        }
    }
    
    public bool CheckInput(KeyCode pressedKey)
    {
        if (!cueActive) return false;
        
        int cueIndex = System.Array.IndexOf(cueTypes, currentCue);
        if (cueIndex == -1) return false;
        
        KeyCode expectedKey = expectedKeys[cueIndex];
        
        if (pressedKey == expectedKey)
        {
            // Correct input! Calculate timing for scoring
            float timingOffset = Time.time - (cueStartTime + currentCueDisplayTime * 0.5f);
            cueActive = false;
            
            if (currentCueText != null)
            {
                currentCueText.text = "";
            }
            
            if (coachAnimator != null)
            {
                coachAnimator.OnCueComplete();
            }
            
            ResetButtonHighlights();
            
            if (timingSystem != null)
            {
                timingSystem.JudgeTiming(timingOffset);
            }
            
            return true;
        }
        else
        {
            // Wrong key pressed during active cue - lose a life
            cueActive = false;
            gameManager.ResetStreak();
            gameManager.TakeDamage();
            gameManager.ShowFeedback("Wrong Key!");
            
            if (currentCueText != null)
            {
                currentCueText.text = "";
            }
            
            if (coachAnimator != null)
            {
                coachAnimator.OnCueComplete();
            }
            
            ResetButtonHighlights();
            return false;
        }
    }
    
    string GetKeyForCue(string cue)
    {
        switch (cue)
        {
            case "Jab": return "J";
            case "Cross": return "K";
            case "Hook": return "L";
            case "Uppercut": return "I";
            case "Block": return "SPACE";
            default: return "?";
        }
    }
    
    void HighlightButton(int buttonIndex)
    {
        if (cueButtons == null || buttonIndex < 0 || buttonIndex >= cueButtons.Length || cueButtons[buttonIndex] == null)
            return;
        
        // Start cool visual effects: glowing rings and particle burst
        StartCoroutine(GlowTrailEffect(buttonIndex));
        StartCoroutine(ParticleBurst(buttonIndex));
        
        // Scale up button and change to bright magenta color
        Transform buttonTransform = cueButtons[buttonIndex].transform;
        buttonTransform.localScale = originalScales[buttonIndex] * 1.3f;
        
        Image buttonImage = cueButtons[buttonIndex].GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.magenta;
        }
    }
    
    void ResetButtonHighlights()
    {
        if (cueButtons == null) return;
        
        for (int i = 0; i < cueButtons.Length && i < 4; i++)
        {
            if (cueButtons[i] != null)
            {
                cueButtons[i].transform.localScale = originalScales[i];
                
                Image buttonImage = cueButtons[i].GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = originalColors[i];
                }
            }
        }
    }
    
    IEnumerator GlowTrailEffect(int buttonIndex)
    {
        // Create expanding cyan glow rings around the button
        Transform buttonTransform = cueButtons[buttonIndex].transform;
        
        for (int i = 0; i < 3; i++)
        {
            GameObject glowRing = new GameObject("GlowRing");
            glowRing.transform.SetParent(buttonTransform);
            glowRing.transform.localPosition = Vector3.zero;
            
            Image glowImage = glowRing.AddComponent<Image>();
            glowImage.color = new Color(0, 1, 1, 0.3f);
            
            RectTransform glowRect = glowRing.GetComponent<RectTransform>();
            glowRect.sizeDelta = Vector2.one * 50f;
            
            float animTime = 0f;
            float duration = 0.5f;
            
            while (animTime < duration)
            {
                animTime += Time.deltaTime;
                float progress = animTime / duration;
                
                glowRect.sizeDelta = Vector2.one * (50f + progress * 100f);
                glowImage.color = new Color(0, 1, 1, 0.3f * (1f - progress));
                
                yield return null;
            }
            
            Destroy(glowRing);
        }
    }
    
    IEnumerator ParticleBurst(int buttonIndex)
    {
        // Create yellow sparkles that burst outward from the button
        Transform buttonTransform = cueButtons[buttonIndex].transform;
        
        for (int i = 0; i < 8; i++)
        {
            GameObject sparkle = new GameObject("Sparkle");
            sparkle.transform.SetParent(buttonTransform.parent);
            sparkle.transform.position = buttonTransform.position;
            
            Image sparkleImage = sparkle.AddComponent<Image>();
            sparkleImage.color = Color.yellow;
            
            RectTransform sparkleRect = sparkle.GetComponent<RectTransform>();
            sparkleRect.sizeDelta = Vector2.one * 10f;
            
            Vector3 direction = Random.insideUnitCircle.normalized;
            float speed = 200f;
            float lifetime = 0.5f;
            float time = 0f;
            
            while (time < lifetime)
            {
                time += Time.deltaTime;
                sparkle.transform.position += direction * speed * Time.deltaTime;
                sparkleImage.color = new Color(1, 1, 0, 1f - (time / lifetime));
                yield return null;
            }
            
            Destroy(sparkle);
        }
    }
}
