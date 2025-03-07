using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 화면 전환을 관리하는 GUI
/// </summary>
public class TransitionManager : SingletonBehaviour<TransitionManager>
{
	/// <summary>
	/// 처음으로 불러올 씬
	/// </summary>
	[SerializeField]
	string _initialSceneName;

	[Space]
	[SerializeField]
	float _fadeDuration;
	[SerializeField]
	CanvasGroup _fadeGroup;
	[SerializeField]
	Slider _progressBar;

	protected void Start()
	{
		if (string.IsNullOrEmpty(_initialSceneName))
		{
			//현재 씬 보여주기
			FadeOut();
		}
		else
		{
			//초기 씬 불러오기
			LoadSceneAsync(_initialSceneName);
		}
	}

	/// <summary>
	/// 해당 이름의 씬을 비동기로 불러오기
	/// </summary>
	/// <param name="sceneName"></param>
	/// <returns></returns>
	public async Awaitable LoadSceneAsync(string sceneName)
	{
		await FadeIn();
		//씬 불러오기
		var loading = SceneManager.LoadSceneAsync(sceneName);
		while (!loading.isDone)
		{
			_progressBar.value = loading.progress;
			await Awaitable.NextFrameAsync();
		}
		_progressBar.value = 1f;
		await FadeOut();
	}

	async Awaitable FadeIn()
	{
		_fadeGroup.DOFade(1f, _fadeDuration);
		await Awaitable.WaitForSecondsAsync(_fadeDuration);
	}
	async Awaitable FadeOut()
	{
		_fadeGroup.DOFade(0f, _fadeDuration);
		await Awaitable.WaitForSecondsAsync(_fadeDuration);
	}
}
