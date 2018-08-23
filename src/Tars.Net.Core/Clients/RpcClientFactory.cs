using System;
using System.Reflection;

namespace Tars.Net.Clients
{
    public class RpcClientFactory : IRpcClientFactory
    {
        public IRpcClientInvoker GetClientInvoker(MethodInfo method)
        {
            //ToDo: 通过method找到rpc 元数据 
            //      并通过元数据生成rpc调用器
            throw new NotImplementedException();
        }
    }
}