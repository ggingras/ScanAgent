using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace QoHash.Core
{
	public interface IScanAgent
	{
		Task<ScanResult> ScanFolderAsync([NotNull]string folder);
	}
}