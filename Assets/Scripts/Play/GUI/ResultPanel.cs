using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 매치 결과를 보여주는 창
/// </summary>
public class ResultPanel : UIBehaviour
{
	[SerializeField]
	SlidePanel _panel;
	[SerializeField]
	ScrollRect _scoreRect;
	/// <summary>
	/// 한 번에 볼 수 있는 점수 패널의 개수
	/// </summary>
	[SerializeField]
	Vector2 _constraintCount;
	[SerializeField]
	GameObject _scorePanelPrefab;

	[Space]
	[SerializeField] Button _backButton;
	[SerializeField] Button _resetButton;
	[SerializeField] Button _nextButton;

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		InitGrid();
	}

	public void Init(ScoreContainer scores, Action backButtonClicked, Action resetButtonClicked, Action nextButtonClicked)
	{
		//점수 패널 생성
		foreach (var score in scores)
		{
			var newPanel = Instantiate(_scorePanelPrefab, _scoreRect.content).GetComponent<ScoreView>();
			newPanel.Init(score);
		}
		InitGrid();
		//버튼 바인딩
		TryBind(_backButton, backButtonClicked);
		TryBind(_resetButton, resetButtonClicked);
		TryBind(_nextButton, nextButtonClicked);
	}
	async Awaitable InitGrid()
	{
		//레이아웃이 초기화될 때까지 대기
		await Awaitable.EndOfFrameAsync();
		//점수 패널의 크기 조절
		Vector2 scorePanelSize = _scoreRect.viewport.rect.size;
		GridLayoutGroup grid = _scoreRect.content.GetComponent<GridLayoutGroup>();
		scorePanelSize -= grid.spacing * (_constraintCount);
		scorePanelSize.x -= grid.padding.left + grid.padding.right;
		scorePanelSize.y -= grid.padding.top + grid.padding.bottom;
		scorePanelSize /= _constraintCount;
		grid.cellSize = scorePanelSize;
	}

	/// <summary>
	/// 버튼 클릭 이벤트에 콜백 함수 연결
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
		_panel.Open();
	}
	public void Close()
	{
		_panel.Close();
	}
}
