using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 종횡비에 따라 호출되는 이벤트
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class AspectRatioEvent : UIBehaviour
{
	/// <summary>
	/// 기준 종횡비
	/// </summary>
	public float ThresholdRatio;

	/// <summary>
	/// 종횡비가 기준보다 클 때
	/// </summary>
	public UnityEvent RatioIsBigger;
	/// <summary>
	/// 종횡비가 기준보다 작을 때
	/// </summary>
	public UnityEvent RatioIsSmaller;

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		var rect = GetComponent<RectTransform>().rect;
		if (rect.width / rect.height > ThresholdRatio)
		{
			RatioIsBigger.Invoke();
		}
		else
		{
			RatioIsSmaller.Invoke();
		}
	}
}
