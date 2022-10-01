using System.Linq;
using System.Text;

// -1 bomb
// 0 empty
// 1 ... 8 numbers of bombs
//

public class Field
{
	public int Rows;
	public int Lines;

	public sbyte[] Cells;

	public Field(int rows, int lines)
	{
		Rows = rows;
		Lines = lines;

		Cells = new sbyte[rows * lines];
	}

	public void InitializeField(int firstPlayIndex, int bombs)
	{
		var random = new System.Random();

		var indexes = Enumerable.Range(0, Rows * Lines).Except(Enumerable.Repeat(firstPlayIndex, 1)).OrderBy(x => random.Next()).Take(bombs);

		foreach (var index in indexes)
		{
			Cells[index] = -1;
			NumberNeighbouringCells(index);
			PrintMatrix(); // debug
		}

		Play(firstPlayIndex);
		PrintMatrix(); // debug
	}

	public void Mark(int index)
	{
		if (Cells[index] < 9)
		{
			Cells[index] += 10;
		}
	}

	public void Play(int index)
	{
		if (Cells[index] == -1)
		{
			UnityEngine.Debug.Log("Game over");
			return;
		}

		if (Cells[index] >= 9)
		{
			return; // Flagged
		}

		Cells[index] += 20;

		if (Cells[index] == 20)
		{
			// Play Neighbours
			PlayNeighnbouringCells(index);
		}
	}

	public void PrintMatrix()
	{
		var stringBuilder = new StringBuilder();

		for (int i = 0; i < Cells.Length; i++)
		{
			stringBuilder.Append($"{Cells[i]:0;X;*}, ");

			if (i % Rows == Rows - 1)
			{
				stringBuilder.AppendLine();
			}
		}

		UnityEngine.Debug.Log(stringBuilder.ToString());
	}

	private void PlayNeighnbouringCells(int index)
	{
		var freeLeft = false;
		if (index % Rows > 0)
		{
			if (Cells[index - 1] != -1)
			{
				Play(index - 1); //left
			}

			freeLeft = true;
		}

		var freeRight = false;
		if (index % Rows < Rows - 1)
		{
			if (Cells[index + 1] != -1)
			{
				Play(index + 1); //right
			}

			freeRight = true;
		}

		if (index >= Rows)
		{
			if (Cells[index - Rows] != -1)
			{
				Play(index - Rows); //up
			}

			if (freeLeft && Cells[index - Rows - 1] != -1)
			{
				Play(index - Rows - 1);
			}

			if (freeRight && Cells[index - Rows + 1] != -1)
			{
				Play(index - Rows + 1);
			}
		}

		if (index < (Rows * Lines) - Rows - 1)
		{
			if (Cells[index + Rows] != -1)
			{
				Play(index + Rows); //down
			}

			if (freeLeft && Cells[index + Rows - 1] != -1)
			{
				Play(index + Rows - 1);
			}

			if (freeRight && Cells[index + Rows + 1] != -1)
			{
				Play(index + Rows + 1);
			}
		}
	}

	private void NumberNeighbouringCells(int index)
	{
		var freeLeft = false;
		if (index % Rows > 0)
		{
			if (Cells[index - 1] != -1)
			{
				Cells[index - 1]++; //left
			}

			freeLeft = true;
		}

		var freeRight = false;
		if (index % Rows < Rows - 1)
		{
			if (Cells[index + 1] != -1)
			{
				Cells[index + 1]++; //right
			}

			freeRight = true;
		}

		if (index >= Rows)
		{
			if (Cells[index - Rows] != -1)
			{
				Cells[index - Rows]++; //up
			}

			if (freeLeft && Cells[index - Rows - 1] != -1)
			{
				Cells[index - Rows - 1]++;
			}

			if (freeRight && Cells[index - Rows + 1] != -1)
			{
				Cells[index - Rows + 1]++;
			}
		}

		if (index < (Rows * Lines) - Rows - 1)
		{
			if (Cells[index + Rows] != -1)
			{
				Cells[index + Rows]++; //down
			}

			if (freeLeft && Cells[index + Rows - 1] != -1)
			{
				Cells[index + Rows - 1]++;
			}

			if (freeRight && Cells[index + Rows + 1] != -1)
			{
				Cells[index + Rows + 1]++;
			}
		}
	}
}
