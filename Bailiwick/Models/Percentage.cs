
namespace Bailiwick.Models
{
    public class Percentage<T>
    {
        public Percentage(T value, double partial)
        {
            Value = value;
            Partial = partial;
        }

        public T Value { get; private set; }
        public double Partial { get; private set; }
    }
}
