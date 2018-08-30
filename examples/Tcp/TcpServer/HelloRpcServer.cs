using System;
using System.Threading;
using System.Threading.Tasks;
using TcpCommon;

namespace TcpServer
{
    public class HelloRpcServer : IHelloRpc
    {
        public string Hello(int no, string name)
        {
            string result = null;
            if (name.Trim().ToLower() == "Victor")
            {
                result = $"{no}: Sorry, {name}";
            }
            else
            {
                result = $"{no}: Hi, {name}";
            }

            Console.WriteLine(result);
            return result + " callback";
        }

        public void HelloHolder(int no, out string name)
        {
            name = no.ToString() + "Vic";
        }

        public async Task HelloOneway(int no, string name)
        {
            await Task.Delay(3000);
            Console.WriteLine($"From oneway - {no}: Hi, {name}");
        }

        public Task<string> HelloTask(int no, string name)
        {
            return Task.FromResult(Hello(no, name));
        }

        public ValueTask<string> HelloValueTask(int no, string name)
        {
            return new ValueTask<string>(Hello(no, name));
        }
    }
}