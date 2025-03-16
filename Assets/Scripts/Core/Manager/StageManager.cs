using UnityEngine;

/// <summary>
/// 스테이지를 관리하는 매니저
/// </summary>
public class StageManager : SingletonBehaviour<StageManager>
{
	public ChapterData CurrentChapter { get; private set; }
	public LevelData CurrentLevel { get; private set; }
	public StageData CurrentStage { get; private set; }

	/// <summary>
	/// 스테이지 전환
	/// </summary>
	public async void LoadStageAsync(ChapterData chapter, LevelData level, StageData stage)
	{
		CurrentChapter = chapter;
		CurrentLevel = level;
		CurrentStage = stage;

		//씬
		await TransitionManager.Instance.LoadSceneAsync("Play");
		//모드
		if (CurrentStage.Mode)
		{
			Instantiate(CurrentStage.Mode);
		}
		else
		{
			Instantiate(CurrentLevel.DefaultStageData.Mode);
		}
		//맵
		if (CurrentStage.Map)
		{
			Instantiate(CurrentStage.Map);
		}
		else
		{
			Instantiate(CurrentLevel.DefaultStageData.Map);
		}
	}
}
