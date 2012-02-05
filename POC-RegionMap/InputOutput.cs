using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class IO
{
	internal static StreamReader myInput = null;

	static IO()
	{
		myInput = File.OpenText("D:\\WORK\\git\\ICPC-Challenge\\POC-RegionMap\\input.txt");
	}

	static public string ReadLine()
	{
		string inStr = myInput.ReadLine();

		return inStr;
	}

	static public void Write(string inStr)
	{
		Console.Write(inStr);
	}

	static public void WriteLine(string inStr)
	{
		Console.WriteLine(inStr);
	}

	internal static void WriteLine()
	{
		Console.WriteLine();
	}

	internal static void ErrorWrite(string p)
	{
		Console.Error.Write(p);
	}

	internal static void ErrorWriteLine(string debug)
	{
		Console.Error.WriteLine(debug);
	}
}

