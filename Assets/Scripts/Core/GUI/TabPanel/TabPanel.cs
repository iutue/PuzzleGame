using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 복수의 탭을 가지는 패널 GUI
/// </summary>
public class TabPanel : UIBehaviour
{
	/// <summary>
	/// 탭 버튼에 대응하는 탭 콘텐츠
	/// </summary>
	[Serializable]
	public struct TabContent
	{
		public TabButton Button;
		public SlidePanel Content;
	}
	[SerializeField]
	TabContent[] _tabContents;
	/// <summary>
	/// 현재 열려있는 탭의 번호
	/// </summary>
	int _currentTabIndex;
	/// <summary>
	/// 최초로 열 탭의 번호
	/// </summary>
	int _initialTabIndex;

	protected override void Start()
	{
		for (int i = 0; i < _tabContents.Length; i++)
		{
			_tabContents[i].Button.Init(i, OnTabButtonClicked);
		}
		//시작 탭 열기
		_currentTabIndex = _initialTabIndex;
		_tabContents[_currentTabIndex].Content.SetState(SlidePanel.State.Focus);
	}

	/// <summary>
	/// 탭 버튼이 클릭됐을 때 호출됨
	/// </summary>
	void OnTabButtonClicked(int index)
	{
		if (_currentTabIndex == index)
		{
			//이미 열린 탭임
			return;
		}

		int oldIndex = _currentTabIndex;
		_currentTabIndex = index;
		if (oldIndex < index)
		{
			//현재 탭의 오른쪽 탭 열기
			_tabContents[oldIndex].Content.Close(SlidePanel.State.Next);
			_tabContents[index].Content.Open(SlidePanel.State.Prev);
		}
		else
		{
			//현재 탭의 왼쪽 탭 열기
			_tabContents[oldIndex].Content.Close(SlidePanel.State.Prev);
			_tabContents[index].Content.Open(SlidePanel.State.Next);
		}
	}
}
