using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

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
			public Boolean HasAnswer => Answer != 0;

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
			while (!TrySolve(board))
			{
				Print(board);
				Thread.Sleep(100);
			}

			Print(board);
        }

        private static Boolean TrySolve(Board board)
        {
			foreach (var row in ValidCellIndexes)
			{
				foreach (var column in ValidCellIndexes)
				{
					var cell = board.Cells[row, column];
					if (cell.HasAnswer)
						continue;
					if (row == 6 && column == 0)
						;
					var otherRowCells = ValidCellIndexes.Select(x => board.Cells[row, x]).Where(x => x != cell).ToArray();
					var otherColumnCells = ValidCellIndexes.Select(x => board.Cells[x, column]).Where(x => x != cell).ToArray();
					var otherSubGridCells = GetOtherSubGridCells(board, row, column);
					var obviousImpossibilities = otherRowCells.Concat(otherColumnCells)
						                                        .Concat(otherSubGridCells)
						                                        .Where(x => x.HasAnswer)
						                                        .Select(x => x.Answer)
						                                        .Distinct()
						                                        .ToArray();
					//var inferredPossibilities = otherRowCells.Concat(otherColumnCells).Concat(otherSubGridCells).Select(x => x.Possibilities).SelectMany(x => x).Distinct();
					var obviousPossibleAnswers = ValidCellValues.Except(obviousImpossibilities).ToArray();
					//var inferredPossibleAnswers = obviousPossibleAnswers.Concat(inferredPossibilities).Distinct().ToArray();
					cell.Possibilities = obviousPossibleAnswers;
					if (cell.HasAnswer)
						goto @return;
				}
			}
			@return:
			return board.Cells.Cast<Cell>().All(x => x.Possibilities.Length == 1);
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

			Console.Clear();
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
