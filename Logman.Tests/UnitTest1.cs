using System;
using System.Data.Common;
using System.Data.SqlClient;
using Logman.Data.SqlServer.Facade;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logman.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dal = new SqlDataAccessLayer();
            var u = dal.GetUnitOfWork();
            var i = dal.GetEventRepository(u);

            var con = u.Connection;
            var com = con.CreateCommand() as SqlCommand;
            var com2 = com;
        }
    }
}
