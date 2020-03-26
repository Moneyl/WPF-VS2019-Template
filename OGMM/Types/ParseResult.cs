
namespace OGMM.Types
{
    public class ParseResult
    {
        public bool Successful { get; }
        public string Message { get; }

        public ParseResult(bool successful, string message = "")
        {
            Successful = successful;
            Message = message;
        }
    }
}
