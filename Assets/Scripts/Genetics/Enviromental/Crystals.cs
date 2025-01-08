using System.Collections.Generic;
using Genetics.Enviromental;
using UnityEngine;
using UnityEngine.Serialization;

namespace Genetics.Environmental
{
    public class Crystal : BaseFood
    {
        [SerializeField] private List<GameObject> crystalsVisualPrefabs;

        protected override void Awake()
        {
            base.Awake();
            gameObject.tag = "Crystal";
            Instantiate(crystalsVisualPrefabs[Random.Range(0, crystalsVisualPrefabs.Count - 1)], transform, true);
        }
    }
}