using System.Globalization;
using Genetics;
using TMPro;
using UnityEngine;

public class CreatureInfoDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text diet;
    [SerializeField] private TMP_Text terrain;
    [SerializeField] private TMP_Text climate;
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text dmg;
    [SerializeField] private TMP_Text speed;
    [SerializeField] private TMP_Text energy;
    [SerializeField] private TMP_Text perception;
    [SerializeField] private TMP_Text joints;


    public void UpdateDisplay(Chromosome chromosome)
    {
        diet.text = chromosome.Diet.ToString();
        terrain.text = chromosome.TerrainAffinity.ToString();
        climate.text = chromosome.ClimateAffinity.ToString();
        hp.text = chromosome.BasicStats.hp.ToString();
        dmg.text = chromosome.BasicStats.dmg.ToString();
        speed.text = chromosome.BasicStats.speed.ToString();
        energy.text = chromosome.BasicStats.energy.ToString();
        perception.text = chromosome.BasicStats.perception.ToString(CultureInfo.InvariantCulture);
        joints.text = chromosome.JointsCount.ToString();
    }
}