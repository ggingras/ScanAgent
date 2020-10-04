// Idea from 
// https://github.com/jozefizso/SystemWrapper

using System;

namespace QoHash.Core.IO
{
	public interface ISystemInfo
	{
		bool Exists { get; }

		DateTime LastWriteTime { get; set; }

		string Name { get; }
	}
}