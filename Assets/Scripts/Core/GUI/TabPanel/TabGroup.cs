using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 복수의 탭을 관리하는 그룹 GUI
/// </summary>
public class TabGroup : UIBehaviour
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
	/// <summary>
	/// 모든 탭
	/// </summary>
	[SerializeField]
	Tab[] _tabs;
	/// <summary>
	/// 현재 열린 탭의 번호
	/// </summary>
	[SerializeField]
	int _currentTabIndex;

	protected override void Start()
	{
		//모든 탭 초기화
		for (int i = 0; i < _tabs.Length; i++)
		{
			_tabs[i].Button.Init(i, OnTabButtonClicked);
		}
		//초기 탭 열기
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
	/// 현재 탭 닫고, 새로운 탭 열기
	/// </summary>
	void OpenTab(int newIndex)
	{
		int oldIndex = _currentTabIndex;
		_currentTabIndex = newIndex;

		Tab oldTab = _tabs[oldIndex];
		Tab newTab = _tabs[newIndex];

		oldTab.Button.Highlight.enabled = false;
		newTab.Button.Highlight.enabled = true;

		if (oldIndex < newIndex)
		{
			//현재 탭보다 다음에 위치한 탭 열기
			oldTab.Content.Close(SlidePanel.State.Next);
			newTab.Content.Open(SlidePanel.State.Prev);
		}
		else
		{
			//현재 탭보다 이전에 위치한 탭 열기
			oldTab.Content.Close(SlidePanel.State.Prev);
			newTab.Content.Open(SlidePanel.State.Next);
		}
	}
}
