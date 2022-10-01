using GameEntities.Entities;
using System;
using UnityEngine;

public class GameView : MonoBehaviour
{
	public CellView CellViewPrefab;

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
		for (int i = 0; i < minesweeperGame.PlayerCells.Length; i++)
		{
			if (minesweeperGame.PlayerCells[i] == -2)
			{
				_cells[i].UpdateText("");
			}
			else if (minesweeperGame.PlayerCells[i] == -1)
			{
				_cells[i].UpdateText("*");
			}
			else
			{
				_cells[i].UpdateText(minesweeperGame.PlayerCells[i].ToString());
			}
		}
	}
}
