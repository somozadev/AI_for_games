using UnityEngine;
using UnityEngine.UI;

public class EvolutionController : MonoBehaviour
{
    public PopulationManager populationManager;
    public Text generationText;

    private void Update()
    {
        generationText.text = "Generation: " + populationManager.generation;
    }
}