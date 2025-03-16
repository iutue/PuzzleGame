using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Scene/Level")]
public class LevelData : ScriptableObject
{
	public LocalizedString Title;
	public Sprite TitleImage;
	public Material TitleMaterial;

	/// <summary>
	/// 스테이지에 정보가 없을 때 기본으로 사용할 스테이지 정보
	/// </summary>
	[Header("Stage")]
	public StageData DefaultStageData;
	public StageData[] Stages;
}
