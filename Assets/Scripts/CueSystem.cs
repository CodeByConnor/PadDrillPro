using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CueSystem : MonoBehaviour
{
    [Header("Cue Settings")]
    public float baseCueDisplayTime = 0.6f;  // Shorter display time - more difficult
    public float baseTimingWindow = 0.2f;    // Tighter timing window - more precise
    public float baseCueCooldown = 0.5f;     // Much faster intervals - more intense
    public float randomVariation = 0.15f;    // Less variation for consistent difficulty
    
    private float currentCueDisplayTime;
    private float currentTimingWindow;
    private float currentCueCooldown;
    
    [Header("UI References")]
    public TextMeshProUGUI currentCueText;
    public GameObject[] cueButtons; // J, C, H, U buttons
    
    [Header("Cue Types")]
    public string[] cueTypes = {"Jab", "Cross", "Hook", "Uppercut", "Block"};
    public KeyCode[] expectedKeys = {KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.I, KeyCode.Space};
    
    private string currentCue = "";
    private bool cueActive = false;
    private float cueStartTime;
    private GameManager gameManager;
    private TimingSystem timingSystem;
    private CoachAnimator coachAnimator;
    
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        timingSystem = FindAnyObjectByType<TimingSystem>();
        coachAnimator = FindAnyObjectByType<CoachAnimator>();
        
        StartCoroutine(CueLoop());
        Debug.Log("CueSystem ready!");
    }
    
    IEnumerator CueLoop()
    {
        yield return new WaitForSeconds(0.3f); // Faster initial start
        
        while (gameManager != null && gameManager.gameActive)
        {
            // Update timing based on current speed multiplier
            UpdateTimingSettings();
            
            ShowRandomCue();
            
            // Much tighter and faster random wait time between cues
            float randomWait = Random.Range(currentCueCooldown * 0.2f, currentCueCooldown * 0.8f);
            yield return new WaitForSeconds(currentCueDisplayTime + randomWait);
        }
    }
    
    void UpdateTimingSettings()
    {
        float speedMultiplier = gameManager.speedMultiplier;
        
        // Faster speed = shorter display time and cooldown
        currentCueDisplayTime = baseCueDisplayTime / speedMultiplier;
        currentTimingWindow = baseTimingWindow / speedMultiplier;
        currentCueCooldown = baseCueCooldown / speedMultiplier;
        
        // Clamp to reasonable minimums for intense gameplay
        currentCueDisplayTime = Mathf.Max(currentCueDisplayTime, 0.3f);  // Minimum 0.3s display
        currentTimingWindow = Mathf.Max(currentTimingWindow, 0.15f);     // Minimum 0.15s window
        currentCueCooldown = Mathf.Max(currentCueCooldown, 0.4f);        // Minimum 0.4s cooldown
    }
    
    void ShowRandomCue()
    {
        if (!gameManager.gameActive) return;
        
        // Pick random cue
        int randomIndex = Random.Range(0, cueTypes.Length);
        currentCue = cueTypes[randomIndex];
        cueActive = true;
        cueStartTime = Time.time;
        
        // Show cue on screen
        if (currentCueText != null)
        {
            currentCueText.text = "PRESS: " + GetKeyForCue(currentCue);
            currentCueText.color = Color.white;
        }
        
        // Move coach to cue position
        if (coachAnimator != null)
        {
            coachAnimator.MoveToCuePosition(currentCue);
        }
        
        // Highlight corresponding button
        HighlightButton(randomIndex);
        
        Debug.Log($"Cue shown: {currentCue}");
        
        // Start timing window
        StartCoroutine(CueTimeout());
    }
    
    IEnumerator CueTimeout()
    {
        yield return new WaitForSeconds(currentCueDisplayTime);
        
        if (cueActive)
        {
            // Player missed the cue
            cueActive = false;
            gameManager.ResetStreak();
            gameManager.ShowFeedback("Miss");
            
            if (currentCueText != null)
            {
                currentCueText.text = "";
            }
            
            // Return coach to original position
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
        
        // Find expected key for current cue
        int cueIndex = System.Array.IndexOf(cueTypes, currentCue);
        if (cueIndex == -1) return false;
        
        KeyCode expectedKey = expectedKeys[cueIndex];
        
        if (pressedKey == expectedKey)
        {
            // Correct input! Calculate timing
            float timingOffset = Time.time - (cueStartTime + currentCueDisplayTime * 0.5f);
            cueActive = false;
            
            if (currentCueText != null)
            {
                currentCueText.text = "";
            }
            
            // Return coach to original position
            if (coachAnimator != null)
            {
                coachAnimator.OnCueComplete();
            }
            
            ResetButtonHighlights();
            
            // Judge timing
            if (timingSystem != null)
            {
                timingSystem.JudgeTiming(timingOffset);
            }
            
            return true;
        }
        else
        {
            // Wrong input
            cueActive = false;
            gameManager.ResetStreak();
            gameManager.ShowFeedback("Wrong Key!");
            
            if (currentCueText != null)
            {
                currentCueText.text = "";
            }
            
            // Return coach to original position
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
        // This will highlight the correct button
        // You can implement this based on your button setup
    }
    
    void ResetButtonHighlights()
    {
        // Reset all button highlights
    }
}
