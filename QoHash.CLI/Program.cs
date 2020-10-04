using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QoHash.Core;

namespace QoHash.CLI
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			Console.WriteLine("Please enter directory to scan: ");
			var path = Console.ReadLine();

			var agent = new ScanAgent();
			var scanResult = await  agent.ScanFolderAsync(path);
			Print(scanResult.Items);

			Console.WriteLine();
			Console.WriteLine($"Total Files : {scanResult.TotalFiles}");
			Console.WriteLine($"Total Size : {scanResult.TotalSize} bytes");

			PrintErrors(scanResult.Errors);
		}

		private static void Print(ItemInfo[] items)
		{
			foreach (var item in items)
				Console.WriteLine($"Name: {item.Name}, Size : {item.Size} bytes, LastWriteTime: {item.LastWriteTime}");
		}

		private static void PrintErrors(List<string> errors)
		{
			if (errors.Count > 0)
			{
				Console.WriteLine($"There were {errors.Count} errors while scanning disk");

				foreach (var error in errors)
					Console.WriteLine(error);
			}
		}
	}
}
