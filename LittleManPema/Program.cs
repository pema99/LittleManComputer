using System;

namespace LittleManPema
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			LittleManComputer LMC = new LittleManComputer();
			LMC.LoadFile("test.lmc");
			LMC.Execute();
		}
	}
}
