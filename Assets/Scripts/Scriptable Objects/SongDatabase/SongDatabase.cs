using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongDatabase", menuName = "Scriptable Objects/SongDatabase")]
public class SongDatabase : ScriptableObject
{
    [SerializeField] private List<MusicStats> songs = new();

    public MusicStats GetRandomSong()
    {
        if (songs == null || songs.Count == 0)
        {
            Debug.LogError("SongDatabase: no songs assigned.");
            return null;
        }
        return songs[Random.Range(0, songs.Count)];
    }

    public MusicStats GetSongByIndex(int index)
    {
        if (songs == null || index < 0 || index >= songs.Count)
        {
            Debug.LogError($"SongDatabase: invalid index {index}.");
            return null;
        }
        return songs[index];
    }

    public MusicStats GetRandomSongByDifficulty(TimingDifficulty difficulty)
    {
        List<MusicStats> songList = new List<MusicStats>();
        if(songs == null || songs.Count == 0)
        {
            Debug.LogError("SongDatabse: no songs assigned.");
            return null;
        }
        for(int i = 0; i < songs.Count; i++)
        {
            if (songs[i].HitTimings.Difficulty == difficulty)
            {
                songList.Add(songs[i]);
            }
        }
        if (songList.Count == 0)
        {
            Debug.LogError($"SongDatabase: no songs found with difficulty {difficulty}.");
            return null;
        }
        return songList[Random.Range(0, songList.Count)];
    }

    public int Count => songs?.Count ?? 0;
}