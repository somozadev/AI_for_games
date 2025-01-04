using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class Population : MonoBehaviour
    {
        [SerializeField] private float areaSize = 20f;
        [SerializeField] private CreatureContainer creaturePrefab;
        [SerializeField] private List<CreatureContainer> _creatures;
        [SerializeField] private int id;

        public List<Creature> GetPopulation() => _creatures.Select(container => container.Creature).ToList();
        public List<Creature> GetPopulationGo() => _creatures.Select(container => container.Creature).ToList();

        public void Init(int size, int id)
        {
            this.id = id;
            _creatures = new List<CreatureContainer>();
            for (var i = 0; i < size; i++)
            {
                var xPos = Random.Range(-areaSize / 2f, areaSize / 2f);
                var zPos = Random.Range(-areaSize / 2f, areaSize / 2f);

                Vector3 randomPos = new Vector3(xPos, 0f, zPos);
                CreatureContainer creature = Instantiate(creaturePrefab, randomPos, Quaternion.identity, transform);
                creature.Init();
                _creatures.Add(creature);
            }
        }

        //todo -> see the impact of the new population 
        public void Init(int size, int id, List<Creature> creatures)
        {
            this.id = id;
            _creatures = new List<CreatureContainer>();
            for (var i = 0; i < size; i++)
            {
                var xPos = Random.Range(-areaSize / 2f, areaSize / 2f);
                var zPos = Random.Range(-areaSize / 2f, areaSize / 2f);

                Vector3 randomPos = new Vector3(xPos, 0f, zPos);
                CreatureContainer creature = Instantiate(creaturePrefab, randomPos, Quaternion.identity, transform);
                creature.Init(creatures[i].Chromosome.GetDna());
                _creatures.Add(creature);
            }
        }
    }
}