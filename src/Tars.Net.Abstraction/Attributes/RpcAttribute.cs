using System;

namespace Tars.Net.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class RpcAttribute : Attribute
    {
    }
}