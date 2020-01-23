using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.UI
{
	public class DialogueController : MonoBehaviour
	{
		public static DialogueController Instance { get; private set; }
		private const string SPLITTER = ColoredText.COLOR_CLOSER + "<color=#00000000>";

		[SerializeField] private RectTransform dialogueBoxParentMain = default;
		[SerializeField] private RectTransform dialogueBoxParentMenu = default;
		private DialogueBox dialogueContainerMain;
		private DialogueBox dialogueContainerMenu;
		private List<QueuedDialogue> quedDialogues;
		private bool displayingDialogue;

		private void Start()
		{
			Instance = this;
			dialogueContainerMain = Instantiate(GameController.CurrentGameSettings.DialogueBoxPrefab, dialogueBoxParentMain);
			dialogueContainerMenu = Instantiate(GameController.CurrentGameSettings.DialogueBoxPrefab, dialogueBoxParentMenu);
			quedDialogues = new List<QueuedDialogue>();
			ClearDisplay();
		}
		public QueuedDialogue QueueDialogue(Dialogue info)
		{
			QueuedDialogue newQueuedDialogue = new QueuedDialogue(info);
			Instance.quedDialogues.Add(newQueuedDialogue);
			if (!Instance.displayingDialogue)
			{
				Instance.StartCoroutine(Instance.DisplayDialogue());
			}
			return newQueuedDialogue;
		}
		private IEnumerator DisplayDialogue()
		{
			displayingDialogue = true;

			while (quedDialogues.Count > 0)
			{
				QueuedDialogue targetDialogue = quedDialogues[0];
				quedDialogues.RemoveAt(0);

				dialogueContainerMain.gameObject.SetActive(false);
				dialogueContainerMenu.gameObject.SetActive(false);

				bool inMain = targetDialogue.BaseInfo.CanvasTarget == CanvasTarget.Main;
				DialogueBox dialogueContainer = inMain ? dialogueContainerMain : dialogueContainerMenu;
				dialogueContainer.gameObject.SetActive(true);

				dialogueContainer.icon = targetDialogue.BaseInfo.dialogueIcon;
				dialogueContainer.TextDisplay.text = "";
				string totalText = "";

				for(int i = 0; i < targetDialogue.BaseInfo.parts.Length; i++)
				{
					string partText = targetDialogue.BaseInfo.parts[i].ToString();
					string nonColoredText = targetDialogue.BaseInfo.parts[i].text;

					int curInsertIndex = 0;
					float indexTimer = 0;

					while(curInsertIndex < nonColoredText.Length)
					{
						if (targetDialogue.ShouldRemove)
							goto DoneDisplaying;

						yield return new WaitForEndOfFrame();

						if(!inMain || !GameController.GameIsPaused)
							indexTimer += Time.unscaledDeltaTime;

						while ((indexTimer >= GameController.CurrentGameSettings.DialogueDelayPerLetter) && (curInsertIndex < nonColoredText.Length))
						{
							if (!GameController.CurrentGameSettings.SkipDelayOnSpace || nonColoredText[curInsertIndex] != ' ')
								indexTimer -= GameController.CurrentGameSettings.DialogueDelayPerLetter;

							curInsertIndex++;
						}
						int size = "<color=#00000000>".Length;
						dialogueContainer.TextDisplay.text = totalText + partText.Insert(curInsertIndex + size, SPLITTER);
					}
					totalText += partText;
					dialogueContainer.TextDisplay.text = totalText;
				}

				targetDialogue.DoneDisplaying = true;
				yield return new WaitUntil(() => targetDialogue.ShouldRemove);
			DoneDisplaying:;
				ClearDisplay();
			}

			displayingDialogue = false;
		}

		private void ClearDisplay()
		{
			dialogueContainerMain.TextDisplay.text = "";
			dialogueContainerMenu.TextDisplay.text = "";

			dialogueContainerMain.gameObject.SetActive(false);
			dialogueContainerMenu.gameObject.SetActive(false);
		}
	}

	public class QueuedDialogue
	{
		public Dialogue BaseInfo;
		public bool ShouldRemove;
		public bool DoneDisplaying;

		public QueuedDialogue(Dialogue BaseInfo)
		{
			this.BaseInfo = BaseInfo;
		}
	}
}