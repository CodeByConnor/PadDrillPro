using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAnimator : MonoBehaviour
{
    [Header("Fighter Sprites")]
    public Sprite idleSprite;
    public Sprite jabSprite;
    public Sprite crossSprite;
    public Sprite hookSprite;
    public Sprite uppercutSprite;
    public Sprite blockSprite;
    
    [Header("Animation Settings")]
    public float animationDuration = 0.3f;
    
    private SpriteRenderer spriteRenderer;
    private Coroutine currentAnimation;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set to idle sprite at start
        if (idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
        
        Debug.Log("FighterAnimator ready!");
    }
    
    public void PlayAnimation(string animationType)
    {
        // Stop any current animation
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        // Start new animation
        currentAnimation = StartCoroutine(AnimateSprite(animationType));
    }
    
    IEnumerator AnimateSprite(string animationType)
    {
        Sprite targetSprite = GetSpriteForAnimation(animationType);
        
        if (targetSprite != null)
        {
            // Change to animation sprite
            spriteRenderer.sprite = targetSprite;
            
            // Wait for animation duration
            yield return new WaitForSeconds(animationDuration);
            
            // Return to idle
            if (idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
            }
        }
        
        currentAnimation = null;
    }
    
    Sprite GetSpriteForAnimation(string animationType)
    {
        switch (animationType.ToLower())
        {
            case "jab":
                return jabSprite;
            case "cross":
                return crossSprite;
            case "hook":
                return hookSprite;
            case "uppercut":
                return uppercutSprite;
            case "block":
                return blockSprite;
            default:
                return idleSprite;
        }
    }
}
