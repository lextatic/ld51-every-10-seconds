using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public int Index;
	public TextMeshProUGUI Text;

	public event Action<int> OnPlayClick;
	public event Action<int> OnMarkClick;
	public event Action<int> OnSmartPlayClick;

	private bool _leftClick;
	private bool _rightClick;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			_leftClick = true;
		}

		if (eventData.button == PointerEventData.InputButton.Right)
		{
			_rightClick = true;
			OnMarkClick.Invoke(Index);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			_rightClick = false;

			if (_leftClick)
			{
				OnSmartPlayClick.Invoke(Index);
				return;
			}
		}

		if (eventData.button == PointerEventData.InputButton.Left)
		{
			_leftClick = false;

			if (_rightClick)
			{
				OnSmartPlayClick.Invoke(Index);
				return;
			}

			OnPlayClick.Invoke(Index);
		}
	}

	public void UpdateText(string newText)
	{
		Text.text = newText;
	}
}
