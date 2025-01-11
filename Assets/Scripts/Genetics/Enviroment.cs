using Genetics.Environmental;
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class Enviroment
    {
        public enums.TerrainType Terrain; // { get; set; }
        public enums.ClimateType Climate; // { get; set; }
        public float Temperature; // { get; set; }
        private static readonly System.Random _random = new System.Random();

        public Enviroment()
        {
            Terrain = (enums.TerrainType)_random.Next(0, System.Enum.GetValues(typeof(enums.TerrainType)).Length);
            Climate = (enums.ClimateType)_random.Next(0, System.Enum.GetValues(typeof(enums.ClimateType)).Length);
            Temperature = 0f;
        }

        public void UpdateTemperature(float currentHour)
        {
            Temperature = CalculateTemperature(currentHour);
        }

        private float CalculateTemperature(float hour)
        {
            float baseTemperature = 0f;

            switch (Climate)
            {
                case enums.ClimateType.Hot:
                    baseTemperature = 30f;
                    break;
                case enums.ClimateType.Cold:
                    baseTemperature = 10f;
                    break;
                case enums.ClimateType.Humid:
                    baseTemperature = 25f;
                    break;
                case enums.ClimateType.Neutral:
                    baseTemperature = 20f;
                    break;
                case enums.ClimateType.Interior:
                    baseTemperature = 15f;
                    break;
            }

            if (hour >= 6f && hour < 18f) // day
                baseTemperature += Mathf.Lerp(0f, 10f, (hour - 6f) / 12f);
            else
                baseTemperature -= Mathf.Lerp(0f, 10f, (hour - 18f) / 12f);

            switch (Terrain)
            {
                case enums.TerrainType.Water:
                    baseTemperature -= 2f;
                    break;
                case enums.TerrainType.Air:
                    baseTemperature -= 5f;
                    break;
                case enums.TerrainType.Ground:
                    baseTemperature += 2f;
                    break;
            }

            return Mathf.Clamp(baseTemperature, -30f, 60f);
        }
    }
}