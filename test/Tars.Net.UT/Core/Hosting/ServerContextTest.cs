using Tars.Net.Hosting;
using Xunit;

namespace Tars.Net.UT.Core.Hosting
{
    public class ServerContextTest
    {
        [Fact]
        public void ServerContextWhenSetShouldTight()
        {
            var context = new ServerContext();
            context.Context.Add("t1", "t1");
            ServerContext.Current = context;
            ServerContext.Current.Context.Add("t2", "t2");
            Assert.Same(context, ServerContext.Current);
            Assert.Same("t1", ServerContext.Current.Context["t1"]);
            Assert.Same("t2", ServerContext.Current.Context["T2"]);
        }
    }
}