using System.Collections.Generic;
using Genetics.Environmental;
using UnityEngine;

namespace Genetics.Enviromental
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private Fruit _fruitPrefab;
        [SerializeField] private Plant _plantPrefab;
        [SerializeField] private Crystal _crystalPrefab;
        [SerializeField] private Lumin _luminPrefab;

        [Range(10, 500)] [SerializeField] private float amountPerFoodType = 50;


        private Queue<Fruit> _fruitPool;
        private Queue<Plant> _plantPool;
        private Queue<Crystal> _crystalPool;
        private Queue<Lumin> _luminPool;

        private void Awake()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            _fruitPool = new Queue<Fruit>();
            _plantPool = new Queue<Plant>();
            _crystalPool = new Queue<Crystal>();
            _luminPool = new Queue<Lumin>();

            for (int i = 0; i < amountPerFoodType; i++)
            {
                _fruitPool.Enqueue(CreateNewFruit());
                _plantPool.Enqueue(CreateNewPlant());
                _crystalPool.Enqueue(CreateNewCrystal());
                _luminPool.Enqueue(CreateNewLumin());
            }
        }

        private Fruit CreateNewFruit()
        {
            Fruit fruit = Instantiate(_fruitPrefab, transform);
            fruit.gameObject.SetActive(false);
            return fruit;
        }

        private Plant CreateNewPlant()
        {
            Plant plant = Instantiate(_plantPrefab, transform);
            plant.gameObject.SetActive(false);
            return plant;
        }

        private Crystal CreateNewCrystal()
        {
            Crystal crystal = Instantiate(_crystalPrefab, transform);
            crystal.gameObject.SetActive(false);
            return crystal;
        }

        private Lumin CreateNewLumin()
        {
            Lumin lumin = Instantiate(_luminPrefab, transform);
            lumin.gameObject.SetActive(false);
            return lumin;
        }

        public void Spawn()
        {
            for (int i = 0; i < amountPerFoodType; i++)
            {
                if (_fruitPool.Count > 0)
                {
                    Fruit fruit = _fruitPool.Dequeue();
                    fruit.transform.position = GetRandomPosition();
                    fruit.gameObject.SetActive(true);
                }

                if (_plantPool.Count > 0)
                {
                    Plant plant = _plantPool.Dequeue();
                    plant.transform.position = GetRandomPosition();
                    plant.gameObject.SetActive(true);
                }

                if (_crystalPool.Count > 0)
                {
                    Crystal crystal = _crystalPool.Dequeue();
                    crystal.transform.position = GetRandomPosition();
                    crystal.gameObject.SetActive(true);
                }

                if (_luminPool.Count > 0)
                {
                    Lumin lumin = _luminPool.Dequeue();
                    lumin.transform.position = GetRandomPosition();
                    lumin.gameObject.SetActive(true);
                }
            }
        }

        public void NewDay()
        {
            Remove();
            Spawn();
        }

        public void Remove()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);

                    if (child.TryGetComponent(out Fruit fruit))
                    {
                        _fruitPool.Enqueue(fruit);
                    }
                    else if (child.TryGetComponent(out Plant plant))
                    {
                        _plantPool.Enqueue(plant);
                    }
                    else if (child.TryGetComponent(out Crystal crystal))
                    {
                        _crystalPool.Enqueue(crystal);
                    }
                    else if (child.TryGetComponent(out Lumin lumin))
                    {
                        _luminPool.Enqueue(lumin);
                    }
                }
            }
        }

        public void Remove(GameObject given)
        {
            if (given.gameObject.activeSelf)
            {
                given.gameObject.SetActive(false);

                if (given.TryGetComponent(out Fruit fruit))
                {
                    _fruitPool.Enqueue(fruit);
                }
                else if (given.TryGetComponent(out Plant plant))
                {
                    _plantPool.Enqueue(plant);
                }
                else if (given.TryGetComponent(out Crystal crystal))
                {
                    _crystalPool.Enqueue(crystal);
                }
                else if (given.TryGetComponent(out Lumin lumin))
                {
                    _luminPool.Enqueue(lumin);
                }
            }
        }

        private Vector3 GetRandomPosition()
        {
            var x = GameManager.Instance.TerrainSize.x / 2 - 40;
            var y = GameManager.Instance.TerrainSize.y / 2 - 40;
            return new Vector3(
                Random.Range(-x, y),
                2f,
                Random.Range(-x, y)
            );
        }
    }
}