
using UnityEngine;
using UnityEngine.Assertions;
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

namespace MazeData
{


    public enum Direction : uint
    {
        North,
        East,
        South,
        West,
        Count
    };

    static public class Util
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
                n--;
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

		public static void DrawCircle(Vector3 _pos, float _radius, Color _color, int _samples = 12)
		{
			Vector3 ray = new Vector3(0.0f, 0.0f, _radius);

			Vector3 start = _pos + ray;
			Vector3 end;

			float deltaAngle = 360.0f / _samples;

			for(int i = 0; i < _samples; ++i)
			{
				ray = Quaternion.AngleAxis(deltaAngle, Vector3.up) * ray;
				end = _pos + ray;
				Debug.DrawLine(start, end, _color, 0.0f, false);
				start = end;
			}
		}

		public static void DebugDraw(Maze _maze, float _scale, Monster _monster)
		{
			var start = new Vector3();
			var end = new Vector3();

			float width = _maze.Width() * _scale;
			float height = _maze.Height() * _scale;

			//draw top border
			start.Set(0.0f, 0.0f, 0.0f);
			end.Set(width, 0.0f, 0.0f);
			Debug.DrawLine(start, end, Color.green, 0.0f, false);

			//draw rightmost border
			start.Set(width, 0.0f, 0.0f);
			end.Set(width, 0.0f, height);
			Debug.DrawLine(start, end, Color.green, 0.0f, false);

			if(_monster != null)
			{
				//draw monster
				start.Set(((float)_monster.GetPosX() + 0.5f) * _scale, 0.0f, ((float)_monster.GetPosY() + 0.5f) * _scale);
				DrawCircle(start, _scale * 0.4f, Color.yellow);
			}

			Func<MazeData.Maze.CellData, Direction, Color> getColor = (curCell, dir) =>
			{
				var color = Color.green;

				if(curCell.TestDir(dir, MazeData.Maze.CellData.IsOpenDoor))
				{
					color = Color.magenta;
					color.a = 0.2f;
				}
				else if(curCell.TestDir(dir, MazeData.Maze.CellData.IsClosedDoor))
				{
					color = Color.red;
				}

				return color;
			};

			for (int j = 0; j < _maze.Height(); ++j)
			{
				for (int i = 0; i < _maze.Width(); ++i)
				{
					var curCell = _maze.GetCell(i, j);

					//west wall
					if(!curCell.TestDir(Direction.West, MazeData.Maze.CellData.IsEmpty))
					{
						start.Set((float)i * _scale, 0.0f, (float)j * _scale);
						end.Set(start.x, 0.0f, start.z + _scale);

						var color = getColor(curCell, Direction.West);
												
						Debug.DrawLine(start, end, color, 0.0f, false);
					}

					//south wall
					if(!curCell.TestDir(Direction.South, MazeData.Maze.CellData.IsEmpty))
					{
						start.Set((float)i * _scale, 0.0f, (float)(j+1) * _scale);
						end.Set(start.x + _scale, 0.0f, start.z);

						var color = getColor(curCell, Direction.South);
												
						Debug.DrawLine(start, end, color, 0.0f, false);
					}
				}
			}
		}

		public static void SortedAdd<T>(this List<T> _list, T _elem, IComparer<T> _comparer)
        {
			int insertAt = _list.BinarySearch(_elem, _comparer);
            Assert.IsTrue(insertAt < 0);
            _list.Insert(~insertAt, _elem);
        }

		public static void DrawPath(int _fromX, int _fromY, List<Direction> _path, float _scale)
        {
			Vector3 start = new Vector3(((float)_fromX + 0.5f) * _scale, 0.0f, ((float)_fromY + 0.5f) * _scale);

			Vector3[] dirVec = {
				new Vector3(0.0f, 0.0f, -_scale), //North
				new Vector3(_scale, 0.0f, 0.0f), //East
				new Vector3(0.0f, 0.0f, _scale), //South
				new Vector3(-_scale, 0.0f, 0.0f), //West
			};

			foreach(var d in _path)
			{
				Vector3 end = start + dirVec[(int)d];
				Debug.DrawLine(start, end, Color.white, 0.0f, false);
				start = end;
			}
		}

		public static List<GameObject> CreateWalls(Maze _fromMaze, Transform[] _wallSource)
		{
			var walls = new List<GameObject>();

			float wallSize = _wallSource[0].GetComponent<Renderer>().bounds.size.x;

			var wallPos = new Vector3();

			var weights = new float[_wallSource.Length];
			float totalWeight = 0.0f;
			for(int i = 0; i < _wallSource.Length; ++i)
			{
				totalWeight += _wallSource[i].GetComponent<MazeWall>().weight;
				weights [i] = totalWeight;
			}

			Func<GameObject> randSourceWall = () => 
			{
				float randVal = UnityEngine.Random.Range(0.0f, weights[weights.Length-1]);

				for(int i = 0; i < weights.Length; ++i)
					if(randVal < weights[i])
						return _wallSource[i].gameObject;
				
				return _wallSource[_wallSource.Length-1].gameObject;
			};

			//top border
			for(int i = 0; i < _fromMaze.Width(); ++i)
			{
				if(_fromMaze.GetCell(i, 0).TestDir(Direction.North, Maze.CellData.IsWall))
				{
					wallPos.Set(((float)i + 0.5f) * wallSize, 0.0f, 0.0f);
					var newWall = (GameObject)UnityEngine.Object.Instantiate(randSourceWall(), wallPos, Quaternion.identity);
					newWall.SetActive(true);
					walls.Add(newWall);
				}
			}

			//rightmost border
			wallPos.x = _fromMaze.Width() * wallSize;
			for(int j = 0; j < _fromMaze.Height(); ++j)
			{
				if(_fromMaze.GetCell(_fromMaze.Width() - 1, j).TestDir(Direction.East, Maze.CellData.IsWall))
				{
					wallPos.z = ((float)j + 0.5f) * wallSize;
					var newWall = (GameObject)UnityEngine.Object.Instantiate(randSourceWall(), wallPos, Quaternion.Euler(0, 90, 0));
					newWall.SetActive(true);
					walls.Add(newWall);
				}
			}

			//west and south walls of each cell
			for (int j = 0; j < _fromMaze.Height(); ++j)
			{
				for (int i = 0; i < _fromMaze.Width(); ++i)
				{
					var curCell = _fromMaze.GetCell(i, j);

					if(curCell.TestDir(Direction.South, Maze.CellData.IsWall))
					{
						wallPos.Set((float)(i+0.5f) * wallSize, 0.0f, (float)(j+1) * wallSize);
						var newWall = (GameObject)UnityEngine.Object.Instantiate(randSourceWall(), wallPos, Quaternion.identity);
						newWall.SetActive(true);
						walls.Add(newWall);
					}

					if(curCell.TestDir(Direction.West, Maze.CellData.IsWall))
					{
						wallPos.Set((float)(i) * wallSize, 0.0f, (float)(j + 0.5f) * wallSize);
						var newWall = (GameObject)UnityEngine.Object.Instantiate(randSourceWall(), wallPos, Quaternion.Euler(0, 90, 0));
						newWall.SetActive(true);
						walls.Add(newWall);
					}
				}
			}

			return walls;
		}

		public static List<GameObject> CreateDoors(Maze _fromMaze, GameObject _doorObj)
		{
			var doors = new List<GameObject>();
			float doorSize = _doorObj.GetComponent<Renderer>().bounds.size.x;
			var doorPos = new Vector3();

			for (int j = 0; j < _fromMaze.Height(); ++j)
			{
				for (int i = 0; i < _fromMaze.Width(); ++i)
				{
					var curCell = _fromMaze.GetCell(i, j);

					if(curCell.TestDir(Direction.South, Maze.CellData.IsDoor))
					{
						doorPos.Set((float)(i+0.5f) * doorSize, 0.0f, (float)(j+1) * doorSize);
						var newDoor = (GameObject)UnityEngine.Object.Instantiate(_doorObj, doorPos, Quaternion.identity);
						newDoor.SetActive(true);
						doors.Add(newDoor);
					}

					if(curCell.TestDir(Direction.West, Maze.CellData.IsDoor))
					{
						doorPos.Set((float)(i) * doorSize, 0.0f, (float)(j + 0.5f) * doorSize);
						var newDoor = (GameObject)UnityEngine.Object.Instantiate(_doorObj, doorPos, Quaternion.Euler(0, 90, 0));
						newDoor.SetActive(true);
						doors.Add(newDoor);
					}
				}
			}

			return doors;
		}
    }



    public class Maze
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

            public static int IsBlocked = ((1 << (int)WallState.Wall) | (1 << (int)WallState.ClosedDoor));
			public static int IsWall = (1 << (int)WallState.Wall);
            public static int IsDoor = ((1 << (int)WallState.OpenDoor) | (1 << (int)WallState.ClosedDoor));
            public static int IsClosedDoor = ((1 << (int)WallState.ClosedDoor));
            public static int IsOpenDoor = ((1 << (int)WallState.OpenDoor));
			public static int IsEmpty = ((1 << (int)WallState.Empty));

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
                return !Array.Exists(walls, w => !TestWallState(w, IsBlocked));
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
				Assert.IsTrue(TestDir(_dir, IsDoor));
                walls[(int)_dir] = WallState.OpenDoor;
            }

            public void CloseDoor(Direction _dir)
            {
				Assert.IsTrue(TestDir(_dir, IsDoor));
                walls[(int)_dir] = WallState.ClosedDoor;
            }

            public int WallCount()
            {
                Predicate<WallState> pred = w => { return TestWallState(w, IsBlocked); };
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
                    if (!TestDir(dir, IsBlocked))
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

		public void ForEachDoor(Action<bool> _action)
		{
			ForEachDoorWithCoord((isOpen, i, j) =>
			{
				_action(isOpen);
			});
		}

		public void ForEachDoorWithCoord(Action<bool, int, int> _action)
		{
			for (var j = 0; j < Height(); ++j)
			{
				for (var i = 0; i < Width(); ++i)
				{
					var cell = mCells[i,j];

					if(cell.TestDir(Direction.West, CellData.IsDoor))
						_action(cell.TestDir(Direction.West, CellData.IsOpenDoor), i, j);
					if(cell.TestDir(Direction.South, CellData.IsDoor))
						_action(cell.TestDir(Direction.South, CellData.IsOpenDoor), i, j);
				}
			}
		}

        private void ForEachCell(Action<CellData> _action)
        {
			ForEachCellWithCoord((cell, i, j) =>
			{
					_action(cell);
			});
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
            if (!mCells[_fromX, _fromY].TestDir(_dir, CellData.IsBlocked))
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

		private class PrioritizedPosComparer : IComparer<PrioritizedPos>
		{
			public int Compare(PrioritizedPos _p1, PrioritizedPos _p2)
			{
				int result = _p1.Item2.CompareTo(_p2.Item2);
				if(result == 0)
					result = _p1.Item1.Item1.CompareTo(_p2.Item1.Item1);
				if(result == 0)
					result = _p1.Item1.Item2.CompareTo(_p2.Item1.Item2);
				return result;
			}
		}

		public List<Direction> FindBestPath(int _fromX, int _fromY, int _toX, int _toY, int _lastPosX = -1, int _lastPosY = -1)
        {
            List<Direction> path = new List<Direction>();

            if (_fromX == _toX && _fromY == _toY)
                return path;

            Func<Position, Position, int> heuristic = (p1, p2) =>
            {
				if(p1.Item1 == _lastPosX && p1.Item2 == _lastPosY)
					return 100000;
					
                return Mathf.Abs(p1.Item1 - p2.Item1) + Mathf.Abs(p1.Item2 - p2.Item2);
            };

			var comparer = new PrioritizedPosComparer();

			Assert.IsTrue(IsInMaze(_fromX, _fromY) && IsInMaze(_toX, _toY));

            Position start = new Position(_fromX, _fromY);
            Position goal = new Position(_toX, _toY);

            var frontier = new List<PrioritizedPos>();
			frontier.SortedAdd(new PrioritizedPos(start, 0), comparer);

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
						frontier.SortedAdd(new PrioritizedPos(next, priority), comparer);
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
					Assert.IsTrue(false);
                    return Direction.Count;
                }
            });

            var curPos = goal;
            while (curPos != start)
            {
				Assert.IsTrue(cameFrom.ContainsKey(curPos));
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
						Assert.IsTrue(_cell.GetWallState(_dir) == nextCell.GetWallState(Util.Opposite(_dir)));
						Assert.IsTrue(_cell.WallCount() != 4);
                    }
                });
            });
        }

        private CellData[,] mCells;
    }


    public class Monster
    {
		public Monster(Maze _maze, int _startX = 0, int _startY = 0)
        {
			mPosX = _startX;
			mPosY = _startY;
            mLastMove = Direction.Count;
            mMaze = _maze;

            mVisitedCount = new int[_maze.Width(), _maze.Height()];
        }

        public void Move(Direction _dir)
        {
            if (mMaze.GetCell(mPosX, mPosY).TestDir(_dir, Maze.CellData.IsBlocked))
            {
				Assert.IsTrue(false);
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
                HomeIn(_targetX, _targetY, true);
                return;
            }

            Move(possibilities.OrderBy(p => p.Item2).ElementAt(0).Item1);
        }

		public List<Direction> HomeIn(int _targetX, int _targetY, bool _considerPrevPos = false)
        {
			int lastPosX = -1;
			int lastPosY = -1;

			if(_considerPrevPos)
			{
				Direction op = Util.Opposite(mLastMove);
				lastPosX = mPosX + Util.xDelta[(int)op];
				lastPosY = mPosY + Util.yDelta[(int)op];
			}
			
			List<Direction> bestPath = mMaze.FindBestPath(mPosX, mPosY, _targetX, _targetY, lastPosX, lastPosY);

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

            return !mMaze.GetCell(mPosX, mPosY).TestDir(_dir, Maze.CellData.IsBlocked);
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
