using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 점수 GUI
/// </summary>
public class ScorePanel : MonoBehaviour
{
	Score _ownerScore;

	[SerializeField]
	TMP_Text _nameText;
	[SerializeField]
	Image _valueIcon;
	[SerializeField]
	TMP_Text _baseValueText;
	[SerializeField]
	TMP_Text _multiplierText;
	[SerializeField]
	TMP_Text _currentValueText;

	public void Init(Score score)
	{
		_ownerScore = score;
		score.Changed += OnValueChanged;

		//UI 초기화
		if (_nameText) _nameText.text = score.Type.DisplayName.GetLocalizedString();
		if (_valueIcon) _valueIcon.sprite = score.Type.Icon;
		if (_baseValueText) _baseValueText.text = score.BaseValue.ToString();
		if (_multiplierText) _multiplierText.text = score.Multiplier.ToString();
		if (_currentValueText) _currentValueText.text = score.CurrentValue.ToString();
	}

	protected void OnDestroy()
	{
		_ownerScore.Changed -= OnValueChanged;
	}

	/// <summary>
	/// 점수가 변경됐을 때 호출됨
	/// </summary>
	void OnValueChanged(Score score, int oldValue, int newValue)
	{
		if (_currentValueText)
		{
			_currentValueText.text = newValue.ToString();
		}
	}
}
