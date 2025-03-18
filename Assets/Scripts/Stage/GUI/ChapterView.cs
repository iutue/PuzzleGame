using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterView : UIBehaviour
{
	[SerializeField]
	ChapterData _chapter;

	[Header("Level")]
	[SerializeField]
	RectTransform _levelViewParent;
	[SerializeField]
	GameObject _levelViewPrefab;

	protected override void Start()
	{
		//레벨 뷰 생성
		for (int i = 0; i < _chapter.Levels.Length; i++)
		{
			var levelView = Instantiate(_levelViewPrefab, _levelViewParent, false).GetComponent<LevelView>();
			levelView.Init(_chapter.Levels[i]);
		}
	}
}
