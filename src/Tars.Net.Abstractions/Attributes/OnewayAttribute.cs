using System;

namespace Tars.Net.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnewayAttribute : Attribute
    {
    }
}