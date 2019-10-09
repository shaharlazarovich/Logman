using System;

namespace Logman.Common.DomainObjects
{
    [Flags]
    public enum EventLevel : short
    {
        Fatal = 1,
        Error = 2,
        Warning = 4,
        Information = 8
    }
}