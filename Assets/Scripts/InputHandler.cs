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
        
        if (Input.GetKeyDown(jabKey))
        {
            Debug.Log("JAB pressed!");
            HandleInput("Jab", jabKey);
        }
        
        if (Input.GetKeyDown(crossKey))
        {
            Debug.Log("CROSS pressed!");
            HandleInput("Cross", crossKey);
        }
        
        if (Input.GetKeyDown(hookKey))
        {
            Debug.Log("HOOK pressed!");
            HandleInput("Hook", hookKey);
        }
        
        if (Input.GetKeyDown(uppercutKey))
        {
            Debug.Log("UPPERCUT pressed!");
            HandleInput("Uppercut", uppercutKey);
        }
        
        if (Input.GetKeyDown(blockKey))
        {
            Debug.Log("BLOCK pressed!");
            HandleInput("Block", blockKey);
        }
        
        // Always check for invalid key presses (even if valid key was pressed)
        CheckForInvalidKeyPress();
    }
    
    void CheckForInvalidKeyPress()
    {
        // Check common problematic keys that players might accidentally press
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
            if (key == jabKey || key == crossKey || key == hookKey || key == uppercutKey || key == blockKey)
                continue;
                
            if (IsIgnoredKey(key))
                continue;
                
            if (Input.GetKeyDown(key))
            {
                Debug.Log($"INVALID KEY PRESSED: {key}");
                HandleInvalidKeyPress(key);
                return;
            }
        }
    }
    
    bool IsIgnoredKey(KeyCode key)
    {
        // Ignore system keys to prevent accidental penalties
        return key == KeyCode.Escape ||
               key == KeyCode.R ||
               key == KeyCode.F1 || key == KeyCode.F2 || key == KeyCode.F3 || key == KeyCode.F4 ||
               key == KeyCode.F5 || key == KeyCode.F6 || key == KeyCode.F7 || key == KeyCode.F8 ||
               key == KeyCode.F9 || key == KeyCode.F10 || key == KeyCode.F11 || key == KeyCode.F12 ||
               (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6) ||
               key == KeyCode.LeftShift || key == KeyCode.RightShift ||
               key == KeyCode.LeftControl || key == KeyCode.RightControl ||
               key == KeyCode.LeftAlt || key == KeyCode.RightAlt ||
               key == KeyCode.CapsLock || key == KeyCode.Numlock || key == KeyCode.ScrollLock;
    }
    
    void HandleInvalidKeyPress(KeyCode invalidKey)
    {
        // Check cooldown to prevent spam penalties
        if (Time.time - lastWrongKeyTime < wrongKeyCooldown)
        {
            return;
        }
        
        // Don't penalize wrong keys during active cues (CueSystem handles it)
        if (cueSystem != null && cueSystem.IsCueActive())
        {
            return;
        }
        
        lastWrongKeyTime = Time.time;
        
        Debug.Log($"Wrong key pressed: {invalidKey} - Player loses a life!");
        
        if (fighterAnimator != null)
        {
            fighterAnimator.PlayAnimation("Hurt");
        }
        
        if (gameManager != null)
        {
            gameManager.TakeDamage();
            gameManager.ShowFeedback("Wrong Key!");
        }
        
        // Play heart loss sound for wrong key penalty
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHeartLossSound();
        }
    }
    
    void HandleInput(string inputType, KeyCode keyPressed)
    {
        Debug.Log($"Player input: {inputType}");
        
        if (fighterAnimator != null)
        {
            fighterAnimator.PlayAnimation(inputType);
        }
        
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