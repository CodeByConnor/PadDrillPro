using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachAnimator : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 25f;       // Much more dramatic dash speed
    public float moveDistance = 2.5f;   // Increased distance for more dramatic movement
    public float snapBackSpeed = 30f;   // Even faster return dash
    
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
    
    // Auto-generated position offsets from original position 
    // Coach realistic fight movement
    private Vector3 jabOffset = new Vector3(2.0f, 0.3f, 0f);       // Jabstep
    private Vector3 crossOffset = new Vector3(1.8f, 0.1f, 0f);     // Cross step
    private Vector3 hookOffset = new Vector3(2.2f, -0.1f, 0f);     // Lateral step
    private Vector3 uppercutOffset = new Vector3(1.9f, -0.6f, 0f); // Uppercut step
    private Vector3 blockOffset = new Vector3(2.1f, 0.4f, 0f);     // Block step
    
    // Realistic movement variation
    private Vector3 positionVariation = new Vector3(0.3f, 0.4f, 0f);    // Much more dramatic variation
    private Vector3 angleVariation = new Vector3(0.2f, 0.3f, 0f);       // Angle changes
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
            
            // Add visual feedback when starting movement
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
                // Dramatic jab step
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 1.2f, positionVariation.x * 0.8f),
                    Random.Range(-angleVariation.y * 1.5f, angleVariation.y * 1.8f),
                    0f
                );
                break;
                
            case "cross":
                // Cross movements with wide variation
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 1.5f, positionVariation.x * 1.0f),
                    Random.Range(-angleVariation.y * 1.2f, angleVariation.y * 1.4f),
                    0f
                );
                break;
                
            case "hook":
                // Lateral hook movements
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 2.0f, positionVariation.x * 1.8f),
                    Random.Range(-angleVariation.y * 1.8f, angleVariation.y * 2.0f),
                    0f
                );
                break;
                
            case "uppercut":
                // Uppercut positioning
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 1.0f, positionVariation.x * 1.0f),
                    Random.Range(-angleVariation.y * 1.5f, angleVariation.y * 1.2f),
                    0f
                );
                break;
                
            case "block":
                // Dramatic blocking movements 
                variation = new Vector3(
                    Random.Range(-positionVariation.x * 1.3f, positionVariation.x * 1.1f),
                    Random.Range(-angleVariation.y * 1.4f, angleVariation.y * 1.6f),
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
            // Create color effects based on cue type
            Color flashColor = GetFlashColorForCue(cueType);
            
            // Quick flash
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(feedbackDuration * 0.3f);
            
            // White flash for impact
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
