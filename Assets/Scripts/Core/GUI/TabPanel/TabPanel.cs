using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 복수의 탭을 가지는 패널 GUI
/// </summary>
public class TabPanel : UIBehaviour
{
	/// <summary>
	/// 탭 버튼과 이에 대응하는 탭 콘텐츠
	/// </summary>
	[Serializable]
	public class Tab
	{
		public TabButton Button;
		public SlidePanel Content;
	}
	[SerializeField]
	Tab[] _tabs;
	/// <summary>
	/// 현재 탭의 번호
	/// </summary>
	[SerializeField]
	int _currentTabIndex;

	protected override void Start()
	{
		for (int i = 0; i < _tabs.Length; i++)
		{
			_tabs[i].Button.Init(i, OnTabButtonClicked);
		}
		//현재 탭으로 초기화
		OpenTab(_currentTabIndex);
	}

	/// <summary>
	/// 탭 버튼이 클릭됐을 때 호출됨
	/// </summary>
	void OnTabButtonClicked(int tabIndex)
	{
		if (_currentTabIndex == tabIndex)
		{
			//이미 열린 탭임
			return;
		}
		OpenTab(tabIndex);
	}

	/// <summary>
	/// 열린 탭을 닫고, 주어진 탭 열기
	/// </summary>
	void OpenTab(int tabIndex)
	{
		int oldIndex = _currentTabIndex;

		Tab oldTab = _tabs[_currentTabIndex];
		Tab newTab = _tabs[tabIndex];
		_currentTabIndex = tabIndex;

		oldTab.Button.Highlight.enabled = false;
		newTab.Button.Highlight.enabled = true;

		if (oldIndex < tabIndex)
		{
			//현재 탭의 오른쪽 탭 열기
			oldTab.Content.Close(SlidePanel.State.Next);
			newTab.Content.Open(SlidePanel.State.Prev);
		}
		else
		{
			//현재 탭의 왼쪽 탭 열기
			oldTab.Content.Close(SlidePanel.State.Prev);
			newTab.Content.Open(SlidePanel.State.Next);
		}
	}
}
