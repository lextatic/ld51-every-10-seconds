using GameEntities.Entities;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameView : MonoBehaviour
{
	public CellView CellViewPrefab;

	public TextMeshProUGUI Flags;
	public TextMeshProUGUI Timer;

	private CellView[] _cells;

	public event Action<int> OnPlayClick;
	public event Action<int> OnMarkClick;

	public void Initialize(MinesweeperGame minesweeperGame)
	{
		_cells = new CellView[minesweeperGame.PlayerCells.Length];

		for (int i = 0; i < minesweeperGame.PlayerCells.Length; i++)
		{
			var newCellView = Instantiate(CellViewPrefab);
			newCellView.transform.SetParent(transform, false);

			newCellView.Index = i;

			newCellView.OnPlayClick += CellView_OnPlayClick;
			newCellView.OnMarkClick += CellView_OnMarkClick;

			_cells[i] = newCellView;
		}

		RefreshMatrixView(minesweeperGame);
	}

	public void StartCount()
	{
		StartCoroutine(CountCoroutine());
	}

	private IEnumerator CountCoroutine()
	{
		Timer.color = Color.white;

		for (int timer = 10; timer >= 1; timer--)
		{
			Timer.text = timer.ToString();

			if (timer <= 3)
			{
				Timer.color = Color.red;
			}

			yield return new WaitForSeconds(1);
		}
	}

	private void CellView_OnMarkClick(int index)
	{
		OnMarkClick.Invoke(index);
	}

	private void CellView_OnPlayClick(int index)
	{
		OnPlayClick.Invoke(index);
	}

	public void RefreshMatrixView(MinesweeperGame minesweeperGame)
	{
		int flagsCount = 10;

		for (int i = 0; i < minesweeperGame.PlayerCells.Length; i++)
		{
			if (minesweeperGame.PlayerCells[i] == -2)
			{
				_cells[i].UpdateText("");
			}
			else if (minesweeperGame.PlayerCells[i] == -1)
			{
				_cells[i].UpdateText("*");
				flagsCount--;
			}
			else
			{
				_cells[i].UpdateText(minesweeperGame.PlayerCells[i].ToString());
			}
		}

		Flags.text = flagsCount.ToString();

		if (flagsCount <= 3)
		{
			Flags.color = Color.red;
		}
		else
		{
			Flags.color = Color.white;
		}
	}
}
