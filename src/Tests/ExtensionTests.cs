using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Flexo;
using Flexo.Extensions;
using NUnit.Framework;
using Should;

namespace Tests
{
    [TestFixture]
    public class ExtensionTests
    {
        [Test]
        public void should_format()
        {
            "oh {0}".ToFormat("hai").ShouldEqual("oh hai");
            "oh".ToFormat().ShouldEqual("oh");
        }

        [Test]
        public void should_foreach_on_ienumerable()
        {
            var count = 0;
            new[] { 1, 2 }.ForEach(x => count += x);
            count.ShouldEqual(3);
        }

        [Test]
        [TestCase(typeof(decimal))]
        [TestCase(typeof(float))]
        [TestCase(typeof(double))]
        [TestCase(typeof(sbyte))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(short))]
        [TestCase(typeof(ushort))]
        [TestCase(typeof(int))]
        [TestCase(typeof(uint))]
        [TestCase(typeof(long))]
        [TestCase(typeof(ulong))]
        public void should_indicate_if_a_value_is_numeric(Type type)
        {
            Convert.ChangeType(1, type).IsNumeric().ShouldBeTrue();
        }

        [Test]
        public void should_create_and_add_an_element()
        {
            var parent = new XElement("parent");
            var child = parent.CreateElement("child");
            child.Name.ShouldEqual("child");
            child.Parent.ShouldEqual(parent);
            parent.Elements().First().ShouldEqual(child);
        }

        [Test]
        public void should_get_xml_xpath()
        {
            var document = XDocument.Parse("<oh><hai><yada yada=\"\"/></hai></oh>");
            document.Root.XPathSelectElement("/oh").GetPath().ShouldEqual("/oh");
            document.Root.XPathSelectElement("/oh/hai/yada").GetPath().ShouldEqual("/oh/hai/yada");
        }

        public class Walkable
        {
            public Walkable Parent { get; set; }
        }

        [Test]
        public void should_walk()
        {
            var node1 = new Walkable();
            var node2 = new Walkable { Parent = node1 };
            var node3 = new Walkable { Parent = node2 };

            var result = node3.Walk(x => x.Parent).ToList();

            result.Count.ShouldEqual(3);
            result[0].ShouldEqual(node3);
            result[1].ShouldEqual(node2);
            result[2].ShouldEqual(node1);
        }

        [Test]
        public void should_aggregate()
        {
            new [] { "h", "a", "i" }.Aggregate().ShouldEqual("hai");
        }
    }
}
