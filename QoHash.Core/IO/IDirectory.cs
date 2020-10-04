// Idea from 
// https://github.com/jozefizso/SystemWrapper

using System.Collections.Generic;
using System.IO;

namespace QoHash.Core.IO
{
	/// <summary>
	/// Subset Wrapper for <see cref="System.IO.Directory" /> class.
	/// </summary>
	public interface IDirectory
	{
		bool Exists(string path);
		
		IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);

		IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
	}
}