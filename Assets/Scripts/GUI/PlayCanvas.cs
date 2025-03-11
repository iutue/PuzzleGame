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
	public TopBarCanvas TopBarCanvas { get; private set; }
	[field: SerializeField]
	public MapCanvas MapCanvas { get; private set; }
	[field: SerializeField]
	public HandCanvas HandCanvas { get; private set; }
	//TODO PlayCanvas 내 자식 캔버스로 옮기기
	[field: SerializeField]
	public ResultCanvas ResultCanvas { get; private set; }

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
	}

	#region Game
	/// <summary>
	/// 게임 초기화 후 호출됨
	/// </summary>
	public void OnGameInitialized(GameModeSetting gameModeSetting, ScoreContainer scoreTable,
		Action backButtonClicked, Action resetButtonClicked, Action nextButtonClicked,
		DragHandler beginDragCard, DragHandler endDragCard, DragHandler dragCard)
	{
		//탑바
		TopBarCanvas.Init(scoreTable["Total"], backButtonClicked, resetButtonClicked);
		//맵
		MapCanvas.Init(gameModeSetting);
		//카드
		HandCanvas.Init(gameModeSetting, beginDragCard, endDragCard, dragCard);
		//결과
		ResultCanvas.Init(scoreTable, backButtonClicked, resetButtonClicked, nextButtonClicked);
	}

	/// <summary>
	/// 게임 시작 후 호출됨
	/// </summary>
	public void OnGameStarted()
	{
		//결과창 닫고
		ResultCanvas.Close();
		//필수창 열기
		TopBarCanvas.Open();
		MapCanvas.Open();
		HandCanvas.Open();
	}

	/// <summary>
	/// 게임 종료 후 호출됨
	/// </summary>
	public void OnGameEnded()
	{
		//탑바만 숨기고
		TopBarCanvas.Close();
		//결과창 열기
		ResultCanvas.Open();
	}

	/// <summary>
	/// 게임 리셋 후 호출됨
	/// </summary>
	public void OnGameReset()
	{

	}
	#endregion
}
