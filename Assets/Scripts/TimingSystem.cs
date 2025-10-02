using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingSystem : MonoBehaviour
{
    [Header("Timing Windows")]
    public float perfectWindow = 0.1f;  // Perfect timing window
    public float goodWindow = 0.25f;    // Good timing window
    public float lateWindow = 0.5f;     // Late timing window
    
    [Header("Scoring")]
    public int perfectScore = 100;
    public int goodScore = 50;
    public int lateScore = 10;
    
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        Debug.Log("TimingSystem ready!");
    }
    
    public string JudgeTiming(float timingOffset)
    {
        float absOffset = Mathf.Abs(timingOffset);
        string result;
        
        if (absOffset <= perfectWindow)
        {
            gameManager.AddScore(perfectScore);
            result = "Perfect";
        }
        else if (absOffset <= goodWindow)
        {
            gameManager.AddScore(goodScore);
            result = "Good";
        }
        else if (absOffset <= lateWindow)
        {
            gameManager.AddScore(lateScore);
            result = "Late";
        }
        else
        {
            gameManager.ResetStreak();
            result = "Miss";
        }
        
        // Show feedback on screen
        gameManager.ShowFeedback(result);
        Debug.Log(result + "!");
        
        return result;
    }
    
    public void ProcessInput(string inputType, float currentTime)
    {
        // This will be connected to the cue system later
        Debug.Log($"Processing {inputType} at time {currentTime}");
    }
}