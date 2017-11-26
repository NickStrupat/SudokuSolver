using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SudokuSolver
{
    class Program
    {
		private const Int32 BoardSize = 9;
		private static readonly Int32 SubgridSize = (Int32)Math.Sqrt(BoardSize);
		private static readonly Int32[] ValidCellValues = Enumerable.Range(1, BoardSize).ToArray();
		private static readonly Int32[] ValidCellIndexes = Enumerable.Range(0, BoardSize).ToArray();

	    private static void ArgumentRangeCheck(Int32 argument, String argumentName, Int32 fromInclusive, Int32 toInclusive)
		{
			if (argument < fromInclusive || argument > toInclusive)
				throw new ArgumentOutOfRangeException(argumentName, $"{argumentName} must be in the range [{fromInclusive}, {toInclusive}]. Value `{argument}` is outside that range.");
		}

		class Cell
		{
			public Int32[] Possibilities;

	        public Int32 Answer => Possibilities.Count(x => x != 0) == 1 ? Possibilities.Single(x => x != 0) : 0;

			public Cell()
			{
				Possibilities = ValidCellValues;
			}

	        public Cell(Int32 answer)
	        {
		        ArgumentRangeCheck(answer, nameof(answer), 1, 9);
				Possibilities = new[] { answer };
			}

			public override String ToString() => String.Join(", ", Possibilities);
		}

        class Board
        {
			public readonly Cell[,] Cells;

	        //public readonly Cell[] GetRow(Int32 row) => ;
	        //public readonly Cell[][] Columns;
	        //public readonly Cell[,][,] SubGrids;

			public Board()
			{
				Cells = new Cell[BoardSize, BoardSize];
				foreach (var row in ValidCellIndexes)
					foreach (var column in ValidCellIndexes)
						Cells[row, column] = new Cell();
			}
        }

        static void Main(string[] args)
        {
            var board = new Board();
            var boardLines = File.ReadAllLines("board.txt").ToList();
	        foreach (var row in ValidCellIndexes)
				foreach (var column in ValidCellIndexes)
					if (Char.IsNumber(boardLines[row][column]))
						board.Cells[row, column] = new Cell((Byte) (boardLines[row][column] - '0'));
            Print(board);
            Solve(board);
            Console.WriteLine();
            Print(board);
            
        }

        private static void Solve(Board board)
        {
			Boolean changed;
			while (board.Cells.Cast<Cell>().Any(x => x.Possibilities.Length != 1))
			//do
			{
				var unsolvedCells = board.Cells.Cast<Cell>().Where(x => x.Possibilities.Length != 1).ToArray();
				changed = false;
				foreach (var row in ValidCellIndexes)
				{
					foreach (var column in ValidCellIndexes)
					{
						var cell = board.Cells[row, column];
						if (cell.Answer != 0)
							continue;
						var otherRowCells = ValidCellIndexes.Where(x => x != row).Select(x => board.Cells[row, x]).ToArray();
						var otherColumnCells = ValidCellIndexes.Where(x => x != column).Select(x => board.Cells[row, column]).ToArray();
						var otherSubGridCells = GetOtherSubGridCells(board, row, column);
						var obviousImpossibilities = otherRowCells.Concat(otherColumnCells).Concat(otherSubGridCells).Where(x => x.Answer != 0).Distinct().ToArray();
						var inferredPossibilities = otherRowCells.Concat(otherColumnCells).Concat(otherSubGridCells).Select(x => x.Possibilities).SelectMany(x => x).Distinct();
						var obviousPossibleAnswers = ValidCellValues.Except(obviousImpossibilities.Select(x => x.Answer)).ToArray();
						var inferredPossibleAnswers = obviousPossibleAnswers.Concat(inferredPossibilities).Distinct().ToArray();
						cell.Possibilities = obviousPossibleAnswers;
					}
				}
			} //while (changed);
		}

	    private static Cell[] GetOtherSubGridCells(Board board, Int32 row, Int32 column)
	    {
		    var otherSubGridCells = new Cell[BoardSize - 1];
		    var i = 0;
		    var subgridRowIndex = row / SubgridSize * SubgridSize;
		    var subgridColumnIndex = column / SubgridSize * SubgridSize;
			for (var subGridRow = subgridRowIndex; subGridRow != subgridRowIndex + 3; subGridRow++)
				for (var subGridColumn = subgridColumnIndex; subGridColumn != subgridColumnIndex + 3; subGridColumn++)
					if (subGridRow != row || subGridColumn != column)
						otherSubGridCells[i++] = board.Cells[subGridRow, subGridColumn];
		    return otherSubGridCells;
	    }

	    //private static void CheckIfRowHas(Byte b, Byte?[,] board)
        //{
            
        //}

        private static void Print(Board board)
        {
   //         for (var i = 0; i != BoardSize; i++)
   //         {
   //             for (var j = 0; j != BoardSize; j++)
   //             {
   //                 var answer = board.Cells[i, j].Answer;
   //                 Console.Write(answer != 0 ? (Char) ('0' + answer) : '-');
   //                 Console.Write(j == 2 || j == 5 ? " | " : " ");
   //             }
   //             Console.WriteLine();
   //             if (i == 2 || i == 5)
   //                 Console.WriteLine("-------|-------|------");
			//}

			//Console.Clear();
	  //      Console.WriteLine();

	        PrintGrid();
			foreach (var row in ValidCellIndexes)
	        {
		        foreach (var column in ValidCellIndexes)
				{
					var cell = board.Cells[row, column];
					if (cell.Answer != 0)
						PrintBigNumber(cell.Answer, row, column);
					else
						PrintPossibilities(cell.Possibilities, row, column);
					//Console.Write(answer != 0 ? (Char)('0' + answer) : '-');
					//Console.Write(column == SubgridSize - 1 || column == SubgridSize * 2 - 1 ? " | " : " ");
				}
				//0x6a j ┘   ┐
				//0x6b k ┐   │
				//0x6c l ┌   │
				//0x6d m └
				//0x6e n ┼
				//0x71 q ─
				//0x74 t ├
				//0x75 u ┤
				//0x76 v ┴
				//0x77 w ┬
				//0x78 x │
			}
        }

	    const int RowMultiplier = 4;
	    const int ColumnMultiplier = 8;

		private static void PrintGrid()
		{
			var lines = new[]
			{
				"╔═══════╤═══════╤═══════╦═══════╤═══════╤═══════╦═══════╤═══════╤═══════╗",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"╟───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────╢",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"╟───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────╢",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"╟═══════╪═══════╪═══════╬═══════╪═══════╪═══════╬═══════╪═══════╪═══════╣",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"╟───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────╢",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"╟───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────╢",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"╟═══════╪═══════╪═══════╬═══════╪═══════╪═══════╬═══════╪═══════╪═══════╣",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"╟───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────╢",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"╟───────┼───────┼───────╫───────┼───────┼───────╫───────┼───────┼───────╢",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"║       │       │       ║       │       │       ║       │       │       ║",
				"╚═══════╧═══════╧═══════╩═══════╧═══════╧═══════╩═══════╧═══════╧═══════╝",
			};
			var foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach (var line in lines)
				Console.WriteLine(line);
			Console.ForegroundColor = foregroundColor;
		}

		private static void PrintPossibilities(Int32[] cellPossibilities, Int32 row, Int32 column)
		{
			var bigRow = row * RowMultiplier + 1;
			var bigColumn = column * ColumnMultiplier + 2;
			
			Console.SetCursorPosition(bigColumn, bigRow);
			for (var i = 0; i != SubgridSize; i++)
			{
				for (var j = 0; j != SubgridSize; j++)
				{
					Console.SetCursorPosition(bigColumn + j * 2, bigRow + i);
					Console.Write(cellPossibilities.Contains((i + 1) * (j + 1)) ? ((i + 1) * (j + 1)).ToString() : " ");
				}
			}
		}

	    private static void PrintBigNumber(Int32 number, Int32 row, Int32 column)
	    {
			if (!ValidCellValues.Contains(number))
				throw new ArgumentOutOfRangeException();

		    var bigRow = row * RowMultiplier + 1;
		    var bigColumn = column * ColumnMultiplier + 2;

		    var numbers = new[]
		    {
			    new []
			    {
					" ─┐ ",
					"  │ ",
					" ─┴─ "
				},
			    new []
			    {
					" ───┐",
					"┌───┘",
					"└─── "
				},
			    new []
			    {
					" ───┐",
					" ───┤",
					" ───┘",
			    },
			    new []
			    {
					"│   │",
					"└───┤",
					"    │",
			    },
			    new []
				{
					"┌─── ",
					"└───┐",
					" ───┘",
				},
			    new []
				{
					"┌─── ",
					"├───┐",
					"└───┘",
				},
			    new []
				{
					" ───┐",
					"    │",
					"    │",
				},
			    new []
				{
					"┌───┐",
					"├───┤",
					"└───┘",
				},
			    new []
				{
					"┌───┐",
					"└───┤",
					" ───┘",
				}
			};
			numbers = new[]
		    {
			    new []
			    {
					" ═╗ ",
					"  ║ ",
					" ═╩═ "
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
					"║   ║",
					"╚═══╣",
					"    ║",
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
					"    ║",
					"    ║",
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

			for (var i = 0; i != SubgridSize; i++)
			{
				Console.SetCursorPosition(bigColumn, bigRow + i);
				Console.WriteLine(numbers[number - 1][i]);
			}

	    }
	}
}
