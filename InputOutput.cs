using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Runtime.InteropServices;

static class IO
{
	internal static StreamWriter inputEcho=null;
	internal static StreamWriter outputEcho=null;
	private static bool errOut = false;

	static IO()
	{
	//	inputEcho = File.CreateText("input.txt");

	//	outputEcho = File.CreateText("output.txt");
		errOut = true;
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
		if (errOut)
		{
			Console.Error.Write(p);
		}
	}

	internal static void ErrorWriteLine(string debug)
	{
		if (errOut)
		{
			Console.Error.WriteLine(debug);
		}
	}

	internal static void Wait4Keypress()
	{
		bool done = false;

		IntPtr stdin = GetStdHandle(Constants.StdError);
		uint eventsRead;
		InputRecord[] records = new InputRecord[1];

		if (stdin == Constants.InvalidHandle)
			throw new Exception("Unable to aquire the standard input handle.");																	

		while (!done)
		{
			GetNumberOfConsoleInputEvents(stdin, out eventsRead);
			if (eventsRead > 0)
			{
				IO.ErrorWriteLine("events"+eventsRead.ToString());
				if (PeekConsoleInput(stdin, records, (uint)records.Length, out eventsRead))
				{
					// Check for a keyborad event
					if (records[0].eventType == EventTypes.MouseEvent)
						break;
			//		else
						// consume non keybord event;
					//	ReadConsoleInput(stdin, records, (uint)records.Length, out eventsRead);
				}
			}
			else
			{
				Thread.Sleep(100);
		
			}
		}
	}

	internal class Constants
	{
		internal const int StdInput = -10;
		internal const int StdOutput = -11;
		internal const int StdError = -12;

		internal static readonly IntPtr InvalidHandle = new IntPtr(-1);
	}

	internal struct COORD
	{
		internal short X;
		internal short Y;
	}

	internal enum EventTypes : ushort
	{
		KeyEvent = 0x01,
		MouseEvent = 0x02,
		WindowBufSizeEvent = 0x04,
		MenuEvent = 0x08,
		FocusEvent = 0x10
	}

	internal struct WindowBufSize
	{
		internal COORD size;
	}

	internal struct MenuEventREcord
	{
		internal uint cmdId;
	}

	internal struct FocusEventRecord
	{
		internal bool setFoucus;
	}
	   
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct KeyEventRecord
	{
		internal bool keyDown;
		internal ushort repeatCount;
		internal ushort virtualKeyCode;
		internal ushort virtualScanCode;
		internal char unicodeChar;
		internal uint controlKeyState;
	}

	internal struct MouseEventRecord
	{
		internal COORD mousePosition;
		internal uint bottonState;
		internal uint controlKeyState;
		internal uint eventFlags;
	}
	internal struct InputRecord
	{
		internal EventTypes eventType;
		internal KeyEventRecord keyEvent;
		internal MouseEventRecord mouseEvent;
		internal WindowBufSize bufSize;
		internal MenuEventREcord menuEvent;
		internal FocusEventRecord focusEvent;
	}

	[DllImport("kernel32.dll", SetLastError = true)] private static extern bool GetNumberOfConsoleInputEvents(IntPtr consoleHandle, out uint numOfEvents);

	[DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr GetStdHandle(int stdHandle);

	[DllImport("kernel32.dll", SetLastError = true)] private static extern bool PeekConsoleInput(IntPtr consoleHandle, [Out] InputRecord[] buffer, uint bufLength, out uint numOfEvents);

	//[DllImport("kernel32.dll", SetLastError = true)]
	//private static extern bool ReadConsoleInput(IntPtr consoleHandle,
	//    [Out] InputRecord[] buffer, uint bufLength, out uint numOfEvents);
}


/*
  using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace ConsoleWithTimeout {
	internal struct COORD {
		internal short X;
		internal short Y;
	}

	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
	internal struct KeyEventRecord {
		internal bool keyDown;
		internal ushort repeatCount;
		internal ushort virtualKeyCode;
		internal ushort virtualScanCode;
		internal char unicodeChar;
		internal uint controlKeyState;
	}

	internal struct MouseEventRecord {
		internal COORD mousePosition;
		internal uint bottonState;
		internal uint controlKeyState;
		internal uint eventFlags;
	}

	internal struct WindowBufSize {
		internal COORD size;
	}

	internal struct MenuEventREcord {
		internal uint cmdId;
	}

	internal struct FocusEventRecord {
		internal bool setFoucus;
	}

	internal struct InputRecord {
		internal EventTypes eventType;
		internal KeyEventRecord keyEvent;
		internal MouseEventRecord mouseEvent;
		internal WindowBufSize bufSize;
		internal MenuEventREcord menuEvent;
		internal FocusEventRecord focusEvent;
	}



	
	class Program {
		private static bool timedOut;

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(int stdHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool PeekConsoleInput(IntPtr consoleHandle,
			[Out] InputRecord[] buffer, uint bufLength, out uint numOfEvents);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool ReadConsoleInput(IntPtr consoleHandle,
			[Out] InputRecord[] buffer, uint bufLength, out uint numOfEvents);

		
		static void Main(string[] args) {

	

			InputRecord[] records = new InputRecord[1];
			uint eventsRead = 0;
			timedOut = false;

			TimerCallback callBack = new TimerCallback(SetTimeOut);
			Timer timer = new Timer(callBack, null, 5000, System.Threading.Timeout.Infinite);

			while (!timedOut) {
				// Get number of events pending
				GetNumberOfConsoleInputEvents(stdin, out eventsRead);
				if (eventsRead > 0) {
					if (PeekConsoleInput(stdin, records, (uint)records.Length, out eventsRead)) {
						// Check for a keyborad event
						if (records[0].eventType == EventTypes.KeyEvent &&
							records[0].keyEvent.keyDown)
							break;
						else
							// consume non keybord event;
							ReadConsoleInput(stdin, records, (uint)records.Length, out eventsRead);
					}
				}    else {
					Thread.Sleep(100);
					Console.WriteLine("Waiting");
				}
			}

			if (timedOut)
				Console.WriteLine("No console input after 5 sec");
			else {
				string input = Console.ReadLine();
				Console.WriteLine("You entered: " + input);
			}
			timer.Dispose();
			Console.WriteLine("Press Enter to exit");
			Console.ReadKey();
		}

		private static void SetTimeOut(object data) {
			timedOut = true;
		}
	}
}






*/