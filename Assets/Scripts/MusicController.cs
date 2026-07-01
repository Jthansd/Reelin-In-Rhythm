using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("Assign Music Clip")]
    [SerializeField] private AudioClip musicClip;

    private AudioSource audioSource;

    [SerializeField] private List<NoteHitTimings> NoteHitTimings = new();

    public static MusicController Instance { get; private set; }

    public event Action OnSongFinished;



    private void Awake()
    {
        Instance = this;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
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

    public List<float> GetNoteTimingsForDifficulty(string difficulty)
    {
        return NoteHitTimings[0].GetTimingsForDifficulty(difficulty);
    }

    public float GetCurrentMusicTime()
    {
        return audioSource.time;
    }

   

}
