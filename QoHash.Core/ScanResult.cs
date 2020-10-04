using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace QoHash.Core
{
	public class ScanResult
	{
		private readonly SortedDictionary<long, ConcurrentQueue<ItemInfo>> _items;
		public List<string> Errors { get; }
		public int TotalFolders;
		public long TotalSize;
		public int TotalFiles;

		public ScanResult()
		{
			Errors = new List<string>();
			_items = new SortedDictionary<long, ConcurrentQueue<ItemInfo>>();
		}

		public void Add(ItemInfo itemInfo)
		{
			//double-checked locking optimization
			if (!_items.ContainsKey(itemInfo.Size))
			{
				//Making SortedDictionary thread-safe
				lock (_items)
				{
					if (!_items.ContainsKey(itemInfo.Size))
						_items.Add(itemInfo.Size, new ConcurrentQueue<ItemInfo>());
				}
			}

			_items[itemInfo.Size].Enqueue(itemInfo);
		}

		public  ItemInfo[] Items
		{
			get
			{
				return _items.SelectMany(x => x.Value).ToArray();
			}
		}
	}
}