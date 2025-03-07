using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 점수의 상태를 표시하는 GUI
/// </summary>
public class ScoreView : UIBehaviour
{
	Score _ownerScore;

	[SerializeField]
	TMP_Text _name;
	[SerializeField]
	Image _icon;
	[SerializeField]
	TMP_Text _baseValue;
	[SerializeField]
	TMP_Text _multiplier;
	[SerializeField]
	TMP_Text _currentValue;

	public void Init(Score score)
	{
		_ownerScore = score;
		score.BaseValueChanged += OnValueChanged;

		//UI 초기화
		if (_name) _name.text = score.Type.DisplayName.GetLocalizedString();
		if (_icon) _icon.sprite = score.Type.Icon;
		if (_baseValue) _baseValue.text = score.BaseValue.ToString();
		if (_multiplier) _multiplier.text = score.Multiplier.ToString();
		if (_currentValue) _currentValue.text = score.CurrentValue.ToString();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_ownerScore.BaseValueChanged -= OnValueChanged;
	}

	/// <summary>
	/// 점수가 변경됐을 때 호출됨
	/// </summary>
	void OnValueChanged(Score score, int oldValue, int newValue)
	{
		//점수 텍스트 업데이트
		if (_baseValue) _baseValue.text = score.BaseValue.ToString();
		if (_currentValue) _currentValue.text = score.CurrentValue.ToString();
	}
}
