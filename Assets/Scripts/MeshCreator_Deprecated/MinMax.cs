namespace MeshCreator
{
    [System.Serializable]
    public struct MinMax<T> where T : System.IComparable<T>
    {
        public T min;
        public T max;

        public MinMax(T min, T max)
        {
            this.min = min;
            this.max = max;
        }
    }
   
}