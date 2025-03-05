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
		_backButton.onClick.AddListener(new UnityAction(backButtonClicked));
		_refreshButton.onClick.AddListener(new UnityAction(refreshButtonClicked));
	}

	void OnTotalScoreChanged(int oldValue, int newValue)
	{
		_scoreText.text = newValue.ToString();
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
