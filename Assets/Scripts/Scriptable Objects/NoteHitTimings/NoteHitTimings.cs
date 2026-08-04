using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoteHitTimings", menuName = "Scriptable Objects/NoteHitTimings")]
public class NoteHitTimings : ScriptableObject
{
    public List<NoteTiming> Easy;
    public List<NoteTiming> Medium;
    public List<NoteTiming> Hard;

    public string EasyList;
    public string MediumList;
    public string HardList;

    [SerializeField] public AudioClip Song;

    public void ParseAllLists()
    {
        Easy = ParseTimingList(EasyList);
        Medium = ParseTimingList(MediumList);
        Hard = ParseTimingList(HardList);
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
            default:
                noteType = NoteType.Tap;
                return false;
        }
    }

    public List<List<NoteTiming>> GetAllTimings()
    {
        return new List<List<NoteTiming>> { Easy, Medium, Hard };
    }

    public List<NoteTiming> GetTimingsForDifficulty(string difficulty)
    {
        return difficulty.ToLower() switch
        {
            "easy" => Easy,
            "medium" => Medium,
            "hard" => Hard,
            _ => throw new System.ArgumentException("Invalid difficulty level")
        };
    }
}