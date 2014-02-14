using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElasticDemo.Models;
namespace ElasticDemo.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            States s = new States();
            string actual = s.returnState("Mf");
            Assert.AreEqual("MS",actual );
        }
    }
}
