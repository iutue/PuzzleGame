using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 복수의 블록 뷰를 관리하는 블록 그룹 뷰
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
	/// 해상도를 고려해서 계산된 최적의 블록 크기
	/// </summary>
	public Vector2 BlockViewSize { get; private set; }
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
		BlockViewSize = _blocksParent.rect.size / squareSize;
		//계산된 블록 크기 반영
		SetBlockViewSize(BlockViewSize);
	}

	/// <summary>
	/// 주어진 크기로 블록 뷰의 크기를 즉시 변경
	/// </summary>
	public void SetBlockViewSize(Vector2 blockViewSize)
	{
		_blocksGrid.cellSize = blockViewSize;
	}
	/// <summary>
	/// 주어진 크기로 블록 뷰의 크기를 부드럽게 변경
	/// </summary>
	public void ChangeBlockViewSize(Vector2 blockViewSize)
	{
		DOTween
			.To(() => _blocksGrid.cellSize, i => _blocksGrid.cellSize = i, blockViewSize, 0.3f)
			.SetEase(Ease.OutExpo);
	}

	/// <summary>
	/// 주어진 위치로 즉시 이동
	/// </summary>
	public void SetPosition(Vector2 position)
	{
		_blocksParent.anchoredPosition = position;
	}
	/// <summary>
	/// 주어진 위치로 부드럽게 이동
	/// </summary>
	public void ChangePosition(Vector2 position)
	{
		_blocksParent
			.DOAnchorPos(position, 0.3f)
			.SetEase(Ease.OutExpo);
	}

	#region Callbacks
	public void OnBeginDrag(PointerEventData eventData) => BeginDrag?.Invoke(this, eventData);
	public void OnDrag(PointerEventData eventData) => Dragging?.Invoke(this, eventData);
	public void OnEndDrag(PointerEventData eventData) => EndDrag?.Invoke(this, eventData);
	#endregion
}
