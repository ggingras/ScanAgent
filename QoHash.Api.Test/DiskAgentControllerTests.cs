using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using QoHash.Api.Controllers;
using QoHash.Api.Dto;
using QoHash.Core;

namespace QoHash.Api.Test
{
	[TestFixture]
	public class DiskAgentControllerTests
	{
		private DiskAgentController _diskAgentController;
		private Mock<IScanAgent> _scanAgentMoq;

		[SetUp]
		public void Setup()
		{
			_scanAgentMoq = new Mock<IScanAgent>();
			_diskAgentController = new DiskAgentController(_scanAgentMoq.Object);
		}

		[Test]
		public async Task GivenScanSucceed_ShouldReturnOKAndScanResult()
		{
			var scanResult = new ScanResult()
			{
				TotalFiles = 1,
				TotalSize = 99,
				TotalFolders = 1
			};
			scanResult.Add(new ItemInfo {LastWriteTime = DateTime.Now, Size = 99, Name = "File1"});

			_scanAgentMoq.Setup(x => x.ScanFolderAsync("ValidPath")).Returns(Task.FromResult(scanResult));
			var result = await _diskAgentController.Scan(new ScanDto {Path = "ValidPath"});

			if (result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
			{
				 objectResult.StatusCode.Should().Be(200);
				 objectResult.Value.GetType().Should().Be(typeof(ScanResult));
			}
		}

		[Test]
		public async Task GivenArgumentExceptionOccurredInScanAgent_ReturnError554InvalidRESTArgument()
		{
			_scanAgentMoq.Setup(x => x.ScanFolderAsync(string.Empty)).Throws<ArgumentException>();
			var result = await _diskAgentController.Scan(new ScanDto {Path = string.Empty});
			((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode.Should().Be(554);
		}

		[Test]
		public async Task GivenUnhandledExceptionOccurred_ReturnError500InternalError()
		{
			_scanAgentMoq.Setup(x => x.ScanFolderAsync(string.Empty)).Throws<Exception>();
			var result = await _diskAgentController.Scan(new ScanDto { Path = string.Empty });
			((Microsoft.AspNetCore.Mvc.ObjectResult)result).StatusCode.Should().Be(500);
		}
	}
}