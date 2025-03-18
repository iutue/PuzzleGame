using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ChapterData", menuName = "Scriptable Objects/Scene/Chapter")]
public class ChapterData : ScriptableObject
{
	public LocalizedString Title;
	public Sprite TitleBackground;
	public Sprite Icon;
	public LevelData[] Levels;
}
