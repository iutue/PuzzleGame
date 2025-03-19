using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageView : UIBehaviour
{
	StageData _owner;

	[SerializeField]
	Button _enterButton;
	[SerializeField]
	TMP_Text _titleText;
	[SerializeField]
	Image _lockImage;

	/// <summary>
	/// 스테이지 입장 버튼이 클릭됐을 때
	/// </summary>
	Action<StageData> _clicekd;

	public void Init(StageData stage, Action<StageData> clicked)
	{
		_owner = stage;
		_clicekd = clicked;
		_titleText.text = (_owner.Index).ToString();
		_enterButton.onClick.AddListener(new UnityAction(OnClicked));

		string levelProgressKey = _owner.ParentLevel.GetPath().Append("_Progress").ToString();
		int savedProgress = PlayerPrefs.GetInt(levelProgressKey, 0);
		bool isOpened = _owner.Index <= savedProgress;
		_enterButton.interactable = isOpened;
		_titleText.enabled = isOpened;
		_lockImage.enabled = !isOpened;
	}

	void OnClicked()
	{
		_clicekd(_owner);
	}
}
