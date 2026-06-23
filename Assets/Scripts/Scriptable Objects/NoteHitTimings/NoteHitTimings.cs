using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoteHitTimings", menuName = "Scriptable Objects/NoteHitTimings")]
public class NoteHitTimings : ScriptableObject
{
    public List<float> Easy;
    public List<float> Medium;
    public List<float> Hard;

    public string EasyList;
    public string MediumList;
    public string HardList;

    [SerializeField] public AudioClip Song;

    public void ParseAllLists()
    {
        Easy = ParseList(EasyList);
        Medium = ParseList(MediumList);
        Hard = ParseList(HardList);
    }

    private List<float> ParseList(string list)
    {
        List<float> result = new();

        if (string.IsNullOrWhiteSpace(list)) {
            return result;
        }

        string[] parts = list.Split(',');

        foreach (string part in parts)
        {
            if (float.TryParse(part.Trim(), out float value))
            {
                result.Add(value);
            }
           
        }
        return result;
    }

    public List<List<float>> GetAllTimings()
    {
        return new List<List<float>> { Easy, Medium, Hard };
    }

    public List<float> GetTimingsForDifficulty(string difficulty)
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