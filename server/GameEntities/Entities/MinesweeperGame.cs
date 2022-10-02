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

		// It's being called multipletimes as events
		public Action OnVictory;
		public Action OnGameOver;

		private readonly sbyte[] _cells;

		private readonly int _columns;
		private readonly int _rows;

		private int _cellsLeft;

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
				_cells[index] += 10;
				PlayerCells[index] = -1;
			}
			else if (_cells[index] < 19)
			{
				_cells[index] -= 10;
				PlayerCells[index] = -2;
			}
		}


		public void Play(int index)
		{
			if (!_initialized)
			{
				InitializeField(index, 10);
				_initialized = true;
			}

			if (_cells[index] == -1)
			{
				Console.WriteLine("Game over");
				OnGameOver.Invoke();
				return;
			}

			if (_cells[index] >= 9)
			{
				return; // flagged
			}

			_cells[index] += 20;
			PlayerCells[index] = (sbyte)(_cells[index] - 20);

			_cellsLeft--;

			if (_cellsLeft <= 0)
			{
				Console.WriteLine("Victory");
				OnVictory.Invoke();
				return;
			}

			if (_cells[index] == 20)
			{
				NeighnbouringCellsAction(index, Play);
			}
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
			NeighnbouringCellsAction(firstPlayIndex, cellIndex => neighbourIndexes.Add(cellIndex));

			var bombIndexes = Enumerable.Range(0, _columns * _rows).Except(neighbourIndexes).OrderBy(x => random.Next()).Take(bombs);

			foreach (var index in bombIndexes)
			{
				_cells[index] = -1;
				NeighnbouringCellsAction(index, cellIndex => _cells[cellIndex]++);
			}

			_cellsLeft = (_columns * _rows) - bombs;
		}

		private void NeighnbouringCellsAction(int index, Action<int> action)
		{
			var freeLeft = false;
			if (index % _columns > 0)  // left
			{
				if (_cells[index - 1] != -1)
				{
					action.Invoke(index - 1);
				}

				freeLeft = true;
			}

			var freeRight = false;
			if (index % _columns < _columns - 1)  // right
			{
				if (_cells[index + 1] != -1)
				{
					action.Invoke(index + 1);
				}

				freeRight = true;
			}

			if (index >= _columns) // up
			{
				if (_cells[index - _columns] != -1)
				{
					action.Invoke(index - _columns);
				}

				if (freeLeft && _cells[index - _columns - 1] != -1) // upper left
				{
					action.Invoke(index - _columns - 1);
				}

				if (freeRight && _cells[index - _columns + 1] != -1) // upper right
				{
					action.Invoke(index - _columns + 1);
				}
			}

			if (index < (_columns * _rows) - _columns - 1)  // down
			{
				if (_cells[index + _columns] != -1)
				{
					action.Invoke(index + _columns);
				}

				if (freeLeft && _cells[index + _columns - 1] != -1) // bottom left
				{
					action.Invoke(index + _columns - 1);
				}

				if (freeRight && _cells[index + _columns + 1] != -1) // bottom right
				{
					action.Invoke(index + _columns + 1);
				}
			}
		}
	}
}
