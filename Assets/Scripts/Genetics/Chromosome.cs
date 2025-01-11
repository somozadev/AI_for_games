using UnityEngine;
using System;

namespace Genetics
{
    //in genetics algo. terms, it's called chromosome. pretty much the collection of parameters we listed in class, called genes 
    [Serializable]
    public class Chromosome
    {
        public enums.DietType Diet;
        public enums.TerrainType TerrainAffinity;
        public enums.ClimateType ClimateAffinity;

        public enums.BasicStats BasicStats;
        public int LimbCount;
        public int JointsCount;
        public float SizeScale;
        public Color32 Color;
        [SerializeField] private string _data;
        private static readonly System.Random _random = new System.Random();

        public Chromosome()
        {

            Diet = (enums.DietType)_random.Next(0, 5);
            TerrainAffinity = (enums.TerrainType)_random.Next(0, 3);
            ClimateAffinity = (enums.ClimateType)_random.Next(0, 5);

            switch (Diet)
            {
                case enums.DietType.Herbivorous:
                    BasicStats = new enums.BasicStats(
                        hp: _random.Next(20, 30),
                        dmg: _random.Next(1, 3),
                        speed: _random.Next(10, 15),
                        energy: _random.Next(20, 30),
                        perception: _random.Next(10, 20));
                    break;
                case enums.DietType.Carnivorous:
                    BasicStats = new enums.BasicStats(
                        hp: _random.Next(10, 20),
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
                        dmg: _random.Next(1, 2),
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
                        hp: _random.Next(25, 40),
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
            Color = new Color32((byte)_random.Next(1, 256), (byte)_random.Next(1, 256), (byte)_random.Next(1, 256),
                255);
            EncodeDna();
        }

        public Chromosome( string dna)
        {
            SetDna(dna);
        }

   

        public string GetDna()
        {
            if (string.IsNullOrEmpty(_data))
            {
                EncodeDna();
            }

            return _data;
        }

        public void ResetDna()
        {
            _data = null;
        }

        public string SetDna(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                DecodeDna(data);
            }
            else
                EncodeDna();

            return _data;
        }


        protected void EncodeDna()
        {
            var diet = (int)Diet;
            var terrainAffinity = (int)TerrainAffinity;
            var climateAffinity = (int)ClimateAffinity;
            // var socialType = (int)SocialType;

            var basicStats =
                $"{BasicStats.initial_hp},{BasicStats.initial_dmg},{BasicStats.initial_speed},{BasicStats.initial_energy},{BasicStats.initial_perception}";
            var basicStatsCurrent =
                $"{BasicStats.hp},{BasicStats.dmg},{BasicStats.speed},{BasicStats.energy},{BasicStats.perception}";

            var limbCount = LimbCount.ToString();
            var jointsCount = JointsCount.ToString();
            var sizeScale = SizeScale.ToString("F2");
            var color = $"{Color.r},{Color.g},{Color.b},{Color.a}";

            _data =
                $"{diet}|{terrainAffinity}|{climateAffinity}|{basicStats}|{basicStatsCurrent}|{limbCount}|{jointsCount}|{sizeScale}|{color}";
        }

        protected void DecodeDna(string dnaData)
        {
            try
            {
                var data = dnaData.Split('|');

                if (data.Length != 9)
                {
                    throw new ArgumentException("Invalid DNA data format.");
                }

                Diet = (enums.DietType)Enum.Parse(typeof(enums.DietType), data[0]);
                TerrainAffinity = (enums.TerrainType)Enum.Parse(typeof(enums.TerrainType), data[1]);
                ClimateAffinity = (enums.ClimateType)Enum.Parse(typeof(enums.ClimateType), data[2]);

                var basicStatsValues = data[3].Split(',');
                BasicStats = new enums.BasicStats(
                    hp: int.Parse(basicStatsValues[0]),
                    dmg: int.Parse(basicStatsValues[1]),
                    speed: int.Parse(basicStatsValues[2]),
                    energy: int.Parse(basicStatsValues[3]),
                    perception: float.Parse(basicStatsValues[4])
                );
                // var basicStatsCurrentValues = data[4].Split(',');
                // BasicStats.hp = int.Parse(basicStatsCurrentValues[0]);
                // BasicStats.dmg = int.Parse(basicStatsCurrentValues[1]);
                // BasicStats.speed = int.Parse(basicStatsCurrentValues[2]);
                // BasicStats.energy = int.Parse(basicStatsCurrentValues[3]);
                // BasicStats.perception = float.Parse(basicStatsCurrentValues[4]);


                LimbCount = int.Parse(data[5]);
                JointsCount = int.Parse(data[6]);
                SizeScale = float.Parse(data[7]);
                var colorValues = data[8].Split(',');
                byte r = ClampByte(colorValues[0]);
                byte g = ClampByte(colorValues[1]);
                byte b = ClampByte(colorValues[2]);
                byte a = ClampByte(colorValues[3]);
                Color = new Color32(r, g, b, a);

                EncodeDna();
            }
            catch (Exception e)
            {
                // Debug.Log(e);
                // throw;
            }
        }

        byte ClampByte(string value)
        {
            byte result;
            if (!byte.TryParse(value, out result))
            {
                return 0;
            }

            return (byte)Mathf.Clamp(result, 0, 255);
        }
    }
}