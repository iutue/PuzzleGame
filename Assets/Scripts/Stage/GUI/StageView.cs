using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageView : UIBehaviour
{
	[SerializeField]
	TMP_Text _stageTitleText;
	[SerializeField]
	Button _enterButton;

	int _stageIndex;
	Action<int> _clicekd;

	public void Init(int stageIndex, StageData stageData, Action<int> clicked)
	{
		_stageIndex = stageIndex;
		_clicekd = clicked;
		_stageTitleText.text = (stageIndex + 1).ToString();
		_enterButton.onClick.AddListener(new UnityAction(OnClicked));
	}

	void OnClicked()
	{
		_clicekd(_stageIndex);
	}
}
