// Idea from 
// https://github.com/jozefizso/SystemWrapper

namespace QoHash.Core.IO
{
	public interface IDirectoryInfoFactory
	{
		IDirectoryInfo Create(string directoryPath);
	}
}