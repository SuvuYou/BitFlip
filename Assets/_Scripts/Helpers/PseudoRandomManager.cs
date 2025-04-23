using System;
using System.Collections.Generic;

namespace PseudoRandom
{
    public enum SystemRandomType
    {
        PathGeneration,
        DungeonGeneration,
        Other
    }

    public static class SystemRandomHolder
    {
        private static Dictionary<SystemRandomType, SystemRandomManager> _systemRandomLookup = new();

        public static int InitSystems(int seed = -1)
        {
            if (seed == -1)
            {
               seed = Guid.NewGuid().GetHashCode();
            }
            
            foreach (SystemRandomType system in Enum.GetValues(typeof(SystemRandomType)))
            {
                _systemRandomLookup[system] = new SystemRandomManager(seed);
            }

            return seed;
        }

        public static SystemRandomManager UseSystem(SystemRandomType system) => _systemRandomLookup[system];
    }

    public class SystemRandomManager
    {
        private System.Random _systemRandom;

        private bool _useSystemRandom = true;

        public SystemRandomManager(int seed, bool useSystem = true)
        {
            _useSystemRandom = useSystem;
            _systemRandom = new System.Random(seed);
            UnityEngine.Random.InitState(seed);
        }

        public int GetRandomInt(int min, int max)
        {
            return _useSystemRandom ? _systemRandom.Next(min, max) : UnityEngine.Random.Range(min, max);
        }

        public float GetRandomFloat(float min = 0, float max = 1)
        {
            return _useSystemRandom ? (float)(_systemRandom.NextDouble() * (max - min) + min) : UnityEngine.Random.Range(min, max);
        }

        public bool GetRandomBool()
        {
            return _useSystemRandom ? _systemRandom.Next(0, 2) == 0 : UnityEngine.Random.value > 0.5f;
        }
    }
}

