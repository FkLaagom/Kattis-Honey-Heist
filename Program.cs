using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace hexagon
{
    public struct Hex
    {
        public Hex(int cellIndex, int x, int y, int z, bool isBlocked)
        {
            CellIndex = cellIndex;
            X = x;
            Y = y;
            Z = z;
            IsBlocked = isBlocked;
        }
        public Hex(int x, int y, int z)
        {
            CellIndex = 0;
            X = x;
            Y = y;
            Z = z;
            IsBlocked = false;
        }

        public int CellIndex;
        public int X;
        public int Y;
        public int Z;
        public bool IsBlocked;

        public override int GetHashCode()
        {
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                hash = hash * 23 + IsBlocked.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Hex))
                return false;

            var hex = (Hex)obj;
            return X == hex.X &&
                   Y == hex.Y &&
                   Z == hex.Z &&
                   IsBlocked == hex.IsBlocked;
        }
    }

    class Program
    {
        public static HashSet<Hex> HexGrid;
        public static HashSet<int> BlockedCells;
        static void Main(string[] args)
        {
            int[] inputA = null;
            string input;
            while (!string.IsNullOrEmpty(input = Console.ReadLine()))
                if (inputA == null)
                    inputA = input.Split(' ').Select(x => int.Parse(x)).ToArray();
                else
                    BlockedCells = new HashSet<int>(input.Split(' ').Select(x => int.Parse(x)).ToArray());

            int _edgeLength = inputA[0];
            int _allowedSteps = inputA[1];
            int _startCellIndex = inputA[2];
            int _targetCellIndex = inputA[3];
    
            HexGrid = CreateHexGrid(_edgeLength);
    
            int? nthStep = StepsToTargetHex(_allowedSteps, _startCellIndex, _targetCellIndex);
    
            if (nthStep == null)
                Console.WriteLine("no");
            else
                Console.WriteLine(nthStep);
        }
    
        public static int? StepsToTargetHex(int allowedSteps, int startCellIndex, int targetCellIndex)
        {
            Hex startPoint = HexGrid.FirstOrDefault(x => x.CellIndex == startCellIndex);
            Hex targetPoint = HexGrid.FirstOrDefault(x => x.CellIndex == targetCellIndex);
            HashSet<Hex> visited = new HashSet<Hex>();
            List<List<Hex>> lastTimeVisited = new List<List<Hex>>(allowedSteps);
            visited.Add(startPoint);
            lastTimeVisited.Add(new List<Hex> { startPoint });
    
            for (int i = 0; i < allowedSteps; i++)
            {
                var thisTimeVisited = new List<Hex>(6);
                foreach (var item in lastTimeVisited[i])
                {
                    foreach (var neighbor in GetNonBlockedNeighbors(item))
                    {
                        if (!visited.Contains(neighbor))
                        {
                            thisTimeVisited.Add(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }
                if (visited.Contains(targetPoint))
                    return i + 1;
                lastTimeVisited.Add(thisTimeVisited);
            }
            return null;
        }
        public static HashSet<Hex> CreateHexGrid(int edgeLength)
        {
            int r = edgeLength;
            int mapLenght = (r * 2) - 1;
            Hex[][] map = new Hex[mapLenght][];
    
            int _cellIndex = 1;
            int columnWidth = r;
    
            bool shrinkColumn = false;
            int xBaseValue = 0;
            int yBaseValue = r - 1;
            int z = (r - 1) * -1;
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = new Hex[columnWidth];
                for (int j = 0; j < columnWidth; j++)
                {
                    map[i][j] = new Hex(_cellIndex, xBaseValue + j, yBaseValue - j, z, BlockedCells.Contains(_cellIndex));
                    _cellIndex++;
                }
                if (shrinkColumn) columnWidth--;
                else if (columnWidth == (r * 2) - 1)
                {
                    shrinkColumn = true;
                    columnWidth--;
                }
                else columnWidth++;
                if (i < r - 1)
                    xBaseValue--;
                if (i > r - 2)
                    yBaseValue--;
                z++;
            }
    
            var HexGrid = new HashSet<Hex>((int)(Math.Pow(r, 3) - Math.Pow(r - 1, 3)));
            foreach (var row in map)
                foreach (var item in row)
                    HexGrid.Add(item);
    
            return HexGrid;
        }
    
        public static IEnumerable<Hex> GetNonBlockedNeighbors(Hex hex)
        {
            var hexes = new HashSet<Hex>(6);
    
            int _x = hex.X;
            int _y = hex.Y;
            int _z = hex.Z;

            hexes.Add(new Hex(x: (_x + 1), y: (_y - 1), z: (_z)));
            hexes.Add(new Hex(x: (_x + 1), y: _y, z: (_z - 1)) );
            hexes.Add(new Hex(x: (_x), y: (_y + 1), z: (_z - 1)));
            hexes.Add(new Hex(x: (_x - 1), y: (_y + 1), z: (_z)));
            hexes.Add(new Hex(x: (_x - 1), y: (_y), z: (_z + 1)));
            hexes.Add(new Hex(x: (_x), y: (_y - 1), z: (_z + 1)));

            // Since the structs created above uses the constructor that automaticly sets the IsBlocked property to false, only hexes that are nonBlocked    andneighbours will be returned
            return hexes.Where(x => HexGrid.Contains(x));
        }
    }
}   
    