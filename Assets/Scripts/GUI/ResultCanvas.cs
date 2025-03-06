using DG.Tweening;
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
	Button _backButton, _retryButton, _nextButton;

	protected void Awake()
	{
		_scoreGrid = _scoreScrollRect.content.GetComponent<GridLayoutGroup>();
	}
	protected void OnRectTransformDimensionsChange()
	{
		InitGrid();
	}

	public void Init(ScoreContainer scoreTable, Action backButtonClicked, Action retryButtonClicked, Action nextButtonClicked)
	{
		//점수 패널 생성
		foreach (var score in scoreTable)
		{
			var newPanel = Instantiate(_scorePanelPrefab, _scoreScrollRect.content).GetComponent<ScorePanel>();
			newPanel.Init(score);
		}
		InitGrid();

		//버튼 바인딩
		TryBind(_backButton, backButtonClicked);
		TryBind(_retryButton, retryButtonClicked);
		TryBind(_nextButton, nextButtonClicked);
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

	/// <summary>
	/// 버튼 이벤트에 콜백 연결
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
		_resultPanel.Open();
	}
	public void Close()
	{
		_resultPanel.Close();
	}
}
