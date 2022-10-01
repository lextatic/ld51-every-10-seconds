using UnityEngine;

public class GameView : MonoBehaviour
{
	public CellView CellViewPrefab;

	private CellView[] _cells;

	private Field _field;

	void Start()
	{
		_field = new Field(10, 10);

		_cells = new CellView[_field.Cells.Length];

		for (int i = 0; i < _field.Cells.Length; i++)
		{
			var newCellView = Instantiate(CellViewPrefab);
			newCellView.transform.parent = transform;

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
		Debug.Log($"Mark: {index}");
		_field.Mark(index);
		RefreshMatrixView();
	}

	private void CellView_OnPlayClick(int index)
	{
		Debug.Log($"Play: {index}");
		_field.Play(index);
		RefreshMatrixView();
	}

	private void RefreshMatrixView()
	{
		for (int i = 0; i < _field.Cells.Length; i++)
		{
			_cells[i].UpdateText(_field.Cells[i].ToString());
		}
	}
}
