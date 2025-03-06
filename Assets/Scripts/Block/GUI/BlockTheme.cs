using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "new BlockTheme", menuName = "Block/Theme")]
public class BlockTheme : ScriptableObject
{
	[Serializable]
	public class BlockTypeTheme
	{
		public Sprite BlockImage;
		public Color BlockColor;
	}

	public BlockTypeTheme Empty;
	public BlockTypeTheme Ghost;
	public BlockTypeTheme Block;
}
