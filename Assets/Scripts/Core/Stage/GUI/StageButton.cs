using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageButton : UIBehaviour
{
	[SerializeField]
	TMP_Text _stageTitleText;
	[SerializeField]
	Button _playButton;

	int _stageIndex;
	Action<int> _clicekd;

	public void Init(int stageIndex, StageData stageData, Action<int> clicked)
	{
		_stageIndex = stageIndex;
		_clicekd = clicked;
		_stageTitleText.text = stageIndex.ToString();
		_playButton.onClick.AddListener(new UnityAction(OnClicked));
	}

	void OnClicked()
	{
		_clicekd(_stageIndex);
	}
}
