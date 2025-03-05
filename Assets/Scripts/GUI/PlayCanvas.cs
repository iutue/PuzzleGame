using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
public class PlayCanvas : MonoBehaviour
{
	[SerializeField]
	TopBarCanvas _topBar;
	[SerializeField]
	SlidePanel _map;
	[SerializeField]
	SlidePanel _cards;

	[SerializeField]
	GameObject _resultPrefab;
	ResultCanvas _result;

	public void Init(Score totalScore, Action backButtonClicked, Action refreshButtonClicked)
	{
		_topBar.Init(totalScore, backButtonClicked, refreshButtonClicked);

	}

	public void StartGame()
	{
		_topBar.Open();
		if (_result) _result.Close();
	}

	public void ShowResult(ScoreTable scoreTable, Action backButtonClicked, Action retryButtonClicked, Action nextButtonClicked)
	{
		_topBar.Close();
		_cards.Close();
		//결과창 생성
		_result = Instantiate(_resultPrefab).GetComponent<ResultCanvas>();
		_result.Init(scoreTable, backButtonClicked, retryButtonClicked, nextButtonClicked);
	}
}
