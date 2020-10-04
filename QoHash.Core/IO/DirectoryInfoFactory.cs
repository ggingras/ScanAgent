// Idea from 
// https://github.com/jozefizso/SystemWrapper

namespace QoHash.Core.IO
{
	public class DirectoryInfoFactory : IDirectoryInfoFactory
	{
		public IDirectoryInfo Create(string directoryPath)
		{
			return new DirectoryInfoAdapter(directoryPath);
		}
	}
}