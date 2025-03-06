using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class TopBarCanvas : MonoBehaviour
{
	[SerializeField]
	SlidePanel _topBarPanel;

	[SerializeField]
	TMP_Text _scoreText;
	[SerializeField]
	Button _backButton;
	[SerializeField]
	Button _refreshButton;

	public void Init(Score totalScore, Action backButtonClicked, Action refreshButtonClicked)
	{
		totalScore.Changed += OnTotalScoreChanged;
		TryBind(_backButton, backButtonClicked);
		TryBind(_refreshButton, refreshButtonClicked);
	}

	void OnTotalScoreChanged(Score score, int oldValue, int newValue)
	{
		_scoreText.text = newValue.ToString();
		DOTween.Sequence()
			.Append(_scoreText.rectTransform.DOScale(1.2f, 0.1f))
			.Append(_scoreText.rectTransform.DOScale(1f, 0.1f));
	}

	/// <summary>
	/// 버튼의 이벤트에 콜백 연결
	/// </summary>
	void TryBind(Button button, Action clicked)
	{
		if (clicked == null)
		{
			button.interactable = false;
			return;
		}
		button.onClick.AddListener(new UnityAction(clicked));
	}

	public void Open()
	{
		_topBarPanel.Open();
	}
	public void Close()
	{
		_topBarPanel.Close();
	}
}
