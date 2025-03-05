using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class TransitionManager : MonoBehaviour
{
	public static TransitionManager Instance { get; private set; }

	[SerializeField]
	CanvasGroup _fadeImage;
	[SerializeField]
	float _fadeDuration;

	[SerializeField]
	Slider _progressBar;

	protected void Awake()
	{
		DontDestroyOnLoad(gameObject);

		Instance = this;
	}
	protected void Start()
	{
		//초기화 후 메인 화면으로 이동
		LoadScene("Main");
	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine(LoadSceneCoroutine(sceneName));
	}
	IEnumerator LoadSceneCoroutine(string sceneName)
	{
		_fadeImage.DOFade(1f, _fadeDuration);

		var sceneLoading = SceneManager.LoadSceneAsync(sceneName);
		sceneLoading.allowSceneActivation = false;
		while (!sceneLoading.isDone)
		{
			_progressBar.value = sceneLoading.progress;
			yield return null;

			if (sceneLoading.progress >= 0.9f)
			{
				_progressBar.value = 1f;
				sceneLoading.allowSceneActivation = true;
				break;
			}
		}
		_fadeImage.DOFade(0f, _fadeDuration);
	}
}
