// Idea from 
// https://github.com/jozefizso/SystemWrapper

using System;
using System.IO;

namespace QoHash.Core.IO
{
	public class FileInfoAdapter : IFileInfo
	{
		private readonly FileInfo _fileInfoInstance;

		public FileInfoAdapter(string fileName)
		{
			_fileInfoInstance = new FileInfo(fileName);
		}

		public bool Exists => _fileInfoInstance.Exists;

		public DateTime LastWriteTime
		{
			get => _fileInfoInstance.LastWriteTime;
			set => _fileInfoInstance.LastWriteTime = value;
		}

		public long Length => _fileInfoInstance.Length;

		public string Name => _fileInfoInstance.Name;
	}
}