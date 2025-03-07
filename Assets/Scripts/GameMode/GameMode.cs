using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 점수, 규칙 등 매치의 상태를 가지고 하나의 매치를 관리함
/// </summary>
public abstract class GameMode : MonoBehaviour
{
	/// <summary>
	/// 맵의 크기
	/// </summary>
	[SerializeField]
	Vector2Int _mapSize;
	/// <summary>
	/// 현재 맵
	/// </summary>
	protected BlockGroup Map;
	/// <summary>
	/// 카드를 놓을 수 있는지 검사한 맵의 블록<br/>
	/// 동일한 블록을 매프레임마다 검사하는 것을 방지함
	/// </summary>
	Block _previousMapBlock;

	/// <summary>
	/// 패에 최대로 가질 수 있는 카드 수
	/// </summary>
	[SerializeField, Range(1, 5)]
	int _maxCardCount = 3;
	/// <summary>
	/// 패에 있는 카드들
	/// </summary>
	protected List<BlockGroup> Cards = new();

	/// <summary>
	/// 필수 GUI
	/// </summary>
	[SerializeField]
	PlayCanvas _playCanvas;
	/// <summary>
	/// 모든 점수
	/// </summary>
	[SerializeField]
	protected ScoreContainer Scores;

	protected void Start()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;

		//최초로 초기화 후 게임 시작
		InitGame();
		StartGame();
	}

	#region Game
	/// <summary>
	/// 게임 초기화, 씬 로드 후 번만 실행됨
	/// </summary>
	void InitGame()
	{
		Scores.Init();
		_playCanvas.OnGameInitialized(_maxCardCount, Scores, OnBackButtonClicked, OnRefreshButtonClicked);

#if UNITY_EDITOR
		Debug.Log("게임 초기화");
#endif
	}

	/// <summary>
	/// 게임 시작, 새로운 매치를 시작할 때마다 호출됨
	/// </summary>
	void StartGame()
	{
		Scores.ResetAll();
		ResetMap();
		ResetCards();

		_playCanvas.OnGameStarted();

#if UNITY_EDITOR
		Debug.Log("게임 시작");
#endif
	}

	/// <summary>
	/// 게임 종료, 매치 종료 조건을 만족하면 호출됨
	/// </summary>
	void EndGame()
	{
		//기록 비교
		var totalScore = Scores["Total"];
		var bestRecord = Scores["BestRecord"];
		string bestRecordName = "BestRecord_" + SceneManager.GetActiveScene().name;
		bestRecord.BaseValue = PlayerPrefs.GetInt(bestRecordName, 0);
		if (totalScore.CurrentValue > bestRecord.CurrentValue)
		{
			//신기록 저장
			PlayerPrefs.SetInt(bestRecordName, totalScore.CurrentValue);
		}

		//결과 출력
		_playCanvas.OnGameEnded(Scores, OnBackButtonClicked, OnRefreshButtonClicked, null);

#if UNITY_EDITOR
		Debug.Log("게임 종료");
#endif
	}

	/// <summary>
	/// 게임 최신화, 매치 상태에 변화가 있을 때마다 호출됨
	/// </summary>
	void UpdateGame()
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
	#endregion

	#region Map
	/// <summary>
	/// 맵 초기화
	/// </summary>
	void ResetMap()
	{
		//맵 초기화
		Map = new BlockGroup(new Block.State[_mapSize.x, _mapSize.y]);
		Map.BlockSpawned += OnMapBlockSpawned;
		Map.BlockDestroyed += OnMapBlockDestroyed;
		//맵 뷰 초기화
		_playCanvas.ResetMap(Map);
	}

	/// <summary>
	/// 규칙에 따라 맵을 최신 상태로 업데이트
	/// </summary>
	protected abstract void UpdateMap();
	#endregion

	#region Card
	/// <summary>
	/// 패의 모든 카드를 버리고 최대 수 만큼 패를 다시 뽑기
	/// </summary>
	void ResetCards()
	{
		//카드 초기화
		Cards.Clear();
		for (int i = 0; i < _maxCardCount; i++)
		{
			//램덤한 모양의 카드 선택
			int randomIndex = Random.Range(0, BlockGroupTemplates.Templates.Count);
			var randomTemplate = BlockGroupTemplates.Templates[randomIndex];
			Cards.Add(new BlockGroup(randomTemplate));
		}
		//카드 뷰 초기화
		_playCanvas.ResetCards(Cards, OnBeginDragCard, OnEndDragCard, OnDragCard);
	}

	/// <summary>
	/// 맵에 카드 배치 시도
	/// </summary>
	bool TryCardPlacement(BlockGroup card, BlockView origin)
	{
		//감지된 블록
		Block currentBlock;
		if (origin)
		{
			currentBlock = origin.OwnerBlock;
		}
		else
		{
			currentBlock = null;
		}
		//이미 배치를 시도한 대상인가
		if (currentBlock == _previousMapBlock)
		{
			return true;
		}
		_previousMapBlock = currentBlock;

		//배치 시도
		Map.Convert(Block.State.Preview, Block.State.Empty);
		if (currentBlock == null)
		{
			//감지된 블록이 없으면 이전의 배치 흔적만 제거
			return true;
		}
		else
		{
			//새로운 블록에 카드 배치 시도
			return Map.TryMerge(card, currentBlock.Position);
		}
	}

	/// <summary>
	/// 카드 배치 확정
	/// </summary>
	void ConfirmCardPlacement(BlockGroupView cardView)
	{
		//카드 사용
		Map.Convert(Block.State.Preview, Block.State.Placed);
		RemoveCard(cardView);
		//게임 업데이트
		UpdateGame();
	}

	/// <summary>
	/// 카드 제거
	/// </summary>
	void RemoveCard(BlockGroupView target)
	{
		Cards.Remove(target.OwnerBlockGroup);
		_playCanvas.RemoveCard(target);

		if (Cards.Count == 0)
		{
			//패에 카드가 없음
			OnHandEmpty();
		}
	}
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
	/// 새로고침 버튼이 눌렸을 때 호출됨
	/// </summary>
	protected virtual void OnRefreshButtonClicked()
	{
		//게임 다시 시작
		StartGame();
	}

	/// <summary>
	/// 다음 버튼이 눌렸을 때 호출됨
	/// </summary>
	protected virtual void OnNextButtonClicked()
	{
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

	/// <summary>
	/// 카드를 집었을 때 호출됨
	/// </summary>
	protected virtual void OnBeginDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
	}

	/// <summary>
	/// 카드를 이동중일 때 호출됨
	/// </summary>
	protected virtual void OnDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		//카드를 배치할 때 기준이 되는 맵의 블록 검출
		var origin = _playCanvas.GetMapBlockAt(cardView.OriginBlockPosition);
		//카드 배치 시도
		if (TryCardPlacement(cardView.OwnerBlockGroup, origin))
		{
			//배치 성공
		}
		else
		{
			//배치 실패
			Map.Convert(Block.State.Preview, Block.State.Empty);
		}
	}

	/// <summary>
	/// 카드를 내려놓았을 때 호출됨
	/// </summary>
	protected virtual void OnEndDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		//맵에 배치된 카드가 있으면
		//TODO[개선] 맵이 아닌 카드로 배치 성공 여부 저장하기
		if (Map.Contains(Block.State.Preview))
		{
			//카드 배치 확정
			ConfirmCardPlacement(cardView);
		}
	}

	/// <summary>
	/// 패에 카드가 하나도 없을 때 호출됨
	/// </summary>
	protected virtual void OnHandEmpty()
	{
		//새로운 카드 뽑기
		ResetCards();
	}
	#endregion
}
