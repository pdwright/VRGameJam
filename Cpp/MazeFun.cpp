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

	class BaseMaze
	{
	protected:
		enum WallState
		{
			Empty,
			Wall,
			OpenDoor,
			ClosedDoor,
		};

		struct CellData
		{
			CellData() { walls.fill(Empty); }

			void Reset(bool _allWalls)
			{
				_allWalls ? walls.fill(Wall) : walls.fill(Empty);
			}

			bool IsFullyWalled() const
			{
				return std::find_if(walls.begin(), walls.end(), [](auto wallState) {return wallState != Wall; }) == walls.end();
			}

			Direction GetFirstHole() const
			{
				for (size_t i = 0; i < walls.size(); ++i)
					if (walls[i] == Empty)
						return static_cast<Direction>(i);
				return static_cast<Direction>(walls.size());
			}

			bool HasWall(Direction _dir) const
			{
				return walls[_dir] == Wall;
			}

			size_t WallCount() const
			{
				return std::count(walls.begin(), walls.end(), Wall);
			}

			void BreakWall(Direction _dir, bool _withDoor)
			{
				walls[_dir] = _withDoor ? OpenDoor : Empty;
			}

			const char* GetChar(Direction _dir) const
			{
				if (_dir == North || _dir == South)
				{
					switch (walls[_dir])
					{
					case Empty: return " ";
					case Wall: return "_";
					case OpenDoor: return ".";
					case ClosedDoor: return "_";
					}
				}

				if (_dir == East || _dir == West)
				{
					switch (walls[_dir])
					{
					case Empty: return " ";
					case Wall: return "|";
					case OpenDoor: return ".";
					case ClosedDoor: return "|";
					}
				}

				return "?";
			}

			void GetNeighbours(vector<Direction>& _neighbours) const
			{
				_neighbours.clear();
				for (size_t i = 0; i < walls.size(); ++i)
				{
					if (walls[i] == Empty || walls[i] == OpenDoor)
						_neighbours.push_back(static_cast<Direction>(i));
				}
			}

		private:
			array<WallState, Direction::Count> walls;
		};

	public:
		BaseMaze() {}

		void Generate()
		{
			Reset();
			CarvePathRecursive(0, 0);
		}

		void Braid(bool _withDoors = false)
		{
			for (int i = 0; i < Width(); ++i)
			{
				for (int j = 0; j < Height(); ++j)
				{
					auto& curCell = GetCell(i, j);
					const size_t wallCount = curCell.WallCount();
					if (wallCount < 3)
						continue;

					const Direction firstHole = curCell.GetFirstHole();

					//try removing middle wall in dead end
					const Direction middleWallDir = static_cast<Direction>((firstHole + 2) % Direction::Count);
					if (!CarvePath(i, j, middleWallDir, _withDoors))
					{
						//if carvePath fails, it's because we are on an edge
						//try removing the first non-edge wall
						Direction nextDir = middleWallDir;
						do
						{
							nextDir = static_cast<Direction>((nextDir + 1) % Direction::Count);
						} while (!CarvePath(i, j, nextDir, false));
					}
				}
			}
		}

		bool IsInMaze(int _x, int _y) const
		{
			return _x >= 0 && _x < Width()
				&& _y >= 0 && _y < Height();
		}

		const CellData& GetCell(int _x, int _y) const { return GetCellInternal(_x, _y); }

		virtual int Width() const = 0;
		virtual int Height() const = 0;

	protected:

		virtual CellData& GetCellInternal(int _x, int _y) = 0;
		virtual const CellData& GetCellInternal(int _x, int _y) const = 0;

		void Reset(bool _allWalls = true)
		{
			for (int i = 0; i < Width(); ++i)
				for (int j = 0; j < Height(); ++j)
					GetCellInternal(i, j).Reset(_allWalls);
		}

		void CarvePathRecursive(int _fromX, int _fromY)
		{
			array<Direction, Direction::Count> directions = { North, East, South, West };

			std::shuffle(directions.begin(), directions.end(), g);

			for (auto dir : directions)
			{
				auto cellInDir = CellInDir(_fromX, _fromY, dir);
				if (!cellInDir)
					continue;

				if (!cellInDir->IsFullyWalled())
					continue;

				CarvePath(_fromX, _fromY, dir);
				CarvePathRecursive(_fromX + xDelta[dir], _fromY + yDelta[dir]);
			}
		}

		bool CarvePath(int _fromX, int _fromY, Direction _dir, bool _withDoor = false)
		{
			auto nextCell = CellInDir(_fromX, _fromY, _dir);

			if (!nextCell)
				return false;

			//if path is already carved return false
			if (!GetCell(_fromX, _fromY).HasWall(_dir))
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

		constexpr static Direction Opposite(Direction _dir) { return static_cast<Direction>((_dir + 2) % Direction::Count); }
	};

	template<int WIDTH, int HEIGHT>
	class Maze : public BaseMaze
	{
	
	public:
		Maze() : BaseMaze()
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


	class Monster
	{
	public:
		Monster(const BaseMaze& _maze)
			: mMaze(_maze)
			, mPosX(0)
			, mPosY(0)
		{
			mVisitedCount.resize(mMaze.Width());
			
			for (int i = 0; i < mMaze.Width(); ++i)
				mVisitedCount[i].resize(mMaze.Height(), 0);
		}

		void Move(Direction _dir)
		{
			mVisitedCount[mPosX][mPosY]++;
			mPosX += xDelta[_dir];
			mPosY += yDelta[_dir];
		}

		void Wander()
		{
			vector<pair<Direction, int>> possibilities;
			possibilities.reserve(Direction::Count);

			const int randOffset = rand();

			for (int i = 0; i < Direction::Count; ++i)
			{
				Direction curDir = static_cast<Direction>((i + randOffset) % Direction::Count);
				
				if (!CanTravelTo(curDir))
				{
					possibilities.emplace_back(curDir, std::numeric_limits<int>::max());
					continue;
				}

				const int nextPosX = mPosX + xDelta[curDir];
				const int nextPosY = mPosY + yDelta[curDir];
				possibilities.emplace_back(curDir, mVisitedCount[nextPosX][nextPosY]);
			}

			std::sort(begin(possibilities), end(possibilities), [](const auto& _lhs, const auto& _rhs) { return _lhs.second < _rhs.second; });

			assert(possibilities[0].second < std::numeric_limits<int>::max());
			Move(possibilities[0].first);
		}


		vector<Direction> HomeIn(int _targetX, int _targetY)
		{
			vector<Direction> bestPath;
			FindBestPath(mPosX, mPosY, _targetX, _targetY, bestPath);

			if(!bestPath.empty())
				Move(bestPath[0]);

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

		void FindBestPath(int _fromX, int _fromY, int _toX, int _toY, vector<Direction>& _path) const
		{
			assert(mMaze.IsInMaze(_fromX, _fromY) && mMaze.IsInMaze(_toX, _toY));

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

				mMaze.GetCell(current.first, current.second).GetNeighbours(neighbours);
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
				const auto prev = cameFrom[current];
				_path.push_back(getDir(prev, current));
				current = prev;
			}
			std::reverse(_path.begin(), _path.end());
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

			return !mMaze.GetCell(mPosX, mPosY).HasWall(_dir);
		}

		int mPosX;
		int mPosY;

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
		grid[curX][curY] = 1;
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

	Maze::Maze<10, 10> baseMaze;
	auto braidedMaze = baseMaze;
	braidedMaze.Braid(true);

	int targetX = rand() % baseMaze.Width();
	int targetY = rand() % baseMaze.Height();

	//auto monster = new Monster(braidedMaze);
	auto monster = std::make_unique<Maze::Monster>(static_cast<Maze::BaseMaze&>(braidedMaze));
	
	std::string s;

	int step = 0;

	do
	{
		/*if(step == 0)
			baseMaze.Generate();
		else
			baseMaze.Braid();*/

		if (s == "g")
		{
			baseMaze.Generate();
			braidedMaze = baseMaze;
			braidedMaze.Braid(true);
			monster.release();
			monster = std::make_unique<Maze::Monster>(static_cast<Maze::BaseMaze&>(braidedMaze));
		}
		else if (s == "t")
		{
			targetX = rand() % baseMaze.Width();
			targetY = rand() % baseMaze.Height();
		}

		system("cls");

		OutputToConsole(braidedMaze, *monster);

		step = (step + 1) % 2;

		//monster.Wander();
		const auto prevPosX = monster->GetPosX();
		const auto prevPosY = monster->GetPosY();
		auto path = monster->HomeIn(targetX, targetY);
		DrawPath(prevPosX, prevPosY, braidedMaze, path);


	} while (getline(std::cin, s));

    return 0;
}

