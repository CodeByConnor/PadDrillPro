using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Punch Sound Effects")]
    public AudioClip jabCrossSound;     // Light punches (jab, cross)
    public AudioClip hookUppercutSound; // Heavy punches (hook, uppercut)
    public AudioClip blockSound;        // Block/defensive sound
    
    [Header("Game Event Sounds")]
    public AudioClip streakSound;       // When player gets a streak
    public AudioClip heartLossSound;    // When player loses health
    public AudioClip missSound;         // When player misses
    
    [Header("Audio Settings")]
    public float masterVolume = 1.0f;
    public float punchVolume = 0.8f;
    public float eventVolume = 0.7f;
    
    private AudioSource audioSource;
    
    // Singleton pattern for easy access
    public static AudioManager Instance;
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        Debug.Log("AudioManager ready!");
    }
    
    void Start()
    {
        // Set up default audio source settings
        audioSource.playOnAwake = false;
        audioSource.volume = masterVolume;
    }
    
    // Play punch sounds based on attack type
    public void PlayPunchSound(string attackType)
    {
        AudioClip clipToPlay = null;
        
        switch (attackType.ToLower())
        {
            case "jab":
            case "cross":
                clipToPlay = jabCrossSound;
                break;
                
            case "hook":
            case "uppercut":
                clipToPlay = hookUppercutSound;
                break;
                
            case "block":
                clipToPlay = blockSound;
                break;
        }
        
        if (clipToPlay != null)
        {
            PlaySound(clipToPlay, punchVolume);
            Debug.Log($"Playing {attackType} sound");
        }
    }
    
    // Play timing feedback sounds
    public void PlayTimingSound(string timingResult)
    {
        AudioClip clipToPlay = null;
        
        switch (timingResult.ToLower())
        {
            case "miss":
                clipToPlay = missSound;
                break;
        }
        
        if (clipToPlay != null)
        {
            PlaySound(clipToPlay, eventVolume);
            Debug.Log($"Playing {timingResult} sound");
        }
    }
    
    // Play streak achievement sound
    public void PlayStreakSound()
    {
        if (streakSound != null)
        {
            PlaySound(streakSound, eventVolume);
            Debug.Log("Playing streak sound");
        }
    }
    
    // Play heart loss sound
    public void PlayHeartLossSound()
    {
        if (heartLossSound != null)
        {
            PlaySound(heartLossSound, eventVolume);
            Debug.Log("Playing heart loss sound");
        }
    }
    
    // Generic method to play any sound with volume control
    void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume * masterVolume);
        }
    }
    
    // Volume controls for runtime adjustment
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        audioSource.volume = masterVolume;
    }
    
    public void SetPunchVolume(float volume)
    {
        punchVolume = Mathf.Clamp01(volume);
    }
    
    public void SetEventVolume(float volume)
    {
        eventVolume = Mathf.Clamp01(volume);
    }
}
