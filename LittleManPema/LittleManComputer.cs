using System;
using System.IO;
using System.Collections.Generic;

namespace LittleManPema
{
	public class LittleManComputer
	{
		public int ProgramCounter { get; set; }
		public int Accumulator { get; set; }
		public int[] RAM { get; set; }

		public const int RamSize = 100;
		public const int MaxValue = 999;

		//Keyword, Opcode, NeedsInput
		public Dictionary<string, Tuple<int, bool>> Opcodes { get; set; }

		public LittleManComputer()
		{
			Opcodes = new Dictionary<string, Tuple<int, bool>>();
			Opcodes.Add("ADD", Tuple.Create(1, true));
			Opcodes.Add("SUB", Tuple.Create(2, true));
			Opcodes.Add("STA", Tuple.Create(3, true));
			Opcodes.Add("LDA", Tuple.Create(5, true));
			Opcodes.Add("BRA", Tuple.Create(6, true));
			Opcodes.Add("BRZ", Tuple.Create(7, true));
			Opcodes.Add("BRP", Tuple.Create(8, true));

			Opcodes.Add("INP", Tuple.Create(901, false));
			Opcodes.Add("OUT", Tuple.Create(902, false));
			Opcodes.Add("HLT", Tuple.Create(0, false));
			Opcodes.Add("COB", Tuple.Create(0, false));

			Reset();
		}

		public void LoadFile(string Path)
		{
			AssembleToRAM(File.ReadAllLines(Path));
		}

		public void AssembleToRAM(string[] Program)
		{
			int LineNum = 0;
			foreach (string Line in Program)
			{
				string[] Args = Line.Split(' ');

				if (Args.Length > 2 || Args.Length <= 0)
					throw new Exception("Incorrect number of arguements on line " + LineNum);

				if (Opcodes.ContainsKey(Args[0]))
				{
					if (Opcodes[Args[0]].Item2)
					{
						if (Args.Length != 2)
							throw new Exception("Incorrect number of arguements on line " + LineNum);

						RAM[LineNum] = int.Parse(Opcodes[Args[0]].Item1.ToString() + Args[1]);
					}
					else
					{
						if (Args.Length > 1)
							throw new Exception("Operation on line " + LineNum + " does not take any parameters");

						RAM[LineNum] = Opcodes[Args[0]].Item1;
					}
					
				}
				else if (Args[0] == "DAT")
				{
					if (Args.Length != 2)
						throw new Exception("Incorrect number of arguements on line " + LineNum);

					RAM[LineNum] = int.Parse(Args[1]);
				}
				else
					throw new Exception("Unknown keyword on line " + LineNum);

				LineNum++;
			}
		}

		public void Execute()
		{
			ProgramCounter = 0;
			Accumulator = 0;

			while (RAM[ProgramCounter] != 0)
			{
				DoStep();
			}
		}
			
		public void DoStep()
		{
			string Instruction = RAM[ProgramCounter].ToString();

			//Check if current instruction is valid
			if (Instruction.Length == 3)
			{
				switch (Instruction.Substring(0, 1))
				{
					//ADD
					case "1":
						Accumulator += RAM[int.Parse(Instruction.Substring(1))];
						break;

					//SUB
					case "2":
						Accumulator -= RAM[int.Parse(Instruction.Substring(1))];
						break;

					//STA
					case "3":
						RAM[int.Parse(Instruction.Substring(1))] = Accumulator;
						break;

					//LDA
					case "5":
						Accumulator = RAM[int.Parse(Instruction.Substring(1))];
						break;

					//BRA
					case "6":
						ProgramCounter = int.Parse(Instruction.Substring(1))-1;
						break;

					//BRZ
					case "7":
						if (Accumulator == 0)
							ProgramCounter = int.Parse(Instruction.Substring(1))-1;
						break;

					//BRZ
					case "8":
						if (Accumulator >= 0)
							ProgramCounter = int.Parse(Instruction.Substring(1))-1;
						break;

					case "9":
						if (Instruction.Substring(1) == "01")
						{
							Accumulator = int.Parse(Console.ReadLine());
						}
						else if (Instruction.Substring(1) == "02")
						{
							Console.WriteLine(Accumulator);
						}
						break;

					default:
						throw new Exception("Unknown opcode");
						break;
				}
			}
			WrapCurrent();
			ProgramCounter++;
		}

		public void WrapCurrent()
		{
			if (RAM[ProgramCounter] > MaxValue)
				RAM[ProgramCounter] = (-MaxValue) + (RAM[ProgramCounter] - (MaxValue+1));
			else if (RAM[ProgramCounter] < -MaxValue)
				RAM[ProgramCounter] = MaxValue - ((-MaxValue+1) - RAM[ProgramCounter]);
		}

		public void Reset()
		{
			ProgramCounter = 0;
			Accumulator = 0;
			RAM = new int[100];
		}
	}
}

