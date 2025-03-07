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
				ApplyTheme(value, OwnerBlock.Type);
			}
		}
	}

	protected override void OnDestroy()
	{
		OwnerBlock.TypeChanged -= OnTypeChanged;
		OwnerBlock.BlockSpawned -= OnBlockSpawned;
		OwnerBlock.BlockDestroyed -= OnBlockDestroyed;
	}

	public void Init(Block owner, BlockTheme theme)
	{
		OwnerBlock = owner;
		Theme = theme;
		OwnerBlock.TypeChanged += OnTypeChanged;
		OwnerBlock.BlockSpawned += OnBlockSpawned;
		OwnerBlock.BlockDestroyed += OnBlockDestroyed;
	}

	/// <summary>
	/// 블록에 테마 변경
	/// </summary>
	void ApplyTheme(BlockTheme theme, BlockType type)
	{
		//블록 종류에 해당하는 테마 선택
		BlockTheme.BlockTypeTheme typeTheme = type switch
		{
			BlockType.Empty => theme.Empty,
			BlockType.Ghost => theme.Ghost,
			BlockType.Block => theme.Block,
			_ => theme.Empty
		};
		//테마 적용
		_image.sprite = typeTheme.BlockImage;
		_image.color = typeTheme.BlockColor;
	}

	#region Callbacks
	void OnTypeChanged(Block block, BlockType oldType, BlockType newType)
	{
		//상태에 맞는 테마 적용
		ApplyTheme(Theme, newType);
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
