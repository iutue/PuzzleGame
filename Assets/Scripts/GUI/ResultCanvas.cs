using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class ResultCanvas : UIBehaviour
{
	[SerializeField]
	SlidePanel _resultPanel;
	[SerializeField]
	ScrollRect _scoreScrollRect;
	[SerializeField]
	Vector2 _constraintCount;
	[SerializeField]
	GameObject _scorePanelPrefab;
	[SerializeField]
	Button _backButton, _retryButton, _nextButton;

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
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
		//버튼 바인딩
		TryBind(_backButton, backButtonClicked);
		TryBind(_retryButton, retryButtonClicked);
		TryBind(_nextButton, nextButtonClicked);
	}
	async Awaitable InitGrid()
	{
		await Awaitable.EndOfFrameAsync();
		Vector2 scorePanelSize = _scoreScrollRect.viewport.rect.size;
		GridLayoutGroup grid = _scoreScrollRect.content.GetComponent<GridLayoutGroup>();
		scorePanelSize -= grid.spacing * (_constraintCount);
		scorePanelSize.x -= grid.padding.left + grid.padding.right;
		scorePanelSize.y -= grid.padding.top + grid.padding.bottom;
		scorePanelSize /= _constraintCount;
		grid.cellSize = scorePanelSize;
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
