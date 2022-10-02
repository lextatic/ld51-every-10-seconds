using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public int Index;
	public TextMeshProUGUI Text;
	public Image Background;
	public Image Flag;
	public Image Bomb;
	public Sprite RevealedSprite;
	public Sprite ClosedSprite;

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

	public void UpdateVisual(int newValue)
	{
		Text.text = "";
		Background.sprite = ClosedSprite;
		Flag.gameObject.SetActive(false);
		Bomb.gameObject.SetActive(false);

		switch (newValue)
		{
			case -2:
				// Closed
				break;

			case -1:
				// Flag
				Flag.gameObject.SetActive(true);
				break;

			case 0:
				// Empty
				Background.sprite = RevealedSprite;
				break;

			default:
				// Number
				Background.sprite = RevealedSprite;
				Text.text = newValue.ToString();
				break;
		}
	}
}
