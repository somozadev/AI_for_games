using System.Collections.Generic;
using Genetics.Enviromental;
using UnityEngine;
using UnityEngine.Serialization;

namespace Genetics.Environmental
{
    public class Lumin : BaseFood
    {
        [SerializeField] private List<GameObject> luminVisualPrefabs;

        protected override void Awake()
        {
            base.Awake();
            gameObject.tag = "Lumin";
            Instantiate(luminVisualPrefabs[Random.Range(0, luminVisualPrefabs.Count - 1)], transform, true);
        }
    }
}