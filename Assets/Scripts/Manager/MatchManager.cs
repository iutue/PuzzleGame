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
	public async Awaitable LoadStageAsync(ChapterData chapter, LevelData level, StageData stage)
	{
		CurrentChapter = chapter;
		CurrentLevel = level;
		CurrentStage = stage;

		//씬 불러오기
		await TransitionManager.Instance.LoadSceneAsync("Play");
		//씬에 모드, 맵 생성
		var modeToSpawn = CurrentStage.Mode.RuntimeKeyIsValid() ? CurrentStage.Mode : CurrentLevel.DefaultStageData.Mode;
		var mapToSpawn = CurrentStage.Map.RuntimeKeyIsValid() ? CurrentStage.Map : CurrentLevel.DefaultStageData.Map;
		//TODO[개선] 모드, 맵 로딩이 끝날 때까지 로딩화면 대기시키기
		modeToSpawn.InstantiateAsync();
		mapToSpawn.InstantiateAsync();
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
		int nextStageIndex = CurrentStage.Index + 1;
		if (nextStageIndex >= CurrentLevel.Stages.Length)
		{
			//현재 스테이지가 마지막임
			nextStage = null;
			return false;
		}
		string progressKey = CurrentLevel.GetPath().Append("_Progress").ToString();
		int progress = PlayerPrefs.GetInt(progressKey, 0);
		if (nextStageIndex > progress)
		{
			//다음 스테이지가 해금되지 않음
			nextStage = null;
			return false;
		}

		nextStage = CurrentLevel.Stages[nextStageIndex];
		return true;
	}

	/// <summary>
	/// 다음 스테이지로 이동
	/// </summary>
	public async Awaitable LoadNextStageAsync()
	{
		if (TryGetNextStage(out var nextStage))
		{
			await LoadStageAsync(CurrentChapter, CurrentLevel, nextStage);
		}
	}
}
