using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

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
				UpdateTheme(value, OwnerBlock.Type);
			}
		}
	}

	protected override void OnDestroy()
	{
		OwnerBlock.TypeChanged -= OnTypeChanged;
		OwnerBlock.BlockSpawned -= OnBlockSpawned;
		OwnerBlock.BlockDestroyed -= OnBlockDestroyed;
	}

	public void Init(Block owner, BlockTheme blockThemeSet)
	{
		OwnerBlock = owner;
		OwnerBlock.TypeChanged += OnTypeChanged;
		OwnerBlock.BlockSpawned += OnBlockSpawned;
		OwnerBlock.BlockDestroyed += OnBlockDestroyed;
		Theme = blockThemeSet;
		//상태 동기화
		OnTypeChanged(owner, owner.Type, owner.Type);
	}

	void UpdateTheme(BlockTheme theme, BlockType type)
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
	/// <summary>
	/// 블록의 종류가 변경됐을 때 호출됨
	/// </summary>
	void OnTypeChanged(Block block, BlockType oldType, BlockType newType)
	{
		UpdateTheme(Theme, newType);
	}

	void OnBlockSpawned(Block block)
	{
		DOTween.Sequence()
			.Append(_image.rectTransform.DOScale(0.9f, 0.1f).SetEase(Ease.OutSine))
			.Append(_image.rectTransform.DOScale(1f, 0.1f).SetEase(Ease.OutSine));
	}

	void OnBlockDestroyed(Block block)
	{
		_image.rectTransform.DOShakeAnchorPos(0.5f, 50f, 300);
	}
	#endregion
}
