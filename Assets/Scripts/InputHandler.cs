using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Input Keys")]
    public KeyCode jabKey = KeyCode.J;
    public KeyCode crossKey = KeyCode.K;
    public KeyCode hookKey = KeyCode.L;
    public KeyCode uppercutKey = KeyCode.I;
    public KeyCode blockKey = KeyCode.Space;
    
    private GameManager gameManager;
    private TimingSystem timingSystem;
    private FighterAnimator fighterAnimator;
    private CueSystem cueSystem;
    
    // Cooldown for wrong key penalties to prevent spam (reduced for better responsiveness)
    private float wrongKeyCooldown = 0.1f;
    private float lastWrongKeyTime = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        timingSystem = GetComponent<TimingSystem>();
        fighterAnimator = FindAnyObjectByType<FighterAnimator>();
        cueSystem = FindAnyObjectByType<CueSystem>();
        Debug.Log("InputHandler ready!");
    }
    
    // Update is called once per frame
    void Update()
    {
        if (gameManager != null && gameManager.gameActive)
        {
            CheckInputs();
        }
    }
    
    void CheckInputs()
    {
        // Check for valid game keys first
        bool validKeyPressed = false;
        
        if (Input.GetKeyDown(jabKey))
        {
            Debug.Log("JAB pressed!");
            HandleInput("Jab", jabKey);
            validKeyPressed = true;
        }
        
        if (Input.GetKeyDown(crossKey))
        {
            Debug.Log("CROSS pressed!");
            HandleInput("Cross", crossKey);
            validKeyPressed = true;
        }
        
        if (Input.GetKeyDown(hookKey))
        {
            Debug.Log("HOOK pressed!");
            HandleInput("Hook", hookKey);
            validKeyPressed = true;
        }
        
        if (Input.GetKeyDown(uppercutKey))
        {
            Debug.Log("UPPERCUT pressed!");
            HandleInput("Uppercut", uppercutKey);
            validKeyPressed = true;
        }
        
        if (Input.GetKeyDown(blockKey))
        {
            Debug.Log("BLOCK pressed!");
            HandleInput("Block", blockKey);
            validKeyPressed = true;
        }
        
        // Always check for invalid key presses (even if valid key was pressed)
        CheckForInvalidKeyPress();
    }
    
    void CheckForInvalidKeyPress()
    {
        // Hardcoded keys to prevent accidental penalties
        KeyCode[] commonKeys = {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H,
            KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
            KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z,
            KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
            KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
            KeyCode.Return, KeyCode.Tab, KeyCode.Backspace, KeyCode.Delete,
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow
        };
        
        foreach (KeyCode key in commonKeys)
        {
            // Skip if it's one of our valid keys
            if (key == jabKey || key == crossKey || key == hookKey || key == uppercutKey || key == blockKey)
                continue;
                
            // Skip ignored keys
            if (IsIgnoredKey(key))
                continue;
                
            if (Input.GetKeyDown(key))
            {
                Debug.Log($"INVALID KEY PRESSED: {key}");
                HandleInvalidKeyPress(key);
                return; // Only process one invalid key per frame
            }
        }
    }
    
    bool IsIgnoredKey(KeyCode key)
    {
        // Ignore these keys to prevent accidental penalties
        return key == KeyCode.Escape ||           // Escape key
               key == KeyCode.R ||                // Restart key
               key == KeyCode.F1 || key == KeyCode.F2 || key == KeyCode.F3 || key == KeyCode.F4 ||
               key == KeyCode.F5 || key == KeyCode.F6 || key == KeyCode.F7 || key == KeyCode.F8 ||
               key == KeyCode.F9 || key == KeyCode.F10 || key == KeyCode.F11 || key == KeyCode.F12 ||
               (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6) ||  // Mouse buttons
               key == KeyCode.LeftShift || key == KeyCode.RightShift ||  // Shift keys
               key == KeyCode.LeftControl || key == KeyCode.RightControl ||  // Ctrl keys
               key == KeyCode.LeftAlt || key == KeyCode.RightAlt ||  // Alt keys
               key == KeyCode.CapsLock || key == KeyCode.Numlock || key == KeyCode.ScrollLock;  // Lock keys
    }
    
    void HandleInvalidKeyPress(KeyCode invalidKey)
    {
        // Check cooldown to prevent spam
        if (Time.time - lastWrongKeyTime < wrongKeyCooldown)
        {
            return; // Still in cooldown, ignore this key press
        }
        
        // Check if this wrong key is being handled by CueSystem (during active cue)
        if (cueSystem != null && cueSystem.IsCueActive())
        {
            return; // Let CueSystem handle wrong keys during active cues
        }
        
        lastWrongKeyTime = Time.time;
        
        Debug.Log($"Wrong key pressed: {invalidKey} - Player loses a life!");
        
        // Trigger fighter animation for getting hit
        if (fighterAnimator != null)
        {
            fighterAnimator.PlayAnimation("Hurt");
        }
        
        // Player loses a life for pressing wrong key
        if (gameManager != null)
        {
            gameManager.TakeDamage();
            gameManager.ShowFeedback("Wrong Key!");
        }
        
        // Play sound for wrong key
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHeartLossSound(); // Use the same sound as taking damage
        }
    }
    
    void HandleInput(string inputType, KeyCode keyPressed)
    {
        Debug.Log($"Player input: {inputType}");
        
        // Guard rail: Check if a cue is active before allowing valid key presses
        if (cueSystem != null && !cueSystem.IsCueActive())
        {
            // No active cue - treat this as a wrong key press
            Debug.Log($"No active cue - {inputType} pressed too early/late!");
            HandleInvalidKeyPress(keyPressed);
            return;
        }
        
        // Trigger fighter animation
        if (fighterAnimator != null)
        {
            fighterAnimator.PlayAnimation(inputType);
        }
        
        // Check with cue system for proper timing
        if (cueSystem != null)
        {
            bool validInput = cueSystem.CheckInput(keyPressed);
            if (validInput)
            {
                // Play punch sound for successful hit
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayPunchSound(inputType);
                }
            }
            else
            {
                // Wrong key or no active cue - don't give points
                Debug.Log("Invalid input timing or wrong key");
            }
        }
        else
        {
            // Fallback to old random system if no cue system
            float randomOffset = Random.Range(-0.3f, 0.3f);
            if (timingSystem != null)
            {
                timingSystem.JudgeTiming(randomOffset);
            }
        }
    }
}