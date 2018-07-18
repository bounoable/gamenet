using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameNet.Tests
{
    [Serializable]
    public class TestClass
    {
        int homo = 3;
        string What { get; } = "homo";
        List<TestClass> liste = new List<TestClass>();

        public void Homo()
        {
            liste.Add(new TestClass());
        }
    }

    public class ServerTest
    {
        [Fact]
        public void Test()
        {
            var ms = new MemoryStream();
            var s = new BinaryFormatter();
            var t = new TestClass();

            t.Homo();
            t.Homo();

            s.Serialize(ms, t);

            var deser = s.Deserialize(ms);

        }
    }
}
