using System;
using System.IO;

namespace HashPassDB
{
	static class HashSteps
	{
		const string path = @"C:\HashPassDB\HashSteps.txt";
		
		public static void ClearSteps()
		{
			StreamWriter sw = File.CreateText(path);
			sw.Close();
		}
		
		public static void WriteStep(string line)
		{
			using(StreamWriter sw = File.AppendText(path))
			{
				sw.WriteLine(line);
				sw.Close();
			}
		}
	}
}
