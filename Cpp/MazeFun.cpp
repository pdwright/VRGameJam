// MazeFun.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <array>
#include <bitset>
#include <random>
#include <algorithm>
#include <string>
#include <iostream>
#include <sstream>
#include <stdlib.h>
#include <vector>
#include <assert.h>
#include <utility>
#include <queue>
#include <unordered_map>
#include <memory>
#include <time.h>       /* time */
#include <bitset>

namespace Maze
{
	using std::array;
	using std::bitset;
	using std::vector;
	using std::pair;


	enum Direction : size_t
	{
		North,
		East,
		South,
		West,
		Count
	};

	namespace
	{
		static array<int, Direction::Count> xDelta = { 0, 1, 0, -1 };
		static array<int, Direction::Count> yDelta = { -1, 0, 1, 0 };

		std::random_device rd;
		std::mt19937 g(rd());
	}

	Direction Opposite(Direction _dir) 
	{ 
		if(_dir == Direction::Count)
			return Direction::Count;
		return static_cast<Direction>((_dir + 2) % Direction::Count); 
	}

	template<typename F>
	void ForEachDir(const F& _f)
	{
		for (size_t i = 0; i < Direction::Count; ++i)
			_f(static_cast<Direction>(i));
	}

	template<typename F>
	void ForEachDirRandomOrder(const F& _f)
	{
		array<Direction, Direction::Count> directions = { North, East, South, West };

		std::shuffle(directions.begin(), directions.end(), g);

		for (auto d : directions)
			_f(d);
	}


	class BaseMaze
	{
	public:
		enum WallState
		{
			Empty,
			Wall,
			OpenDoor,
			ClosedDoor,

			Count
		};

		static const std::bitset<WallState::Count> IsWalled;
		static const std::bitset<WallState::Count> IsDoor;
		static const std::bitset<WallState::Count> IsClosedDoor;
		static const std::bitset<WallState::Count> IsOpenDoor;

	protected:
		struct CellData
		{
			CellData() { walls.fill(Empty); }

			void Reset(bool _allWalls)
			{
				_allWalls ? walls.fill(Wall) : walls.fill(Empty);
			}

			bool IsFullyWalled() const
			{
				return std::find_if(walls.begin(), walls.end(), [](WallState wallState) {return !IsWalled.test(wallState); }) == walls.end();
			}

			Direction GetFirstHole() const
			{
				for (size_t i = 0; i < walls.size(); ++i)
					if (walls[i] == Empty)
						return static_cast<Direction>(i);
				return static_cast<Direction>(walls.size());
			}

			bool TestDir(Direction _dir, const std::bitset<WallState::Count>& _cond) const
			{
				return _cond.test(walls[_dir]);
			}

			void OpenDoor(Direction _dir)
			{
				assert(TestDir(_dir, IsDoor));
				walls[_dir] = WallState::OpenDoor;
			}

			void CloseDoor(Direction _dir)
			{
				assert(TestDir(_dir, IsDoor));
				walls[_dir] = WallState::ClosedDoor;
			}

			size_t WallCount() const
			{
				return std::count_if(walls.begin(), walls.end(), [](WallState w){ return IsWalled.test(w); });
			}

			void BreakWall(Direction _dir, bool _withDoor)
			{
				walls[_dir] = _withDoor ? WallState::OpenDoor : Empty;
			}

			void FlipDoor(Direction _dir)
			{
				if(walls[_dir] == WallState::OpenDoor)
					walls[_dir] = WallState::ClosedDoor;
				else if(walls[_dir] == WallState::ClosedDoor)
					walls[_dir] = WallState::OpenDoor;
			}

			const char* GetChar(Direction _dir) const
			{
				if (_dir == North || _dir == South)
				{
					switch (walls[_dir])
					{
					case WallState::Empty: return " ";
					case WallState::Wall: return "_";
					case WallState::OpenDoor: return ".";
					case WallState::ClosedDoor: return "=";
					}
				}

				if (_dir == East || _dir == West)
				{
					switch (walls[_dir])
					{
					case WallState::Empty: return " ";
					case WallState::Wall: return "|";
					case WallState::OpenDoor: return ".";
					case WallState::ClosedDoor: return "/";
					}
				}

				return "?";
			}

			void GetNeighbours(vector<Direction>& _neighbours) const
			{
				_neighbours.clear();

				ForEachDir([&_neighbours, this](Direction dir)
				{
					if(!TestDir(dir, IsWalled))
						_neighbours.push_back(dir);
				});
			}

			//should be used for validation only
			WallState GetWallState(Direction _dir) const
			{ return walls[_dir]; }

		private:
			array<WallState, Direction::Count> walls;
		};

	public:
		BaseMaze() {}

		template<typename F>
		void ForEachCell(const F& _f) const
		{
			for (int i = 0; i < Width(); ++i)
				for (int j = 0; j < Height(); ++j)
					_f(GetCellInternal(i, j));
		}

		void Generate()
		{
			Reset();
			CarvePathRecursive(0, 0);
		}

		void Braid(bool _withDoors = false)
		{
			ForEachCellWithCoord([_withDoors, this](CellData& curCell, int x, int y) 
			{
				const size_t wallCount = curCell.WallCount();
				if (wallCount < 3)
					return;

				const Direction firstHole = curCell.GetFirstHole();

				//try removing middle wall in dead end
				const Direction middleWallDir = static_cast<Direction>((firstHole + 2) % Direction::Count);
				if (!CarvePath(x, y, middleWallDir, _withDoors))
				{
					//if carvePath fails, it's because we are on an edge
					//try removing the first non-edge wall
					Direction nextDir = middleWallDir;
					do
					{
						nextDir = static_cast<Direction>((nextDir + 1) % Direction::Count);
					} while (!CarvePath(x, y, nextDir, false));
				}
			});
		}

		bool IsInMaze(int _x, int _y) const
		{
			return _x >= 0 && _x < Width()
				&& _y >= 0 && _y < Height();
		}

		const CellData& GetCell(int _x, int _y) const { return GetCellInternal(_x, _y); }

		virtual int Width() const = 0;
		virtual int Height() const = 0;


		void ShuffleDoors(float _closedDoorChance)
		{
			_closedDoorChance *= 100.0f;
			ForEachCellWithCoord([_closedDoorChance, this](CellData& curCell, int _x, int _y)
			{
				ForEachDir([&curCell, _closedDoorChance, _x, _y, this](Direction dir)
				{
					if(!curCell.TestDir(dir, IsDoor))
						return;

					auto nextCell = CellInDir(_x, _y, dir);

					if(static_cast<float>(rand() % 100) < _closedDoorChance)
					{
						curCell.CloseDoor(dir);
						if(nextCell)
							nextCell->CloseDoor(Opposite(dir));
					}
					else
					{
						curCell.OpenDoor(dir);
						if(nextCell)
							nextCell->OpenDoor(Opposite(dir));
					}
				});
			});
		}


		void FindBestPath(int _fromX, int _fromY, int _toX, int _toY, vector<Direction>& _path) const
		{
			assert(IsInMaze(_fromX, _fromY) && IsInMaze(_toX, _toY));

			typedef std::pair<int, int> Position;
			typedef std::pair<Position, int> PrioritizedPos;

			struct Compare
			{
				bool operator()(const PrioritizedPos& _p1, const PrioritizedPos& _p2) const
				{ return _p1.second > _p2.second; };
			};

			struct Hash
			{
				size_t operator()(const Position& _pos) const
				{ return static_cast<size_t>((_pos.first << (sizeof(size_t) * 4)) + _pos.second); }
			};

			auto heuristic = [](const Position& _p1, const Position& _p2) 
			{
				return std::abs(static_cast<int>(_p1.first) - static_cast<int>(_p2.first))
					+ std::abs(static_cast<int>(_p1.second) - static_cast<int>(_p2.second));
			};

			const Position start(_fromX, _fromY);
			const Position goal(_toX, _toY);

			std::priority_queue<PrioritizedPos, vector<PrioritizedPos>, Compare> frontier;
			frontier.emplace(start, 0);

			std::unordered_map<Position, Position, Hash> cameFrom;
			std::unordered_map<Position, int, Hash> costSoFar;

			cameFrom[start] = start;
			costSoFar[start] = 0;

			vector<Direction> neighbours;
			neighbours.reserve(Direction::Count);

			while (!frontier.empty())
			{
				const auto current = frontier.top().first;
				frontier.pop();

				if(current == goal)
					break;

				GetCell(current.first, current.second).GetNeighbours(neighbours);
				for (auto d : neighbours)
				{
					const Position next(current.first + xDelta[d], current.second + yDelta[d]);
					const int newCost = costSoFar[current] + 1;

					if (!costSoFar.count(next) || newCost < costSoFar[next])
					{
						costSoFar[next] = newCost;
						const int priority = newCost + heuristic(next, goal);
						frontier.emplace(next, priority);
						cameFrom[next] = current;
					}
				}
			}

			_path.clear();

			auto getDir = [](const Position& _from, const Position& _to) 
			{
				const Position diff(_to.first - _from.first, _to.second - _from.second);
				if (diff == Position(0, -1))
					return Direction::North;
				else if (diff == Position(1, 0))
					return Direction::East;
				else if (diff == Position(0, 1))
					return Direction::South;
				else if (diff == Position(-1, 0))
					return Direction::West;
				else
				{ 
					assert(false);
					return Direction::Count;
				}

			};

			auto current = goal;
			while (current != start)
			{
				assert(cameFrom.count(current));
				const auto prev = cameFrom[current];
				_path.push_back(getDir(prev, current));
				current = prev;
			}
			std::reverse(_path.begin(), _path.end());
		}

		void ValidateMazeState()
		{
			ForEachCellWithCoord([this](const CellData& _cell, int _x, int _y)
			{
				ForEachDir([_cell, _x, _y, this](Direction _dir)
				{
					auto nextCell = CellInDir(_x, _y, _dir);
					if(nextCell)
					{
						assert(_cell.GetWallState(_dir) == nextCell->GetWallState(Opposite(_dir)));
						assert(_cell.WallCount() != 4);
					}
				});
			});
		}

	protected:

		virtual CellData& GetCellInternal(int _x, int _y) = 0;
		virtual const CellData& GetCellInternal(int _x, int _y) const = 0;

		template<typename F>
		void ForEachCell(const F& _f)
		{
			for (int i = 0; i < Width(); ++i)
				for (int j = 0; j < Height(); ++j)
					_f(GetCellInternal(i, j));
		}

		template<typename F>
		void ForEachCellWithCoord(const F& _f)
		{
			for (int i = 0; i < Width(); ++i)
				for (int j = 0; j < Height(); ++j)
					_f(GetCellInternal(i, j), i, j);
		}

		void Reset(bool _allWalls = true)
		{
			ForEachCell([_allWalls](CellData& _cell) { _cell.Reset(_allWalls); });
		}

		void CarvePathRecursive(int _fromX, int _fromY)
		{
			ForEachDirRandomOrder([this, _fromX, _fromY](Direction dir)
			{
				auto cellInDir = CellInDir(_fromX, _fromY, dir);
				if (!cellInDir)
					return;

				if (!cellInDir->IsFullyWalled())
					return;

				CarvePath(_fromX, _fromY, dir);
				CarvePathRecursive(_fromX + xDelta[dir], _fromY + yDelta[dir]);
			});
		}

		bool CarvePath(int _fromX, int _fromY, Direction _dir, bool _withDoor = false)
		{
			auto nextCell = CellInDir(_fromX, _fromY, _dir);

			if (!nextCell)
				return false;

			//if path is already carved return false
			if (!GetCell(_fromX, _fromY).TestDir(_dir, IsWalled))
				return false;

			GetCellInternal(_fromX, _fromY).BreakWall(_dir, _withDoor);
			nextCell->BreakWall(Opposite(_dir), _withDoor);
			return true;
		}

		CellData* CellInDir(int _fromX, int _fromY, Direction _dir)
		{
			const int destX = _fromX + xDelta[_dir];
			const int destY = _fromY + yDelta[_dir];

			if (destX < 0 || destX >= static_cast<int>(Width()) || destY < 0 || destY >= static_cast<int>(Height()))
				return nullptr;

			return &GetCellInternal(destX, destY);
		}
	};

	const std::bitset<BaseMaze::WallState::Count> BaseMaze::IsWalled((1 << BaseMaze::WallState::Wall) | (1 << BaseMaze::WallState::ClosedDoor));
	const std::bitset<BaseMaze::WallState::Count> BaseMaze::IsDoor((1 << BaseMaze::WallState::OpenDoor) | (1 << BaseMaze::WallState::ClosedDoor));
	const std::bitset<BaseMaze::WallState::Count> BaseMaze::IsClosedDoor((1 << BaseMaze::WallState::ClosedDoor));
	const std::bitset<BaseMaze::WallState::Count> BaseMaze::IsOpenDoor((1 << BaseMaze::WallState::OpenDoor));


	//2 alloc strategies because why not
	template<int WIDTH, int HEIGHT>
	class StaticMaze : public BaseMaze
	{
	
	public:
		StaticMaze() : BaseMaze()
		{
			Generate();
		}		

		virtual int Width() const override { return WIDTH; }
		virtual int Height() const override { return HEIGHT; }

	protected:
		virtual CellData& GetCellInternal(int _x, int _y) override { return mCells[_x][_y]; }
		virtual const CellData& GetCellInternal(int _x, int _y) const override { return mCells[_x][_y]; }

	private:
		array<array<CellData, HEIGHT>, WIDTH> mCells;
	};

	class Maze : public BaseMaze
	{
	public:
		Maze(int _width, int _height) : BaseMaze()
		{
			mCells.resize(_width);
			for(auto& col : mCells)
				col.resize(_height);

			Generate();
		}

		virtual int Width() const override { return mCells.size(); }
		virtual int Height() const override { return mCells[0].size(); }

	protected:
		virtual CellData& GetCellInternal(int _x, int _y) override { return mCells[_x][_y]; }
		virtual const CellData& GetCellInternal(int _x, int _y) const override { return mCells[_x][_y]; }

	private:
		vector<vector<CellData>> mCells;
	};


	class Monster
	{
	public:
		Monster(const BaseMaze& _maze)
			: mMaze(_maze)
			, mPosX(0)
			, mPosY(0)
			, mLastMove(Direction::Count)
		{
			mVisitedCount.resize(mMaze.Width());
			
			for (int i = 0; i < mMaze.Width(); ++i)
				mVisitedCount[i].resize(mMaze.Height(), 0);
		}

		void Move(Direction _dir)
		{
			if(mMaze.GetCell(mPosX, mPosY).TestDir(_dir, BaseMaze::IsWalled))
			{
				assert(false);
				return;
			}

			mVisitedCount[mPosX][mPosY]++;
			mPosX += xDelta[_dir];
			mPosY += yDelta[_dir];
			mLastMove = _dir;
		}

		void Wander(int _targetX = 0, int _targetY = 0, float _fHomeInProbability = 0.0f)
		{
			typedef pair<Direction, int> PrioritizedDirection;
			vector<PrioritizedDirection> possibilities;
			possibilities.reserve(Direction::Count);

			const int randOffset = rand();

			const Direction dirToExclude = Opposite(mLastMove);

			ForEachDirRandomOrder([this, &possibilities, dirToExclude](Direction curDir)
			{
				if(curDir == dirToExclude)
					return;

				if (!CanTravelTo(curDir))
					return;

				const int nextPosX = mPosX + xDelta[curDir];
				const int nextPosY = mPosY + yDelta[curDir];
				possibilities.emplace_back(curDir, mVisitedCount[nextPosX][nextPosY]);
			});


			if(possibilities.empty())
			{
				if(CanTravelTo(mLastMove))
					Move(mLastMove);
				else
					Move(dirToExclude);
				return;
			}

			if(possibilities.size() > 1 && static_cast<float>(rand() % 100) < _fHomeInProbability * 100.0f)
			{
				HomeIn(_targetX, _targetY);
				return;
			}

			std::sort(begin(possibilities), 
					  end(possibilities), 
					  [](const PrioritizedDirection& _lhs, const PrioritizedDirection& _rhs) { return _lhs.second < _rhs.second; });

			assert(possibilities[0].second < std::numeric_limits<int>::max());
			Move(possibilities[0].first);
		}


		vector<Direction> HomeIn(int _targetX, int _targetY)
		{
			vector<Direction> bestPath;
			mMaze.FindBestPath(mPosX, mPosY, _targetX, _targetY, bestPath);

			if(!bestPath.empty())
				Move(bestPath[0]);

			bestPath.erase(bestPath.begin(), bestPath.begin()+1);
			return bestPath;
		}


		bool IsAtPos(int _x, int _y) const
		{
			return mPosX == _x && mPosY == _y; 
		}

		int GetPosX() const { return mPosX; }
		int GetPosY() const { return mPosY; }

	private:
		int MoveCost(int _fromX, int _fromY, Direction _dir)
		{
			int destX = _fromX + xDelta[_dir];
			int destY = _fromY + yDelta[_dir];

			if (!IsValidPos(destX, destY))
				return std::numeric_limits<int>::max();

			return mVisitedCount[destX][destY];
		}

		bool IsValidPos(int _x, int _y) const
		{
			return _x >= 0 && _x < static_cast<int>(mVisitedCount.size()) && _y >= 0 && _y < static_cast<int>(mVisitedCount[0].size());
		}

		bool CanTravelTo(Direction _dir) const
		{
			const int nextPosX = mPosX + xDelta[_dir];
			const int nextPosY = mPosY + yDelta[_dir];

			if (!IsValidPos(nextPosX, nextPosY))
				return false;

			return !mMaze.GetCell(mPosX, mPosY).TestDir(_dir, BaseMaze::IsWalled);
		}

		int mPosX;
		int mPosY;

		Direction mLastMove;

		const BaseMaze& mMaze;

		vector<vector<int>> mVisitedCount;
	};


	void OutputToConsole(const BaseMaze& _maze, const Monster& _monster)
	{
		std::string mazeAsString;

		//draw top border
		for (int i = 0; i < _maze.Width(); ++i)
		{
			mazeAsString += " ";
			mazeAsString += _maze.GetCell(i, 0).GetChar(North);
		}
		mazeAsString += "\n";

		for (int j = 0; j < _maze.Height(); ++j)
		{
			for (int i = 0; i < _maze.Width(); ++i)
			{
				const auto& curCell = _maze.GetCell(i, j);

				mazeAsString += curCell.GetChar(West);

				if (_monster.IsAtPos(i, j))
					mazeAsString += "*";
				else
					mazeAsString += curCell.GetChar(South);
			}

			mazeAsString += _maze.GetCell(_maze.Width() - 1, j).GetChar(East);
			mazeAsString += "\n";
		}

		std::cout << mazeAsString;
	}


	void DrawPath(int _fromX, int _fromY, const BaseMaze& _maze, vector<Direction>& _path)
	{
		if (_path.empty())
			return;

		vector<vector<int>> grid;
		grid.resize(_maze.Width());
		for (auto& v : grid)
			v.resize(_maze.Height(), -1);

		int counter = 0;

		int curX = _fromX;
		int curY = _fromY;
		grid[curX][curY] = counter++;
		for (auto d : _path)
		{
			curX += xDelta[d];
			curY += yDelta[d];
			if(curX < static_cast<int>(grid.size()) && curY < static_cast<int>(grid[0].size()))
				grid[curX][curY] = (counter++ % 10);
		}

		
		for (size_t j = 0; j < grid[0].size(); ++j)
		{
			for (size_t i = 0; i < grid.size(); ++i)
			{
				if (grid[i][j] >= 0)
					std::cout << " " << grid[i][j];
				else
					std::cout << "  ";
			}
			std::cout << "\n";
		}
	}
}


int main()
{
	srand(static_cast<unsigned int>(time(NULL)));

	//Maze::Maze baseMaze(30, 30);
	Maze::StaticMaze<30, 30> braidedMaze;
	braidedMaze.Braid(true);

	int targetX = rand() % braidedMaze.Width();
	int targetY = rand() % braidedMaze.Height();

	//auto monster = new Monster(braidedMaze);
	//auto monster = std::make_unique<Maze::Monster>(static_cast<Maze::BaseMaze&>(braidedMaze));
	std::unique_ptr<Maze::Monster> monster(new Maze::Monster(braidedMaze));
	
	std::string s;

	int step = 0;

	const bool wander = true;
	const float homeInRatio = 0.0f;
	const float closedDoorRatio = 0.25f;

	braidedMaze.ShuffleDoors(closedDoorRatio);

	do
	{
		braidedMaze.ValidateMazeState();

		if (s == "g")
		{
			braidedMaze.Generate();
			braidedMaze.Braid(true);
			monster.release();
			//monster = std::make_unique<Maze::Monster>(static_cast<Maze::BaseMaze&>(braidedMaze));
			monster = std::unique_ptr<Maze::Monster>(new Maze::Monster(braidedMaze));

			braidedMaze.ShuffleDoors(closedDoorRatio);
		}
		else if (s == "t")
		{
			targetX = rand() % braidedMaze.Width();
			targetY = rand() % braidedMaze.Height();
		}

		//OutputToConsole(braidedMaze, *monster);

		++step;

		if(step % 10 == 0)
			braidedMaze.ShuffleDoors(closedDoorRatio);

		std::vector<Maze::Direction> path;

		if(wander)
		{
			monster->Wander(targetX, targetY, homeInRatio);
			braidedMaze.FindBestPath(monster->GetPosX(), monster->GetPosY(), targetX, targetY, path);
		}
		else
		{
			path = monster->HomeIn(targetX, targetY);
		}

		system("cls");
		OutputToConsole(braidedMaze, *monster);
		DrawPath(monster->GetPosX(), monster->GetPosY(), braidedMaze, path);

	} while (getline(std::cin, s));

    return 0;
}

