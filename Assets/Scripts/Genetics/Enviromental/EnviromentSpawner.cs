using System;
using System.Collections.Generic;
using System.Linq;
using Genetics.Environmental;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Genetics.Enviromental
{
    public class EnviromentSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enviromentPrefab;
        [SerializeField] private Vector2 terrainSize;
        [Range(1, 100)] [SerializeField] private int numberOfAreas;
        [SerializeField] private float areasSpacing = 0.5f;

        [SerializeField] private List<GameObject> areasSpawned;

        public void Spawn()
        {
            if (areasSpawned != null)
            {
                terrainSize = GameManager.Instance.TerrainSize;

                foreach (var obj in areasSpawned)
                    Destroy(obj);
                areasSpawned.Clear();
            }
            else
            {
                areasSpawned = new List<GameObject>();
                terrainSize = GameManager.Instance.TerrainSize;
            }

            float areaWidth = Mathf.Sqrt((terrainSize.x * terrainSize.y) / numberOfAreas);
            Vector3 origin = new Vector3(-terrainSize.x / 2, 0, -terrainSize.y / 2);
            for (int i = 0; i < numberOfAreas; i++)
            {
                Vector3 position;
                var attempts = 0; //if area already placed, try again up to 10 times 

                do
                {
                    var x = Random.Range(origin.x, origin.x + terrainSize.x);
                    var z = Random.Range(origin.z, origin.z + terrainSize.y);
                    position = new Vector3(x, 0, z);
                    attempts++;
                } while (!CanPlace(position, areaWidth) && attempts < 10);

                if (attempts < 10)
                {
                    GameObject newObject = Instantiate(enviromentPrefab, position, Quaternion.identity);
                    newObject.transform.localScale = new Vector3(areaWidth, 1, areaWidth);
                    areasSpawned.Add(newObject);
                    newObject.GetComponent<EnviromentArea>().RandomEnviroment();
                }
            }
        }

        private bool CanPlace(Vector3 position, float areaWidth)
        {
            return areasSpawned == null ||
                   areasSpawned.All(obj =>
                       !(Vector3.Distance(obj.transform.position, position) < areaWidth + areasSpacing));
        }
    }
}