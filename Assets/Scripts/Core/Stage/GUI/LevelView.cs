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
	Image _titleImage;
	[SerializeField]
	RectTransform _stageButtonParent;
	[SerializeField]
	GameObject _stageButtonPrefab;

	public void Init(LevelData levelData)
	{
		_ownerLevel = levelData;
		_titleText.text = levelData.Title.GetLocalizedString();
		_titleImage.sprite = levelData.TitleImage;
		for (int i = 0; i < levelData.Stages.Length; i++)
		{
			var stageButton = Instantiate(_stageButtonPrefab, _stageButtonParent, false).GetComponent<StageButton>();
			stageButton.Init(i, levelData.Stages[i], OnStageButtonClicked);
		}
	}

	void OnStageButtonClicked(int stageIndex)
	{
		StageManager.Instance.LoadStageAsync(null, _ownerLevel, _ownerLevel.Stages[stageIndex]);
	}
}
