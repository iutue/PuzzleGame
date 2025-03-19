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
public class TopBar : UIBehaviour
{
	[SerializeField]
	SlidePanel _panel;
	[SerializeField]
	TMP_Text _scoreText;

	//이전
	[SerializeField]
	Button _backButton;
	public event Action BackButtonClicekd;
	//다시
	[SerializeField]
	Button _resetButton;
	public event Action ResetButtonClicekd;

	public void Init(Score totalScore)
	{
		totalScore.BaseValueChanged += OnTotalScoreChanged;
		_backButton.onClick.AddListener(new UnityAction(OnBackButtonClicekd));
		_resetButton.onClick.AddListener(new UnityAction(OnResetButtonClicked));
	}

	public void Open() => _panel.Open();
	public void Close() => _panel.Close();

	#region Callbacks
	void OnBackButtonClicekd() => BackButtonClicekd?.Invoke();
	void OnResetButtonClicked() => ResetButtonClicekd?.Invoke();

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
	#endregion
}
