using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float roundDuration = 60f;
    public int targetBPM = 120;
    
    [Header("Score System")]
    public int score = 0;
    public int streak = 0;
    public int maxStreak = 0;
    
    [Header("Game State")]
    public bool gameActive = false;
    public float timeRemaining;
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI feedbackText;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManager started!");
        StartRound();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            timeRemaining -= Time.deltaTime;
            
            if (timeRemaining <= 0)
            {
                EndRound();
            }
            
            UpdateUI();
        }
    }
    
    public void StartRound()
    {
        gameActive = true;
        timeRemaining = roundDuration;
        score = 0;
        streak = 0;
        Debug.Log("Round Started!");
    }
    
    public void EndRound()
    {
        gameActive = false;
        Debug.Log($"Round Ended! Final Score: {score}, Max Streak: {maxStreak}");
    }
    
    public void AddScore(int points)
    {
        score += points;
        streak++;
        if (streak > maxStreak)
        {
            maxStreak = streak;
        }
    }
    
    public void ResetStreak()
    {
        streak = 0;
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
            
        if (streakText != null)
            streakText.text = "Streak: " + streak;
            
        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
    }
    
    public void ShowFeedback(string feedback)
    {
        if (feedbackText != null)
        {
            feedbackText.text = feedback;
            
            // Change color based on feedback
            switch (feedback)
            {
                case "Perfect":
                    feedbackText.color = Color.green;
                    break;
                case "Good":
                    feedbackText.color = Color.yellow;
                    break;
                case "Late":
                    feedbackText.color = Color.orange;
                    break;
                case "Miss":
                    feedbackText.color = Color.red;
                    break;
            }
            
            // Clear feedback after 1 second
            StartCoroutine(ClearFeedback());
        }
    }
    
    IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(1f);
        if (feedbackText != null)
            feedbackText.text = "";
    }
}