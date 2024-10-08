namespace Genetics
{
    public class Algorithm
    {
        private Population _initialPopulation;

        //https://medium.com/@byanalytixlabs/a-complete-guide-to-genetic-algorithm-advantages-limitations-more-738e87427dbb good source of info related to this algorithm . i'll use it in the report
        private void GenerateInitialPopulation(){}
        private void Fitness(){} //evaluates each individual from the population, giving them a score
        private void Selection(){}//select parents for the breeding , here for description but in our instance prob. each creature has its own conditions for breeding given
        /*there are tons of selection methods, like roulette wheel, stochastic universal sampling, tournament, elitism, random, truncation,
         steady state, linear and non linear ranking, age-base, genitor selection (fitness based)*/
        private void Crossover(){}//through crossover of parents chromosome
        /*To perform the selection of genes from the parents, one point crossover, two point , uniform, livery, inheritable algorithms,
         k-point, multipoint, half-uniform, shuffle, uniform, matrix, 3+ parents, linear, single arithmetic, partially mapped, cycled... */
        private void Mutation(){}//performed after breeding occurs 
        /*Flip bit mutation, gaussian, exchange / swap, */
        //recalc fitness and repeat from selection untill population converges          
    }
}