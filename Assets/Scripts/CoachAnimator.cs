using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachAnimator : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;       // Much faster dash speed
    public float moveDistance = 1.5f;
    public float snapBackSpeed = 20f;   // Even faster return dash
    
    [Header("Visual Feedback")]
    public bool showMovementFeedback = true;
    public float feedbackDuration = 0.08f;  // Shorter flash for faster movement
    public bool useDynamicPositioning = true; // Add subtle randomness to positions
    
    [Header("Position Settings")]
    public Transform jabPosition;
    public Transform crossPosition;
    public Transform hookPosition;
    public Transform uppercutPosition;
    public Transform blockPosition;
    
    [Header("Auto-Generated Positions")]
    public bool useAutoPositions = true;
    
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isReturning = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 currentMoveTarget;
    
    // Auto-generated position offsets from original position (overlap fighter positioning)
    // Coach dashes to overlap/touch the fighter for realistic pad contact
    private Vector3 jabOffset = new Vector3(0.2f, 0.3f, 0f);       // Overlap fighter for jab contact
    private Vector3 crossOffset = new Vector3(0.1f, 0.1f, 0f);     // Overlap fighter for cross contact  
    private Vector3 hookOffset = new Vector3(0.3f, -0.1f, 0f);     // Overlap fighter for hook contact
    private Vector3 uppercutOffset = new Vector3(0.2f, -0.4f, 0f); // Overlap fighter for uppercut contact
    private Vector3 blockOffset = new Vector3(0.4f, 0.2f, 0f);     // Close overlap for blocking
    
    // Minimal variation ranges (tight overlap positioning)
    private Vector3 positionVariation = new Vector3(0.05f, 0.08f, 0f);  // Tiny variation to maintain overlap
    private Vector3 angleVariation = new Vector3(0.03f, 0.1f, 0f);      // Small angle changes
    
    void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition;
        
        // Get sprite renderer for visual feedback
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Set up auto positions if not manually assigned
        if (useAutoPositions)
        {
            SetupAutoPositions();
        }
        
        Debug.Log("CoachAnimator ready!");
    }
    
    void SetupAutoPositions()
    {
        // Create empty GameObjects for positions if they don't exist
        if (jabPosition == null)
            jabPosition = CreatePositionMarker("JabPosition", originalPosition + jabOffset);
        if (crossPosition == null)
            crossPosition = CreatePositionMarker("CrossPosition", originalPosition + crossOffset);
        if (hookPosition == null)
            hookPosition = CreatePositionMarker("HookPosition", originalPosition + hookOffset);
        if (uppercutPosition == null)
            uppercutPosition = CreatePositionMarker("UppercutPosition", originalPosition + uppercutOffset);
        if (blockPosition == null)
            blockPosition = CreatePositionMarker("BlockPosition", originalPosition + blockOffset);
    }
    
    Transform CreatePositionMarker(string name, Vector3 position)
    {
        GameObject marker = new GameObject(name);
        marker.transform.position = position;
        marker.transform.SetParent(transform.parent); // Same parent as coach
        return marker.transform;
    }
    
    void Update()
    {
        // Smooth movement towards target position with dynamic speed
        if (isMoving)
        {
            float currentSpeed = isReturning ? snapBackSpeed : moveSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            
            // Check if we've reached the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isMoving = false;
                isReturning = false;
            }
        }
    }
    
    public void MoveToCuePosition(string cueType)
    {
        Vector3 baseTarget = GetPositionForCue(cueType);
        
        if (baseTarget != Vector3.zero)
        {
            // Add dynamic variation for unpredictable positioning
            Vector3 dynamicTarget = baseTarget;
            
            if (useDynamicPositioning)
            {
                // Add random variation based on cue type
                Vector3 randomVariation = GetRandomVariation(cueType);
                dynamicTarget += randomVariation;
            }
            
            targetPosition = dynamicTarget;
            currentMoveTarget = dynamicTarget;
            isMoving = true;
            isReturning = false;
            
            // Add dramatic visual feedback when starting movement
            if (showMovementFeedback)
            {
                StartCoroutine(DramaticMovementFeedback(cueType));
            }
            
            Debug.Log($"Coach aggressively dashing to {cueType} position at {dynamicTarget}");
        }
    }
    
    public void ReturnToOriginal()
    {
        targetPosition = originalPosition;
        isMoving = true;
        isReturning = true; // Use faster snap-back speed
        Debug.Log("Coach snapping back to original position");
    }
    
    Vector3 GetPositionForCue(string cueType)
    {
        switch (cueType.ToLower())
        {
            case "jab":
                return jabPosition != null ? jabPosition.position : originalPosition + jabOffset;
            case "cross":
                return crossPosition != null ? crossPosition.position : originalPosition + crossOffset;
            case "hook":
                return hookPosition != null ? hookPosition.position : originalPosition + hookOffset;
            case "uppercut":
                return uppercutPosition != null ? uppercutPosition.position : originalPosition + uppercutOffset;
            case "block":
                return blockPosition != null ? blockPosition.position : originalPosition + blockOffset;
            default:
                return originalPosition;
        }
    }
    
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
    
    Vector3 GetRandomVariation(string cueType)
    {
        // Create minimal variations that keep coach in contact range
        Vector3 variation = Vector3.zero;
        
        switch (cueType.ToLower())
        {
            case "jab":
                // Quick jabs with tiny adjustments to stay in range
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 0.5f, positionVariation.x * 0.5f),
                    Random.Range(-angleVariation.y * 0.8f, angleVariation.y * 1.0f),
                    0f
                );
                break;
                
            case "cross":
                // Crosses with slight positioning but stay close
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 0.7f, positionVariation.x * 0.3f),
                    Random.Range(-angleVariation.y * 0.6f, angleVariation.y * 0.8f),
                    0f
                );
                break;
                
            case "hook":
                // Hooks with small lateral movement in contact range
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 0.8f, positionVariation.x * 0.6f),
                    Random.Range(-angleVariation.y * 1.0f, angleVariation.y * 1.0f),
                    0f
                );
                break;
                
            case "uppercut":
                // Uppercuts stay very close for low contact
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 0.4f, positionVariation.x * 0.4f),
                    Random.Range(-angleVariation.y * 0.5f, angleVariation.y * 0.7f),
                    0f
                );
                break;
                
            case "block":
                // Blocks stay extremely close for contact
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 0.3f, positionVariation.x * 0.3f),
                    Random.Range(-angleVariation.y * 0.5f, angleVariation.y * 0.8f),
                    0f
                );
                break;
        }
        
        return variation;
    }
    
    // Call this when cue times out or is completed
    public void OnCueComplete()
    {
        StartCoroutine(DelayedReturn());
    }
    
    IEnumerator DelayedReturn()
    {
        yield return new WaitForSeconds(0.2f); // Slightly faster return
        ReturnToOriginal();
    }
    
    IEnumerator DramaticMovementFeedback(string cueType)
    {
        if (spriteRenderer != null)
        {
            // Create dramatic color effects based on cue type
            Color flashColor = GetFlashColorForCue(cueType);
            
            // Quick intense flash
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(feedbackDuration * 0.3f);
            
            // Brief white flash for impact
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(feedbackDuration * 0.4f);
            
            // Return to original with slight color tint during movement
            Color tintedColor = Color.Lerp(originalColor, flashColor, 0.3f);
            spriteRenderer.color = tintedColor;
            yield return new WaitForSeconds(feedbackDuration * 0.3f);
            
            spriteRenderer.color = originalColor;
        }
    }
    
    Color GetFlashColorForCue(string cueType)
    {
        switch (cueType.ToLower())
        {
            case "jab":
                return Color.yellow;      // Quick jab = yellow flash
            case "cross":
                return Color.red;         // Power cross = red flash
            case "hook":
                return new Color(1f, 0.5f, 0f); // Hook = orange flash
            case "uppercut":
                return Color.magenta;     // Uppercut = magenta flash
            case "block":
                return Color.cyan;        // Block = cyan flash
            default:
                return Color.white;
        }
    }
}
