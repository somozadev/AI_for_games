using System;
using System.Collections.Generic;
using System.Linq;
using Genetics;
using TMPro;
using UnityEngine;

public class AlgorithmCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text populationId;
    [SerializeField] private TMP_Text amountOfCreatures;
    [SerializeField] private TMP_Text averageFitness;
    [SerializeField] private TMP_Text averageDeviation;
    [SerializeField] private TMP_Text standardDeviation;
    [SerializeField] private TMP_Text fitnessRange;
    [SerializeField] private TMP_Text medianFitness;
    [SerializeField] private TMP_Text highestFitness;
    [SerializeField] private TMP_Text lowestFitness;


    public void DisplayPopulationData(Population population)
    {
        var fitness = population.GetPopulation()
            .Select(pop => pop.fitness)
            .ToList();
        if (fitness.Count == 0)
            return;
        var sortedFitness = fitness.OrderBy(f => f).ToList();
        var average = fitness.Average();
        var deviation = fitness.Average(f => Math.Abs(f - average));
        var median = sortedFitness.Count % 2 == 0
            ? (sortedFitness[sortedFitness.Count / 2 - 1] + sortedFitness[sortedFitness.Count / 2]) / 2
            : sortedFitness[sortedFitness.Count / 2];
        var max = fitness.Max();
        var min = fitness.Min();
        var stdDev = Mathf.Sqrt(fitness.Average(f => Mathf.Pow(f - average, 2)));
        var range = max - min;

        populationId.text = "Population " + population.GetId().ToString();
        amountOfCreatures.text = population.GetPopulation().Count.ToString();
        UpdateTMPText(averageFitness, average);
        UpdateTMPText(averageDeviation, deviation);
        UpdateTMPText(medianFitness, median);
        UpdateTMPText(highestFitness, max);
        UpdateTMPText(lowestFitness, min);
        UpdateTMPText(standardDeviation, stdDev);
        UpdateTMPText(fitnessRange, range);

        SavePopulationData(average, deviation, median, max, min, stdDev, range);
    }

    private void SavePopulationData(float average, float deviation, float median, float max, float min, float stdDev,
        float range)
    {
        CSVManager csvManager = new CSVManager("population_data.csv");
        string[] data = new string[]
        {
            Time.time.ToString(),
            average.ToString("F2"),
            deviation.ToString("F2"),
            median.ToString("F2"),
            max.ToString("F2"),
            min.ToString("F2"),
            stdDev.ToString("F2"),
            range.ToString("F2")
        };
        csvManager.AppendData(data);
    }

    private void UpdateTMPText(TMP_Text textElement, float newValue)
    {
        if (float.TryParse(textElement.text, out float previousValue))
        {
            if (newValue > previousValue)
            {
                textElement.color = Color.green;
            }
            else if (newValue < previousValue)
            {
                textElement.color = Color.red;
            }
            else
            {
                textElement.color = Color.white;
            }
        }
        else
        {
            textElement.color = Color.white;
        }

        textElement.text = $"{newValue:F2}";
    }
}