using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _8_QueenProblem
{
    class Program
    {
        Random rando = new Random();
        queen[,] board = new queen[8, 8];
        List<queen> queenList = new List<queen>();
        int counter = 0;

        static void Main(string[] args)
        {
            Program myProgram = new Program();
            myProgram.Run();
        }
        public void Run()
        {
            MakeBoard();
            PrintBoard();
            Console.WriteLine(CheckBoard().ToString());
            Console.ReadKey();
        }
        public void MakeBoard()
        {
            int x;
            int y;
            while (counter < 8)
            {
                x = rando.Next(0, 8);
                Thread.Sleep(50);
                y = rando.Next(0, 8);
                if (board[x, y] == null)
                {
                    queen queen = new queen(x, y);
                    board[x, y] = queen;
                    queenList.Add(queen);
                    counter++;
                }
            }
        }
        public void PrintBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i,j] == null)
                    {
                        Console.Write(0 + " ");
                    }
                    else
                    {
                        Console.Write("+" + " ");
                    }
                }
                Console.WriteLine();
            }
        }
        public int CheckBoard()
        {
            int result = 0;
            foreach (var Q in queenList)
            {
                foreach (var otherQ in queenList)
                {
                    if (Q.x == otherQ.x || Q.y ==otherQ.y || (Q.x+Q.y) == (otherQ.x + otherQ.y) || (Q.x - Q.y) == (otherQ.x - otherQ.y))
                    {
                        result++;
                    }
                }
            }
            return (result -8) / 2;
        }
        public void DoMove()
        {
        }

    }
}
