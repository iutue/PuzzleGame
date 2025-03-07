using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 블록 뷰를 관리하는 블록 그룹 GUI
/// </summary>
public class BlockGroupView : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public BlockGroup OwnerBlockGroup { get; private set; }

	BlockView[,] _blockViews;
	[SerializeField]
	GameObject _blockViewPrefab;
	[SerializeField]
	RectTransform _blocksParent;
	[SerializeField]
	GridLayoutGroup _blocksGrid;
	/// <summary>
	/// 화면상에서 블록 하나의 크기
	/// </summary>
	public Vector2 CellSize { get; private set; }
	/// <summary>
	/// [0, 0] 블록의 위치
	/// </summary>
	public Vector2 OriginBlockPosition => _blocksParent.GetChild(0).position;

	public delegate void DragHandler(BlockGroupView target, PointerEventData eventData);
	public event DragHandler BeginDrag;
	public event DragHandler Dragging;
	public event DragHandler EndDrag;

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		InitGrid();
	}

	public void Init(BlockGroup owner, BlockTheme blockThemeSet)
	{
		OwnerBlockGroup = owner;

		//모든 블록 뷰 초기화
		_blockViews = new BlockView[OwnerBlockGroup.Size.x, OwnerBlockGroup.Size.y];
		for (int x = 0; x < OwnerBlockGroup.Size.x; x++)
		{
			for (int y = 0; y < OwnerBlockGroup.Size.y; y++)
			{
				var newBlockView = Instantiate(_blockViewPrefab, _blocksParent).GetComponent<BlockView>();
				newBlockView.Init(OwnerBlockGroup[x, y], blockThemeSet);
				_blockViews[x, y] = newBlockView;
			}
		}
		InitGrid();
	}

	async Awaitable InitGrid()
	{
		await Awaitable.EndOfFrameAsync();
		_blocksGrid.constraintCount = OwnerBlockGroup.Size.x;
		//셀 크기를 3*3 크기의 정사각형보다 작게 제한
		float squareSize = Mathf.Max(3, OwnerBlockGroup.Size.x, OwnerBlockGroup.Size.y);
		CellSize = _blocksParent.rect.size / squareSize;
		_blocksGrid.cellSize = CellSize;
	}

	#region Callbacks
	public void OnBeginDrag(PointerEventData eventData) => BeginDrag?.Invoke(this, eventData);
	public void OnDrag(PointerEventData eventData) => Dragging?.Invoke(this, eventData);
	public void OnEndDrag(PointerEventData eventData) => EndDrag?.Invoke(this, eventData);

	/// <summary>
	/// 플레이어가 카드를 들면 호출됨
	/// </summary>
	public void StartDragging(Vector2 mapCellSize)
	{
		//맵과 카드의 크기를 일치시킴
		DOTween
			.To(() => _blocksGrid.cellSize, i => _blocksGrid.cellSize = i, mapCellSize, 0.3f)
			.SetEase(Ease.OutExpo);
	}
	/// <summary>
	/// 플레이어가 카드를 끄는 동안 호출됨
	/// </summary>
	public void Drag(Vector2 position)
	{
		_blocksParent.position = position;
	}
	/// <summary>
	/// 플레이어가 카드를 놓으면 호출됨
	/// </summary>
	public void StopDragging()
	{
		//위치 초기화
		_blocksParent
			.DOAnchorPos(Vector2.zero, 0.3f)
			.SetEase(Ease.OutExpo);
		//크기 초기화
		DOTween
			.To(() => _blocksGrid.cellSize, i => _blocksGrid.cellSize = i, CellSize, 0.3f)
			.SetEase(Ease.OutExpo);
	}
	#endregion
}
