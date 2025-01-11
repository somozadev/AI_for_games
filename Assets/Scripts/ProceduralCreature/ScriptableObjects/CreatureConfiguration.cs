using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu( menuName = "ScriptableObjects/CreatureConfiguration", order = 1)]
public class CreatureConfiguration : ScriptableObject
{
    [FormerlySerializedAs("bodyLenght")] public int bodyLength;
    public float[] bodySizes; //or use a graph / equation 
    public int legsInBodyN; //how many joints of the body has legs
    public int legsPerBodyN; 
    
    //public LegsConfiguration legsConfiguration;
    //public headsConfiguration headConfiguration;

} 
