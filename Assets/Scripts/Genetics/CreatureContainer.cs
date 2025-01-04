using System;
using ProceduralCreature;
using UnityEngine;

namespace Genetics
{
    public class CreatureContainer : MonoBehaviour
    {
        public Creature Creature;
        [SerializeField] private Body controller;
        
        private void Awake()
        {
            controller = GetComponentInChildren<Body>();
        }

        public void Init()
        {
            Creature = new Creature();
        }
        public void Init(string dna)
        {
            Creature = new Creature(dna);
        }
    }
}