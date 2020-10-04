using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using QoHash.Core.IO;
using Moq;

namespace QoHash.Core.Test
{
	[TestFixture]
	public class ScanAgentTests
	{
		private ScanAgent _agent;
		private Mock<IDirectory> _directoryMock;
		private Mock<IDirectoryInfoFactory> _directoryInfoFactoryMock;
		private Mock<IFileInfoFactory> _fileInfoFactoryMock;

		[SetUp]
		public void Setup()
		{
			_directoryMock = new Mock<IDirectory>();
			_directoryInfoFactoryMock = new Mock<IDirectoryInfoFactory>();
			_fileInfoFactoryMock = new Mock<IFileInfoFactory>();

			_agent = new ScanAgent(_directoryMock.Object, _directoryInfoFactoryMock.Object, _fileInfoFactoryMock.Object);
		}

		[TestCase("")]
		[TestCase(null)]
		[TestCase("z:\folderthatdoesnotexists")]
		public void GivenInvalidPath_ShouldThrowArgumentException(string invalidPath)
		{
			var directoryInfoMock = new Mock<IDirectoryInfo>();
			directoryInfoMock.Setup(x => x.Exists).Returns(false);
			_directoryInfoFactoryMock.Setup(x => x.Create(invalidPath)).Returns(directoryInfoMock.Object);

			Func<Task> a = async () => await _agent.ScanFolderAsync(invalidPath);
			a.Should().Throw<ArgumentException>();
		}

		[Test]
		public async Task GivenUnauthorizedExceptionWhileScanningDirectory_ShouldContinue()
		{
			const string validPath = "path";
			MoqDirectory(validPath);

			_directoryMock.Setup(x => x.EnumerateDirectories(validPath, "*", SearchOption.TopDirectoryOnly)).Throws<UnauthorizedAccessException>();

			var scanResult = await _agent.ScanFolderAsync(validPath);
			scanResult.Errors.Count.Should().Be(1);
		}

		[Test]
		public async Task GivenExceptionWhileScanningFile_ShouldContinue()
		{
			const string validPath = "path";
			const string file1 = "file1";
			MoqDirectory(validPath);

			_fileInfoFactoryMock.Setup(x => x.Create(file1)).Throws<Exception>();
			_directoryMock.Setup(x => x.EnumerateFiles(validPath, "*", SearchOption.TopDirectoryOnly)).Returns(new List<string> { file1 });

			var scanResult = await _agent.ScanFolderAsync(validPath);
			scanResult.Errors.Count.Should().Be(1);
		}

		[Test]
		public async Task GivenDirectoryExists_ShouldScanRootDirectory()
		{
			const string validPath = "path";

			MoqDirectory(validPath);

			await _agent.ScanFolderAsync(validPath);

			_directoryMock.Verify(x => x.EnumerateDirectories(validPath, "*", SearchOption.TopDirectoryOnly), Times.Once);
			_directoryMock.Verify(x => x.EnumerateFiles(validPath, "*", SearchOption.TopDirectoryOnly), Times.Once);
		}

		[Test]
		public async Task GivenDirectoryContainSubDirectories_ShouldGetDirectoryInfo()
		{
			const string validPath = "path";
			const string dir1 = "dir1";
			const string dir2 = "dir2";

			MoqDirectory(validPath);
			MoqDirectory(dir1);
			MoqDirectory(dir2);

			_directoryMock.Setup(x => x.EnumerateDirectories(validPath, "*", SearchOption.TopDirectoryOnly)).Returns(new List<string> { dir1, dir2 });

			var scanResult = await _agent.ScanFolderAsync(validPath);
			scanResult.TotalFolders.Should().Be(3);
			scanResult.TotalSize.Should().Be(0);
		}

		[Test]
		public async Task GivenDirectoryContainsFiles_ShouldGetFileInfo()
		{
			const string validPath = "path";
			const string file1 = "file1";
			const string file2 = "file2";

			MoqDirectory(validPath);
			MoqFile("file1", 2);
			MoqFile("file2", 1);

			_directoryMock.Setup(x => x.EnumerateFiles(validPath, "*", SearchOption.TopDirectoryOnly)).Returns(new List<string> { file1, file2 });

			var scanResult = await _agent.ScanFolderAsync(validPath);
			scanResult.TotalFiles.Should().Be(2);
			scanResult.TotalSize.Should().Be(3);
		}

		private void MoqFile(string fileName, long size)
		{
			var fileInfoMock = new Mock<IFileInfo>();
			fileInfoMock.Setup(x => x.Length).Returns(size);
			_fileInfoFactoryMock.Setup(x => x.Create(fileName)).Returns(fileInfoMock.Object);
		}

		private void MoqDirectory(string directoryName)
		{
			var directoryInfoMock = new Mock<IDirectoryInfo>();
			directoryInfoMock.Setup(x => x.Exists).Returns(true);
			_directoryInfoFactoryMock.Setup(x => x.Create(directoryName)).Returns(directoryInfoMock.Object);

		}
	}
}