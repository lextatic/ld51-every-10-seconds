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

	public GameObject Hourglass;

	private CellView[] _cells;

	public event Action<int> OnPlayClick;
	public event Action<int> OnMarkClick;
	public event Action<int> OnSmartPlayClick;

	private Coroutine _coroutine;

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
			newCellView.OnSmartPlayClick += CellView_OnSmartPlayClick;

			_cells[i] = newCellView;
		}

		RefreshMatrixView(minesweeperGame);
	}

	public void ShowHourglass()
	{
		Hourglass.SetActive(true);
	}

	public void StartCount()
	{
		Hourglass.SetActive(false);
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
		}
		_coroutine = StartCoroutine(CountCoroutine());
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

		_coroutine = StartCoroutine(CountCoroutine());
	}

	private void CellView_OnMarkClick(int index)
	{
		OnMarkClick.Invoke(index);
	}

	private void CellView_OnPlayClick(int index)
	{
		OnPlayClick.Invoke(index);
	}

	private void CellView_OnSmartPlayClick(int index)
	{
		OnSmartPlayClick.Invoke(index);
	}

	public void RefreshMatrixView(MinesweeperGame minesweeperGame)
	{
		int flagsCount = 10;

		for (int i = 0; i < minesweeperGame.PlayerCells.Length; i++)
		{
			_cells[i].UpdateVisual(minesweeperGame.PlayerCells[i], ref flagsCount);
		}

		Flags.text = flagsCount.ToString();

		if (flagsCount <= 3)
		{
			Flags.color = Color.yellow;
		}
		else
		{
			Flags.color = Color.white;
		}
	}
}
