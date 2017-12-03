using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SudokuSolver
{
	class Program
    {
        static void Main(String[] args)
        {
            var board = new Board();
            var boardLines = File.ReadAllLines("board.txt").ToList();
	        foreach (var row in Board.ValidCellIndexes)
				foreach (var column in Board.ValidCellIndexes)
					if (Char.IsNumber(boardLines[row][column]))
						board.Cells[row, column] = new Board.Cell((Byte) (boardLines[row][column] - '0'), isOriginal:true);

	        var backgroundColor = Console.BackgroundColor;
	        Console.BackgroundColor = Board.BackgroundColor;
	        var foregroundColor = Console.ForegroundColor;
	        Console.ForegroundColor = Board.ForegroundColor;

			Board.PrintGrid();
	        board.Print();
			Thread.Sleep(100);
			while (!TrySolve(board))
			{
				board.Print();
				Thread.Sleep(100);
			}
	        board.Print();
	        Console.ForegroundColor = foregroundColor;
	        Console.BackgroundColor = backgroundColor;
			Console.WriteLine();
		}

        private static Boolean TrySolve(Board board)
        {
			foreach (var row in Board.ValidCellIndexes)
			{
				foreach (var column in Board.ValidCellIndexes)
				{
					var cell = board.Cells[row, column];
					if (cell.HasAnswer)
						continue;

					var rowCells = Board.ValidCellIndexes.Select(x => board.Cells[row, x]);
					var columnCells = Board.ValidCellIndexes.Select(x => board.Cells[x, column]);
					var subGridCells = GetOtherSubGridCells(board, row, column);

				    var allRelatedCells = rowCells.Concat(columnCells).Concat(subGridCells);

				    var obviousImpossibilities = allRelatedCells.Where(x => x != cell)
					                                            .Where(x => x.HasAnswer)
					                                            .Select(x => x.Answer)
					                                            .Distinct();

					var obviousPossiblilities = Board.ValidCellValues.Except(obviousImpossibilities)
					                                           .ToArray();
					cell.Possibilities = obviousPossiblilities;
					if (cell.HasAnswer)
						goto @return;
				}
			}
			@return:
			return board.Cells.Cast<Board.Cell>().All(x => x.Possibilities.Length == 1);
		}

	    private static Board.Cell[] GetOtherSubGridCells(Board board, Int32 row, Int32 column)
	    {
		    var otherSubGridCells = new Board.Cell[Board.Size];
		    var i = 0;
		    var rowIndex = row / Board.SubgridSize * Board.SubgridSize;
		    var columnIndex = column / Board.SubgridSize * Board.SubgridSize;
			for (var subGridRow = rowIndex; subGridRow != rowIndex + 3; subGridRow++)
				for (var subGridColumn = columnIndex; subGridColumn != columnIndex + 3; subGridColumn++)
					otherSubGridCells[i++] = board.Cells[subGridRow, subGridColumn];
		    return otherSubGridCells;
	    }
	}
}
