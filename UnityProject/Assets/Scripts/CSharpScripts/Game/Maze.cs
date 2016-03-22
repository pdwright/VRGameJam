using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Position = Tuple<int, int>;
using PrioritizedPos = Tuple<Tuple<int, int>, int>;

[System.Serializable]
public class Tuple<T1, T2>
{
    public T1 Item1;
    public T2 Item2;

    private static readonly IEqualityComparer Item1Comparer = EqualityComparer<T1>.Default;
    private static readonly IEqualityComparer Item2Comparer = EqualityComparer<T2>.Default;

    public Tuple(T1 first, T2 second)
    {
        this.Item1 = first;
        this.Item2 = second;
    }

    public override string ToString()
    {
        return string.Format("<{0}, {1}>", Item1, Item2);
    }

    public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
    {
        if (Tuple<T1, T2>.IsNull(a) && !Tuple<T1, T2>.IsNull(b))
            return false;

        if (!Tuple<T1, T2>.IsNull(a) && Tuple<T1, T2>.IsNull(b))
            return false;

        if (Tuple<T1, T2>.IsNull(a) && Tuple<T1, T2>.IsNull(b))
            return true;

        return
            a.Item1.Equals(b.Item1) &&
            a.Item2.Equals(b.Item2);
    }

    public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + Item1.GetHashCode();
        hash = hash * 23 + Item2.GetHashCode();
        return hash;
    }

    public override bool Equals(object obj)
    {
        var other = obj as Tuple<T1, T2>;
        if (object.ReferenceEquals(other, null))
            return false;
        else
            return Item1Comparer.Equals(Item1, other.Item1) &&
                   Item2Comparer.Equals(Item2, other.Item2);
    }

    private static bool IsNull(object obj)
    {
        return object.ReferenceEquals(obj, null);
    }
}

namespace MazeFunCSharp
{
    

    enum Direction : uint
    {
        North,
        East,
        South,
        West,
        Count
    };

    static class Util
    {
        public static Direction Opposite(Direction _dir)
        {
            if (_dir == Direction.Count)
                return Direction.Count;
            return (Direction)(((int)_dir + 2) % (int)Direction.Count);
        }

        public static void ForEachDir(Action<Direction> _action)
        {
            for (int i = 0; i < (int)Direction.Count; ++i)
                _action((Direction)i);
        }

        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = UnityEngine.Random.Range(0, n);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        private static Direction[] directions = new Direction[4] { Direction.North, Direction.East, Direction.South, Direction.West };

        public static void ForEachDirRandomOrder(Action<Direction> _action)
        {
            Shuffle(directions);

            foreach (var dir in directions)
                _action(dir);
        }

        public static int[] xDelta = new int[4] { 0, 1, 0, -1 };
        public static int[] yDelta = new int[4] { -1, 0, 1, 0 };

        public static void OutputToConsole(Maze _maze, Monster _monster)
        {
            //draw top border
            for (int i = 0; i < _maze.Width(); ++i)
            {
                Console.Write(" ");
                Console.Write(_maze.GetCell(i, 0).GetChar(Direction.North));
            }
            Console.Write("\n");

            for (int j = 0; j < _maze.Height(); ++j)
            {
                for (int i = 0; i < _maze.Width(); ++i)
                {
                    var curCell = _maze.GetCell(i, j);

                    Console.Write(curCell.GetChar(Direction.West));

                    if (_monster.IsAtPos(i, j))
                        Console.Write("*");
                    else
                        Console.Write(curCell.GetChar(Direction.South));
                }

                Console.Write(_maze.GetCell(_maze.Width() - 1, j).GetChar(Direction.East));
                Console.Write("\n");
            }
        }

        public static void SortedAdd<T>(this List<T> _list, T _elem)
        {
            int insertAt = _list.BinarySearch(_elem);
            Debug.Assert(insertAt < 0);
            _list.Insert(~insertAt, _elem);
        }

        public static void DrawPath(int _fromX, int _fromY, Maze _maze, List<Direction> _path)
        {
            if (_path.Count == 0)
                return;

            int[,] grid = new int[_maze.Width(), _maze.Height()];
            for (int i = 0; i < grid.GetLength(0); ++i)
                for (int j = 0; j < grid.GetLength(1); ++j)
                    grid[i, j] = -1;

            int counter = 0;

            int curX = _fromX;
            int curY = _fromY;
            grid[curX, curY] = counter++;

            foreach (var d in _path)
            {
                curX += xDelta[(int)d];
                curY += yDelta[(int)d];
                if (curX < _maze.Width() && curY < _maze.Height())
                    grid[curX, curY] = (counter++ % 10);
            }


            for (int j = 0; j < grid.GetLength(1); ++j)
            {
                for (int i = 0; i < grid.GetLength(0); ++i)
                {
                    if (grid[i, j] >= 0)
                        Console.Write(" " + grid[i, j]);
                    else
                        Console.Write("  ");
                }
                Console.Write("\n");
            }
        }
    }




    class Maze
    {
        public class CellData
        {
            public enum WallState : int
            {
                Empty,
                Wall,
                OpenDoor,
                ClosedDoor,

                Count
            };

            public static int IsWalled = ((1 << (int)WallState.Wall) | (1 << (int)WallState.ClosedDoor));
            public static int IsDoor = ((1 << (int)WallState.OpenDoor) | (1 << (int)WallState.ClosedDoor));
            public static int IsClosedDoor = ((1 << (int)WallState.ClosedDoor));
            public static int IsOpenDoor = ((1 << (int)WallState.OpenDoor));

            public CellData()
            {
                walls = new WallState[(int)Direction.Count];
                for (uint i = 0; i < walls.Length; ++i)
                    walls[i] = WallState.Empty;
            }

            public void Reset(bool _allWalls)
            {
                for (uint i = 0; i < walls.Length; ++i)
                    walls[i] = (_allWalls ? WallState.Wall : WallState.Empty);
            }

            public bool IsFullyWalled()
            {
                return !Array.Exists(walls, w => !TestWallState(w, IsWalled));
            }

            private bool TestWallState(WallState _state, int _cond)
            {
                return (_cond & (1 << (int)_state)) != 0;
            }

            public bool TestDir(Direction _dir, int _cond)
            {
                return TestWallState(walls[(int)_dir], _cond);
            }

            public Direction GetFirstHole()
            {
                for (uint i = 0; i < (uint)Direction.Count; ++i)
                    if (walls[i] == WallState.Empty)
                        return (Direction)i;

                return Direction.Count;
            }

            public void OpenDoor(Direction _dir)
            {
                Debug.Assert(TestDir(_dir, IsDoor));
                walls[(int)_dir] = WallState.OpenDoor;
            }

            public void CloseDoor(Direction _dir)
            {
                Debug.Assert(TestDir(_dir, IsDoor));
                walls[(int)_dir] = WallState.ClosedDoor;
            }

            public int WallCount()
            {
                Predicate<WallState> pred = w => { return TestWallState(w, IsWalled); };
                return Array.FindAll(walls, pred).Length;
            }

            public void BreakWall(Direction _dir, bool _withDoor)
            {
                walls[(int)_dir] = _withDoor ? WallState.OpenDoor : WallState.Empty;
            }

            public void FlipDoor(Direction _dir)
            {
                if (walls[(int)_dir] == WallState.OpenDoor)
                    walls[(int)_dir] = WallState.ClosedDoor;
                else if (walls[(int)_dir] == WallState.ClosedDoor)
                    walls[(int)_dir] = WallState.OpenDoor;
            }

            public String GetChar(Direction _dir)
            {
                if (_dir == Direction.North || _dir == Direction.South)
                {
                    switch (walls[(int)_dir])
                    {
                        case WallState.Empty: return " ";
                        case WallState.Wall: return "_";
                        case WallState.OpenDoor: return ".";
                        case WallState.ClosedDoor: return "=";
                    }
                }

                if (_dir == Direction.East || _dir == Direction.West)
                {
                    switch (walls[(int)_dir])
                    {
                        case WallState.Empty: return " ";
                        case WallState.Wall: return "|";
                        case WallState.OpenDoor: return ".";
                        case WallState.ClosedDoor: return "/";
                    }
                }

                return "?";
            }

            public List<Direction> GetNeighbours()
            {
                List<Direction> neighbours = new List<Direction>((int)Direction.Count);

                Util.ForEachDir(dir =>
                {
                    if (!TestDir(dir, IsWalled))
                        neighbours.Add(dir);
                });

                return neighbours;
            }

            //should be used for validation only
            public WallState GetWallState(Direction _dir)
            { return walls[(int)_dir]; }

            private WallState[] walls;
        }

        public Maze(int _w, int _h)
        {
            mCells = new CellData[_w, _h];
            for (int i = 0; i < _w; ++i)
                for (int j = 0; j < _h; ++j)
                    mCells[i, j] = new CellData();

            Generate();
        }

        public int Width() { return mCells.GetLength(0); }
        public int Height() { return mCells.GetLength(1); }

        public CellData GetCell(int _x, int _y) { return mCells[_x, _y]; }

        private void ForEachCell(Action<CellData> _action)
        {
            foreach (var cell in mCells)
                _action(cell);
        }

        private void ForEachCellWithCoord(Action<CellData, int, int> _action)
        {
            for (var j = 0; j < Height(); ++j)
                for (var i = 0; i < Width(); ++i)
                    _action(mCells[i, j], i, j);
        }

        protected void Reset(bool _allWalls = true)
        {
            ForEachCell(cell => cell.Reset(_allWalls));
        }

        public void Generate()
        {
            Reset();
            CarvePathRecursive(0, 0);
        }

        public void Braid(bool _withDoors = false)
        {
            ForEachCellWithCoord((curCell, x, y) =>
            {
                var wallCount = curCell.WallCount();
                if (wallCount < 3)
                    return;

                Direction firstHole = curCell.GetFirstHole();

                //try removing middle wall in dead end
                Direction middleWallDir = (Direction)(((int)firstHole + 2) % (int)Direction.Count);
                if (!CarvePath(x, y, middleWallDir, _withDoors))
                {
                    //if carvePath fails, it's because we are on an edge
                    //try removing the first non-edge wall
                    Direction nextDir = middleWallDir;
                    do
                    {
                        nextDir = (Direction)(((int)nextDir + 1) % (int)Direction.Count);
                    } while (!CarvePath(x, y, nextDir, false));
                }
            });
        }

        private void CarvePathRecursive(int _fromX, int _fromY)
        {
            Util.ForEachDirRandomOrder(dir =>
            {
                var cellInDir = CellInDir(_fromX, _fromY, dir);
                if (cellInDir == null)
                    return;

                if (!cellInDir.IsFullyWalled())
                    return;

                CarvePath(_fromX, _fromY, dir);
                CarvePathRecursive(_fromX + Util.xDelta[(int)dir], _fromY + Util.yDelta[(int)dir]);
            });
        }

        private bool CarvePath(int _fromX, int _fromY, Direction _dir, bool _withDoor = false)
        {
            var nextCell = CellInDir(_fromX, _fromY, _dir);

            if (nextCell == null)
                return false;

            //if path is already carved return false
            if (!mCells[_fromX, _fromY].TestDir(_dir, CellData.IsWalled))
                return false;

            mCells[_fromX, _fromY].BreakWall(_dir, _withDoor);
            nextCell.BreakWall(Util.Opposite(_dir), _withDoor);
            return true;
        }

        public void ShuffleDoors(float _closedDoorChance)
        {
            ForEachCellWithCoord((curCell, x, y) =>
            {
                Util.ForEachDir(dir =>
                {
                    if (!curCell.TestDir(dir, CellData.IsDoor))
                        return;

                    var nextCell = CellInDir(x, y, dir);

                    if (UnityEngine.Random.Range(0.0f, 1.0f) < _closedDoorChance)
                    {
                        curCell.CloseDoor(dir);
                        if (nextCell != null)
                            nextCell.CloseDoor(Util.Opposite(dir));
                    }
                    else
                    {
                        curCell.OpenDoor(dir);
                        if (nextCell != null)
                            nextCell.OpenDoor(Util.Opposite(dir));
                    }
                });
            });
        }

        private CellData CellInDir(int _fromX, int _fromY, Direction _dir)
        {
            int destX = _fromX + Util.xDelta[(int)_dir];
            int destY = _fromY + Util.yDelta[(int)_dir];

            if (destX < 0 || destX >= Width() || destY < 0 || destY >= Height())
                return null;

            return mCells[destX, destY];
        }

        public bool IsInMaze(int _x, int _y)
        {
            return _x >= 0 && _x < Width()
                && _y >= 0 && _y < Height();
        }

        public List<Direction> FindBestPath(int _fromX, int _fromY, int _toX, int _toY)
        {
            List<Direction> path = new List<Direction>();

            if (_fromX == _toX && _fromY == _toY)
                return path;

            Func<Position, Position, int> heuristic = ((p1, p2) =>
            {
                return Mathf.Abs(p1.Item1 - p2.Item1) + Mathf.Abs(p1.Item2 - p2.Item2);
            });

            Debug.Assert(IsInMaze(_fromX, _fromY) && IsInMaze(_toX, _toY));

            Position start = new Position(_fromX, _fromY);
            Position goal = new Position(_toX, _toY);
            
            var frontier = new List<PrioritizedPos>();
            frontier.SortedAdd(new PrioritizedPos(start, 0));
            
            var cameFrom = new Dictionary<Position, Position>();
            var costSoFar = new Dictionary<Position, int>();

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count != 0)
            {
                var current = frontier[0].Item1;
                frontier.RemoveAt(0);

                if (current == goal)
                    break;

                var neighbours = GetCell(current.Item1, current.Item2).GetNeighbours();
                foreach (var d in neighbours)
                {
                    Position next = new Position(current.Item1 + Util.xDelta[(int)d], current.Item2 + Util.yDelta[(int)d]);
                    int newCost = costSoFar[current] + 1;

                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        int priority = newCost + heuristic(next, goal);
                        frontier.SortedAdd(new PrioritizedPos(next, priority));
                        cameFrom[next] = current;
                    }
                }
            }

            Func<Position, Position, Direction> getDir = ((p1, p2) =>
            {
                int diffX = p2.Item1 - p1.Item1;
                int diffY = p2.Item2 - p1.Item2;
                if (diffX == 0 && diffY == -1)
                    return Direction.North;
                else if (diffX == 1 && diffY == 0)
                    return Direction.East;
                else if (diffX == 0 && diffY == 1)
                    return Direction.South;
                else if (diffX == -1 && diffY == 0)
                    return Direction.West;
                else
                {
                    Debug.Assert(false);
                    return Direction.Count;
                }
            });

            var curPos = goal;
            while (curPos != start)
            {
                Debug.Assert(cameFrom.ContainsKey(curPos));
                var prev = cameFrom[curPos];
                path.Add(getDir(prev, curPos));
                curPos = prev;
            }
            path.Reverse();

            return path;
        }

        public void ValidateMazeState()
        {
            ForEachCellWithCoord((_cell, _x, _y) =>
            {
                Util.ForEachDir(_dir =>
                {
                    var nextCell = CellInDir(_x, _y, _dir);
                    if (nextCell != null)
                    {
                        Debug.Assert(_cell.GetWallState(_dir) == nextCell.GetWallState(Util.Opposite(_dir)));
                        Debug.Assert(_cell.WallCount() != 4);
                    }
                });
            });
        }

        private CellData[,] mCells;
    }


    class Monster
    {
        public Monster(Maze _maze)
        {
            mPosX = 0;
            mPosY = 0;
            mLastMove = Direction.Count;
            mMaze = _maze;

            mVisitedCount = new int[_maze.Width(), _maze.Height()];
        }

        public void Move(Direction _dir)
        {
            if (mMaze.GetCell(mPosX, mPosY).TestDir(_dir, Maze.CellData.IsWalled))
            {
                Debug.Assert(false);
                return;
            }

            mVisitedCount[mPosX, mPosY]++;
            mPosX += Util.xDelta[(int)_dir];
            mPosY += Util.yDelta[(int)_dir];
            mLastMove = _dir;
        }

        public void Wander(int _targetX = 0, int _targetY = 0, float _fHomeInProbability = 0.0f)
        {
            if (mPosX == _targetX && mPosY == _targetY)
                return;

            var possibilities = new List<Tuple<Direction, int>>();

            Direction dirToExclude = Util.Opposite(mLastMove);

            Util.ForEachDirRandomOrder(curDir =>
            {
                if (curDir == dirToExclude)
                    return;

                if (!CanTravelTo(curDir))
                    return;

                int nextPosX = mPosX + Util.xDelta[(int)curDir];
                int nextPosY = mPosY + Util.yDelta[(int)curDir];
                possibilities.Add(new Tuple<Direction, int>(curDir, mVisitedCount[(int)nextPosX, (int)nextPosY]));
            });

            if (possibilities.Count == 0)
            {
                if (CanTravelTo(mLastMove))
                    Move(mLastMove);
                else if (CanTravelTo(dirToExclude))
                    Move(dirToExclude);
                return;
            }

            if (possibilities.Count > 1 && UnityEngine.Random.Range(0, 1.0f) < _fHomeInProbability)
            {
                HomeIn(_targetX, _targetY);
                return;
            }

            Move(possibilities.OrderBy(p => p.Item2).ElementAt(0).Item1);
        }

        public List<Direction> HomeIn(int _targetX, int _targetY)
        {
            List<Direction> bestPath = mMaze.FindBestPath(mPosX, mPosY, _targetX, _targetY);

            if (bestPath.Count != 0)
            {
                Move(bestPath[0]);
                bestPath.RemoveAt(0);
            }
            return bestPath;
        }

        private bool CanTravelTo(Direction _dir)
        {
            int nextPosX = mPosX + Util.xDelta[(int)_dir];
            int nextPosY = mPosY + Util.yDelta[(int)_dir];

            if (!IsValidPos(nextPosX, nextPosY))
                return false;

            return !mMaze.GetCell(mPosX, mPosY).TestDir(_dir, Maze.CellData.IsWalled);
        }

        private bool IsValidPos(int _x, int _y)
        {
            return _x >= 0 && _x < mVisitedCount.GetLength(0) && _y >= 0 && _y < mVisitedCount.GetLength(1);
        }

        public bool IsAtPos(int _x, int _y)
        {
            return mPosX == _x && mPosY == _y;
        }

        public int GetPosX() { return mPosX; }
        public int GetPosY() { return mPosY; }

        private int mPosX;
        private int mPosY;

        private Direction mLastMove;

        private Maze mMaze;

        private int[,] mVisitedCount;
    }
}
