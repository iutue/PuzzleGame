using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Scene/Level")]
public class LevelData : ScriptableObject
{
	/// <summary>
	/// 이 레벨이 속한 챕터
	/// </summary>
	[HideInInspector]
	public ChapterData ParentChapter;
	/// <summary>
	/// 속한 챕터에서 이 레벨의 순서
	/// </summary>
	[HideInInspector]
	public int Index;


	public LocalizedString Title;
	public Sprite TitleBackground;
	public Material TitleMaterial;

	/// <summary>
	/// 스테이지에 정보가 없을 때 기본으로 사용할 스테이지 정보
	/// </summary>
	[Header("Stage")]
	public StageData DefaultStageData;
	public StageData[] Stages;

	protected void OnValidate()
	{
		//스테이지의 부모 설정
		for (int i = 0; i < Stages.Length; i++)
		{
			var stage = Stages[i];
			if (stage != null)
			{
				stage.ParentLevel = this;
				stage.Index = i;
			}
		}
	}

	public StringBuilder GetPath()
	{
		return ParentChapter.GetPath().Append("-").Append(Index);
	}
}
