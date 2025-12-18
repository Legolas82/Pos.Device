namespace POS.Core.ViewModels
{
    public class ScannerViewModel
    {
        public string Data { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $@"{Data}
                      {Type}";
        }
    }
}
