using System;
using System.CodeDom;
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
						board.Cells[row, column] = new Cell((Byte) (boardLines[row][column] - '0'), isOriginal:true);
	        var backgroundColor = Console.BackgroundColor;
	        Console.BackgroundColor = ConsoleColor.White;
	        var foregroundColor = Console.ForegroundColor;
	        Console.ForegroundColor = ConsoleColor.Black;
			PrintGrid();
			Print(board);
			while (!TrySolve(board))
			{
				Print(board);
				Thread.Sleep(100);
			}
			Print(board);
	        Console.ForegroundColor = foregroundColor;
	        Console.BackgroundColor = backgroundColor;
			Console.WriteLine();
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

					var rowCells = ValidCellIndexes.Select(x => board.Cells[row, x]);
					var columnCells = ValidCellIndexes.Select(x => board.Cells[x, column]);
					var subGridCells = GetOtherSubGridCells(board, row, column);

				    var allRelatedCells = rowCells.Concat(columnCells).Concat(subGridCells);

				    var obviousImpossibilities = allRelatedCells.Where(x => x != cell)
					                                            .Where(x => x.HasAnswer)
					                                            .Select(x => x.Answer)
					                                            .Distinct();

					var obviousPossiblilities = ValidCellValues.Except(obviousImpossibilities)
					                                           .ToArray();
					cell.Possibilities = obviousPossiblilities;
					if (cell.HasAnswer)
						goto @return;
				}
			}
			@return:
			return board.Cells.Cast<Cell>().All(x => x.Possibilities.Length == 1);
		}

	    private static Cell[] GetOtherSubGridCells(Board board, Int32 row, Int32 column)
	    {
		    var otherSubGridCells = new Cell[BoardSize];
		    var i = 0;
		    var rowIndex = row / SubgridSize * SubgridSize;
		    var columnIndex = column / SubgridSize * SubgridSize;
			for (var subGridRow = rowIndex; subGridRow != rowIndex + 3; subGridRow++)
				for (var subGridColumn = columnIndex; subGridColumn != columnIndex + 3; subGridColumn++)
					otherSubGridCells[i++] = board.Cells[subGridRow, subGridColumn];
		    return otherSubGridCells;
	    }

        private static void Print(Board board)
		{
			foreach (var row in ValidCellIndexes)
	        {
		        foreach (var column in ValidCellIndexes)
				{
					var cell = board.Cells[row, column];
				    if (cell.HasAnswer)
				        PrintBigNumber(cell, row, column);
				    else
				        PrintPossibilities(cell.Possibilities, row, column);
				}
			}
		}

	    const int RowMultiplier = 4;
	    const int ColumnMultiplier = 8;

		private static void PrintGrid()
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

		private static void PrintPossibilities(Int32[] cellPossibilities, Int32 row, Int32 column)
		{
			var bigRow = row * RowMultiplier + 1;
			var bigColumn = column * ColumnMultiplier + 2;
			
			Console.SetCursorPosition(bigColumn, bigRow);
			var foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Black;
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

		private static void PrintBigNumber(Cell cell, Int32 row, Int32 column)
	    {
		    var bigRow = row * RowMultiplier + 1;
		    var bigColumn = column * ColumnMultiplier + 2;
			
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
		    Console.ForegroundColor = cell.IsOriginal ? ConsoleColor.Black : ConsoleColor.Red;
			for (var i = 0; i != SubgridSize; i++)
			{
				Console.SetCursorPosition(bigColumn, bigRow + i);
				Console.WriteLine(numbers[cell.Answer - 1][i]);
			}
		    Console.ForegroundColor = foregroundColor;
		}
	}
}
