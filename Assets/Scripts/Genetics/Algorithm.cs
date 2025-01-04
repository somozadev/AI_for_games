using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Genetics
{
    //https://medium.com/@byanalytixlabs/a-complete-guide-to-genetic-algorithm-advantages-limitations-more-738e87427dbb good source of info related to this algorithm . i'll use it in the report
    //https://nature-of-code-2nd-edition.netlify.app/genetic-algorithms/#how-genetic-algorithms-work this explains quite good how the algorithm works in it's basics
    public class Algorithm : MonoBehaviour
    {
        [Header("Techniques")] [SerializeField]
        private enums.SelectionType _selectionType;

        [SerializeField] private enums.CrossoverType _crossoverType;
        [SerializeField] private enums.MutationType _mutationType;

        [FormerlySerializedAs("_initialPopulation")] [Space(10)]
        public Population _currentPopulation;

        [SerializeField] private Population populationPrefab;
        private int _generationId = 1;
        [SerializeField] private int _populationSize = 50;

        [Space(10)] [ReadOnly] [SerializeField]
        private float _elapsedTime;

        [SerializeField] private float _timeScale = 1f;
        [SerializeField] private float _maxTime = 600f;


        private void Start()
        {
            GenerateInitialPopulation();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _maxTime)
                NewGeneration();
        }

        private void NewGeneration()
        {
            //delete old generation? 
            List<Creature> newPopulationCreatures = new List<Creature>();

            _generationId++;
            _elapsedTime = 0;
            newPopulationCreatures = Selection();
            newPopulationCreatures = Crossover(newPopulationCreatures);
            newPopulationCreatures = Mutation(newPopulationCreatures);

            _currentPopulation = Instantiate(populationPrefab, Vector3.zero, quaternion.identity, transform);
            _currentPopulation.Init(_populationSize, _generationId, newPopulationCreatures);
            //create the new generation 
        }


        private void OnValidate()
        {
            Time.timeScale = _timeScale;
        }

        private void GenerateInitialPopulation()
        {
            _currentPopulation = Instantiate(populationPrefab, Vector3.zero, quaternion.identity, transform);
            _currentPopulation.Init(_populationSize, _generationId);
            _generationId++;
        }

        //select parents for the breeding , here for description but in our instance prob. each creature has its own conditions for breeding given
        /*there are tons of selection methods, like roulette wheel, stochastic universal sampling, tournament, elitism, random, truncation,
         steady state, linear and non linear ranking, age-base, genitor selection (fitness based)*/

        private List<Creature> Selection()
        {
            List<Creature> selectedCreatures = new List<Creature>();
            switch (_selectionType)
            {
                case enums.SelectionType.RouletteWheel:
                    selectedCreatures = RouletteWheelSelection();
                    break;
                case enums.SelectionType.Tournament:
                    selectedCreatures = TournamentSelection();
                    break;
                case enums.SelectionType.Elitism:
                    selectedCreatures = ElitismSelection();
                    break;
                case enums.SelectionType.Random:
                    selectedCreatures = RandomSelection();
                    break;
            }

            return selectedCreatures;
        }

        //through crossover of parents chromosome
        /*To perform the selection of genes from the parents, one point crossover, two point , uniform, livery, inheritable algorithms,
       k-point, multipoint, half-uniform, shuffle, uniform, matrix, 3+ parents, linear, single arithmetic, partially mapped, cycled... */
        private List<Creature> Crossover(List<Creature> selectedCreatures)
        {
            List<Creature> crossedCreatures = new List<Creature>();
            switch (_crossoverType)
            {
                case enums.CrossoverType.OnePoint:
                    crossedCreatures = OnePointCrossover(selectedCreatures);
                    break;
                case enums.CrossoverType.TwoPoint:
                    crossedCreatures = TwoPointCrossover(selectedCreatures);
                    break;
                case enums.CrossoverType.Uniform:
                    crossedCreatures = UniformCrossover(selectedCreatures);
                    break;
            }

            return crossedCreatures;
        }

        //performed after breeding occurs 
        /*Flip bit mutation, gaussian, exchange / swap, */
        //recalc fitness and repeat from selection untill population converges       
        private List<Creature> Mutation(List<Creature> crossedCreatures)
        {
            List<Creature> mutatedCreatures = new List<Creature>();
            switch (_mutationType)
            {
                case enums.MutationType.FlipBit:
                    mutatedCreatures = FlipBitMutation(crossedCreatures);
                    break;
                case enums.MutationType.Gaussian:
                    mutatedCreatures = GaussianMutation(crossedCreatures);
                    break;
                case enums.MutationType.Uniform:
                    mutatedCreatures = UniformMutation(crossedCreatures);
                    break;
            }

            return mutatedCreatures;
        }

        #region Selection Methods

        /* Fitness value proporcional to the chances of select an individual. The greater the fitness, the higher the chance to be picked, but every individual got a chance of being picked */
        private List<Creature> RouletteWheelSelection()
        {
            var totalFitness = _currentPopulation.GetPopulation().Sum(creature => creature.Fitness);
            List<Creature> selectedParents = new List<Creature>();
            for (int i = 0; i < _currentPopulation.GetPopulation().Count; i++)
            {
                var randomVal = Random.Range(0f, totalFitness);
                var cumulativeFitness = 0f;
                foreach (var creature in _currentPopulation.GetPopulation())
                {
                    cumulativeFitness += creature.Fitness;
                    if (!(cumulativeFitness >= randomVal)) continue;
                    selectedParents.Add(creature);
                    break;
                }
            }

            return selectedParents;
        }

        /* A random group of individuals is selected, and the best one among them is chosen (the one with the greatest fitness value) */
        private List<Creature> TournamentSelection()
        {
            List<Creature> selectedParents = new List<Creature>();
            for (int i = 0; i < _currentPopulation.GetPopulation().Count; i++)
            {
                var tournament = new List<Creature>();
                int tournamentSize = 5;
                for (int j = 0; j < tournamentSize; j++)
                {
                    Creature randomCreature =
                        _currentPopulation.GetPopulation()[Random.Range(0, _currentPopulation.GetPopulation().Count)];
                    tournament.Add(randomCreature);
                }

                Creature bestCreature = tournament.OrderByDescending(c => c.Fitness).First();
                selectedParents.Add(bestCreature);
            }

            return selectedParents;
        }

        /* Always selects the best individuals, the elite 10% in this case and form there picks randomly to fill-in the new population size */
        private List<Creature> ElitismSelection()
        {
            List<Creature> sortedCreatures =
                _currentPopulation.GetPopulation().OrderByDescending(c => c.Fitness).ToList();
            var elitismCount = Mathf.FloorToInt(_currentPopulation.GetPopulation().Count * 0.1f);

            List<Creature> eliteGroup = sortedCreatures.Take(elitismCount).ToList();
            List<Creature> matingGroup = sortedCreatures.Skip(elitismCount)
                .Take(_currentPopulation.GetPopulation().Count - elitismCount).ToList();
            List<Creature> selectedParents = new List<Creature>();

            selectedParents.AddRange(eliteGroup);
            while (selectedParents.Count < _populationSize)
                selectedParents.Add(matingGroup[Random.Range(0, matingGroup.Count)]);

            return selectedParents;
        }

        /* Simple select individuals randomly without considering their fitness */
        private List<Creature> RandomSelection()
        {
            List<Creature> selectedParents = new List<Creature>();
            for (int i = 0; i < _populationSize; i++)
            {
                Creature randomCreature =
                    _currentPopulation.GetPopulation()[Random.Range(0, _currentPopulation.GetPopulation().Count)];
                selectedParents.Add(randomCreature);
            }

            return selectedParents;
        }

        #endregion

        #region Crossover Methods

        /* Perform one-point crossover on a pair of parents to produce two offspring.  Each parent chromosome is split at a randomly selected point, and genetic material is
         swapped between the parents at that point to create two new offspring. The offspring inherit genes from both parents at random positions within their chromosomes. */
        private List<Creature> OnePointCrossover(List<Creature> selectedCreatures)
        {
            List<Creature> offspring = new List<Creature>();

            for (int i = 0; i < selectedCreatures.Count; i += 2)
            {
                Creature parent1 = selectedCreatures[i];
                Creature parent2 = selectedCreatures[i + 1];

                string parent1Dna = parent1.Chromosome.GetDna();
                string parent2Dna = parent2.Chromosome.GetDna();

                var crossoverPoint = Random.Range(1, parent1Dna.Length);

                Creature offspring1 = new Creature();
                Creature offspring2 = new Creature();


                string offspring1Dna = parent1Dna.Substring(0, crossoverPoint) + parent2Dna.Substring(crossoverPoint);
                string offspring2Dna = parent2Dna.Substring(0, crossoverPoint) + parent1Dna.Substring(crossoverPoint);

                offspring1.Chromosome = new Chromosome(offspring1Dna);
                offspring2.Chromosome = new Chromosome(offspring2Dna);

                offspring.Add(offspring1);
                offspring.Add(offspring2);
            }

            return offspring;
        }

        /* Perform two-point crossover on a pair of parents to produce two offspring. Two points are randomly selected on the parents chromosomes, and the genetic material 
           between these two points is swapped between the parents. This results in two offspring, each inheriting parts of both parents chromosomes from the selected regions. */
        private List<Creature> TwoPointCrossover(List<Creature> selectedCreatures)
        {
            List<Creature> offspring = new List<Creature>();

            for (int i = 0; i < selectedCreatures.Count; i += 2)
            {
                Creature parent1 = selectedCreatures[i];
                Creature parent2 = selectedCreatures[i + 1];

                string parent1Dna = parent1.Chromosome.GetDna();
                string parent2Dna = parent2.Chromosome.GetDna();

                var crossoverPoint1 = Random.Range(1, parent1Dna.Length - 1);
                var crossoverPoint2 = Random.Range(crossoverPoint1, parent1Dna.Length);

                Creature offspring1 = new Creature();
                Creature offspring2 = new Creature();


                string offspring1Dna = parent1Dna.Substring(0, crossoverPoint1) +
                                       parent2Dna.Substring(crossoverPoint1, crossoverPoint2 - crossoverPoint1) +
                                       parent1Dna.Substring(crossoverPoint2);
                string offspring2Dna = parent2Dna.Substring(0, crossoverPoint1) +
                                       parent1Dna.Substring(crossoverPoint1, crossoverPoint2 - crossoverPoint1) +
                                       parent2Dna.Substring(crossoverPoint2);

                offspring1.Chromosome = new Chromosome(offspring1Dna);
                offspring2.Chromosome = new Chromosome(offspring2Dna);

                offspring.Add(offspring1);
                offspring.Add(offspring2);
            }

            return offspring;
        }

        /* Perform uniform crossover on a pair of parents to produce two offspring. In uniform crossover, genes from each parent are randomly chosen to be inherited by the offspring.
         Each gene from the parents is independently chosen to come from one or the other parent, leading to offspring that can have a more random mix of genes compared to other crossover methods. */
        private List<Creature> UniformCrossover(List<Creature> selectedCreatures)
        {
            List<Creature> offspring = new List<Creature>();
            for (int i = 0; i < selectedCreatures.Count; i += 2)
            {
                Creature parent1 = selectedCreatures[i];
                Creature parent2 = selectedCreatures[i + 1];

                Creature offspring1 = new Creature();
                Creature offspring2 = new Creature();

                string parent1Dna = parent1.Chromosome.GetDna();
                string parent2Dna = parent2.Chromosome.GetDna();

                StringBuilder offspring1Dna = new StringBuilder();
                StringBuilder offspring2Dna = new StringBuilder();

                for (int j = 0; j < parent1Dna.Length; j++)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        offspring1Dna.Append(parent1Dna[j]);
                        offspring2Dna.Append(parent2Dna[j]);
                    }
                    else
                    {
                        offspring1Dna.Append(parent2Dna[j]);
                        offspring2Dna.Append(parent1Dna[j]);
                    }
                }

                offspring1.Chromosome = new Chromosome(offspring1Dna.ToString());
                offspring2.Chromosome = new Chromosome(offspring2Dna.ToString());

                offspring.Add(offspring1);
                offspring.Add(offspring2);
            }

            return offspring;
        }

        #endregion

        #region Mutation Methods

        /* Flip Bit Mutation randomly flips a bit (0 to 1 or 1 to 0) in each creature's chromosome in the population,
         introducing small variations in the genetic material of each individual. In this  case, the 20% of the bits are mutated. */
        private List<Creature> FlipBitMutation(List<Creature> crossedCreatures)
        {
            List<Creature> mutatedCreatures = new List<Creature>();
            mutatedCreatures.AddRange(crossedCreatures);
            foreach (var creature in mutatedCreatures)
            {
                var chromosome = creature.Chromosome;
                string dna = chromosome.GetDna();
                string[] dnaSegments = dna.Split('|');

                for (int i = 0; i < dnaSegments.Length; i++)
                {
                    char[] binarySegment = ConvertToBinarySegment(dnaSegments[i]);
                    var mutations = Random.Range(1, binarySegment.Length / 5); //  ~20% of the bits

                    for (int j = 0; j < mutations; j++)
                    {
                        var randomIndex = Random.Range(0, binarySegment.Length);
                        binarySegment[randomIndex] = binarySegment[randomIndex] == '0' ? '1' : '0';
                    }

                    dnaSegments[i] = ConvertFromBinarySegment(binarySegment, dnaSegments[i]);
                }

                var mutatedDna = string.Join("|", dnaSegments);
                chromosome.SetDna(mutatedDna);
            }

            return mutatedCreatures;
        }

        /* Gaussian Mutation adds a small random value to a gene in each creature's chromosome in the population,
        based on a Gaussian distribution, allowing for smoother and more continuous changes in traits. */
        private List<Creature> GaussianMutation(List<Creature> crossedCreatures)
        {
            float mutationStrength = 0.2f;
            List<Creature> mutatedCreatures = new List<Creature>();
            mutatedCreatures.AddRange(crossedCreatures);
            foreach (var creature in mutatedCreatures)
            {
                var chromosome = creature.Chromosome;
                var randomGeneIndex = Random.Range(0, 12); //12 variables in chromosome data
                switch (randomGeneIndex)
                {
                    case 0:
                        chromosome.Diet =
                            (enums.DietType)Random.Range(0, Enum.GetValues(typeof(enums.DietType)).Length);
                        break;
                    case 1:
                        chromosome.TerrainAffinity =
                            (enums.TerrainType)Random.Range(0, Enum.GetValues(typeof(enums.TerrainType)).Length);
                        break;
                    case 2:
                        chromosome.ClimateAffinity =
                            (enums.ClimateType)Random.Range(0, Enum.GetValues(typeof(enums.ClimateType)).Length);
                        break;
                    case 3:
                        chromosome.BasicStats.hp = Mathf.Clamp(
                            chromosome.BasicStats.hp + Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 10)),
                            0, 100);
                        break;
                    case 4:
                        chromosome.BasicStats.speed = Mathf.Clamp(
                            chromosome.BasicStats.speed +
                            Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 10)),
                            0, 100);
                        break;
                    case 5:
                        chromosome.BasicStats.dmg = Mathf.Clamp(
                            chromosome.BasicStats.dmg +
                            Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 10)),
                            0, 100);
                        break;
                    case 6:
                        chromosome.BasicStats.energy = Mathf.Clamp(
                            chromosome.BasicStats.energy +
                            Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 10)),
                            0, 100);
                        break;
                    case 7:
                        chromosome.BasicStats.perception = Mathf.Clamp(
                            chromosome.BasicStats.perception +
                            Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 10)),
                            0, 100);
                        break;
                    case 8:
                        chromosome.LimbCount = Mathf.Clamp(
                            chromosome.LimbCount + Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 5)),
                            0, 10);
                        break;
                    case 9:
                        chromosome.JointsCount = Mathf.Clamp(
                            chromosome.JointsCount + Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 5)),
                            0, 10);
                        break;
                    case 10:
                        chromosome.SizeScale = Mathf.Clamp(
                            chromosome.SizeScale + Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 5)),
                            0.25f, 2.0f);
                        break;
                    case 11:
                        chromosome.Color.r = (byte)Mathf.Clamp(
                            chromosome.Color.r + Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 255)),
                            0, 255);
                        chromosome.Color.g = (byte)Mathf.Clamp(
                            chromosome.Color.g + Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 255)),
                            0, 255);
                        chromosome.Color.b = (byte)Mathf.Clamp(
                            chromosome.Color.b + Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 255)),
                            0, 255);
                        break;
                }
            }

            return mutatedCreatures;
        }

        /* Similar to GaussianMutation but without the Gaussian. Instead, it defines a uniform incremental mutation. In this case, slightly based on the current values */
        private List<Creature> UniformMutation(List<Creature> crossedCreatures)
        {
            List<Creature> mutatedCreatures = new List<Creature>();
            mutatedCreatures.AddRange(crossedCreatures);
            foreach (var creature in mutatedCreatures)
            {
                var chromosome = creature.Chromosome;
                var randomGeneIndex = Random.Range(0, 12); //12 variables in chromosome data
                switch (randomGeneIndex)
                {
                    case 0:
                        chromosome.Diet =
                            (enums.DietType)Random.Range(0, Enum.GetValues(typeof(enums.DietType)).Length);
                        break;
                    case 1:
                        chromosome.TerrainAffinity =
                            (enums.TerrainType)Random.Range(0, Enum.GetValues(typeof(enums.TerrainType)).Length);
                        break;
                    case 2:
                        chromosome.ClimateAffinity =
                            (enums.ClimateType)Random.Range(0, Enum.GetValues(typeof(enums.ClimateType)).Length);
                        break;
                    case 3:
                        chromosome.BasicStats.hp += Random.Range(0, chromosome.BasicStats.hp / 5);
                        break;
                    case 4:
                        chromosome.BasicStats.speed += Random.Range(0, chromosome.BasicStats.speed / 5);
                        break;
                    case 5:
                        chromosome.BasicStats.dmg += Random.Range(0, chromosome.BasicStats.dmg / 5);
                        break;
                    case 6:
                        chromosome.BasicStats.energy += Random.Range(0, chromosome.BasicStats.energy / 5);
                        break;
                    case 7:
                        chromosome.BasicStats.perception += Random.Range(0, chromosome.BasicStats.perception / 5);
                        break;
                    case 8:
                        chromosome.LimbCount += Random.Range(0, chromosome.LimbCount / 2);
                        break;
                    case 9:
                        chromosome.JointsCount += Random.Range(0, chromosome.JointsCount / 2);
                        break;
                    case 10:
                        chromosome.SizeScale += Random.Range(0.25f, chromosome.SizeScale / 2 + 1.25f);
                        break;
                    case 11:
                        chromosome.Color = new Color(
                            Random.Range(0f, 256f),
                            Random.Range(0f, 256f),
                            Random.Range(0f, 256f));
                        break;
                }
            }

            return mutatedCreatures;
        }

        private char[] ConvertToBinarySegment(string segment)
        {
            if (int.TryParse(segment, out int intValue))
            {
                return Convert.ToString(intValue, 2).PadLeft(32, '0').ToCharArray();
            }

            return segment.ToCharArray();
        }

        private string ConvertFromBinarySegment(char[] binarySegment, string originalSegment)
        {
            if (int.TryParse(originalSegment, out _))
            {
                int intValue = Convert.ToInt32(new string(binarySegment), 2);
                return intValue.ToString();
            }

            return new string(binarySegment);
        }

        // Using Box-Muller transform for normal distribution
        private float NormalDistribution(float mean, float stddev)
        {
            float u1 = 1.0f - Random.value;
            float u2 = 1.0f - Random.value;
            float randStdNormal =
                Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
            return mean + stddev * randStdNormal;
        }

        #endregion
    }
}