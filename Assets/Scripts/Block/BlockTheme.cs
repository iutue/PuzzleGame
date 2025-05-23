using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 블록 뷰에 적용할 수 있는 테마
/// </summary>
[CreateAssetMenu(fileName = "PlaySetting", menuName = "Scriptable Objects/BlockTheme")]
public class BlockTheme : ScriptableObject
{
	/// <summary>
	/// 블록 상태에 대응하는 테마
	/// </summary>
	[Serializable]
	public class StateTheme
	{
		public Sprite BlockImage;
		public Color BlockColor;
		public GameObject BlockObject;
	}

	//블록 상태에 따라 적용할 테마들
	public StateTheme Empty;
	public StateTheme Ghost;
	public StateTheme Block;
}
