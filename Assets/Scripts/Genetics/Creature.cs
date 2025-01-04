using ProceduralCreature;
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class Creature
    {
        public Chromosome Chromosome;
        private float fitness;

        private enums.TerrainType currentTerrain;
        private enums.ClimateType currentClimate;

        public float Fitness
        {
            get
            {
                UpdateFitness();
                return fitness;
            }
        }

        public Creature()
        {
            Chromosome = new Chromosome();
        }

        public Creature(string dna)
        {
            Chromosome = new Chromosome(dna);
        }


        #region Fitness Related

        public void OnHourPassed()
        {
            if (Chromosome.BasicStats.energy >= 5)
                Chromosome.BasicStats.energy -= 5;
        }

        public void OnEatFood() //maybe each food gives different energy amounts
        {
            if (Chromosome.BasicStats.EnergyLevels < 100)
                Chromosome.BasicStats.energy += 10;
            UpdateFitness();
        }

        public void OnRestPerformed()
        {
            if (Chromosome.BasicStats.EnergyLevels < 100)
                Chromosome.BasicStats.energy += 1;
            if (Chromosome.BasicStats.HpLevels < 100)
                Chromosome.BasicStats.hp += 5;
            UpdateFitness();
        }

        public void OnDamageReceived(Creature attacker)
        {
            var baseDamage = attacker.Chromosome.BasicStats.dmg;
            var jointFactor = 1f / Chromosome.JointsCount;
            var sizeFactor = 1f / Chromosome.SizeScale;
            Chromosome.BasicStats.hp -= Mathf.Max(1, Mathf.RoundToInt(baseDamage * jointFactor * sizeFactor));
            UpdateFitness();
        }

        public void OnMovePerformed()
        {
            Chromosome.BasicStats.energy -= Mathf.CeilToInt(2 * Chromosome.SizeScale);
            UpdateFitness();
        }

        private void UpdateFitness()
        {
            fitness = 0;
            //unique per creature(?) if predator speed is > important to sprint chase, or prey energy and perception to escape 
            fitness += Chromosome.BasicStats.hp * .25f;
            fitness += Chromosome.BasicStats.energy * .3f;
            fitness += Chromosome.BasicStats.speed * .25f;
            fitness += Chromosome.BasicStats.perception * .15f;

            if (IsInSuitableEnvironment()) //if in correct terrain area, fitness increased 
                fitness += 10;
            if (IsInSuitableClimate()) //if in correct climatic area, fitness increased
                fitness += 10;
            if (Chromosome.BasicStats.HpLevels < 5) //if close to die, fitness reduced 
                fitness -= 20;
            if (Chromosome.BasicStats.EnergyLevels > 90) //if close to full energy, assumed creature just eated
                fitness += 15;
            if (Chromosome.BasicStats.EnergyLevels ==
                100) //if full energy, assumed creature decided to not move, penalzied 
                fitness -= 5;
        }

        public void UpdateEnviroment(Enviroment enviroment)
        {
            currentClimate = enviroment.Climate;
            currentTerrain = enviroment.Terrain;
        }

        public bool IsInSuitableEnvironment()
        {
            return (currentTerrain == Chromosome.TerrainAffinity);
        }

        public bool IsInSuitableClimate()
        {
            return (currentClimate == Chromosome.ClimateAffinity);
        }

        #endregion
    }
}