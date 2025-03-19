using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ChapterData", menuName = "Scriptable Objects/Scene/Chapter")]
public class ChapterData : ScriptableObject
{
	//TODO 게임 내 모든 챕터를 관리하는 챕터 테이블 만들기
	public int Index;

	public LocalizedString Title;
	public Sprite TitleBackground;
	public Sprite Icon;
	public LevelData[] Levels;

	protected void OnValidate()
	{
		//레벨 에셋의 부모 설정
		for (int i = 0; i < Levels.Length; i++)
		{
			var level = Levels[i];
			if (level != null)
			{
				level.ParentChapter = this;
				level.Index = i;
				EditorUtility.SetDirty(level);
			}
		}
	}

	public StringBuilder GetPath()
	{
		return new StringBuilder(Index);
	}
}
