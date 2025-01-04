using System;
using Genetics.DecisionTree;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Genetics
{
    public class CreatureAI : MonoBehaviour
    {
        private Creature _creature;
        [SerializeField] private Node _tree;
        [SerializeField] private float stateTimer;
        [SerializeField] private Enviroment currentEnviroment;

        private void Awake()
        {
            _creature = GetComponent<Creature>();
            stateTimer = Random.Range(2f, 5f);

            // _tree = new HealthNode(30f); 
            // _tree = new EnergyNode(20f); 
        }

    }
}