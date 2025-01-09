using System;
using ProceduralCreature;
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class Creature
    {
        public Chromosome Chromosome;
        [ReadOnly] public float fitness;

        [ReadOnly] public enums.TerrainType currentTerrain;
        [ReadOnly] public enums.ClimateType currentClimate;

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
            if (Chromosome.BasicStats.energy <= 0)
                Chromosome.BasicStats.energy = 0;
            UpdateFitness();
        }

        public void OnEatFood()
        {
            if (Chromosome.BasicStats.EnergyLevels < 100)
                Chromosome.BasicStats.energy += 10;
            if (Chromosome.BasicStats.EnergyLevels > 100)
                Chromosome.BasicStats.energy = Chromosome.BasicStats.initial_energy;
            UpdateFitness();
        }

        public void OnRestPerformed()
        {
            if (Chromosome.BasicStats.EnergyLevels < 100)
                Chromosome.BasicStats.energy += 3;
            if (Chromosome.BasicStats.HpLevels < 100)
                Chromosome.BasicStats.hp += 5;

            if (Chromosome.BasicStats.EnergyLevels > 100)
                Chromosome.BasicStats.energy = Chromosome.BasicStats.initial_energy;
            if (Chromosome.BasicStats.HpLevels > 100)
                Chromosome.BasicStats.hp = Chromosome.BasicStats.initial_hp;
            UpdateFitness();
        }

        public void OnDamageReceived(Creature attacker)
        {
            var baseDamage = attacker.Chromosome.BasicStats.dmg;
            var jointFactor = 1f / Chromosome.JointsCount;
            var sizeFactor = 1f / Chromosome.SizeScale;
            Chromosome.BasicStats.hp -= Mathf.Max(1, Mathf.RoundToInt(baseDamage * jointFactor * sizeFactor));
            if (Chromosome.BasicStats.hp <= 0)
                Chromosome.BasicStats.hp = 0;
            UpdateFitness();
        }

        public void OnMovePerformed()
        {
            Chromosome.BasicStats.energy -= Mathf.CeilToInt(2 * Chromosome.SizeScale);
            if (Chromosome.BasicStats.energy <= 0)
                Chromosome.BasicStats.energy = 0;
            UpdateFitness();
        }

        private void UpdateFitness()
        {
            fitness = 0;
            switch (Chromosome.Diet)
            {
                case enums.DietType.Herbivorous:
                    fitness += Chromosome.BasicStats.hp * .25f;
                    fitness += Chromosome.BasicStats.energy * .3f;
                    fitness += Chromosome.BasicStats.speed * .2f;
                    fitness += Chromosome.BasicStats.perception * .15f;
                    fitness += Chromosome.BasicStats.dmg * .05f;
                    break;
                case enums.DietType.Carnivorous:
                    fitness += Chromosome.BasicStats.hp * .15f;
                    fitness += Chromosome.BasicStats.energy * .2f;
                    fitness += Chromosome.BasicStats.speed * .35f;
                    fitness += Chromosome.BasicStats.perception * .2f;
                    fitness += Chromosome.BasicStats.dmg * .20f;
                    break;
                case enums.DietType.Omnivorous:
                    fitness += Chromosome.BasicStats.hp * .25f;
                    fitness += Chromosome.BasicStats.energy * .3f;
                    fitness += Chromosome.BasicStats.speed * .25f;
                    fitness += Chromosome.BasicStats.perception * .15f;
                    fitness += Chromosome.BasicStats.dmg * .1f;
                    break;
                case enums.DietType.Crystavorous:
                    fitness += Chromosome.BasicStats.hp * .4f;
                    fitness += Chromosome.BasicStats.energy * .3f;
                    fitness += Chromosome.BasicStats.speed * .05f;
                    fitness += Chromosome.BasicStats.perception * .25f;
                    fitness += Chromosome.BasicStats.dmg * .02f;
                    break;
                case enums.DietType.Frugivorous:
                    fitness += Chromosome.BasicStats.hp * .2f;
                    fitness += Chromosome.BasicStats.energy * .3f;
                    fitness += Chromosome.BasicStats.speed * .25f;
                    fitness += Chromosome.BasicStats.perception * .15f;
                    fitness += Chromosome.BasicStats.dmg * .05f;

                    break;
                case enums.DietType.Lumivorous:
                    fitness += Chromosome.BasicStats.hp * .2f;
                    fitness += Chromosome.BasicStats.energy * .3f;
                    fitness += Chromosome.BasicStats.speed * .05f;
                    fitness += Chromosome.BasicStats.perception * .25f;
                    fitness += Chromosome.BasicStats.dmg * .02f;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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