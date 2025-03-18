using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelView : UIBehaviour
{
	LevelData _ownerLevel;

	[SerializeField]
	TMP_Text _titleText;
	[SerializeField]
	Image _titleBackground;
	[SerializeField]
	RectTransform _stageButtonParent;
	[SerializeField]
	GameObject _stageButtonPrefab;

	public void Init(LevelData levelData)
	{
		_ownerLevel = levelData;
		_titleText.text = levelData.Title.GetLocalizedString();
		_titleBackground.sprite = levelData.TitleBackground;
		//스테이지 버튼 생성
		for (int i = 0; i < levelData.Stages.Length; i++)
		{
			var stageView = Instantiate(_stageButtonPrefab, _stageButtonParent, false).GetComponent<StageView>();
			stageView.Init(i, levelData.Stages[i], OnStageButtonClicked);
		}
	}

	void OnStageButtonClicked(int stageIndex)
	{
		MatchManager.Instance.LoadStageAsync(null, _ownerLevel, _ownerLevel.Stages[stageIndex]);
	}
}
