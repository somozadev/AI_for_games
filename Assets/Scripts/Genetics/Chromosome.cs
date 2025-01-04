using UnityEngine;
using System;
namespace Genetics
{
    //in genetics algo. terms, it's called chromosome. pretty much the collection of parameters we listed in class, called genes 
    [System.Serializable]
    public class Chromosome
    {
        public enums.DietType Diet; //{ get; set; }
        public enums.TerrainType TerrainAffinity; // { get; set; }
        public enums.ClimateType ClimateAffinity; // { get; set; }
        public enums.SocialType SocialType;
        public enums.BasicStats BasicStats; // { get; set; }
        public int LimbCount; // { get; set; }
        public int JointsCount; // { get; set; }
        public float SizeScale; // { get; set; }
        public Color32 Color; // { get; set; }

        [SerializeField] private string _data;
        private static readonly System.Random _random = new System.Random();
        
        public Chromosome()
        {
            Diet = (enums.DietType)_random.Next(0, 5);
            SocialType = (enums.SocialType)_random.Next(0, 2);
            TerrainAffinity = (enums.TerrainType)_random.Next(0, 3);
            ClimateAffinity = (enums.ClimateType)_random.Next(0, 5);

            switch (Diet)
            {
                case enums.DietType.Herbivorous:
                    BasicStats = new enums.BasicStats(
                        hp: _random.Next(10, 20),
                        dmg: _random.Next(1, 3),
                        speed: _random.Next(10, 15),
                        energy: _random.Next(20, 30),
                        perception: _random.Next(10, 20));
                    break;
                case enums.DietType.Carnivorous:
                    BasicStats = new enums.BasicStats(
                        hp: _random.Next(20, 30),
                        dmg: _random.Next(10, 15),
                        speed: _random.Next(15, 20),
                        energy: _random.Next(10, 15),
                        perception: _random.Next(5, 10));
                    break;
                case enums.DietType.Omnivorous:
                    BasicStats = new enums.BasicStats(
                        hp: _random.Next(15, 25),
                        dmg: _random.Next(1, 10),
                        speed: _random.Next(10, 20),
                        energy: _random.Next(10, 20),
                        perception: _random.Next(5, 20));
                    break;
                case enums.DietType.Crystavorous:
                    BasicStats = new enums.BasicStats(
                        hp: _random.Next(25, 35),
                        dmg: _random.Next(0, 2),
                        speed: _random.Next(5, 8),
                        energy: _random.Next(15, 30),
                        perception: _random.Next(5, 5));
                    break;
                case enums.DietType.Frugivorous:
                    BasicStats = new enums.BasicStats(
                        hp: _random.Next(5, 12),
                        dmg: _random.Next(2, 5),
                        speed: _random.Next(18, 25),
                        energy: _random.Next(10, 25),
                        perception: _random.Next(12, 20));
                    break;
                case enums.DietType.Lumivorous:
                    BasicStats = new enums.BasicStats(
                        hp: _random.Next(25, 30),
                        dmg: _random.Next(10, 20),
                        speed: _random.Next(1, 2),
                        energy: _random.Next(35, 50),
                        perception: _random.Next(1, 2));
                    break;
                default:
                    BasicStats = new enums.BasicStats(
                        hp: _random.Next(10, 15),
                        dmg: _random.Next(3, 7),
                        speed: _random.Next(10, 15),
                        energy: _random.Next(10, 20),
                        perception: _random.Next(5, 15));
                    break;
            }

            LimbCount = _random.Next(0, 2);
            JointsCount = _random.Next(1, 4);
            SizeScale = (float)_random.NextDouble() * (2.0f - 0.25f) + 0.250f;
            Color = new Color32((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256),
                255);
            GenerateDnaData();
        }

        public string GetDnaData()
        {
            if (string.IsNullOrEmpty(_data))
            {
                GenerateDnaData();
            }

            return _data;
        }

        protected void GenerateDnaData()
        {
            var diet = (int)Diet;
            var terrainAffinity = (int)TerrainAffinity;
            var climateAffinity = (int)ClimateAffinity;
            var socialType = (int)SocialType;

            var basicStats = (BasicStats.initial_hp << 0) |
                             (BasicStats.initial_dmg << 8) |
                             (BasicStats.initial_speed << 16) |
                             (BasicStats.initial_energy << 24);

            var basicStatsCurrent = (BasicStats.hp << 0) |
                                    (BasicStats.dmg << 8) |
                                    (BasicStats.speed << 16) |
                                    (BasicStats.energy << 24);

            var limbCount = LimbCount;
            var jointsCount = JointsCount;
            var sizeScale = Mathf.FloorToInt(SizeScale * 100f); // Scale the size to an integer value
            var color = (Color.r << 24) | (Color.g << 16) | (Color.b << 8) | Color.a; // Combine RGBA into an int

            _data =
                $"{diet}|{terrainAffinity}|{climateAffinity}|{socialType}|{basicStats}|{basicStatsCurrent}|{limbCount}|{jointsCount}|{sizeScale}|{color}";
        }
    }
}