using System.Collections.Generic;
using Genetics.Enviromental;
using UnityEngine;
using UnityEngine.Serialization;

namespace Genetics.Environmental
{
    public class Plant : BaseFood
    {
        [SerializeField] private List<GameObject> plantsVisualPrefabs;

        protected override void Awake()
        {
            base.Awake();
            gameObject.tag = "Plant";
            Instantiate(plantsVisualPrefabs[Random.Range(0, plantsVisualPrefabs.Count - 1)], transform, true);
        }
    }
}