using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueSpawner : MonoBehaviour
{
    [Header("Cue Settings")]
    public float cueInterval = 2f; // Time between cues
    public string[] cueTypes = {"Jab", "Cross", "Hook", "Uppercut", "Block"};
    
    [Header("References")]
    public Transform cueSpawnPoint;
    
    private GameManager gameManager;
    private float nextCueTime;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        nextCueTime = Time.time + cueInterval;
        Debug.Log("CueSpawner ready!");
    }
    
    // Update is called once per frame
    void Update()
    {
        if (gameManager != null && gameManager.gameActive)
        {
            if (Time.time >= nextCueTime)
            {
                SpawnRandomCue();
                nextCueTime = Time.time + cueInterval;
            }
        }
    }
    
    void SpawnRandomCue()
    {
        string randomCue = cueTypes[Random.Range(0, cueTypes.Length)];
        Debug.Log($"Spawning cue: {randomCue}");
        
        // This will create visual cues later
        // For now, just log to console
    }
    
    public void SpawnSpecificCue(string cueType)
    {
        Debug.Log($"Spawning specific cue: {cueType}");
    }
}