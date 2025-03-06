using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PlayCanvas : MonoBehaviour
{
	[SerializeField]
	TopBarCanvas _topBar;
	[SerializeField]

	SlidePanel _map;
	[SerializeField]
	RectTransform _mapParent;
	BlockGroupView _mapView;

	[SerializeField]
	SlidePanel _cards;
	[SerializeField]
	RectTransform _cardParent;
	List<BlockGroupView> _cardViews;

	[SerializeField]
	GameObject _resultPrefab;
	ResultCanvas _result;

	public void Init(
		//TopBar
		ScoreContainer scoreTable, Action backButtonClicked, Action refreshButtonClicked
		//Map

		//Cards

		)
	{
		_topBar.Init(scoreTable["Total"], backButtonClicked, refreshButtonClicked);

	}

	public void StartGame()
	{
		_topBar.Open();
		_map.Open();
		_cards.Open();
		if (_result) _result.Close();
	}

	public void ShowResult(ScoreContainer scoreTable, Action backButtonClicked, Action retryButtonClicked, Action nextButtonClicked)
	{
		_topBar.Close();
		//결과 표시
		_result = Instantiate(_resultPrefab).GetComponent<ResultCanvas>();
		_result.Init(scoreTable, backButtonClicked, retryButtonClicked, nextButtonClicked);
	}
}
