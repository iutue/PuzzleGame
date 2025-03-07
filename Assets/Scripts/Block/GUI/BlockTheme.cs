using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 블록 뷰에 적용할 수 있는 테마
/// </summary>
[CreateAssetMenu(fileName = "new BlockTheme", menuName = "Block/Theme")]
public class BlockTheme : ScriptableObject
{
	/// <summary>
	/// 블록 상태에 대응하는 테마
	/// </summary>
	[Serializable]
	public class BlockTypeTheme
	{
		public Sprite BlockImage;
		public Color BlockColor;
	}

	//블록 상태에 따라 적용할 테마들
	public BlockTypeTheme Empty;
	public BlockTypeTheme Ghost;
	public BlockTypeTheme Block;
}
