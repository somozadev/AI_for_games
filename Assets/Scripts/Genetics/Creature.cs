namespace Genetics
{
    public abstract class Creature
    {
        public Chromosome Chromosome { get; private set; }

        public Creature()
        {
            // generate random chromosome from a min - max values
        }
    }
}