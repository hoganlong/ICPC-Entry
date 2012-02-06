using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class IO
{
	internal static StreamWriter inputEcho=null;
	internal static StreamWriter outputEcho=null;

	static IO()
	{
	//	inputEcho = File.CreateText("input.txt");

	//	outputEcho = File.CreateText("output.txt");
	}

	static public string ReadLine()
	{
		string inStr = Console.ReadLine();

		if (inputEcho != null) inputEcho.WriteLine(inStr);

		return inStr;
	}

	static public void Write(string inStr)
	{
		if (outputEcho != null) outputEcho.Write(inStr);

		Console.Write(inStr);
	}

	static public void WriteLine(string inStr)
	{
		if (outputEcho != null) outputEcho.WriteLine(inStr);

		Console.WriteLine(inStr);
	}


	internal static void WriteLine()
	{
		if (outputEcho != null) outputEcho.WriteLine();

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

