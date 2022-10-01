using UnityEngine;

public class GameView : MonoBehaviour
{
	public CellView CellViewPrefab;

	private CellView[] _cells;

	private Field _field;

	void Start()
	{
		_field = new Field(10, 10);

		_cells = new CellView[_field.CellsCount];

		for (int i = 0; i < _field.CellsCount; i++)
		{
			var newCellView = Instantiate(CellViewPrefab);
			newCellView.transform.SetParent(transform, false);

			newCellView.Index = i;

			newCellView.OnPlayClick += CellView_OnPlayClick;
			newCellView.OnMarkClick += CellView_OnMarkClick;

			_cells[i] = newCellView;
		}


		_field.InitializeField(50, 10);
		//		field.PrintMatrix();

		RefreshMatrixView();
	}

	private void CellView_OnMarkClick(int index)
	{
		_field.Mark(index);
		RefreshMatrixView();
	}

	private void CellView_OnPlayClick(int index)
	{
		_field.Play(index);
		RefreshMatrixView();
	}

	private void RefreshMatrixView()
	{
		for (int i = 0; i < _field.CellsCount; i++)
		{
			if (_field.PlayerCells[i] == -2)
			{
				_cells[i].UpdateText("");
			}
			else if (_field.PlayerCells[i] == -1)
			{
				_cells[i].UpdateText("*");
			}
			else
			{
				_cells[i].UpdateText(_field.PlayerCells[i].ToString());
			}
		}
	}
}
