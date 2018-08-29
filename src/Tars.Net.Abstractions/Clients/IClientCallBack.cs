using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public interface IClientCallBack
    {
        int NewCallBackId();

        Task<Response> NewCallBackTask(int id, int timeout, string servantName, string funcName);

        void CallBack(Response msg);
    }
}