using UnityEngine;

[CreateAssetMenu(fileName = "MusicStats", menuName = "Scriptable Objects/MusicStats")]
public class MusicStats : ScriptableObject
{
    [SerializeField] private AudioClip music;
    [SerializeField] private NoteHitTimings hitTimings;

    public AudioClip Music => music;
    public NoteHitTimings HitTimings => hitTimings;
}
