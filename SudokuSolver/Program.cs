using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace SudokuSolver
{
    class Program
    {
        class Cell
        {
            public Byte Answer;
            public Byte[] Possibilities;

            public Cell(byte answer = 0) => Answer = answer;

            //private readonly BitVector32 possibilities;

            //public IEnumerable<Byte> Possibilities => Enumerable.Range(0, 8).Select(x => possibilities[x]).Select(((b, i) => b ? i : -1)).Where(x => x != -1).Cast<Byte>();
        }

        struct Board
        {
            public readonly Cell[,] Cells;

            public Board(Cell[,] array)
            {
                if (array.GetLength(0) != 9 || array.GetLength(1) != 9 || array.Cast<Cell>().Any(x => x == null))
                    throw new Exception();
                Cells = array;
            }
        }

        static void Main(string[] args)
        {
            var cells = new Cell[9, 9];
            for (var i = 0; i < cells.GetLength(0); i++)
                for (var j = 0; j < cells.GetLength(1); j++)
                    cells[i,j] = new Cell();
            var board = new Board(cells);
            var boardLines = File.ReadAllLines("board.txt").ToList();
            for (var i = 0; i != 9; i++)
                for (var j = 0; j != 9; j++)
                    if (Char.IsNumber(boardLines[i][j]))
                        board.Cells[i, j] = new Cell((Byte) (boardLines[i][j] - '0'));
            Print(board);
            Solve(board);
            Console.WriteLine();
            Print(board);
            
        }

        private static void Solve(Board board)
        {
            Boolean changed;
            do
            {
                changed = false;
                for (var i = 0; i != 9; i++)
                {
                    for (var j = 0; j != 9; j++)
                    {
                        var cell = board.Cells[i, j];
                        //if (/*cell.Answer == 0 && */cell.Possibilities == null)
                        {
                            // this cell hasn't been evaluated for possibilities yet
                            var otherRowCells = Enumerable.Range(0, 9).Where(x => x != i).Select(x => board.Cells[j, x]).ToArray();
                            var otherColumnCells = Enumerable.Range(0, 9).Where(x => x != j).Select(x => board.Cells[x, i]).ToArray();
                            var otherSubGridCells = new Cell[8];
                            var index = 0;
                            for (var i2 = i / 3 * 3; i2 != i / 3 * 3 + 3; i2++)
                                for (var j2 = j / 3 * 3; j2 != j / 3 * 3 + 3; j2++)
                                    if (i2 != i || j2 != j)
                                    {
                                        //Console.WriteLine(i2 + " " + j2);
                                        otherSubGridCells[index++] = board.Cells[i2, j2];
                                    }
                            var obviousImpossibilities = otherRowCells.Concat(otherColumnCells).Concat(otherSubGridCells).Where(x => x.Answer != 0).Distinct().ToArray();
                            var inferredPossibilities = otherRowCells.Concat(otherColumnCells).Concat(otherSubGridCells).Select(x => x.Possibilities).SelectMany(x => x ?? Array.Empty<byte>()).Distinct().ToArray();
                            var obviousPossibleAnswers = Enumerable.Range(1, 9).Select(x => (byte)x).Except(obviousImpossibilities.Select(x => x.Answer)).ToArray();
                            var inferredPossibleAnswers = obviousPossibleAnswers.Concat(inferredPossibilities).Distinct().ToArray();
                            if (obviousPossibleAnswers.Length == 1)
                            {
                                changed = true;
                                cell.Answer = obviousPossibleAnswers.Single();
                                cell.Possibilities = Array.Empty<Byte>();
                            }
                            else
                            {
                                cell.Possibilities = obviousPossibleAnswers;
                            }
                        }
                    }
                }
            } while (changed);
        }

        //private static void CheckIfRowHas(Byte b, Byte?[,] board)
        //{
            
        //}

        private static void Print(Board board)
        {
            for (var i = 0; i != 9; i++)
            {
                for (var j = 0; j != 9; j++)
                {
                    var answer = board.Cells[i, j].Answer;
                    Console.Write(answer != 0 ? (Char) ('0' + answer) : '-');
                    Console.Write(j == 2 || j == 5 ? " | " : " ");
                }
                Console.WriteLine();
                if (i == 2 || i == 5)
                    Console.WriteLine("------|-------|------");
            }
        }
    }
}
