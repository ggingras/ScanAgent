using System.Collections.Generic;

namespace QoHash.Web.Models
{
	public class ScanResult
	{
		public int TotalFolders;
		public long TotalSize;
		public int TotalFiles;
		public ItemInfo[] Items;
		public List<string> Errors;
	}
}