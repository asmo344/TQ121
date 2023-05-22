namespace Tyrafos
{
    public class SummingVariable
    {
        public SummingVariable(int count = 1, int avg = 1)
        {
            this.Count = count;
            this.Average = avg;
        }

        public int Average { get; private set; }
        public int Count { get; private set; }
    }
}