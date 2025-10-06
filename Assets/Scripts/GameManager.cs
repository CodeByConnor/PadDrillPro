using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float roundDuration = 90f;   // Increased to 90 seconds for more gameplay
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
    public int hitsForSpeedIncrease = 3;     // Changed from 5 to 3
    public float speedMultiplier = 1.0f;
    public float maxSpeedMultiplier = 4.0f;  // Increased max for more challenge
    
    [Header("Game State")]
    public bool gameActive = false;
    public float timeRemaining;
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI enduranceLabel;  // "Endurance:" label
    public GameObject gameOverPanel;        // Game over screen panel
    public TextMeshProUGUI finalScoreText; // Final score display
    public TextMeshProUGUI restartText;     // Restart instruction text
    
    private HeartUI heartUI;
    
    // Start is called before the first frame update
    void Start()
    {
        heartUI = FindAnyObjectByType<HeartUI>();
        
        // Hide game over panel at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
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
        
        // Check for restart input when game is over
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
        
        // Check for speed increase every 3 hits
        if (successfulHits % hitsForSpeedIncrease == 0 && speedMultiplier < maxSpeedMultiplier)
        {
            speedMultiplier *= 1.5f;  // 50% increase each time - faster progression
            ShowFeedback($"SPEED UP! x{speedMultiplier:F1}");
            Debug.Log($"Speed increased to {speedMultiplier:F1}x after {successfulHits} hits");
        }
    }
    
    public void TakeDamage()
    {
        currentHealth--;
        Debug.Log($"Health lost! Remaining: {currentHealth}");
        
        // Play heart loss sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHeartLossSound();
        }
        
        // Trigger heart loss animation
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
        TakeDamage(); // Lose health when streak is broken
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
            
        if (streakText != null)
            streakText.text = "Streak: " + streak;
            
        if (timerText != null)
            timerText.text = "Round Timer: " + Mathf.Ceil(timeRemaining).ToString();
            
        if (enduranceLabel != null)
            enduranceLabel.text = "Endurance:";
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
            restartText.text = "Press R to Restart";
        }
    }
    
    public void RestartGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Reset all game state
        StartRound();
        
        // Clear any lingering feedback
        if (feedbackText != null)
            feedbackText.text = "";
            
        Debug.Log("Game restarted!");
    }
}