using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Creature : MonoBehaviour
{
    public float speed;
    public float size;
    public int limbs;
    public int wings;
    public int eyes;
    public int children;
    public float intelect;
    public float FOV;
    public double swim;
    public string Diet;
    public string MType;
    public double LaysEggs;
    public double FormPack;

    public Color color;
    private Vector3 targetPosition;

    private void Start()
    {
        SetRandomTargetPosition();
        ApplyAttributes();
    }

    private void Update()
    {
        MoveTowardTarget();
    }

    // Mutate attributes randomly
    
    public void Mate()
    {

    }
    
    public void Mutate()
    {
        speed += Random.Range(-0.5f, 0.5f);
        size += Random.Range(-0.2f, 0.3f);
        size = Mathf.Clamp(size, 0.5f, Random.value);
        color = new Color(Random.value, Random.value, Random.value);
        limbs += Random.Range(1, 8);
    }

    // Apply the attributes
    private void ApplyAttributes()
    {
        transform.localScale = Vector3.one * size;
        GetComponent<Renderer>().material.color = color;
        limbs = limbs++;
    }

    // Move toward a target
    private void MoveTowardTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetRandomTargetPosition();
        }
    }

    private void SetRandomTargetPosition()
    {
        targetPosition = new Vector3(Random.Range(-10f, 10f), transform.position.y, Random.Range(-10f, 10f));
    }

    // Fitness function based on some criteria (e.g., how close it gets to center)
    public float CalculateFitness()
    {
        return 1f / Vector3.Distance(transform.position, Vector3.zero);
    }
}