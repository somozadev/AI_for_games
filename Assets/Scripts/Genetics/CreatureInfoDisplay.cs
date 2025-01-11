using System.Globalization;
using Genetics;
using TMPro;
using UnityEngine;

public class CreatureInfoDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text fitness;
    [SerializeField] private TMP_Text currentState;

    [SerializeField] private TMP_Text diet;
    [SerializeField] private TMP_Text terrain;
    [SerializeField] private TMP_Text climate;
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text dmg;
    [SerializeField] private TMP_Text speed;
    [SerializeField] private TMP_Text energy;
    [SerializeField] private TMP_Text perception;
    [SerializeField] private TMP_Text joints;

    public void UpdateDisplay(Creature creature, string state)
    {
        Chromosome chromosome = creature.Chromosome;
        fitness.text = creature.Fitness.ToString(CultureInfo.InvariantCulture);
        diet.text = chromosome.Diet.ToString();
        terrain.text = chromosome.TerrainAffinity.ToString();
        climate.text = chromosome.ClimateAffinity.ToString();
        hp.text = chromosome.BasicStats.hp.ToString();
        dmg.text = chromosome.BasicStats.dmg.ToString();
        speed.text = chromosome.BasicStats.speed.ToString();
        energy.text = chromosome.BasicStats.energy.ToString();
        perception.text = chromosome.BasicStats.perception.ToString(CultureInfo.InvariantCulture);
        joints.text = chromosome.JointsCount.ToString();
        currentState.text = state;
    }
}


/*    public void UpdateDisplay(Creature creature)
    {
        Chromosome chromosome = creature.Chromosome;
        UpdateTMPText(fitness, creature.Fitness);
        UpdateTMPText(diet, chromosome.Diet);
        UpdateTMPText(terrain, chromosome.TerrainAffinity);
        UpdateTMPText(climate, chromosome.ClimateAffinity);
        UpdateTMPText(hp, chromosome.BasicStats.hp);
        UpdateTMPText(dmg, chromosome.BasicStats.dmg);
        UpdateTMPText(speed, chromosome.BasicStats.speed);
        UpdateTMPText(energy, chromosome.BasicStats.energy);
        UpdateTMPText(perception, chromosome.BasicStats.perception);
        UpdateTMPText(joints, chromosome.JointsCount);
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

    private void UpdateTMPText(TMP_Text textElement, int newValue)
    {
        UpdateTMPText(textElement, (float)newValue);
    }

    private void UpdateTMPText(TMP_Text textElement, string newValue)
    {
        textElement.color = Color.white;
        textElement.text = newValue;
    }

    private void UpdateTMPText<TEnum>(TMP_Text textElement, TEnum newValue) where TEnum : Enum
    {
        string previousValue = textElement.text;
        if (!newValue.ToString().Equals(previousValue))
        {
            textElement.color = new Color(1f, 0.647f, 0f);
        }
        else
        {
            textElement.color = Color.white;
        }

        textElement.text = newValue.ToString();
    }
}*/