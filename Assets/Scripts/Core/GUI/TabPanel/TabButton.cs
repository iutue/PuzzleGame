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
	/// <summary>
	/// 이름
	/// </summary>
	[SerializeField]
	LocalizedString _displayName;
	[SerializeField]
	TMP_Text _displayNameText;
	/// <summary>
	/// 아이콘
	/// </summary>
	[SerializeField]
	Sprite _icon;
	[SerializeField]
	Image _iconImage;
	/// <summary>
	/// 하이라이트 이미지
	/// </summary>
	[field: SerializeField]
	public Image Highlight { get; private set; }

	/// <summary>
	/// 그룹에서 이 탭의 번호
	/// </summary>
	int _index;
	Action<int> _clicked;

	public void Init(int index, Action<int> clicked)
	{
		_index = index;
		_clicked = clicked;
		GetComponent<Button>().onClick.AddListener(new UnityAction(OnClicked));
		_displayName.StringChanged += OnDisplayNameChanged;
	}

	private void OnDisplayNameChanged(string value)
	{
		_displayNameText.text = value;
	}

	/// <summary>
	/// 탭 버튼이 클릭됐을 때 호출됨
	/// </summary>
	void OnClicked() => _clicked(_index);
}
