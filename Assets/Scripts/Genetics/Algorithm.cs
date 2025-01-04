using System;
using UnityEngine;

namespace Genetics
{
    public class Algorithm : MonoBehaviour
    {
        public Population _initialPopulation;
        private int _generationId = 0;
        [SerializeField] private int _populationSize = 50;

        [ReadOnly] [SerializeField] private float _elapsedTime;
        [SerializeField] private float _timeScale = 1f;
        [SerializeField] private float _maxTime = 600f;


        private void Start()
        {
            //initial population
            //calculate fitness 
            _initialPopulation = new Population(_populationSize);
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _maxTime)
                NewGeneration();
        }

        private void NewGeneration()
        {
            _generationId++;
            Selection();
            Crossover();
            Mutation();
            //calculate fitness again 
        }
        

        private void OnValidate()
        {
            Time.timeScale = _timeScale;
        }

        //https://medium.com/@byanalytixlabs/a-complete-guide-to-genetic-algorithm-advantages-limitations-more-738e87427dbb good source of info related to this algorithm . i'll use it in the report
        //https://nature-of-code-2nd-edition.netlify.app/genetic-algorithms/#how-genetic-algorithms-work this explains quite good how the algorithm works in it's basics
        private void GenerateInitialPopulation()
        {
            _initialPopulation = new Population(_populationSize);
            _generationId++;
        }

        private void Fitness()
        {
            // we need to create a fitness function for each "monster" genotype. prob can divide them into omnivors, carnivors, hervivors... and create a fitness function for each of them(?)
        } //evaluates each individual from the population, giving them a score

        private void Selection()
        {
            // fitness foreach element in population
        } //select parents for the breeding , here for description but in our instance prob. each creature has its own conditions for breeding given

        /*there are tons of selection methods, like roulette wheel, stochastic universal sampling, tournament, elitism, random, truncation,
         steady state, linear and non linear ranking, age-base, genitor selection (fitness based)*/
        private void Crossover()
        {
            
        } //through crossover of parents chromosome

        /*To perform the selection of genes from the parents, one point crossover, two point , uniform, livery, inheritable algorithms,
         k-point, multipoint, half-uniform, shuffle, uniform, matrix, 3+ parents, linear, single arithmetic, partially mapped, cycled... */
        private void Mutation()
        {
            
        } //performed after breeding occurs 
        /*Flip bit mutation, gaussian, exchange / swap, */
        //recalc fitness and repeat from selection untill population converges          
    }
}