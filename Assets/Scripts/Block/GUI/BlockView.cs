using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 블록 GUI
/// </summary>
public class BlockView : MonoBehaviour
{
	public Block OwnerBlock { get; private set; }

	/// <summary>
	/// 블록의 테마
	/// </summary>
	BlockThemeSet _blockThemeSet;
	/// <summary>
	/// 테마를 적용할 이미지
	/// </summary>
	[SerializeField]
	Image _image;

	public void Init(Block owner, BlockThemeSet blockThemeSet)
	{
		OwnerBlock = owner;
		OwnerBlock.TypeChanged += OnTypeChanged;
		_blockThemeSet = blockThemeSet;
		//블록과 동기화
		OnTypeChanged(owner.Type, owner.Type);
	}

	protected void OnDestroy()
	{
		OwnerBlock.TypeChanged -= OnTypeChanged;
	}

	/// <summary>
	/// 블록에 테마 적용
	/// </summary>
	/// <param name="theme"></param>
	void ApplyTheme(BlockTheme theme)
	{
		_image.sprite = theme.BlockImage;
		_image.color = theme.BlockColor;
	}

	#region Callbacks
	/// <summary>
	/// 블록의 종류가 변경됐을 때 호출됨
	/// </summary>
	void OnTypeChanged(BlockType previousType, BlockType currentType)
	{
		switch (currentType)
		{
			case BlockType.Empty:
				ApplyTheme(_blockThemeSet.Empty);
				break;
			case BlockType.Ghost:
				ApplyTheme(_blockThemeSet.Ghost);
				break;
			case BlockType.Block:
				ApplyTheme(_blockThemeSet.Block);
				break;
		}
	}
	#endregion
}
