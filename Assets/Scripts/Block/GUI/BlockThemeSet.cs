using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 블록 테마 모음
/// </summary>
[CreateAssetMenu(fileName = "new BlockTheme", menuName = "Block/Theme")]
public class BlockThemeSet : ScriptableObject
{
	public BlockTheme Empty;
	public BlockTheme Ghost;
	public BlockTheme Block;
}

/// <summary>
/// 블록의 테마
/// </summary>
[Serializable]
public class BlockTheme
{
	public Sprite BlockImage;
	public Color BlockColor;
}
