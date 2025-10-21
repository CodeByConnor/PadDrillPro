using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float roundDuration = 90f;
    public int targetBPM = 120;
    
    [Header("Score System")]
    public int score = 0;
    public int streak = 0;
    public int maxStreak = 0;
    public int successfulHits = 0;
    
    [Header("Health System")]
    public int maxHealth = 5;
    public int currentHealth = 5;
    
    [Header("Difficulty System")]
    public int hitsForSpeedIncrease = 3;
    public float speedMultiplier = 1.0f;
    public float maxSpeedMultiplier = 4.0f;
    
    [Header("Game State")]
    public bool gameActive = false;
    public float timeRemaining;
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI enduranceLabel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI restartText;
    
    private HeartUI heartUI;
    
    // Start is called before the first frame update
    void Start()
    {
        heartUI = FindAnyObjectByType<HeartUI>();
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        if (feedbackText != null)
            feedbackText.text = "";
            
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
        
        if (!gameActive && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    
    public void StartRound()
    {
        gameActive = true;
        timeRemaining = roundDuration;
        score = 0;
        streak = 0;
        successfulHits = 0;
        currentHealth = maxHealth;
        speedMultiplier = 1.0f;
        
        Debug.Log($"Round Started! Duration: {roundDuration} seconds, Time Remaining: {timeRemaining}");
    }
    
    public void EndRound()
    {
        gameActive = false;
        ShowGameOverScreen();
        Debug.Log($"Round Ended! Final Score: {score}, Max Streak: {maxStreak}");
    }
    
    public void AddScore(int points)
    {
        score += points;
        streak++;
        successfulHits++;
        
        if (streak > maxStreak)
        {
            maxStreak = streak;
        }
        
        // Play streak sound for every 3 hits in a row
        if (streak > 0 && streak % 3 == 0)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayStreakSound();
            }
        }
        
        // Check for speed increase every 3 hits - makes game more intense
        if (successfulHits % hitsForSpeedIncrease == 0 && speedMultiplier < maxSpeedMultiplier)
        {
            speedMultiplier *= 1.5f;
            ShowFeedback($"SPEED UP! x{speedMultiplier:F1}");
            Debug.Log($"Speed increased to {speedMultiplier:F1}x after {successfulHits} hits");
        }
    }
    
    public void TakeDamage()
    {
        if (currentHealth <= 0 || !gameActive)
        {
            return;
        }
        
        currentHealth--;
        Debug.Log($"Health lost! Remaining: {currentHealth}");
        
        // Play heart loss sound and animate heart disappearing
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHeartLossSound();
        }
        
        if (heartUI != null)
        {
            heartUI.AnimateHeartLoss();
        }
        
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    
    public void GameOver()
    {
        gameActive = false;
        ShowGameOverScreen();
        ShowFeedback("GAME OVER!");
        Debug.Log($"GAME OVER! Final Score: {score}, Max Streak: {maxStreak}");
    }
    
    public void ResetStreak()
    {
        streak = 0;
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
            
        if (streakText != null)
            streakText.text = "STREAK: " + streak;
            
        if (timerText != null)
            timerText.text = "ROUND TIMER: " + Mathf.Ceil(timeRemaining).ToString();
            
        if (enduranceLabel != null)
            enduranceLabel.text = "ENDURANCE:";
    }
    
    public void ShowFeedback(string feedback)
    {
        if (feedbackText != null)
        {
            feedbackText.text = feedback;
            
            // Change color based on feedback type for visual feedback
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
            
            StartCoroutine(ClearFeedback());
        }
    }
    
    IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(1f);
        if (feedbackText != null)
            feedbackText.text = "";
    }
    
    void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (finalScoreText != null)
        {
            finalScoreText.text = $"FINAL SCORE: {score}\nMAX STREAK: {maxStreak}\nTIME SURVIVED: {Mathf.Ceil(roundDuration - timeRemaining)}s";
        }
        
        if (restartText != null)
        {
            restartText.text = "PRESS R TO RESTART";
        }
    }
    
    public void RestartGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (heartUI != null)
        {
            heartUI.ResetHearts();
        }
        
        StartRound();
        
        if (feedbackText != null)
            feedbackText.text = "";
            
        Debug.Log("Game restarted!");
    }
}