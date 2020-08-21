namespace Versandtracks0rBlazor.Models
{
    public class Toast
    {
        public string Message { get; set; }
        public ToastType ToastType { get; set; }
        public int? Timeout { get; set; }
    }
}