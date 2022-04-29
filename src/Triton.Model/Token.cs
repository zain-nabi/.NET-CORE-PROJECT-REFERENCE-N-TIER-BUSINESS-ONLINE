namespace Triton.Model
{
    public class Token
    {
        public string expires { get; set; }
        public string issued { get; set; }
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
    }
}
