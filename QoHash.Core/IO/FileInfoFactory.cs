// Idea from 
// https://github.com/jozefizso/SystemWrapper

namespace QoHash.Core.IO
{
	public class FileInfoFactory : IFileInfoFactory
	{
		public IFileInfo Create(string fileName)
		{
			return new FileInfoAdapter(fileName);
		}
	}
}
