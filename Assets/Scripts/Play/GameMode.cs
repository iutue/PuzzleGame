using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 점수, 규칙 등 매치의 상태를 가지고 하나의 매치를 관리함
/// </summary>
public abstract class GameMode : MonoBehaviour
{
	[field: Header("Match"), SerializeField]
	protected ScoreContainer Scores { get; private set; }
	/// <summary>
	/// 한 턴의 제한 시간
	/// </summary>
	[field: SerializeField, Range(0, 60)]
	protected int TimeLimit { get; private set; }

	/// <summary>
	/// 탑바
	/// </summary>
	[SerializeField]
	TopBar _topBar;
	/// <summary>
	/// 결과 창
	/// </summary>
	[SerializeField]
	ResultPanel _resultPanel;

	protected GameMap Map { get; private set; }
	/// <summary>
	/// 플레이어가 선택한 블록
	/// </summary>
	protected Block SelectedBlock;
	protected BlockView Origin;
	protected virtual void OnEnable()
	{
		_topBar.BackButtonClicekd += OnBackButtonClicked;
		_topBar.ResetButtonClicekd += OnResetButtonClicked;
		_resultPanel.BackButtonClicked += OnBackButtonClicked;
		_resultPanel.ResetButtonClicked += OnResetButtonClicked;
	}
	protected virtual void OnDisable()
	{
		_topBar.BackButtonClicekd -= OnBackButtonClicked;
		_topBar.ResetButtonClicekd -= OnResetButtonClicked;
		_resultPanel.BackButtonClicked -= OnBackButtonClicked;
		_resultPanel.ResetButtonClicked -= OnResetButtonClicked;
	}

	#region Game
	/// <summary>
	/// 게임 초기화, 씬 로드 후 번만 실행됨
	/// </summary>
	public virtual void InitGame(GameMap map)
	{
		//맵 초기화
		Map = map;
		Map.Init();
		Map.MapBlockSpawned += OnMapBlockSpawned;
		Map.MapBlockDestroyed += OnMapBlockDestroyed;

		//TODO Score 컨테이너를 별도의 에셋으로 분리하기
		Scores.Init();
		_topBar.Init(Scores["Total"]);
		MapView.Init(_mapThemes);
		_resultPanel.Init(Scores);

#if UNITY_EDITOR
		Debug.Log("게임 초기화");
#endif
	}

	/// <summary>
	/// 게임 재설정, 게임의 모든 정보를 초기화할 때 호출됨
	/// </summary>
	protected virtual void ResetGame()
	{
		ResetMap();
		Scores.ResetAll();

#if UNITY_EDITOR
		Debug.Log("게임 재설정");
#endif
	}

	/// <summary>
	/// 게임 시작, 새로운 매치를 시작할 때마다 호출됨
	/// </summary>
	public virtual void StartGame()
	{
		//모든 정보 초기화
		ResetGame();

		_resultPanel.Close();
		_topBar.Open();
		Map.Open();

#if UNITY_EDITOR
		Debug.Log("게임 시작");
#endif
	}

	/// <summary>
	/// 게임 종료, 매치 종료 조건을 만족하면 호출됨
	/// </summary>
	protected virtual void EndGame()
	{
		//기록 비교
		var totalScore = Scores["Total"];
		var bestRecord = Scores["BestRecord"];
		StringBuilder stagePath = MatchManager.Instance.CurrentStage.GetPath();
		string bestRecordKey = stagePath.Append("_BestRecord").ToString();
		bestRecord.BaseValue = PlayerPrefs.GetInt(bestRecordKey, 0);
		if (totalScore.CurrentValue > bestRecord.CurrentValue)
		{
			//신기록 저장
			PlayerPrefs.SetInt(bestRecordKey, totalScore.CurrentValue);
		}
		if (CheckUnlockCondition())
		{
			//다음 스테이지 해금
			MatchManager.Instance.UnlockNextStage();
		}
		if (MatchManager.Instance.TryGetNextStage(out var _))
		{
			//다음 스테이지 버튼 활성화
			_resultPanel.NextButtonClicked += OnNextButtonClicked;
		}

		_topBar.Close();
		_resultPanel.Open();

#if UNITY_EDITOR
		Debug.Log("게임 종료");
#endif
	}

	/// <summary>
	/// 게임 최신화, 매치 상태에 변화가 있을 때마다 호출됨
	/// </summary>
	protected virtual void UpdateGame()
	{
		//맵 최신화
		UpdateMap();

		//게임 종료 조건 검사
		if (CheckEndCondition())
		{
			EndGame();
		}
	}

	/// <summary>
	/// 게임 종료 조건을 만족하는가
	/// </summary>
	protected abstract bool CheckEndCondition();

	/// <summary>
	/// 다음 스테이지를 잠금 해제할 수 있는가
	/// </summary>
	protected abstract bool CheckUnlockCondition();
	#endregion

	#region Map
	/// <summary>
	/// 맵 초기화
	/// </summary>
	protected virtual void ResetMap()
	{
		Map.ResetMap();
	}

	/// <summary>
	/// 규칙에 따라 맵을 최신 상태로 업데이트
	/// </summary>
	protected abstract void UpdateMap();
	#endregion

	#region Callbacks
	/// <summary>
	/// 뒤로가기 버튼이 눌렸을 떄 호출됨
	/// </summary>
	protected virtual void OnBackButtonClicked()
	{
		//메인 화면으로 돌아가기
		TransitionManager.Instance.LoadSceneAsync("Main");
	}

	/// <summary>
	/// 리셋 버튼이 눌렸을 때 호출됨
	/// </summary>
	protected virtual void OnResetButtonClicked()
	{
		//게임 다시 시작
		StartGame();
	}

	/// <summary>
	/// 다음 버튼이 눌렸을 때 호출됨
	/// </summary>
	protected virtual void OnNextButtonClicked()
	{
		//다음 스테이지로 이동
		MatchManager.Instance.LoadNextStageAsync();
	}

	/// <summary>
	/// 맵에 블록이 배치됐을 때 호출됨
	/// </summary>
	protected virtual void OnMapBlockSpawned(Block block)
	{
	}

	/// <summary>
	/// 맵의 블록이 제거됐을 때 호출됨
	/// </summary>
	protected virtual void OnMapBlockDestroyed(Block block)
	{
	}
	#endregion
}
