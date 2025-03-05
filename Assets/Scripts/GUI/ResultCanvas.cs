using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class ResultCanvas : MonoBehaviour
{
	[SerializeField]
	Vector2 _constraintCount;

	[SerializeField]
	SlidePanel _resultPanel;

	[SerializeField]
	ScrollRect _scoreScrollRect;
	GridLayoutGroup _scoreGrid;

	[SerializeField]
	GameObject _scorePanelPrefab;

	[SerializeField]
	ScorePanel _bestRecordPanel, _totalScorePanel;

	[SerializeField]
	Button _backButton, _retryButton, _nextButton;

	protected void Awake()
	{
		_scoreGrid = _scoreScrollRect.content.GetComponent<GridLayoutGroup>();
	}
	protected void OnRectTransformDimensionsChange()
	{
		InitGrid();
	}

	public void Init(ScoreTable scoreTable, Action backButtonClicked, Action retryButtonClicked, Action nextButtonClicked)
	{
		//점수 패널 생성
		foreach (var score in scoreTable)
		{
			//최고 기록
			if (score.Type.Name.Equals("BestRecord"))
			{
				_bestRecordPanel.Init(score);
			}
			//총 점수
			else if (score.Type.Name.Equals("Total"))
			{
				_totalScorePanel.Init(score);
			}
			//이외
			else
			{
				var newPanel = Instantiate(_scorePanelPrefab, _scoreScrollRect.content).GetComponent<ScorePanel>();
				newPanel.Init(score);
			}
		}
		InitGrid();

		_backButton.onClick.AddListener(new UnityAction(backButtonClicked));
		_retryButton.onClick.AddListener(new UnityAction(retryButtonClicked));
		_nextButton.onClick.AddListener(new UnityAction(nextButtonClicked));
	}
	void InitGrid()
	{
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(InitGridCoroutine());
		}
	}
	IEnumerator InitGridCoroutine()
	{
		yield return new WaitForEndOfFrame();
		Vector2 scorePanelSize = _scoreScrollRect.viewport.rect.size;
		scorePanelSize -= _scoreGrid.spacing * (_constraintCount);
		scorePanelSize.x -= _scoreGrid.padding.left + _scoreGrid.padding.right;
		scorePanelSize.y -= _scoreGrid.padding.top + _scoreGrid.padding.bottom;
		scorePanelSize /= _constraintCount;
		_scoreGrid.cellSize = scorePanelSize;
	}

	public void Open()
	{
		_resultPanel.Open();
	}
	public void Close()
	{
		_resultPanel.Close();
	}
}
