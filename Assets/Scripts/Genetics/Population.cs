using System.Collections.Generic;
using UnityEngine;

namespace Genetics
{
    [System.Serializable]
    public class Population
    {
        [SerializeField] private List<Creature> _creatures;

        public Population(int size)
        {
            _creatures = new List<Creature>();
            for (int i = 0; i < size; i++)
            {
                
                _creatures.Add(new Creature());
            }
        }
    }
}