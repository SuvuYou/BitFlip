using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEventsSO", menuName = "ScriptableObjects/SoundEventsSO")]
public class SoundEventsSO : ScriptableObject
{
    public enum GameSound
    {

    }

    public UglySerializableDictionary<GameSound, GameSoundInfo> SoundLookup;
}

[System.Serializable]
public class GameSoundInfo
{
    public List<AudioClip> SoundClips;

    public int InstanceLimit = 2;

    public bool ShouldStopOldestSourceOnLimitReached = false;
}