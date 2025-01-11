using System.Collections.Generic;
using Genetics.Enviromental;
using UnityEngine;

namespace Genetics.Environmental
{
    public class Fruit : BaseFood
    {

        [SerializeField] private List<GameObject> fruitsVisualPrefabs; 
        protected override void Awake()
        {
            base.Awake();
            gameObject.tag = "Fruit";
            Instantiate(fruitsVisualPrefabs[Random.Range(0, fruitsVisualPrefabs.Count - 1)], transform, true);
        }
        
    }
}