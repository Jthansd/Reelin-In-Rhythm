using System.Collections.Generic;
using UnityEngine;

public enum TimingDifficulty
{
    Easy,
    Medium,
    Hard
}
[CreateAssetMenu(fileName = "NoteHitTimings", menuName = "Scriptable Objects/NoteHitTimings")]
public class NoteHitTimings : ScriptableObject
{
    public List<NoteTiming> noteTimings;
    [SerializeField] private TimingDifficulty timingDifficulty;

    public string noteList;

    //easy: 1.84,2.30,2.76,5.53,6.00,6.46,9.23,9.69,10.15,12.92,13.38,13.84,16.61,17.07,17.52,20.30,20.76,21.22,24.00,24.46,24.92,27.69,28.15,28.61 
    //medium: 3.69,4.15,4.61,4.84,5.07,5.19,7.38,7.84,8.30,8.53,8.76,8.88,11.07,11.53,11.99,12.22,12.45,12.57,14.76,15.22,15.68,15.91,16.14,16.26,18.45,18.91,19.37,19.60,19.83,19.95,22.14,22.60,23.06,23.29,23.52,23.64,25.83,26.29,26.75,26.98,27.21,27.33,29.52,29.98,30.44,30.67,30.90,31.02

    [SerializeField] public AudioClip Song;

    public TimingDifficulty Difficulty => timingDifficulty;

    public void ParseAllLists()
    {
        noteTimings = ParseTimingList(noteList);
        
    }

    private List<NoteTiming> ParseTimingList(string list)
    {
        List<NoteTiming> result = new();
        if (string.IsNullOrWhiteSpace(list))
        {
            return result;
        }

        string[] entries = list.Split(',');
        foreach (string entry in entries)
        {
            NoteTiming? timing = ParseEntry(entry.Trim());
            if (timing.HasValue)
            {
                result.Add(timing.Value);
            }
        }
        return result;
    }

    private NoteTiming? ParseEntry(string entry)
    {
        if (string.IsNullOrWhiteSpace(entry))
            return null;

        string[] fields = entry.Split(':');

        // Field 0: hitTime (required)
        if (!float.TryParse(fields[0].Trim(), out float hitTime))
        {
            Debug.LogWarning($"NoteHitTimings: could not parse hitTime from entry '{entry}', skipping.");
            return null;
        }

        // Field 1: type (optional, defaults to Tap)
        NoteType noteType = NoteType.Tap;
        if (fields.Length >= 2 && !string.IsNullOrWhiteSpace(fields[1]))
        {
            if (!TryParseNoteType(fields[1].Trim(), out noteType))
            {
                Debug.LogWarning($"NoteHitTimings: unrecognized note type '{fields[1]}' in entry '{entry}', defaulting to Tap.");
                noteType = NoteType.Tap;
            }
        }

        // Field 2: extra (currently only meaningful for Hold -> releaseTime)
        float releaseTime = 0f;
        if (noteType == NoteType.Hold)
        {
            if (fields.Length >= 3 && float.TryParse(fields[2].Trim(), out float parsedRelease))
            {
                releaseTime = parsedRelease;
            }
            else
            {
                Debug.LogWarning($"NoteHitTimings: hold note '{entry}' missing a valid releaseTime, defaulting to hitTime (zero-length hold).");
                releaseTime = hitTime;
            }
        }

        return new NoteTiming
        {
            hitTime = hitTime,
            noteType = noteType,
            releaseTime = releaseTime
        };
    }

    private bool TryParseNoteType(string raw, out NoteType noteType)
    {
        switch (raw.ToLower())
        {
            case "tap":
                noteType = NoteType.Tap;
                return true;
            case "hold":
                noteType = NoteType.Hold;
                return true;
            case "altkey":
                noteType = NoteType.Altkey;
                return true;
            case "skip":
                noteType = NoteType.Skip;
                return true;
            default:
                noteType = NoteType.Tap;
                return false;
        }
    }

    public List<NoteTiming> GetTimings()
    {
        return noteTimings;
    }

    public int GetTotalNoteCount()
    {
        return noteTimings.Count;
    }
}