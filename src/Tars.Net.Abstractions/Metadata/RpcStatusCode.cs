namespace Tars.Net.Metadata
{
    public enum RpcStatusCode
    {
        ServerSuccess = 0,
        ServerDecodeErr = -1,
        ServerEncodeErr = -2,
        ServerNoFuncErr = -3,
        ServerNoServantErr = -4,
        ServerResetGrid = -5,
        ServerQueueTimeout = -6,
        AsyncCallTimeout = -7,
        ProxyConnectErr = -8,
        ServerOverload = -9,
        ServerUnknownErr = -99,
    }
}