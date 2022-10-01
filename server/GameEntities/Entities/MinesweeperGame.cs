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
		public int Columns { get; set; }


		private readonly sbyte[] _cells;

		private readonly int _rows;

		private int _cellsLeft;

		private bool _initialized;

		public int CellsCount => _cells.Length;

		public MinesweeperGame(int columns, int rows)
		{
			Columns = columns;
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

				if (i % Columns == Columns - 1)
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

				if (i % Columns == Columns - 1)
				{
					stringBuilder.AppendLine();
				}
			}

			return stringBuilder.ToString();
		}

		private void InitializeField(int firstPlayIndex, int bombs)
		{
			var random = new Random();

			var neighbourIndexes = new List<int>(firstPlayIndex);
			NeighnbouringCellsAction(firstPlayIndex, cellIndex => neighbourIndexes.Add(cellIndex));

			var indexes = Enumerable.Range(0, Columns * _rows).Except(neighbourIndexes).OrderBy(x => random.Next()).Take(bombs);

			foreach (var index in indexes)
			{
				_cells[index] = -1;
				NeighnbouringCellsAction(index, cellIndex => _cells[cellIndex]++);
			}

			_cellsLeft = (Columns * _rows) - bombs;
		}

		private void NeighnbouringCellsAction(int index, Action<int> action)
		{
			var freeLeft = false;
			if (index % Columns > 0)  // left
			{
				if (_cells[index - 1] != -1)
				{
					action.Invoke(index - 1);
				}

				freeLeft = true;
			}

			var freeRight = false;
			if (index % Columns < Columns - 1)  // right
			{
				if (_cells[index + 1] != -1)
				{
					action.Invoke(index + 1);
				}

				freeRight = true;
			}

			if (index >= Columns) // up
			{
				if (_cells[index - Columns] != -1)
				{
					action.Invoke(index - Columns);
				}

				if (freeLeft && _cells[index - Columns - 1] != -1) // upper left
				{
					action.Invoke(index - Columns - 1);
				}

				if (freeRight && _cells[index - Columns + 1] != -1) // upper right
				{
					action.Invoke(index - Columns + 1);
				}
			}

			if (index < (Columns * _rows) - Columns - 1)  // down
			{
				if (_cells[index + Columns] != -1)
				{
					action.Invoke(index + Columns);
				}

				if (freeLeft && _cells[index + Columns - 1] != -1) // bottom left
				{
					action.Invoke(index + Columns - 1);
				}

				if (freeRight && _cells[index + Columns + 1] != -1) // bottom right
				{
					action.Invoke(index + Columns + 1);
				}
			}
		}
	}

}
