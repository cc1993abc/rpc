namespace Tars.Net.Codecs
{
    public class Request
    {
        public int Id { get; set; }
        public string ServantName { get; set; }
        public string FuncName { get; set; }
        public bool IsOneway { get; set; }
    }
}