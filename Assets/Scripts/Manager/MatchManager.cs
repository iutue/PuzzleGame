using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 매치를 관리하는 매니저
/// </summary>
public class MatchManager : SingletonBehaviour<MatchManager>
{
	public ChapterData CurrentChapter { get; private set; }
	public LevelData CurrentLevel { get; private set; }
	public StageData CurrentStage { get; private set; }

	/// <summary>
	/// 스테이지 불러오기
	/// </summary>
	public async Awaitable LoadStageAsync(StageData stage)
	{
		if (!stage.IsUnlocked)
		{
			//스테이지가 잠김
			return;
		}

		CurrentStage = stage;
		CurrentLevel = stage.ParentLevel;
		CurrentChapter = CurrentLevel.ParentChapter;

		//씬 불러오기
		await TransitionManager.Instance.LoadSceneAsync("Play");
		//씬에 모드, 맵 생성
		var modeToSpawn = CurrentStage.Mode.RuntimeKeyIsValid() ? CurrentStage.Mode : CurrentLevel.DefaultStageData.Mode;
		var mapToSpawn = CurrentStage.Map.RuntimeKeyIsValid() ? CurrentStage.Map : CurrentLevel.DefaultStageData.Map;
		//TODO[개선] 모드, 맵 로딩이 끝날 때까지 로딩화면 대기시키기
		var modeLoading = modeToSpawn.InstantiateAsync();
		var mapLoading = mapToSpawn.InstantiateAsync();

		var mode = await modeLoading.Task;
		var map = await mapLoading.Task;

		await Awaitable.NextFrameAsync();
		
		var gameMode = mode.GetComponent<GameMode>();
		var gameMap = map.GetComponent<GameMap>();

		gameMode.InitGame(gameMap);
		gameMode.StartGame();
	}

	/// <summary>
	/// 현재 스테이지의 다음 스테이지를 해금
	/// </summary>
	public void UnlockNextStage()
	{
		string progressKey = CurrentLevel.GetPath().Append("_Progress").ToString();
		int progress = PlayerPrefs.GetInt(progressKey, 0);
		int nextStageIndex = CurrentStage.Index + 1;
		if (nextStageIndex > progress)
		{
			PlayerPrefs.SetInt(progressKey, nextStageIndex);
		}
	}

	/// <summary>
	/// 입장 가능한 다음 스테이지 반환
	/// </summary>
	public bool TryGetNextStage(out StageData nextStage)
	{
		nextStage = null;
		int nextChapterIndex = CurrentChapter.Index + 1;
		int nextLevelIndex = CurrentLevel.Index + 1;
		int nextStageIndex = CurrentStage.Index + 1;
		//다음 스테이지 탐색
		if (nextStageIndex < CurrentLevel.Stages.Length)
		{
			nextStage = CurrentLevel.Stages[nextStageIndex];
		}
		//다음 레벨 탐색
		else if (nextLevelIndex < CurrentChapter.Levels.Length)
		{
			LevelData nextLevel = CurrentChapter.Levels[nextLevelIndex];
			nextStage = nextLevel.Stages[0];
		}
		//다음 챕터 탐색
		else if (false)
		{

		}

		return nextStage != null && nextStage.IsUnlocked;
	}

	/// <summary>
	/// 다음 스테이지로 이동
	/// </summary>
	public async Awaitable LoadNextStageAsync()
	{
		if (TryGetNextStage(out var nextStage))
		{
			await LoadStageAsync(nextStage);
		}
	}
}
