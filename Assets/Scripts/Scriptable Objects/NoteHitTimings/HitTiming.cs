using System;

[Serializable]
public struct NoteTiming
{
    public float hitTime;
    public NoteType noteType;
    public float releaseTime; // only meaningful when noteType == Hold
}