using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("Heart Settings")]
    public GameObject heartPrefab;
    public Sprite heartSprite;    
    public Transform heartContainer;  // Parent object for hearts
    public int maxHearts = 5;
    public float heartSize = 1.0f;  // Scale multiplier for heart size
    
    private List<GameObject> heartObjects = new List<GameObject>();
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        Debug.Log("HeartUI Start - GameManager found: " + (gameManager != null));
        Debug.Log("HeartContainer assigned: " + (heartContainer != null));
        Debug.Log("HeartSprite assigned: " + (heartSprite != null));
        CreateHearts();
    }
    
    void CreateHearts()
    {
        Debug.Log("Creating hearts...");
        
        // Clear existing hearts
        foreach (GameObject heart in heartObjects)
        {
            if (heart != null)
                DestroyImmediate(heart);
        }
        heartObjects.Clear();
        
        // Check if container exists
        if (heartContainer == null)
        {
            Debug.LogError("HeartContainer is null! Please assign it in the inspector.");
            return;
        }
        
        // Create heart objects
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heart = CreateHeart();
            heartObjects.Add(heart);
            Debug.Log($"Created heart {i + 1}");
        }
        
        UpdateHeartDisplay();
    }
    
    GameObject CreateHeart()
    {
        GameObject heart;
        
        if (heartPrefab != null)
        {
            // Use provided heart prefab
            heart = Instantiate(heartPrefab, heartContainer);
            heart.transform.localScale = Vector3.one * heartSize;  // Apply size multiplier
        }
        else
        {
            // Create simple heart 
            heart = new GameObject("Heart");
            heart.transform.SetParent(heartContainer);
            
            Image heartImage = heart.AddComponent<Image>();
            
            // Use provided sprite or default to red color
            if (heartSprite != null)
            {
                heartImage.sprite = heartSprite;
            }
            else
            {
                heartImage.color = Color.red;
            }
            
            // Set size - make them more visible
            RectTransform rectTransform = heart.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(30, 30);  // Base size
            rectTransform.localScale = Vector3.one * heartSize;  // Apply size multiplier
        }
        
        return heart;
    }
    
    void Update()
    {
        if (gameManager != null)
        {
            UpdateHeartDisplay();
        }
    }
    
    void UpdateHeartDisplay()
    {
        if (gameManager == null) return;
        
        for (int i = 0; i < heartObjects.Count; i++)
        {
            if (heartObjects[i] != null)
            {
                // Show heart if player has this much health remaining
                bool shouldShow = i < gameManager.currentHealth;
                heartObjects[i].SetActive(shouldShow);
            }
        }
    }
    
    public void AnimateHeartLoss()
    {
        StartCoroutine(HeartLossAnimation());
    }
    
    public void ResetHearts()
    {
        // Reset all hearts to full visibility
        for (int i = 0; i < heartObjects.Count; i++)
        {
            if (heartObjects[i] != null)
            {
                GameObject heart = heartObjects[i];
                
                // Reset scale
                heart.transform.localScale = Vector3.one * heartSize;
                
                // Reset color
                Image heartImage = heart.GetComponent<Image>();
                if (heartImage != null)
                {
                    heartImage.color = new Color(heartImage.color.r, heartImage.color.g, heartImage.color.b, 1f);
                }
                
                // Make sure it's active
                heart.SetActive(true);
            }
        }
        
        Debug.Log("Hearts reset to full visibility");
    }
    
    IEnumerator HeartLossAnimation()
    {
        // Find the heart that just got lost
        int lostHeartIndex = gameManager.currentHealth; // This is now the lost heart
        
        if (lostHeartIndex < heartObjects.Count && heartObjects[lostHeartIndex] != null)
        {
            GameObject lostHeart = heartObjects[lostHeartIndex];
            
            Vector3 originalScale = lostHeart.transform.localScale;
            
            // Scale up briefly
            float timer = 0f;
            while (timer < 0.1f)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(1f, 1.5f, timer / 0.1f);
                lostHeart.transform.localScale = originalScale * scale;
                yield return null;
            }
            
            // Scale back down
            timer = 0f;
            while (timer < 0.2f)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(1.5f, 1f, timer / 0.2f);
                lostHeart.transform.localScale = originalScale * scale;
                yield return null;
            }
            
            // Fade out
            Image heartImage = lostHeart.GetComponent<Image>();
            if (heartImage != null)
            {
                timer = 0f;
                Color originalColor = heartImage.color;
                while (timer < 0.3f)
                {
                    timer += Time.deltaTime;
                    float alpha = Mathf.Lerp(1f, 0f, timer / 0.3f);
                    heartImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    yield return null;
                }
            }
        }
    }
}
