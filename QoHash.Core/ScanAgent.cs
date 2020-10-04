using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using QoHash.Core.IO;

namespace QoHash.Core
{
	public class ScanAgent : IScanAgent
	{
		private readonly IDirectory _directory;
		private readonly IDirectoryInfoFactory _directoryInfoFactory;
		private readonly IFileInfoFactory _fileInfoFactory;

		public ScanAgent()
			:this(new DirectoryAdapter(), new DirectoryInfoFactory(), new FileInfoFactory())
		{ }

		public ScanAgent(IDirectory directory, IDirectoryInfoFactory directoryInfoFactory, IFileInfoFactory fileInfoFactory)
		{
			_fileInfoFactory = fileInfoFactory;
			_directoryInfoFactory = directoryInfoFactory;
			_directory = directory;
		}

		public async Task<ScanResult> ScanFolderAsync([NotNull]string folder)
		{
			var directoryInfo = _directoryInfoFactory.Create(folder);
			if (directoryInfo.Exists)
			{
				return await Task.Factory.StartNew(() =>
				{
					var scanResult = new ScanResult();
					ProcessFolder(scanResult, folder);
					return scanResult;
				});
			}
			else
				throw new ArgumentException("Path does not exists");
		}

		private long ProcessFolder(ScanResult scanResult, string folder)
		{
			long folderSize = 0;
			try
			{
				//Parallel execution of scanning sub folders
				Parallel.ForEach(_directory.EnumerateDirectories(folder, "*", SearchOption.TopDirectoryOnly),
					new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
					s => Interlocked.Add(ref folderSize, ProcessFolder(scanResult, s)));

				//Parallel execution of scanning files
				Parallel.ForEach(_directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly),
					new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
					(s) => Interlocked.Add(ref folderSize, ProcessFile(scanResult, s)));

				AddInfo(scanResult, _directoryInfoFactory.Create(folder), folderSize);

				//Making addition thread-safe
				Interlocked.Add(ref scanResult.TotalFolders, 1);
			}
			catch (Exception ex)
			{
				scanResult.Errors.Add($"There was an {ex.GetType()} exception when scanning {folder}.");
			}

			return folderSize;
		}
		
		private long ProcessFile(ScanResult scanResult, string file)
		{
			try
			{
				var fileInfo = _fileInfoFactory.Create(file);
				AddInfo(scanResult, fileInfo, fileInfo.Length);

				//Making addition thread-safe
				Interlocked.Add(ref scanResult.TotalFiles, 1);
				Interlocked.Add(ref scanResult.TotalSize, fileInfo.Length);

				return fileInfo.Length;
			}
			catch (Exception ex)
			{
				scanResult.Errors.Add($"There was an {ex.GetType()} exception when scanning {file}.");
			}

			return 0;
		}

		private void AddInfo(ScanResult scanResult, ISystemInfo systemInfo, long size)
		{
			var info = new ItemInfo
			{
				Name = systemInfo.Name,
				LastWriteTime = systemInfo.LastWriteTime,
				Size = size,
			};

			scanResult.Add(info);
		}
	}
}
