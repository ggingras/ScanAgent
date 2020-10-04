// Idea from 
// https://github.com/jozefizso/SystemWrapper

using System;
using System.IO;

namespace QoHash.Core.IO
{
	public class DirectoryInfoAdapter : IDirectoryInfo
	{
		private readonly DirectoryInfo _directoryInfo;

		public DirectoryInfoAdapter(string path)
		{
			_directoryInfo = new DirectoryInfo(path);
		}
		
		public bool Exists => _directoryInfo.Exists;

		public DateTime LastWriteTime
		{
			get => _directoryInfo.LastWriteTime;
			set => _directoryInfo.LastWriteTime = value;
		}

		public string Name => _directoryInfo.Name;
	}
}