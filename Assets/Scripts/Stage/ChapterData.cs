using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ChapterData", menuName = "Scriptable Objects/Scene/Chapter")]
public class ChapterData : ScriptableObject
{
	public LocalizedString Title;
	public LocalizedString Description;
	public LevelData[] Levels;
}
