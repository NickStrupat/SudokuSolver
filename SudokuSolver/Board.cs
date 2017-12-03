using System;
using System.Linq;

namespace SudokuSolver
{
	class Board
	{
		public const Int32 Size = 9;

		public static readonly Int32 SubgridSize = (Int32)Math.Sqrt(Size);
		public static readonly Int32[] ValidCellValues = Enumerable.Range(1, Size).ToArray();
		public static readonly Int32[] ValidCellIndexes = Enumerable.Range(0, Size).ToArray();

		public const ConsoleColor BackgroundColor = ConsoleColor.White;
		public const ConsoleColor ForegroundColor = ConsoleColor.Black;

		public class Cell
		{
			public const ConsoleColor OriginalAnswerForegroundColor = ConsoleColor.Black;
			public const ConsoleColor FoundAnswerForegroundColor = ConsoleColor.Red;
			public const ConsoleColor PossibilitiesForegroundColor = ConsoleColor.Black;

			public readonly Boolean IsOriginal;
			public Int32[] Possibilities;

			public Int32 Answer => Possibilities.Count(x => x != 0) == 1 ? Possibilities.Single(x => x != 0) : 0;
			public Boolean HasAnswer => Answer != 0;

			public Cell()
			{
				Possibilities = ValidCellValues;
			}

			public Cell(Int32 answer, Boolean isOriginal)
			{
				IsOriginal = isOriginal;
				Possibilities = new[] { answer };
			}

			public override String ToString() => String.Join(", ", Possibilities);
		}

		public readonly Cell[,] Cells;

		public Board()
		{
			Cells = new Cell[Size, Size];
			foreach (var row in ValidCellIndexes)
				foreach (var column in ValidCellIndexes)
					Cells[row, column] = new Cell();
		}

		public static void PrintGrid()
		{
			var lines = new[]
			{
				"┌───────┬───────┬───────╥───────┬───────┬───────╥───────┬───────┬───────┐",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"├───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────┤",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"├───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────┤",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"╞═══════╪═══════╪═══════╬═══════╪═══════╪═══════╬═══════╪═══════╪═══════╡",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"├───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────┤",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"├───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────┤",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"╞═══════╪═══════╪═══════╬═══════╪═══════╪═══════╬═══════╪═══════╪═══════╡",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"├───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────┤",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"├───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────┤",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"│       │       │       ║       │       │       ║       │       │       │",
				"└───────┴───────┴───────╨───────┴───────┴───────╨───────┴───────┴───────┘",
			};
			foreach (var line in lines)
				Console.WriteLine(line);
		}

	    const Int32 RowPrintMultiplier = 4;
	    const Int32 ColumnPrintMultiplier = 8;

	    public void Print()
	    {
		    foreach (var row in ValidCellIndexes)
		    {
			    foreach (var column in ValidCellIndexes)
			    {
				    var cell = Cells[row, column];
				    if (cell.HasAnswer)
					    PrintBigNumber(cell, row, column);
				    else
					    PrintPossibilities(cell.Possibilities, row, column);
			    }
			}

			// Local helper function
		    void PrintPossibilities(Int32[] cellPossibilities, Int32 row, Int32 column)
		    {
			    var bigRow = row * RowPrintMultiplier + 1;
			    var bigColumn = column * ColumnPrintMultiplier + 2;

			    Console.SetCursorPosition(bigColumn, bigRow);
			    var foregroundColor = Console.ForegroundColor;
			    Console.ForegroundColor = Cell.PossibilitiesForegroundColor;
			    var number = 1;
			    for (var i = 0; i != SubgridSize; i++)
			    {
				    for (var j = 0; j != SubgridSize; j++)
				    {
					    Console.SetCursorPosition(bigColumn + j * 2, bigRow + i);
					    Console.Write(cellPossibilities.Contains(number) ? number.ToString() : " ");
					    number++;
				    }
			    }
			    Console.ForegroundColor = foregroundColor;
		    }
		}

		private static void PrintBigNumber(Cell cell, Int32 row, Int32 column)
	    {
		    var bigRow = row * RowPrintMultiplier + 1;
		    var bigColumn = column * ColumnPrintMultiplier + 2;

		    var numbers = new[]
		    {
			    new []
			    {
				    "  ╖  ",
				    "  ║  ",
				    "  ╨  "
			    },
			    new []
			    {
				    " ═══╗",
				    "╔═══╝",
				    "╚═══ ",
			    },
			    new []
			    {
				    " ═══╗",
				    " ═══╣",
				    " ═══╝",
			    },
			    new []
			    {
				    "╥   ╥",
				    "╚═══╣",
				    "    ╨",
			    },
			    new []
			    {
				    "╔═══ ",
				    "╚═══╗",
				    " ═══╝",
			    },
			    new []
			    {
				    "╔═══ ",
				    "╠═══╗",
				    "╚═══╝",
			    },
			    new []
			    {
				    "╔═══╗",
				    "    ╫",
				    "    ╨",
			    },
			    new []
			    {
				    "╔═══╗",
				    "╠═══╣",
				    "╚═══╝",
			    },
			    new []
			    {
				    "╔═══╗",
				    "╚═══╣",
				    " ═══╝",
			    }
		    };

		    var foregroundColor = Console.ForegroundColor;
		    Console.ForegroundColor = cell.IsOriginal ? Cell.OriginalAnswerForegroundColor : Cell.FoundAnswerForegroundColor;
		    for (var i = 0; i != SubgridSize; i++)
		    {
			    Console.SetCursorPosition(bigColumn, bigRow + i);
			    Console.WriteLine(numbers[cell.Answer - 1][i]);
		    }
		    Console.ForegroundColor = foregroundColor;
	    }
	}
}
