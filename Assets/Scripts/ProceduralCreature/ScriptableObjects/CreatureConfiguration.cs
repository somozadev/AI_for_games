using UnityEngine;


[CreateAssetMenu( menuName = "ScriptableObjects/CreatureConfiguration", order = 1)]
public class CreatureConfiguration : ScriptableObject
{
    public int bodyLenght;
    public float[] bodySizes; //or use a graph / equation 
    public int legsInBodyN; //how many joints of the body has legs
    public int legsPerBodyN; 
    
    //public LegsConfiguration legsConfiguration;
    //public headsConfiguration headConfiguration;

} 
