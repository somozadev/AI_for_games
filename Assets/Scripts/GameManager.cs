using Genetics;
using Genetics.Enviromental;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public DayNight DayNight { get; private set; }
    public EnviromentSpawner EnviromentSpawner { get; private set; }
    public FoodSpawner FoodSpawner { get; private set; }
    public Vector2 TerrainSize { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        DayNight = GetComponentInChildren<DayNight>();
        FoodSpawner = GetComponentInChildren<FoodSpawner>();
        EnviromentSpawner = GetComponentInChildren<EnviromentSpawner>();
        TerrainSize = new Vector2(1600, 1600);
    }
}