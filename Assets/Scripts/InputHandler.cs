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
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        timingSystem = GetComponent<TimingSystem>();
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
            HandleInput("Jab");
        }
        
        if (Input.GetKeyDown(crossKey))
        {
            Debug.Log("CROSS pressed!");
            HandleInput("Cross");
        }
        
        if (Input.GetKeyDown(hookKey))
        {
            Debug.Log("HOOK pressed!");
            HandleInput("Hook");
        }
        
        if (Input.GetKeyDown(uppercutKey))
        {
            Debug.Log("UPPERCUT pressed!");
            HandleInput("Uppercut");
        }
        
        if (Input.GetKeyDown(blockKey))
        {
            Debug.Log("BLOCK pressed!");
            HandleInput("Block");
        }
    }
    
    void HandleInput(string inputType)
    {
        Debug.Log($"Player input: {inputType}");
        
        // For now, just trigger timing judgment with a random offset
        // Later this will be based on actual cue timing
        float randomOffset = Random.Range(-0.3f, 0.3f);
        
        if (timingSystem != null)
        {
            timingSystem.JudgeTiming(randomOffset);
        }
    }
}