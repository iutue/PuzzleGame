using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 블록의 모습을 표현하는 GUI
/// </summary>
public class BlockView : UIBehaviour
{
	public Block OwnerBlock { get; private set; }

	/// <summary>
	/// 테마를 적용할 이미지
	/// </summary>
	[SerializeField]
	Image _image;
	/// <summary>
	/// 현재 블록에 적용된 테마
	/// </summary>
	BlockTheme _theme;
	public BlockTheme Theme
	{
		get => _theme;
		private set
		{
			if (value != _theme)
			{
				_theme = value;
				ApplyTheme(value, OwnerBlock.CurrentState);
			}
		}
	}

	protected override void OnDestroy()
	{
		OwnerBlock.StateChanged -= OnStateChanged;
		OwnerBlock.BlockSpawned -= OnBlockSpawned;
		OwnerBlock.BlockDestroyed -= OnBlockDestroyed;
	}

	public void Init(Block owner, BlockTheme theme)
	{
		OwnerBlock = owner;
		Theme = theme;
		OwnerBlock.StateChanged += OnStateChanged;
		OwnerBlock.BlockSpawned += OnBlockSpawned;
		OwnerBlock.BlockDestroyed += OnBlockDestroyed;
	}

	/// <summary>
	/// 블록에 테마 변경
	/// </summary>
	void ApplyTheme(BlockTheme theme, Block.State state)
	{
		//블록 종류에 해당하는 테마 선택
		BlockTheme.StateTheme stateTheme = state switch
		{
			Block.State.Empty => theme.Empty,
			Block.State.Preview => theme.Ghost,
			Block.State.Placed => theme.Block,
			_ => theme.Empty
		};
		//테마 적용
		_image.sprite = stateTheme.BlockImage;
		_image.color = stateTheme.BlockColor;
	}

	#region Callbacks
	void OnStateChanged(Block block, Block.State oldState, Block.State newState)
	{
		//상태에 맞는 테마 적용
		ApplyTheme(Theme, newState);
	}

	void OnBlockSpawned(Block block)
	{
		DOTween.Sequence()
			.Append(_image.rectTransform.DOScale(0.9f, 0.1f))
			.Append(_image.rectTransform.DOScale(1f, 0.05f));
	}
	
	void OnBlockDestroyed(Block block)
	{
		_image.rectTransform.DOShakeAnchorPos(0.3f, 50f, 500);
	}
	#endregion
}
