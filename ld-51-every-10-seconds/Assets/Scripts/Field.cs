using System;
using System.Linq;
using System.Text;

// -1 bomb
// 0 empty
// 1 ... 8 numbers of bombs
//

public class Field
{
	private readonly sbyte[] _cells;
	private readonly sbyte[] _playerCells;

	private readonly int _rows;
	private readonly int _lines;

	private int _cellsLeft;

	public int CellsCount => _cells.Length;

	public sbyte[] PlayerCells
	{
		get => _playerCells;

		private set
		{

		}
	}

	public Field(int rows, int lines)
	{
		_rows = rows;
		_lines = lines;

		_cells = new sbyte[rows * lines];
		_playerCells = new sbyte[rows * lines];
		Array.Fill<sbyte>(_playerCells, -2);
	}

	public void InitializeField(int firstPlayIndex, int bombs)
	{
		var random = new Random();
		var indexes = Enumerable.Range(0, _rows * _lines).Except(Enumerable.Repeat(firstPlayIndex, 1)).OrderBy(x => random.Next()).Take(bombs);

		foreach (var index in indexes)
		{
			_cells[index] = -1;
			NeighnbouringCellsAction(index, cellIndex => _cells[cellIndex]++);
		}

		_cellsLeft = (_rows * _lines) - bombs;

		Play(firstPlayIndex);
	}

	public void Mark(int index)
	{
		if (_cells[index] < 9)
		{
			_cells[index] += 10;
			_playerCells[index] = -1;
		}
		else if (_cells[index] < 19)
		{
			_cells[index] -= 10;
			_playerCells[index] = -2;
		}
	}

	public void Play(int index)
	{
		if (_cells[index] == -1)
		{
			UnityEngine.Debug.Log("Game over");
			return;
		}

		if (_cells[index] >= 9)
		{
			return; // flagged
		}

		_cells[index] += 20;
		_playerCells[index] = (sbyte)(_cells[index] - 20);

		_cellsLeft--;

		if (_cellsLeft <= 0)
		{
			UnityEngine.Debug.Log("Victory");
			return;
		}

		if (_cells[index] == 20)
		{
			NeighnbouringCellsAction(index, Play);
		}
	}

	public void PrintMatrix()
	{
		var stringBuilder = new StringBuilder();

		for (int i = 0; i < _cells.Length; i++)
		{
			stringBuilder.Append($"{_cells[i]:0;X;*}, ");

			if (i % _rows == _rows - 1)
			{
				stringBuilder.AppendLine();
			}
		}

		UnityEngine.Debug.Log(stringBuilder.ToString());
	}

	private void NeighnbouringCellsAction(int index, Action<int> action)
	{
		var freeLeft = false;
		if (index % _rows > 0)  // left
		{
			if (_cells[index - 1] != -1)
			{
				action.Invoke(index - 1);
			}

			freeLeft = true;
		}

		var freeRight = false;
		if (index % _rows < _rows - 1)  // right
		{
			if (_cells[index + 1] != -1)
			{
				action.Invoke(index + 1);
			}

			freeRight = true;
		}

		if (index >= _rows) // up
		{
			if (_cells[index - _rows] != -1)
			{
				action.Invoke(index - _rows);
			}

			if (freeLeft && _cells[index - _rows - 1] != -1) // upper left
			{
				action.Invoke(index - _rows - 1);
			}

			if (freeRight && _cells[index - _rows + 1] != -1) // upper right
			{
				action.Invoke(index - _rows + 1);
			}
		}

		if (index < (_rows * _lines) - _rows - 1)  // down
		{
			if (_cells[index + _rows] != -1)
			{
				action.Invoke(index + _rows);
			}

			if (freeLeft && _cells[index + _rows - 1] != -1) // bottom left
			{
				action.Invoke(index + _rows - 1);
			}

			if (freeRight && _cells[index + _rows + 1] != -1) // bottom right
			{
				action.Invoke(index + _rows + 1);
			}
		}
	}
}
