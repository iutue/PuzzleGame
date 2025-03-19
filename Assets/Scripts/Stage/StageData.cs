using System;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class StageData
{
	/// <summary>
	/// 이 스테이지가 속한 레벨
	/// </summary>
	[HideInInspector]
	public LevelData ParentLevel;
	/// <summary>
	/// 속한 레벨에서 이 스테이지의 순서
	/// </summary>
	[HideInInspector]
	public int Index;

	public AssetReferenceGameObject Mode;
	public AssetReferenceGameObject Map;

	/// <summary>
	/// 스테이지부터 챕터까지의 경로
	/// </summary>
	public StringBuilder GetPath()
	{
		return ParentLevel.GetPath().Append("-").Append(Index);
	}
}
