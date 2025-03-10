using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 매치 관련 버튼과 매치 상태를 보여주는 바
/// </summary>
[RequireComponent(typeof(Canvas))]
public class TopBarCanvas : UIBehaviour
{
	[SerializeField]
	SlidePanel _topBar;

	[SerializeField]
	TMP_Text _scoreText;
	[SerializeField]
	Button _backButton;
	[SerializeField]
	Button _resetButton;

	public void Init(Score totalScore, Action backButtonClicked, Action resetButtonClicked)
	{
		totalScore.BaseValueChanged += OnTotalScoreChanged;
		TryBind(_backButton, backButtonClicked);
		TryBind(_resetButton, resetButtonClicked);
	}

	/// <summary>
	/// 총점이 변경됐을 때 호출됨
	/// </summary>
	void OnTotalScoreChanged(Score score, int oldValue, int newValue)
	{
		_scoreText.text = newValue.ToString();
		DOTween.Sequence()
			.Append(_scoreText.rectTransform.DOScale(1.2f, 0.1f))
			.Append(_scoreText.rectTransform.DOScale(1f, 0.1f));
	}

	/// <summary>
	/// 버튼의 클릭 이벤트에 콜백 함수 연결
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
		_topBar.Open();
	}
	public void Close()
	{
		_topBar.Close();
	}
}
