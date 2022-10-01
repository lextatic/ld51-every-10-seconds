using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellView : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
	public int Index;
	public TextMeshProUGUI Text;

	public event Action<int> OnPlayClick;
	public event Action<int> OnMarkClick;

	public void OnPointerClick(PointerEventData eventData)
	{

	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			OnMarkClick.Invoke(Index);
			//Field.Mark(Index);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			OnPlayClick.Invoke(Index);
			//Field.Play(Index);
		}
	}

	public void UpdateText(string newText)
	{
		Text.text = newText;
	}
}
