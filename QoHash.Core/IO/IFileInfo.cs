// Idea from 
// https://github.com/jozefizso/SystemWrapper

namespace QoHash.Core.IO
{
	/// <summary>
	/// Subset Wrapper for  <see cref="T:System.IO.FileInfo" />  class.
	/// </summary>
	public interface IFileInfo : ISystemInfo
	{
		long Length { get;  }
	}
}