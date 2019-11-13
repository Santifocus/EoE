using System.Collections;
using UnityEngine;

namespace EoE.UI
{
	public class TransitionUI : MonoBehaviour
	{
		[SerializeField] private float standardTransitionTime = default;
		[SerializeField] private Vector2 standardStartPos = default;
		[SerializeField] private Vector2 standardEndPos = default;
		[SerializeField] private bool disableOnEndPos = true;
		[SerializeField] private bool ignoreTimeScale = default;

		public delegate void OnFinishCall();
		public RectTransform rTransform { get; private set; }
		private bool inTransition;
		private bool curState;

		private void Start()
		{
			rTransform = transform as RectTransform;
			if (rTransform == null)
				throw new System.Exception("Transition UI cannot be used on non Rect Transforms.");
		}
		public void ChangeTransitionState(bool newState, OnFinishCall finishCall = null)
		{
			if (inTransition)
				StopAllCoroutines();

			curState = newState;
			gameObject.SetActive(true);
			StartCoroutine(Goto(curState ? standardStartPos : standardEndPos, finishCall));
		}
		public void CustomTransition(Vector2 targetPos, OnFinishCall finishCall = null, float? time = null)
		{
			if (inTransition)
				StopAllCoroutines();

			curState = true;
			gameObject.SetActive(true);
			StartCoroutine(Goto(targetPos, finishCall, time));
		}

		private IEnumerator Goto(Vector2 targetPos, OnFinishCall finishCall = null, float? time = null)
		{
			inTransition = true;
			Vector2 startPos = rTransform.anchoredPosition;
			float timer = 0;
			float finalTime = time.HasValue ? time.Value : standardTransitionTime;

			while (timer < finalTime)
			{
				yield return new WaitForEndOfFrame();
				timer += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

				rTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, timer / finalTime);
			}

			rTransform.anchoredPosition = targetPos;
			inTransition = false;

			if (!curState && disableOnEndPos)
			{
				gameObject.SetActive(false);
			}

			if (finishCall != null)
				finishCall?.Invoke();
		}
	}
}