// Idea from 
// https://github.com/jozefizso/SystemWrapper

using System.Collections.Generic;
using System.IO;

namespace QoHash.Core.IO
{
	public class DirectoryAdapter : IDirectory
	{
		public bool Exists(string path)
		{
			return Directory.Exists(path);
		}

		public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateDirectories(path, searchPattern, searchOption);
		}

		public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFiles(path, searchPattern, searchOption);
		}
	}
}