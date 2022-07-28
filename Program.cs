//Made by A.M aka EgoDev on 23/07/2022

using System;
using System.Collections.Generic;
using System.Linq;


namespace SudokuSolver
{
    internal class Program
    {
        static int[,] SudokuTable = new int[9, 9];


        public static void LogoShow()
        {
            //hard coded not ideal but I can't think of a algoritm that could generate this pattern
            List<string> logoDis = new List<string> () {
                "                                @                                \n",
                "                               @@@                               \n",
                "                              @@@@@                              \n",
                "                             @@@@@@@                             \n",
                "                            @@@@@@@@@                            \n",
                "                           @@@@@@@@@@@                           \n",
                "                          @@@@@@ @@@@@@                          \n",
                "                         @@@@@@   @@@@@@                         \n",
                "             @@@        @@@@@@     @@@@@@        @@@             \n",
                "            @@@@@@     @@@@@@       @@@@@@     @@@@@@            \n",
                "           @@@@@@@@@, @@@@@@         @@@@@@ @@@@@@@@@@           \n",
                "          @@@@@@@@@@@@@ @@@           @@@@@@ @@@@@@@@@@          \n",
                "         @@@@@@* @@@@@@@@              @@@@@@ @@ *@@@@@@         \n",
                "        @@@@@@     @@@@@@@@           @ @@@@@@     @@@@@@        \n",
                "       @@@@@@     @@ @@@@@@@@       @@@@ @@@@@@     @@@@@@       \n",
                "      @@@@@@     @@@@@ *@@@@@@@   @@@@@@@ @@@@@@     @@@@@@      \n",
                "     @@@@@@     @@@@@@    @@@@@@@@@@@@@    @@@@@@     @@@@@@     \n",
                "   .@@@@@@     @@@@@@       @@@@@@@@@       @@@@@@     @@@@@@    \n",
                "  /@@@@@@     @@@@@@          @@@@@          @@@@@@     @@@@@@.  \n",
                " &@@@@@@     @@@@@@             @             @@@@@@     @@@@@@( \n",
                "@@@@@@@     @@@@@@                             @@@@@@     @@@@@@&\n"};
            ConsoleColor[] colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));
            foreach (string item in logoDis)
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                int randnm = rand.Next(colors.GetLength(0));
                if (randnm != 0)
                {
                    Console.ForegroundColor = colors[randnm];
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write(item);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        //per linkare le cordiante a la loro entropia
        public struct CordEntropy
        {
            public CordEntropy(int x, int y, int[] entropy)
            {
                X = x;
                Y = y;
                ENTROPY = entropy; 
            }
            public int X { get; }
            public int Y { get; }
            public int[] ENTROPY { get; }
        }

        //tetermina la cordinata sinitra del quadtante data una x o una y
        private static int DetQuadrantBegin(int num)
        {
            if (num <= 2)
            {
                return 0;
            }
            else if (3 <= num && num <= 5)
            {
                return 3;
            }
            else
            {
                return 6;
            }
        }

        //orina i vari struct per la lungezza della loro entropia
        private static List<CordEntropy> StructSort(List<CordEntropy> arr)
        {
            CordEntropy support;
            for (int i = 0; i < arr.Count - 1; i++)
            {
                for (int j = 0; j < arr.Count - i - 1; j++)
                {
                    if (arr[j].ENTROPY.GetLength(0) > arr[j+1].ENTROPY.GetLength(0))
                    {
                        support = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = support;  
                    }
                }
            }
            return arr;
        } 

        //mostra il sudoku
        public static void Show()
        {
            for (int i = 0; i < SudokuTable.GetLength(0); i++)
            {
                for (int j = 0; j < SudokuTable.GetLength(0); j++)
                {
                    if (SudokuTable[i,j] != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(" " + SudokuTable[i, j] + " ");
                    //Console.WriteLine("i :" + i + " j :"+ j);
                }
                Console.WriteLine("\n");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        // carica il sudoku da il file di testo
        public static void LoadSudokutable()
        {
            int j = 0;
            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\InputSudoku.txt");
            string sFilePath = System.IO.Path.GetFullPath(sFile);
            string[] lines = System.IO.File.ReadAllLines(sFilePath);
            foreach (string line in lines)
            {
                j++;
                string[] num = line.Split(' ');
                for (int i = 0; i < SudokuTable.GetLength(0); i++)
                {
                    SudokuTable[j - 1, i] = (num[i] == "x") ? 0 : Int32.Parse(num[i]);
                }
            }
        }

        //calcola l'etropia singola 
        public static int[] Probability(int x, int y)
        {
            int[] entropy = {1, 2 ,3 ,4 ,5 ,6 ,7, 8, 9};
            //controllo per y
            for (int i = 0; i < SudokuTable.GetLength(0); i++)
            {
                if (SudokuTable[i,y] != 0)
                {
                    if (Array.Exists(entropy, ele => ele == SudokuTable[i,y]))
                    {
                        entropy = entropy.Where(val => val != SudokuTable[i,y]).ToArray();
                    }
                }
            }
            if (entropy.GetLength(0) != 0)
            {
                // controllo per x
                for (int i = 0; i < SudokuTable.GetLength(0); i++)
                {
                    if (SudokuTable[x, i] != 0)
                    {
                        if (Array.Exists(entropy, ele => ele == SudokuTable[x, i]))
                        {
                            entropy = entropy.Where(val => val != SudokuTable[x, i]).ToArray();
                        }
                    }
                }
                if (entropy.GetLength(0) != 0)
                {
                    x = DetQuadrantBegin(x);
                    y = DetQuadrantBegin(y);
                    for (int i = x; i < 3; i++)
                    {
                        for (int j = y; j < 3; j++)
                        {
                            if (SudokuTable[i, j] != 0)
                            {
                                if (Array.Exists(entropy, ele => ele == SudokuTable[i, j]))
                                {
                                    entropy = entropy.Where(val => val != SudokuTable[i, j]).ToArray();
                                }
                            }
                        }
                    }
                }
            }
            return entropy;
        }

        // calcola l'etropia e l'allega a delle cordinate restiuedno un array di entrambe
        public static CordEntropy[] CalculteEntropy()
        {
            List<CordEntropy> generalentropy = new List<CordEntropy> { };

            for (int i = 0; i < SudokuTable.GetLength(0); i++)
            {
                for (int j = 0; j < SudokuTable.GetLength(0); j++)
                {
                    if (SudokuTable[i, j] == 0)
                    {
                        CordEntropy toAdd = new CordEntropy(i, j, Probability(i, j));
                        generalentropy.Add(toAdd);
                    }
                }
            }
            generalentropy = StructSort(generalentropy);
            /*
            //debugging purpes
            foreach (CordEntropy item in generalentropy)
            {
                Console.WriteLine("y: " + item.X + " x: " + item.Y);
                for (int i = 0; i < item.ENTROPY.GetLength(0); i++)
                {
                    Console.Write(" " + item.ENTROPY[i] + " ");
                }
                Console.Write("\n\n");
            }
            */
            return generalentropy.ToArray();
        }
        //controlla se il sudoku è corretto
        public static bool isCorect()
        {
            List<int> numeri = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = 0; i < SudokuTable.GetLength(0); i++)
            {
                if (numeri.Contains(SudokuTable[0, i]))
                {
                    numeri.Remove(SudokuTable[0, i]);
                }
                else
                {
                    return false;
                }
            }
            numeri = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = 0; i < SudokuTable.GetLength(0); i++)
            {
                if (numeri.Contains(SudokuTable[0, i]))
                {
                    numeri.Remove(SudokuTable[0, i]);
                }
                else 
                {
                    return false; 
                }  
            }
            for (int i = 0; i < 6; i+=3)
            {
                for (int j = 0; j < 6; j+=3)
                {
                    numeri = new List<int>(){ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    for (int k = i; k < i + 3; k++)
                    {
                        for (int l = j; l < j + 3; l++)
                        {
                            if (numeri.Contains(SudokuTable[k, l]))
                            {
                                numeri.Remove(SudokuTable[k, l]);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;   
        }

        // si occupa di controllare che il sudoku sia possibile e ripete finche non è corretto
        static void SolveSudouk(bool ShowSud = false)
        {
            int countcicles = 0;
            CordEntropy placeholder = new CordEntropy(0, 0, new int[1]);
            int lastNumIndeciseve = 0;
            bool stuck;
            while (!isCorect())
            {
                Console.WriteLine("Tentativo N " + countcicles++ + "\n");
                CordEntropy[] allEntropy = { placeholder };
                stuck = true;
                LoadSudokutable();
                while (allEntropy.GetLength(0) >= 1 && stuck)
                {
                    allEntropy = CalculteEntropy();
                    foreach (CordEntropy item in allEntropy)
                    {
                        if (item.ENTROPY.GetLength(0) == 0)
                        {
                            stuck = false;
                        }
                        else
                        {
                            if (lastNumIndeciseve != allEntropy.GetLength(0))
                            {
                                if (item.ENTROPY.GetLength(0) < 2)
                                {
                                    SudokuTable[item.X, item.Y] = item.ENTROPY[0];
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                Random rand = new Random(Guid.NewGuid().GetHashCode());
                                int randnum = rand.Next(item.ENTROPY.GetLength(0));
                                SudokuTable[item.X, item.Y] = item.ENTROPY[randnum];
                                break;
                            }
                            if (ShowSud && countcicles < 8)
                            {
                                Show();
                                System.Threading.Thread.Sleep(10);
                                Console.Clear();
                            }
                        }
                    }
                    lastNumIndeciseve = allEntropy.GetLength(0);
                }
            }
        }

        static void Main(string[] args)
        {
            bool ready = false;
            LogoShow();

            Console.WriteLine("Benvenuto Utente, questo programma risolve sudoku :-D");
            while (!ready)
            {
                Console.Write("Selezionare un opzione: Help, Solve o Show (più lento del 500%) ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Attenzione Case sensitive!");
                Console.ForegroundColor = ConsoleColor.White;
                String input = Console.ReadLine();
                switch (input)
                {
                    case "Help":
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("il programma analizza un sudoku inserito all interno del file InputSudoku.txt " +
                                "seguedno un format che separa ogni numero e spazio vuoto con uno spazio, e segnala gli spazi vuoti del sudoku come x");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        }
                    case "Solve":
                        {
                            Console.Clear();
                            SolveSudouk();
                            ready = true;
                            break;
                        }
                    case "Show":
                        {
                            Console.Clear();
                            ready = true;
                            SolveSudouk(ready);
                            break;
                        }
                    default:
                        Console.WriteLine("Input non riconiosciuto");
                        break;
                }
            }
            SolveSudouk(true);
            Show();
            Console.WriteLine("il sudoku è risolto: " + isCorect());
            Console.ReadLine();
        }
    }
}
