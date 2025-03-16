using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 점수, 규칙 등 매치의 상태를 가지고 하나의 매치를 관리함
/// </summary>
public abstract class GameMode : MonoBehaviour
{
	/// <summary>
	/// 게임 상태
	/// </summary>
	protected GameState State { get; private set; }

	/// <summary>
	/// 현재 맵
	/// </summary>
	protected BlockGroup Map;
	/// <summary>
	/// 맵의 크기
	/// </summary>
	[SerializeField]
	Vector2Int _initialMapSize;
	/// <summary>
	/// 카드를 놓을 수 있는지 검사한 맵의 블록<br/>
	/// 동일한 블록을 매프레임마다 검사하는 것을 방지함
	/// </summary>
	Block _previousMapBlock;
	/// <summary>
	/// 패에 있는 카드들
	/// </summary>
	protected List<BlockGroup> Cards = new();

	[Header("GUI")]
	[SerializeField]
	PlayCanvas _playCanvas;

	protected void Start()
	{
		//초기화 후 바로 게임 시작
		InitGame();
		StartGame();
	}

	#region Game
	/// <summary>
	/// 게임 초기화, 씬 로드 후 번만 실행됨
	/// </summary>
	void InitGame()
	{
		State = Instantiate(State);

		State.Scores.Init();
		_playCanvas.OnGameInitialized(
			State,
			OnBackButtonClicked, OnResetButtonClicked, null,
			OnBeginDragCard, OnEndDragCard, OnDragCard);

#if UNITY_EDITOR
		Debug.Log("게임 초기화");
#endif
	}

	/// <summary>
	/// 게임 시작, 새로운 매치를 시작할 때마다 호출됨
	/// </summary>
	void StartGame()
	{
		//모든 정보 초기화
		ResetGame();
		//시작
		_playCanvas.OnGameStarted();

		DrawCards();

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
		var totalScore = State.Scores["Total"];
		var bestRecord = State.Scores["BestRecord"];
		string bestRecordName = "BestRecord_" + SceneManager.GetActiveScene().name;
		bestRecord.BaseValue = PlayerPrefs.GetInt(bestRecordName, 0);
		if (totalScore.CurrentValue > bestRecord.CurrentValue)
		{
			//신기록 저장
			PlayerPrefs.SetInt(bestRecordName, totalScore.CurrentValue);
		}
		//결과 출력
		_playCanvas.OnGameEnded();

#if UNITY_EDITOR
		Debug.Log("게임 종료");
#endif
	}

	/// <summary>
	/// 게임 재설정, 게임의 모든 정보를 초기화할 때 호출됨
	/// </summary>
	void ResetGame()
	{
		ResetMap();
		ResetCards();
		State.Scores.ResetAll();
		_playCanvas.OnGameReset();

#if UNITY_EDITOR
		Debug.Log("게임 재설정");
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
		var mapTemplate = new Block.State[_initialMapSize.x, _initialMapSize.y];
		Map = new BlockGroup(mapTemplate);
		Map.BlockSpawned += OnMapBlockSpawned;
		Map.BlockDestroyed += OnMapBlockDestroyed;
		//뷰 초기화
		_playCanvas.MapView.ResetMap(Map);
	}

	/// <summary>
	/// 규칙에 따라 맵을 최신 상태로 업데이트
	/// </summary>
	protected abstract void UpdateMap();
	#endregion

	#region Card
	/// <summary>
	/// 패 버리기
	/// </summary>
	void ResetCards()
	{
		//카드 초기화
		Cards.Clear();
		//뷰 초기화
		_playCanvas.HandView.ResetCards(Cards);
	}

	/// <summary>
	/// 카드 뽑기
	/// </summary>
	void DrawCards()
	{
		for (int i = 0; i < State.DrawCount; i++)
		{
			if (Cards.Count >= State.HandCapacity)
			{
				//더 이상 카드를 뽑을 수 없음
				break;
			}
			var newCard = CreateCard();
			AddCard(newCard);
		}
	}

	/// <summary>
	/// 새로운 카드 생성
	/// </summary>
	BlockGroup CreateCard()
	{
		//램덤한 모양의 카드 생성
		int randomIndex = Random.Range(0, BlockGroupTemplates.Templates.Count);
		var randomTemplate = BlockGroupTemplates.Templates[randomIndex];
		return new BlockGroup(randomTemplate);
	}

	/// <summary>
	/// 패에 카드 추가
	/// </summary>
	void AddCard(BlockGroup card)
	{
		Cards.Add(card);
		_playCanvas.HandView.OnCardAdded(Cards.Count - 1, card);
	}
	/// <summary>
	/// 패의 카드 제거
	/// </summary>
	void RemoveCard(BlockGroupView target)
	{
		Cards.Remove(target.OwnerBlockGroup);
		_playCanvas.HandView.OnCardRemoved(target);

		if (Cards.Count == 0)
		{
			//패에 카드가 없음
			OnHandEmpty();
		}
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
		_playCanvas.HandView.OnBeginDragCard(cardView, _playCanvas.MapView.BlockViewSize);
	}

	/// <summary>
	/// 카드를 이동중일 때 호출됨
	/// </summary>
	protected virtual void OnDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		//카드를 배치할 때 기준이 되는 맵의 블록 검출
		var origin = _playCanvas.MapView.GetMapBlockAt(cardView.OriginBlockPosition);
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
		_playCanvas.HandView.OnDragCard(cardView, eventData.position);
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
		_playCanvas.HandView.OnEndDragCard(cardView);
	}

	/// <summary>
	/// 패에 카드가 하나도 없을 때 호출됨
	/// </summary>
	protected virtual void OnHandEmpty()
	{
		//새로운 카드 뽑기
		DrawCards();
	}
	#endregion
}
