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
            return;

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
                    // TODO: solve the board here!
				}
			}
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
