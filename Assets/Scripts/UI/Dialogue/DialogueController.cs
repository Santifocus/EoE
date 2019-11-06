using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class DialogueController : MonoBehaviour
	{
		public static DialogueController Instance { get; private set; }
		private const string COLOR_CLOSER = "</color>";
		private const string HIDE_COLOR = "<color=#00000000>";

		private DialogueBox dialogueContainer;
		private Queue<Dialogue> quedDialogues;
		private bool displayingDialogue;
		private void Start()
		{
			Instance = this;
			dialogueContainer = Instantiate(GameController.CurrentGameSettings.DialogueBoxPrefab, transform);
			quedDialogues = new Queue<Dialogue>();
			ClearDisplay();
		}
		public static void ShowDialogue(Dialogue newDialogue)
		{
			Instance.quedDialogues.Enqueue(newDialogue);
			if (!Instance.displayingDialogue)
			{
				Instance.StartCoroutine(Instance.DisplayDialogue());
			}
		}
		private IEnumerator DisplayDialogue()
		{
			while(quedDialogues.Count > 0)
			{
				if (displayingDialogue)
				{
					//Clear the old dialogue
					yield return new WaitForSeconds(GameController.CurrentGameSettings.DelayToNextDialogue);
					ClearDisplay();
				}
				else
				{
					displayingDialogue = true;
				}

				yield return new WaitForSeconds(GameController.CurrentGameSettings.ShowDialogueBaseDelay); 

				dialogueContainer.gameObject.SetActive(true);
				Dialogue targetDialogue = quedDialogues.Dequeue();
				string currentText = targetDialogue.ToString();
				int stringIndex = 0;

				for (int i = 0; i < targetDialogue.parts.Length; i++)
				{
					//Add the color opener for this part
					string colorOpener = ColorToColorOpener(targetDialogue.parts[i].TextColor);
					currentText = currentText.Insert(stringIndex, colorOpener);
					stringIndex += colorOpener.Length;

					for(int j = 0; j < targetDialogue.parts[i].Text.Length; j++)
					{
						if (GameController.CurrentGameSettings.SkipDelayOnSpace)
						{
							float delay = targetDialogue.parts[i].Text[j] == ' ' ? 0 : GameController.CurrentGameSettings.DialogueDelayPerLetter;
							yield return new WaitForSeconds(delay);
						}
						else
							yield return new WaitForSeconds(GameController.CurrentGameSettings.DialogueDelayPerLetter);

						stringIndex++;
						dialogueContainer.textDisplay.text = currentText.Insert(stringIndex, HIDE_COLOR);
					}

					//Now close the color of this part
					currentText = currentText.Insert(stringIndex, COLOR_CLOSER);
					stringIndex += COLOR_CLOSER.Length;
					dialogueContainer.textDisplay.text = currentText.Insert(stringIndex, HIDE_COLOR);
				}
				if(targetDialogue.onFinish != null)
					targetDialogue.onFinish?.Invoke();
			}

			yield return new WaitForSeconds(GameController.CurrentGameSettings.DelayToNextDialogue);
			ClearDisplay();
			displayingDialogue = false;
		}

		private string ColorToColorOpener(Color col)
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(col) + ">";
		}

		private void ClearDisplay()
		{
			dialogueContainer.textDisplay.text = "";
			dialogueContainer.gameObject.SetActive(false);
		}
	}

	[System.Serializable]
	public class Dialogue
	{
		public DialoguePart[] parts;
		public int totalTextLenght;

		public delegate void OnFinishDialogue();
		public OnFinishDialogue onFinish;
		public Dialogue(OnFinishDialogue onFinish, params (string, Color)[] parts)
		{
			this.parts = new DialoguePart[parts.Length];
			this.onFinish = onFinish;
			for (int i = 0; i < parts.Length; i++)
			{
				totalTextLenght += parts[i].Item1.Length;
				this.parts[i] = new DialoguePart(parts[i].Item1, parts[i].Item2);
			}
		}
		public override string ToString()
		{
			string fullText = "";
			for(int i = 0; i < parts.Length; i++)
			{
				fullText += parts[i].Text;
			}
			return fullText;
		}

		[System.Serializable]
		public class DialoguePart
		{
			public string Text;
			public Color TextColor;
			public DialoguePart(string Text, Color TextColor)
			{
				this.Text = Text;
				this.TextColor = TextColor;
			}
		}
	}
}