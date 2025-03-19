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
	//이전
	[SerializeField] Button _backButton;
	public event Action BackButtonClicked;
	[SerializeField] Button _resetButton;
	//다시
	public event Action ResetButtonClicked;
	[SerializeField] Button _nextButton;
	//다음
	event Action _nextButtonClicked;
	public event Action NextButtonClicked
	{
		add
		{
			_nextButtonClicked += value;
			_nextButton.interactable = _nextButtonClicked != null;
		}
		remove
		{
			_nextButtonClicked -= value;
			_nextButton.interactable = _nextButtonClicked != null;
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		InitGrid();
	}

	public void Init(ScoreContainer scores)
	{
		//점수 패널 생성
		foreach (var score in scores)
		{
			var newPanel = Instantiate(_scorePanelPrefab, _scoreRect.content).GetComponent<ScoreView>();
			newPanel.Init(score);
		}
		InitGrid();

		_backButton.onClick.AddListener(new UnityAction(OnBackButtonClicked));
		_resetButton.onClick.AddListener(new UnityAction(OnResetButtonClicked));
		_nextButton.onClick.AddListener(new UnityAction(OnNextButtonClicked));
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

	public void Open()
	{
		_panel.Open();
	}
	public void Close()
	{
		_panel.Close();
		_nextButtonClicked = null;
		_nextButton.interactable = false;
	}

	void OnBackButtonClicked()
	{
		BackButtonClicked?.Invoke();
	}
	void OnResetButtonClicked()
	{
		ResetButtonClicked?.Invoke();
	}
	void OnNextButtonClicked()
	{
		_nextButtonClicked?.Invoke();
	}
}
