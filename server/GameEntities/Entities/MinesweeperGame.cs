using GameBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEntities.Entities
{
	// -1 bomb
	// 0 empty
	// 1 ... 8 numbers of bombs
	//

	[Serializable]
	public class MinesweeperGame : BaseEntity
	{
		public sbyte[] PlayerCells { get; set; }
		public int CellsCount => _cells.Length;

		private readonly sbyte[] _cells;

		private readonly int _columns;
		private readonly int _rows;

		private int _cellsLeft;
		private int _flagsLeft;

		private bool _initialized;

		public MinesweeperGame(int columns, int rows)
		{
			_columns = columns;
			_rows = rows;

			_cells = new sbyte[columns * rows];
			PlayerCells = new sbyte[columns * rows];

			for (int i = 0; i < PlayerCells.Length; i++)
			{
				PlayerCells[i] = -2;
			}

			_initialized = false;
		}

		public void Mark(int index)
		{
			if (_cells[index] < 9)
			{
				if (_flagsLeft <= 0)
				{
					return;
				}

				_cells[index] += 10;
				PlayerCells[index] = -1;
				_flagsLeft--;
			}
			else if (_cells[index] < 19)
			{
				_cells[index] -= 10;
				PlayerCells[index] = -2;
				_flagsLeft++;
			}
		}


		public int Play(int index)
		{
			if (!_initialized)
			{
				//InitializeField(index, 1); // for tests (instant win)
				InitializeField(index, 10);
				_initialized = true;
			}

			if (_cells[index] == -1)
			{
				Console.WriteLine("Game over");

				for (int i = 0; i < _cells.Length; i++)
				{
					if (_cells[i] == -1)
					{
						PlayerCells[i] = -3; // missing bomb
					}
					else if (_cells[i] > 9 && _cells[i] <= 18)
					{
						PlayerCells[i] = -4; // wrong flag
					}
				}

				PlayerCells[index] = -5; // clicked wrong bomb

				return -1;
			}

			if (_cells[index] >= 9)
			{
				return 0; // flagged or revealed
			}

			_cells[index] += 20;
			PlayerCells[index] = (sbyte)(_cells[index] - 20);

			_cellsLeft--;

			if (_cellsLeft <= 0)
			{
				Console.WriteLine("Victory");

				for (int i = 0; i < _cells.Length; i++)
				{
					if (_cells[i] == -1)
					{
						PlayerCells[i] = -1;
						_flagsLeft--;
					}
				}

				return 1;
			}

			if (_cells[index] == 20)
			{
				return NeighnbouringCellsAction(index, Play);
			}

			return 0;
		}

		public int SmartPlay(int index)
		{
			if (!_initialized)
			{
				return 0;
			}

			// Only click on numbers from 1 to 7
			var number = _cells[index];
			if (number >= 21 && number <= 27)
			{
				int flagsPlaced = 0;
				NeighnbouringCellsAction(index, cellIndex =>
				{
					if (_cells[cellIndex] >= 9 && _cells[cellIndex] < 19)
					{
						flagsPlaced++;
					}

					return 0;
				});

				if (flagsPlaced + 20 == number)
				{
					return NeighnbouringCellsAction(index, cellIndex =>
					{
						if (_cells[cellIndex] < 9)
						{
							Play(cellIndex);
						}

						return 0;
					}, -99);
				}
			}

			return 0;
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

			for (int i = 0; i < _cells.Length; i++)
			{
				stringBuilder.Append($"{_cells[i]:+0;-0; *}, ");

				if (i % _columns == _columns - 1)
				{
					stringBuilder.AppendLine();
				}
			}

			return stringBuilder.ToString();
		}

		public string ToStringPlayer()
		{
			var stringBuilder = new StringBuilder();

			for (int i = 0; i < PlayerCells.Length; i++)
			{
				stringBuilder.Append($"{PlayerCells[i]:+0;-0; *}, ");

				if (i % _columns == _columns - 1)
				{
					stringBuilder.AppendLine();
				}
			}

			return stringBuilder.ToString();
		}

		private void InitializeField(int firstPlayIndex, int bombs)
		{
			var random = new Random();

			var neighbourIndexes = new List<int>() { firstPlayIndex };
			NeighnbouringCellsAction(firstPlayIndex, cellIndex =>
			{
				neighbourIndexes.Add(cellIndex);
				return 0;
			});

			var bombIndexes = Enumerable.Range(0, _columns * _rows).Except(neighbourIndexes).OrderBy(x => random.Next()).Take(bombs);

			foreach (var index in bombIndexes)
			{
				_cells[index] = -1;
				NeighnbouringCellsAction(index, cellIndex => _cells[cellIndex]++);
			}

			_cellsLeft = (_columns * _rows) - bombs;
			_flagsLeft = bombs;
		}

		private int NeighnbouringCellsAction(int index, Func<int, int> action, int ignoreCellValue = -1)
		{
			int result = 0;
			int output;

			var freeLeft = false;
			if (index % _columns > 0)  // left
			{
				if (_cells[index - 1] != ignoreCellValue)
				{
					output = action.Invoke(index - 1);
					if (output != 0)
					{
						result = output;
					}
				}

				freeLeft = true;
			}

			var freeRight = false;
			if (index % _columns < _columns - 1)  // right
			{
				if (_cells[index + 1] != ignoreCellValue)
				{
					output = action.Invoke(index + 1);
					if (output != 0)
					{
						result = output;
					}
				}

				freeRight = true;
			}

			if (index >= _columns) // up
			{
				if (_cells[index - _columns] != ignoreCellValue)
				{
					output = action.Invoke(index - _columns);
					if (output != 0)
					{
						result = output;
					}
				}

				if (freeLeft && _cells[index - _columns - 1] != ignoreCellValue) // upper left
				{
					output = action.Invoke(index - _columns - 1);
					if (output != 0)
					{
						result = output;
					}
				}

				if (freeRight && _cells[index - _columns + 1] != ignoreCellValue) // upper right
				{
					output = action.Invoke(index - _columns + 1);
					if (output != 0)
					{
						result = output;
					}
				}
			}

			if (index < (_columns * _rows) - _columns - 1)  // down
			{
				if (_cells[index + _columns] != ignoreCellValue)
				{
					output = action.Invoke(index + _columns);
					if (output != 0)
					{
						result = output;
					}
				}

				if (freeLeft && _cells[index + _columns - 1] != ignoreCellValue) // bottom left
				{
					output = action.Invoke(index + _columns - 1);
					if (output != 0)
					{
						result = output;
					}
				}

				if (freeRight && _cells[index + _columns + 1] != ignoreCellValue) // bottom right
				{
					output = action.Invoke(index + _columns + 1);
					if (output != 0)
					{
						result = output;
					}
				}
			}

			return result;
		}
	}
}
