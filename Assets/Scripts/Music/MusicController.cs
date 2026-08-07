using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("Assign Music Clip")]
    [SerializeField] private SongDatabase songDatabase;
    private AudioClip currentMusicClip;
    private AudioSource audioSource;
    private NoteHitTimings currentNoteHitTimings;
    private MusicStats currentMusicStats;

    public static MusicController Instance { get; private set; }
    public event Action OnSongFinished;

    private void Awake()
    {
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = currentMusicClip;
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (audioSource.clip == null) return;

        if (audioSource.isPlaying == false && audioSource.time > 0f)
        {
            OnSongFinished?.Invoke();
        }
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    public List<NoteTiming> GetNoteTimings()
    {
        if(currentNoteHitTimings != null)
        {
            return currentNoteHitTimings.GetTimings();
        }

        return new List<NoteTiming>();
    }

    public float GetCurrentMusicTime()
    {
        return audioSource.time;
    }

    public int GetNoteCount()
    {
        return currentNoteHitTimings.GetTotalNoteCount();
    }

    public void SetDifficulty(TimingDifficulty difficulty)
    {
        currentMusicStats = songDatabase.GetRandomSongByDifficulty(difficulty);

        if (currentMusicStats == null)
        {
            Debug.LogError($"DetermineDifficulty: no song found for tier {difficulty}, fishing encounter cannot proceed correctly.");
        }

        
    }

    public void OnStartReelWheel()
    {
        SetSong(currentMusicStats);

        PlayMusic();
    }

    public void SetSong(MusicStats songStats)
    {
        SetAudioClip();
        SetNoteTimings(); 
    }

    private void SetAudioClip()
    {
        if (currentMusicStats != null)
        {
            currentMusicClip = currentMusicStats.Music;
            audioSource.clip = currentMusicClip;
        }
    }

    private void SetNoteTimings()
    {
        if(currentMusicStats != null)
        {
            currentNoteHitTimings = currentMusicStats.HitTimings;
        }
    }
}