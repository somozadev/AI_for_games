using System;
using Genetics.Environmental;
using ProceduralCreature;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Genetics
{
    public class CreatureContainer : MonoBehaviour
    {
        public Creature Creature;
        private SphereCollider _collider;
        [SerializeField] private CreatureInfoDisplay infoDisplay;
        [SerializeField] private Body controller;
        [SerializeField] private CreaturePlayerController _movableController;
        [SerializeField] private AgentStateManager _stateManager;
        
        public UnityEvent<Collider> onTriggerEnterEvent;


        public CreaturePlayerController GetCreatureController() => _movableController;
        public Body GetHeadPoint() => controller;
        private void Awake()
        {
            controller = GetComponentInChildren<Body>();
            _collider = gameObject.AddComponent<SphereCollider>();
            infoDisplay = GetComponentInChildren<CreatureInfoDisplay>();
            _stateManager = GetComponent<AgentStateManager>();
            _collider.isTrigger = true;
        }



        public void Init()
        {
            Creature = new Creature();
            controller.ConfigureBody(this,Creature.Chromosome.Color, Creature.Chromosome.JointsCount, Creature.Chromosome.LimbCount, Creature.Chromosome.SizeScale);
            _movableController = controller.points[0].GetComponent<CreaturePlayerController>();
        }

        public void Init(string dna)
        {
            Creature = new Creature(dna);
            controller.ConfigureBody(this,Creature.Chromosome.Color, Creature.Chromosome.JointsCount, Creature.Chromosome.LimbCount, Creature.Chromosome.SizeScale);
            _movableController = controller.points[0].GetComponent<CreaturePlayerController>();
        }

        private void Update()
        {
            infoDisplay.UpdateDisplay(Creature, _stateManager.GetCurrentState().stateName);
        }
    }
}