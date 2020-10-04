using FluentAssertions;
using NUnit.Framework;

namespace QoHash.Core.Test
{
	[TestFixture]
	public class ScanResultTests
	{
		private ScanResult _scanResult;

		[SetUp]
		public void Setup()
		{
			_scanResult = new ScanResult();
		}

		[Test]
		public void ResultShouldBeSortedBySize()
		{
			_scanResult.Add(new ItemInfo { Size = 99, Name = "File1" });
			_scanResult.Add(new ItemInfo { Size = 10, Name = "File2" });

			_scanResult.Items[0].Name.Should().Be("File2");
			_scanResult.Items[1].Name.Should().Be("File1");
		}
	}
}
