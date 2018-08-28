using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public interface IClientCallBack
    {
        void CallBack(Response msg);
    }
}