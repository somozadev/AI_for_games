using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public GameObject creaturePrefab;
    public int populationSize = 50;
    public List<Creature> creatures;
    public int generation = 1;
    public float generationDuration = 1000f;

    private void Start()
    {
        InitializePopulation();
        StartCoroutine(EvolvePopulation());
    }

    private void InitializePopulation()
    {
        creatures = new List<Creature>();
        for (int i = 0; i < populationSize; i++)
        {
            GameObject creatureObj = Instantiate(creaturePrefab, new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)), Quaternion.identity);
            Creature creature = creatureObj.GetComponent<Creature>();
            creature.Mutate();  // Initial random attributes
            creatures.Add(creature);
        }
    }

    private IEnumerator EvolvePopulation()
    {
        while (true)
        {
            yield return new WaitForSeconds(generationDuration);

            List<Creature> selectedCreatures = SelectFittest();

            // Create next generation
            foreach (Creature creature in creatures)
            {
                Destroy(creature.gameObject);
            }

            creatures.Clear();

            foreach (Creature parent in selectedCreatures)
            {
                for (int i = 0; i < populationSize / selectedCreatures.Count; i++)
                {
                    Creature child = Instantiate(creaturePrefab).GetComponent<Creature>();
                    child.speed = parent.speed;
                    child.size = parent.size;
                    child.color = parent.color;
                    child.Mutate();
                    creatures.Add(child);
                }
            }
            generation++;
        }
    }

    // Selects fittest creatures for reproduction
    private List<Creature> SelectFittest()
    {
        creatures.Sort((c1, c2) => c2.CalculateFitness().CompareTo(c1.CalculateFitness()));
        int selectionSize = Mathf.CeilToInt(populationSize * 0.2f);  // Top 20% selection
        return creatures.GetRange(0, selectionSize);
    }
}