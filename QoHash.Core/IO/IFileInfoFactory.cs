// Idea from 
// https://github.com/jozefizso/SystemWrapper

namespace QoHash.Core.IO
{
	public interface IFileInfoFactory
	{
		IFileInfo Create(string fileName);
	}
}