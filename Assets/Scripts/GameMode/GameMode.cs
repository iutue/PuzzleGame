using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 게임 전체를 관리하는 매니저 클래스
/// </summary>
public abstract class GameMode : MonoBehaviour
{
	/// <summary>
	/// 모든 점수
	/// </summary>
	[SerializeField]
	protected ScoreTable ScoreTable;

	#region BlockGroups
	[Header("Map")]
	/// <summary>
	/// 초기의 맵 크기
	/// </summary>
	[SerializeField]
	Vector2Int _initialMapSize;
	protected BlockGroup Map;
	/// <summary>
	/// 카드를 놓을 수 있는지 검사한 블록<br/>
	/// 동일한 블록을 매프레임마다 검사하는 것을 방지함
	/// </summary>
	Block _previousBlock;

	[Header("Cards")]
	/// <summary>
	/// 패에 최대로 가질 수 있는 카드 수
	/// </summary>
	[SerializeField, Range(1, 3)]
	int _drawCount = 3;
	/// <summary>
	/// 현재 가지고 있는 카드들
	/// </summary>
	protected List<BlockGroup> Cards = new();
	#endregion

	#region GUI
	[Header("GUI")]
	[SerializeField]
	PlayCanvas _playCanvas;

	[SerializeField]
	GameObject _blockGroupViewPrefab;
	/// <summary>
	/// 맵의 부모
	/// </summary>
	[Header("Map View")]
	[SerializeField]
	RectTransform _mapParent;
	BlockGroupView _mapView;
	/// <summary>
	/// 맵 블록의 테마
	/// </summary>
	[SerializeField]
	BlockThemeSet _mapBlockThemeSet;
	GraphicRaycaster _mapRaycaster;
	List<RaycastResult> _mapRaycastresult = new();

	/// <summary>
	/// 카드들의 부모
	/// </summary>
	[Header("Card Views")]
	[SerializeField]
	RectTransform _cardsParent;
	List<BlockGroupView> _cardViews = new();
	/// <summary>
	/// 카드를 집었을 때 포인터와 카드의 상대 위치
	/// </summary>
	[SerializeField]
	Vector2 _cardOffset;
	/// <summary>
	/// 카드 블록의 테마들
	/// </summary>
	[SerializeField]
	BlockThemeSet[] _CardBlockThemeSets;
	#endregion

	protected void Awake()
	{
		_mapRaycaster = _mapParent.GetComponentInParent<GraphicRaycaster>();
	}
	protected void Start()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;

		InitGame();
		StartGame();
	}

	/// <summary>
	/// 게임 초기화
	/// </summary>
	void InitGame()
	{
		ScoreTable.Init();
		_playCanvas.Init(ScoreTable, OnBackButtonClicked, OnRefreshButtonClicked);

	}

	/// <summary>
	/// 게임 시작
	/// </summary>
	void StartGame()
	{
		ScoreTable.ResetAll();

		InitMap();
		InitCards();

		//GUI 초기화
		_playCanvas.StartGame();

#if UNITY_EDITOR
		Debug.Log("게임 시작");
#endif
	}
	/// <summary>
	/// 게임 종료
	/// </summary>
	void EndGame()
	{
		var totalScore = ScoreTable["Total"];
		var bestRecord = ScoreTable["BestRecord"];
		string bestRecordName = "BestRecord_" + SceneManager.GetActiveScene().name;
		bestRecord.BaseValue = PlayerPrefs.GetInt(bestRecordName, 0);
		if (totalScore.CurrentValue > bestRecord.BaseValue)
		{
			//신기록 저장
			PlayerPrefs.SetInt(bestRecordName, totalScore.CurrentValue);
		}

		//결과 출력
		_playCanvas.ShowResult(ScoreTable, OnBackButtonClicked, OnRefreshButtonClicked, null);

#if UNITY_EDITOR
		Debug.Log("게임 종료");
#endif
	}
	/// <summary>
	/// 게임 최신화
	/// </summary>
	void UpdateGame()
	{
		//맵 업데이트
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
	/// <returns></returns>
	protected abstract bool CheckEndCondition();

	/// <summary>
	/// 맵 초기화
	/// </summary>
	void InitMap()
	{
		//맵 초기화
		Map = new BlockGroup(new BlockType[_initialMapSize.x, _initialMapSize.y]);
		Map.BlockSpawned += OnMapBlockSpawned;
		Map.BlockDestroyed += OnMapBlockDestroyed;
		//맵 뷰 초기화
		if (_mapView)
		{
			Destroy(_mapView.gameObject);
		}
		_mapView = Instantiate(_blockGroupViewPrefab, _mapParent).GetComponent<BlockGroupView>();
		_mapView.Init(Map, _mapBlockThemeSet);
	}
	/// <summary>
	/// 맵 업데이트
	/// </summary>
	protected abstract void UpdateMap();

	/// <summary>
	/// 가지고 있는 카드를 버리고 최대 수 만큼 카드 뽑기
	/// </summary>
	void InitCards()
	{
		//카드 초기화
		Cards.Clear();
		for (int i = 0; i < _drawCount; i++)
		{
			//램덤한 카드 선택
			var randomTemplate = BlockGroupTemplates.Templates[Random.Range(0, BlockGroupTemplates.Templates.Count)];
			var newCard = new BlockGroup(randomTemplate);
			Cards.Add(newCard);
		}
		//카드 뷰 초기화
		foreach (var cardView in _cardViews)
		{
			Destroy(cardView.gameObject);
		}
		_cardViews.Clear();
		for (int i = 0; i < Cards.Count; i++)
		{
			//새로운 뷰 생성
			var newCardView = Instantiate(_blockGroupViewPrefab, _cardsParent.GetChild(i)).GetComponent<BlockGroupView>();
			newCardView.Init(Cards[i], _CardBlockThemeSets[Random.Range(0, _CardBlockThemeSets.Length)]);
			newCardView.BeginDrag = OnBeginDragCard;
			newCardView.EndDrag = OnEndDragCard;
			newCardView.Dragging = OnDragCard;
			_cardViews.Add(newCardView);
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
		if (currentBlock == _previousBlock)
		{
			return true;
		}
		_previousBlock = currentBlock;

		//맵 업데이트
		Map.Convert(BlockType.Ghost, BlockType.Empty);
		if (currentBlock == null)
		{
			//블록이 없으면 배치 흔적만 제거하면 됨
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
	/// <param name="cardView"></param>
	void ConfirmCardPlacement(BlockGroupView cardView)
	{
		//카드 사용
		Map.Convert(BlockType.Ghost, BlockType.Block);
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
		_cardViews.Remove(target);
		Destroy(target.gameObject);

		//카드를 다 사용했으면
		if (Cards.Count == 0)
		{
			//새로운 카드 뽑기
			InitCards();
		}
	}

	#region Callbacks
	/// <summary>
	/// 뒤로가기 버튼이 눌렸을 떄 호출됨
	/// </summary>
	protected virtual void OnBackButtonClicked()
	{
		//메인 화면으로 돌아가기
		TransitionManager.Instance.LoadScene("Main");
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
		//TODO 다음 레벨이 있으면 다음 씬으로 전환
	}

	/// <summary>
	/// 맵에 블록이 배치됐을 때 호출됨
	/// </summary>
	protected virtual void OnMapBlockSpawned()
	{
	}
	/// <summary>
	/// 맵의 블록이 제거됐을 때 호출됨
	/// </summary>
	protected virtual void OnMapBlockDestroyed()
	{
	}

	/// <summary>
	/// 카드를 집었을 때 호출됨
	/// </summary>
	void OnBeginDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		cardView.StartDragging(_mapView.CellSize);
	}
	/// <summary>
	/// 카드를 이동중일 때 호출됨
	/// </summary>
	void OnDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		cardView.Drag(eventData.position + _cardOffset * Screen.height);

		//카드를 배치할 때 기준이 되는 블록 검출
		BlockView origin = null;
		eventData.position = cardView.OriginBlockPosition;
		_mapRaycastresult.Clear();
		_mapRaycaster.Raycast(eventData, _mapRaycastresult);
		foreach (var uiElement in _mapRaycastresult)
		{
			if (uiElement.gameObject.TryGetComponent<BlockView>(out origin))
			{
				//블록 발견
				break;
			}
		}

		//카드 배치 시도
		if (TryCardPlacement(cardView.OwnerBlockGroup, origin))
		{
			//배치 성공
		}
		else
		{
			//배치 실패
			Map.Convert(BlockType.Ghost, BlockType.Empty);
		}
	}
	/// <summary>
	/// 카드를 내려놓았을 때 호출됨
	/// </summary>
	void OnEndDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		if (!Map.Contains(BlockType.Ghost))
		{
			//카드를 배치할 수 없는 위치임
			cardView.StopDragging();
			return;
		}

		//카드 배치 확정
		ConfirmCardPlacement(cardView);
	}
	#endregion
}
