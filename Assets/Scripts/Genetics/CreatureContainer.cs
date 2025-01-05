using System;
using Genetics.Environmental;
using ProceduralCreature;
using UnityEngine;

namespace Genetics
{
    public class CreatureContainer : MonoBehaviour
    {
        public Creature Creature;
        private SphereCollider _collider;
        [SerializeField] private CreatureInfoDisplay infoDisplay;
        [SerializeField] private Body controller;

        private void Awake()
        {
            controller = GetComponentInChildren<Body>();
            _collider = gameObject.AddComponent<SphereCollider>();
            infoDisplay = GetComponentInChildren<CreatureInfoDisplay>();
            _collider.isTrigger = true;
        }



        public void Init()
        {
            Creature = new Creature();
            controller.ConfigureBody(Creature.Chromosome.Color, Creature.Chromosome.JointsCount, Creature.Chromosome.LimbCount, Creature.Chromosome.SizeScale);
        }

        public void Init(string dna)
        {
            Creature = new Creature(dna);
            controller.ConfigureBody(Creature.Chromosome.Color, Creature.Chromosome.JointsCount, Creature.Chromosome.LimbCount, Creature.Chromosome.SizeScale);
        }

        private void Update()
        {
            infoDisplay.UpdateDisplay(Creature.Chromosome);
        }
    }
}