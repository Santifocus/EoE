using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.UI
{
	public class NotificationController : MonoBehaviour
	{
		private float RESORTING_STOP_THRESHOLD = 0.1f;

		[SerializeField] private NotificationDisplay displayPrefab = default;
		[SerializeField] private RectTransform notificationParent = default;
		[SerializeField] private float lineHeight = default;
		[SerializeField] private float resortingSpeed = 3;

		private List<NotificationDisplay> activeNotifications;
		private PoolableObject<NotificationDisplay> notificationInstancePool;
		public static NotificationController Instance { get; private set; }
		private bool resorting;
		private void Start()
		{
			Instance = this;
			activeNotifications = new List<NotificationDisplay>();
			notificationInstancePool = new PoolableObject<NotificationDisplay>(15, true, displayPrefab, notificationParent);
		}
		public NotificationDisplay AddNotification(Notification info)
		{
			NotificationDisplay newNotification = notificationInstancePool.GetPoolObject();
			newNotification.Setup(info);
			activeNotifications.Add(newNotification);
			newNotification.rectTransform.anchoredPosition = new Vector2(newNotification.rectTransform.anchoredPosition.x, activeNotifications.Count * -lineHeight);
			return newNotification;
		}
		public void RemoveNotification(NotificationDisplay display)
		{
			for(int i = 0; i < activeNotifications.Count; i++)
			{
				if(activeNotifications[i] == display)
				{
					activeNotifications.RemoveAt(i);
					display.ReturnToPool();
					resorting = true;
					break;
				}
			}
		}
		private void Update()
		{
			if (resorting)
			{
				float lerpPoint = Time.deltaTime * resortingSpeed;
				float highestDif = 0;
				for (int i = 0; i < activeNotifications.Count; i++)
				{
					Vector2 targetPos = new Vector2(activeNotifications[i].rectTransform.anchoredPosition.x, i * -lineHeight);
					activeNotifications[i].rectTransform.anchoredPosition = Vector2.Lerp(activeNotifications[i].rectTransform.anchoredPosition, targetPos, lerpPoint);
					float dif = Mathf.Abs(targetPos.y - activeNotifications[i].rectTransform.anchoredPosition.y);
					if (highestDif < dif)
						highestDif = dif;
				}

				if(highestDif <= RESORTING_STOP_THRESHOLD)
				{
					resorting = false;
					for (int i = 0; i < activeNotifications.Count; i++)
					{
						activeNotifications[i].rectTransform.anchoredPosition = new Vector2(activeNotifications[i].rectTransform.anchoredPosition.x, i * -lineHeight);
					}
				}
			}
		}
	}
}