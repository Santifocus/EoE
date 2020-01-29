using System.Collections;
using UnityEngine;

namespace EoE.UI
{
	public class TransitionUI : MonoBehaviour
	{
		public RectTransform rTransform;
		[SerializeField] private float standardTransitionTime = default;
		[SerializeField] private Vector2 activePosition = default;
		[SerializeField] private Vector2 disabledPosition = default;
		[SerializeField] private bool disableOnDisabledPos = true;
		[SerializeField] private bool ignoreTimeScale = default;

		public bool trueState => inTransition ? !curState : curState;
		private bool inTransition;
		private bool curState;

		private Vector2 startPos;
		private Vector2 targetPos;
		private float transitionTime;
		private float finalTransitionTime;
		private System.Action finishCall;

		public void ChangeTransitionState(bool newState, System.Action finishCall = null)
		{
			curState = newState;
			inTransition = true;
			gameObject.SetActive(true);

			this.startPos = rTransform.anchoredPosition;
			this.targetPos = curState ? activePosition : disabledPosition;
			this.transitionTime = 0;
			this.finalTransitionTime = standardTransitionTime;
			this.finishCall = finishCall;
		}
		public void CustomTransition(Vector2 targetPos, System.Action finishCall = null, float? time = null)
		{
			curState = true;
			inTransition = true;
			gameObject.SetActive(true);

			this.startPos = rTransform.anchoredPosition;
			this.targetPos = targetPos;
			this.transitionTime = 0;
			this.finalTransitionTime = time ?? standardTransitionTime;
			this.finishCall = finishCall;
		}
		private void Update()
		{
			if (inTransition)
			{
				transitionTime += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
				rTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, transitionTime / finalTransitionTime);
				if(transitionTime >= finalTransitionTime)
				{
					inTransition = false;
					rTransform.anchoredPosition = targetPos;
					if (finishCall != null)
					{
						finishCall?.Invoke();
					}
					if (!curState && disableOnDisabledPos)
					{
						gameObject.SetActive(false);
					}
				}
			}
		}
	}
}