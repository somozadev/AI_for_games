namespace Genetics
{
    [System.Serializable]
    public class Creature
    {
        public Chromosome Chromosome;

        public Creature()
        {
            Chromosome = new Chromosome();
        }

        public void OnEatFood() //maybe each food gives different energy amounts
        {
            Chromosome.BasicStats.energy += 10;
            UpdateFitness();
        }

        public void OnDamageReceived()
        {
            Chromosome.BasicStats.hp -= 10;
            UpdateFitness();
        }
        
        
        private void UpdateFitness(){}
        
        public float EvaluateFitnessTotal(Enviroment env)
        {
            float fitness = 0;

            if (Chromosome.ClimateAffinity == env.Climate) fitness += 10;
            if (Chromosome.TerrainAffinity == env.Terrain) fitness += 10;

            // if (Chromosome.Diet == enums.DietType.Herbivorous)
            //     fitness += env.PlantAvailability * 10;
            // else if (Chromosome.Diet == enums.DietType.Carnivorous && env.PreyAvailability > 0)
            //     fitness += env.PreyAvailability * 10;
            //
            // // Otros factores (como supervivencia o velocidad)
            //
            // fitness += Chromosome.BasicStats.speed;

            return fitness;
        }

    }
}