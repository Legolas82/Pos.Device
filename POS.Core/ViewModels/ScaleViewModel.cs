namespace POS.Core.ViewModels
{
    public class ScaleViewModel
    {
        public decimal Weight { get; set; }

        public override string ToString()
        {
            return $"Weight: {Weight}";
        }
    }
}
