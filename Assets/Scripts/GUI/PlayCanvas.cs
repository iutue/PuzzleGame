using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static BlockGroupView;

/// <summary>
/// 매치의 주요 캔버스를 관리하는 창
/// </summary>
[RequireComponent(typeof(Canvas))]
public class PlayCanvas : UIBehaviour
{
	[field: SerializeField]
	public TopBarCanvas TopBar { get; private set; }
	[field: SerializeField]
	public MapView MapView { get; private set; }
	[field: SerializeField]
	public HandView HandView { get; private set; }
	[field: SerializeField]
	public ResultCanvas ResultPanel { get; private set; }

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
	}

	#region Game
	/// <summary>
	/// 게임 초기화 후 호출됨
	/// </summary>
	public void OnGameInitialized(GameState gameModeData,
		Action backButtonClicked, Action resetButtonClicked, Action nextButtonClicked,
		DragHandler beginDragCard, DragHandler endDragCard, DragHandler dragCard)
	{
		//탑바
		TopBar.Init(gameModeData.Scores["Total"], backButtonClicked, resetButtonClicked);
		//맵
		MapView.Init(gameModeData);
		//카드
		HandView.Init(gameModeData, beginDragCard, endDragCard, dragCard);
		//결과
		ResultPanel.Init(gameModeData.Scores, backButtonClicked, resetButtonClicked, nextButtonClicked);
	}

	/// <summary>
	/// 게임 시작 후 호출됨
	/// </summary>
	public void OnGameStarted()
	{
		//결과창 닫고
		ResultPanel.Close();
		//필수창 열기
		TopBar.Open();
		MapView.Open();
		HandView.Open();
	}

	/// <summary>
	/// 게임 종료 후 호출됨
	/// </summary>
	public void OnGameEnded()
	{
		//탑바만 숨기고
		TopBar.Close();
		//결과창 열기
		ResultPanel.Open();
	}

	/// <summary>
	/// 게임 리셋 후 호출됨
	/// </summary>
	public void OnGameReset()
	{

	}
	#endregion
}
