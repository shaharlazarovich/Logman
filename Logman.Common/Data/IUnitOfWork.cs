using System;
using System.Data;
using System.Data.Common;

namespace Logman.Common.Data
{
    public interface IUnitOfWork : IDisposable
    {
        DbConnection Connection { get; }
    }
}