using System.Collections.Generic;

namespace Genetics
{
    public class Population
    {
        private List<Creature> _creatures;

        public Population(int size)
        {
            _creatures = new List<Creature>(size); 
        }
    }
}