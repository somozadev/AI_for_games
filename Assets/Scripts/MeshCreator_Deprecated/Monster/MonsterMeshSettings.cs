using UnityEngine;
using System;
namespace MonsterCreator
{
    [Serializable]
    public class MonsterMeshSettings
    {
        [SerializeField] private float radius = 0.05f;
        [SerializeField] private float length = 0.1f;
        [SerializeField] [Range(4, 25)] private int segments = 14;
        [SerializeField] [Range(2, 25)] private int rings = 4;

        public float Radius => radius;
        public float Length => length;
        public int Segments => segments;
        public int Rings => rings;
    }
}