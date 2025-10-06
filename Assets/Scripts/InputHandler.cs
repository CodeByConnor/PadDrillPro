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
    }
    
    void HandleInput(string inputType, KeyCode keyPressed)
    {
        Debug.Log($"Player input: {inputType}");
        
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