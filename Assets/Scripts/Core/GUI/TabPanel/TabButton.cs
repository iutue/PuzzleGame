using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;

/// <summary>
/// 탭을 열 때 사용하는 버튼 GUI
/// </summary>
[RequireComponent(typeof(Button))]
public class TabButton : UIBehaviour
{
	[SerializeField]
	LocalizedString _title;
	[SerializeField]
	GameObject _titlePanel;
	[SerializeField]
	TMP_Text _titleText;

	[SerializeField]
	Sprite _icon;
	[SerializeField]
	GameObject _iconPanel;
	[SerializeField]
	Image _iconImage;

	[field: SerializeField]
	public Image Highlight { get; private set; }

	/// <summary>
	/// 속한 탭 그룹에서 이 버튼의 순서
	/// </summary>
	int _index;
	Action<int> _clicked;

	public void Init(int index, Action<int> clicked)
	{
		_index = index;
		_clicked = clicked;
		GetComponent<Button>().onClick.AddListener(new UnityAction(OnClicked));

		_titlePanel.SetActive(_title != null);
		if (_title != null)
		{
			_title.StringChanged += OnTitleChanged;
		}

		_iconPanel.SetActive(_icon != null);
		_iconImage.sprite = _icon;
	}

	#region Callbakcs
	void OnTitleChanged(string value)
	{
		_titleText.text = value;
	}

	void OnClicked() => _clicked(_index);
	#endregion
}
