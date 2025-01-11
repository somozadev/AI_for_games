using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        [Space(10)] [SerializeField] private AlgorithmCanvas _algorithmCanvas;

        private void Awake()
        {
            _algorithmCanvas = GetComponentInChildren<AlgorithmCanvas>();
            GenerateInitialPopulation();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _maxTime)
            {
                NewGeneration();
                _elapsedTime = 0;
            }
        }

        private void NewGeneration()
        {
            //delete old generation? 
            List<Creature> newPopulationCreatures = new List<Creature>();

            _generationId++;
            newPopulationCreatures = Selection();
            newPopulationCreatures = Crossover(newPopulationCreatures);
            newPopulationCreatures = Mutation(newPopulationCreatures);
            _algorithmCanvas.DisplayPopulationData(_currentPopulation);
            // _currentPopulation.gameObject.SetActive(false);
            Destroy(_currentPopulation.gameObject);
            _currentPopulation = Instantiate(populationPrefab, Vector3.zero, Quaternion.identity, transform);
            _currentPopulation.Init(_populationSize, _generationId, newPopulationCreatures);
            //create the new generation 
        }


        private void OnValidate()
        {
            Time.timeScale = _timeScale;
        }

        private void GenerateInitialPopulation()
        {
            _currentPopulation = Instantiate(populationPrefab, Vector3.zero, Quaternion.identity, transform);
            _currentPopulation.Init(_populationSize, _generationId);
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

            for (int i = 0; i < selectedCreatures.Count - 1; i += 2)
            {
                Creature parent1 = selectedCreatures[i];
                Creature parent2 = selectedCreatures[i + 1];

                string parent1Dna = parent1.Chromosome.GetDna();
                string parent2Dna = parent2.Chromosome.GetDna();


                int maxCrossoverPoint = Mathf.Min(parent1Dna.Length, parent2Dna.Length);
                int crossoverPoint = Random.Range(1, maxCrossoverPoint);

                Creature offspring1 = new Creature(null);
                Creature offspring2 = new Creature(null);


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


                int maxCrossoverPoint1 = Mathf.Min(parent1Dna.Length, parent2Dna.Length);
                int crossoverPoint1 = Random.Range(1, maxCrossoverPoint1 - 1);
                var crossoverPoint2 = Random.Range(crossoverPoint1, maxCrossoverPoint1);

                Creature offspring1 = new Creature(null);
                Creature offspring2 = new Creature(null);


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

            for (int i = 0; i < selectedCreatures.Count - 1; i += 2)
            {
                Creature parent1 = selectedCreatures[i];
                Creature parent2 = selectedCreatures[i + 1];

                Creature offspring1 = new Creature(null);
                Creature offspring2 = new Creature(null);

                string parent1Dna = parent1.Chromosome.GetDna();
                string parent2Dna = parent2.Chromosome.GetDna();

                int minLength = Mathf.Min(parent1Dna.Length, parent2Dna.Length);

                StringBuilder offspring1Dna = new StringBuilder();
                StringBuilder offspring2Dna = new StringBuilder();

                for (int j = 0; j < minLength; j++)
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
                string[] dnaBitSegments = dna.Split('|');

                for (int i = 0; i < dnaBitSegments.Length; i++)
                {
                    if (IsNumericSegment(dnaBitSegments[i]))
                    {
                        char[] binarySegment = ConvertToBinarySegment(dnaBitSegments[i]);
                        int bitsToMutate = Mathf.CeilToInt(binarySegment.Length * 0.2f);

                        for (int j = 0; j < bitsToMutate; j++)
                        {
                            int bitIndex = GetBiasedBitIndex(binarySegment.Length);
                            binarySegment[bitIndex] = binarySegment[bitIndex] == '0' ? '1' : '0';
                        }

                        dnaBitSegments[i] = ConvertFromBinarySegment(binarySegment, dnaBitSegments[i]);

                        dnaBitSegments[i] = ClampValue(dnaBitSegments[i], i);
                    }
                    else if (IsCommaSeparatedValues(dnaBitSegments[i]))
                    {
                        string[] values = dnaBitSegments[i].Split(',');

                        for (int j = 0; j < values.Length; j++)
                        {
                            char[] binaryValue = ConvertToBinarySegment(values[j]);

                            if (binaryValue.Length > 0)
                            {
                                int bitIndex = GetBiasedBitIndex(binaryValue.Length);
                                binaryValue[bitIndex] = binaryValue[bitIndex] == '0' ? '1' : '0';
                            }

                            values[j] = ConvertFromBinarySegment(binaryValue, values[j]);
                        }

                        string parsedValues = string.Join(",", values);
                        parsedValues = ClampValue(parsedValues, i);
                        dnaBitSegments[i] = parsedValues;
                    }
                }

                var mutatedDna = string.Join("|", dnaBitSegments);
                chromosome.ResetDna();
                chromosome.SetDna(mutatedDna);
            }

            return mutatedCreatures;
        }


        /* Gaussian Mutation adds a small random value to a gene in each creature's chromosome in the population,
        based on a Gaussian distribution, allowing for smoother and more continuous changes in traits. */
private List<Creature> GaussianMutation(List<Creature> crossedCreatures)
{
    float mutationStrength = 2f;
    List<Creature> mutatedCreatures = new List<Creature>(crossedCreatures);

    foreach (var creature in mutatedCreatures)
    {
        var chromosome = creature.Chromosome;
        int geneCount = 12; // Número total de genes posibles
        int randomGeneIndex = Random.Range(0, geneCount);

        switch (randomGeneIndex)
        {
            case 0: // Diet
                chromosome.Diet = (enums.DietType)Random.Range(0, Enum.GetValues(typeof(enums.DietType)).Length);
                break;
            case 1: // Terrain Affinity
                chromosome.TerrainAffinity = (enums.TerrainType)Random.Range(0, Enum.GetValues(typeof(enums.TerrainType)).Length);
                break;
            case 2: // Climate Affinity
                chromosome.ClimateAffinity = (enums.ClimateType)Random.Range(0, Enum.GetValues(typeof(enums.ClimateType)).Length);
                break;
            case 3: // HP
                chromosome.BasicStats.hp = Mathf.Clamp(
                    chromosome.BasicStats.hp + Mathf.RoundToInt(NormalDistribution(1, mutationStrength * 10)),
                    0, 100);
                break;
            case 4: // Speed
                chromosome.BasicStats.speed = Mathf.Clamp(
                    chromosome.BasicStats.speed + Mathf.RoundToInt(NormalDistribution(1, mutationStrength * 10)),
                    0, 100);
                break;
            case 5: // Damage
                chromosome.BasicStats.dmg = Mathf.Clamp(
                    chromosome.BasicStats.dmg + Mathf.RoundToInt(NormalDistribution(1, mutationStrength * 10)),
                    0, 100);
                break;
            case 6: // Energy
                chromosome.BasicStats.energy = Mathf.Clamp(
                    chromosome.BasicStats.energy + Mathf.RoundToInt(NormalDistribution(1, mutationStrength * 10)),
                    0, 100);
                break;
            case 7: // Perception
                chromosome.BasicStats.perception = Mathf.Clamp(
                    chromosome.BasicStats.perception + Mathf.RoundToInt(NormalDistribution(1, mutationStrength * 10)),
                    0, 100);
                break;
            case 8: // Limb Count
                chromosome.LimbCount = Mathf.Clamp(
                    chromosome.LimbCount + Mathf.RoundToInt(NormalDistribution(0, mutationStrength * 2)),
                    0, 10);
                break;
            case 9: // Joint Count
                chromosome.JointsCount = Mathf.Clamp(
                    chromosome.JointsCount + Mathf.RoundToInt(NormalDistribution(1, mutationStrength * 3)),
                    0, 10);
                break;
            case 10: // Size Scale
                chromosome.SizeScale = Mathf.Clamp(
                    chromosome.SizeScale + (float)NormalDistribution(1, mutationStrength),
                    0.25f, 2.0f);
                break;
            case 11: // Color Mutation
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
                var randomGeneIndex = Random.Range(0, 12);
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
                        chromosome.BasicStats.hp += Random.Range(1, chromosome.BasicStats.hp / 5);
                        break;
                    case 4:
                        chromosome.BasicStats.speed += Random.Range(1, chromosome.BasicStats.speed / 5);
                        break;
                    case 5:
                        chromosome.BasicStats.dmg += Random.Range(1, chromosome.BasicStats.dmg / 5);
                        break;
                    case 6:
                        chromosome.BasicStats.energy += Random.Range(1, chromosome.BasicStats.energy / 5);
                        break;
                    case 7:
                        chromosome.BasicStats.perception += Random.Range(1, chromosome.BasicStats.perception / 5);
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
                            Random.Range(1f, 256f),
                            Random.Range(1f, 256f),
                            Random.Range(1f, 256f));
                        break;
                }
            }

            return mutatedCreatures;
        }

        #region Bit-Operations

        private int GetBiasedBitIndex(int length)
        {
            float biasFactor = 2.0f;
            float[] probabilities = new float[length];

            for (int i = 0; i < length; i++)
            {
                probabilities[i] = Mathf.Pow(biasFactor, i);
            }

            float sum = probabilities.Sum();
            for (int i = 0; i < length; i++)
            {
                probabilities[i] /= sum;
            }

            float randomValue = Random.value;
            float cumulativeProbability = 0.0f;

            for (int i = 0; i < length; i++)
            {
                cumulativeProbability += probabilities[i];
                if (randomValue < cumulativeProbability)
                {
                    return i;
                }
            }

            return length - 1;
        }

        private string ClampValue(string value, int segmentIndex)
        {
            switch (segmentIndex)
            {
                case 0:
                    return ClampInt(value, 0, 5);
                case 1:
                    return ClampInt(value, 0, 2);
                case 2:
                    return ClampInt(value, 0, 4);
                case 3:
                case 4:
                    string[] subValues = value.Split(',');
                    subValues[0] = ClampInt(subValues[0], 0, 60);
                    subValues[1] = ClampInt(subValues[1], 0, 35);
                    subValues[2] = ClampInt(subValues[2], 0, 50);
                    subValues[3] = ClampInt(subValues[3], 0, 100);
                    subValues[4] = ClampInt(subValues[4], 0, 40);
                    return string.Join(",", subValues);
                case 5:
                    return ClampInt(value, 0, 120);
                case 6:
                    return ClampInt(value, 1, 60);
                case 7:
                    return ClampFloat(value, 0.1f, 5.0f);
                case 8:
                    string[] rgbaValues = value.Split(',');
                    rgbaValues[0] = ClampInt(rgbaValues[0], 0, 255);
                    rgbaValues[1] = ClampInt(rgbaValues[1], 0, 255);
                    rgbaValues[2] = ClampInt(rgbaValues[2], 0, 255);
                    rgbaValues[3] = ClampInt(rgbaValues[3], 0, 255);
                    return string.Join(",", rgbaValues);
                default:
                    return value;
            }
        }

        private string ClampInt(string value, int min, int max)
        {
            int intValue = int.Parse(value);
            intValue = Mathf.Clamp(intValue, min, max);
            return intValue.ToString();
        }

        private string ClampFloat(string value, float min, float max)
        {
            float floatValue = float.Parse(value);
            floatValue = Mathf.Clamp(floatValue, min, max);
            return floatValue.ToString("0.##");
        }

        private bool IsNumericSegment(string segment)
        {
            return int.TryParse(segment, out _);
        }

        private bool IsFloatSegment(string segment)
        {
            return float.TryParse(segment, out _);
        }

        private bool IsCommaSeparatedValues(string segment)
        {
            return segment.Contains(",");
        }

        private char[] ConvertToBinarySegment(string segment)
        {
            int intValue;
            if (int.TryParse(segment, out intValue))
            {
                return Convert.ToString(intValue, 2).PadLeft(8, '0').ToCharArray();
            }

            if (float.TryParse(segment, out float floatValue))
            {
                intValue = BitConverter.SingleToInt32Bits(floatValue);
                return Convert.ToString(intValue, 2).PadLeft(16, '0').ToCharArray();
            }

            return segment.ToCharArray();
        }

        private string ConvertFromBinarySegment(char[] binarySegment, string originalSegment)
        {
            if (int.TryParse(originalSegment, out _))
            {
                var intValue = Convert.ToInt32(new string(binarySegment), 2);
                return intValue.ToString();
            }

            if (float.TryParse(originalSegment, out _))
            {
                var intValue = Convert.ToInt32(new string(binarySegment), 2);
                var floatValue = BitConverter.Int32BitsToSingle(intValue);
                return floatValue.ToString("0.##");
            }

            return new string(binarySegment);
        }

        #endregion

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