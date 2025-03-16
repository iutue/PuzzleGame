using UnityEngine;

/// <summary>
/// 매치를 관리하는 매니저
/// </summary>
public class MatchManager : SingletonBehaviour<MatchManager>
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

		//씬 불러오기
		await TransitionManager.Instance.LoadSceneAsync("Play");
		//씬에 모드, 맵 생성
		var modeToSpawn = CurrentStage.Mode ? CurrentStage.Mode : CurrentLevel.DefaultStageData.Mode;
		var mapToSpawn = CurrentStage.Map ? CurrentStage.Map : CurrentLevel.DefaultStageData.Map;
		var mode = Instantiate(modeToSpawn);
		var map = Instantiate(mapToSpawn);


	}
}
