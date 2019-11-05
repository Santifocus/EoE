using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EoE.Entities;
using UnityEngine.UI;
using EoE.Controlls;

namespace EoE.UI
{
	public class CharacterMenuMain : CharacterMenuPage
	{
		private const int PLAYER_MODEL_DISPLAY_LAYER = 14;
		[Space(10)]
		[SerializeField] private TextMeshProUGUI levelDisplay = default;
		[SerializeField] private RawImage modelDisplay = default;
		[SerializeField] private Camera modelRendererCameraPrefab = default;
		[SerializeField] private Vector3 cameraStandardRotation = new Vector3(30, 180, 0);
		[SerializeField] private Vector3 cameraToModelDistance = new Vector3(0,2,8);

		[Space(5)]
		[SerializeField] private Vector2 cameraVerticalAngleClamps = new Vector2(30, -30);
		[SerializeField] private Vector2 cameraRotationSpeed = new Vector2(4, 2);
		[SerializeField] private float cameraSmoothTime = 2;

		[Space(10)]
		[SerializeField] private TextMeshProUGUI healthDisplay = default;
		[SerializeField] private TextMeshProUGUI manaDisplay = default;
		[SerializeField] private TextMeshProUGUI enduranceDisplay = default;
		[SerializeField] private TextMeshProUGUI defenseDisplay = default;
		[SerializeField] private TextMeshProUGUI atkDisplay = default;
		[SerializeField] private TextMeshProUGUI mgaDisplay = default;
		[SerializeField] private TextMeshProUGUI skillpointDisplay = default;

		private bool playerModelSetup;
		private Transform cameraAnchor;
		private Camera modelRenderer;
		private Transform playerModel;
		private Vector2 curRotation;
		private Vector2 targetRotation;

		private void Update()
		{
			if (!ActivePage || !playerModelSetup)
				return;

			targetRotation += InputController.CharacterRotate * Time.unscaledDeltaTime * cameraRotationSpeed;

			//targetRotation.x %= 360;
			//curRotation.x %= 360;
			targetRotation.y = Mathf.Clamp(targetRotation.y, cameraVerticalAngleClamps.x, cameraVerticalAngleClamps.y);
			curRotation = new Vector2(Mathf.LerpAngle(curRotation.x, targetRotation.x, Time.unscaledDeltaTime / cameraSmoothTime), Mathf.LerpAngle(curRotation.y, targetRotation.y, Time.unscaledDeltaTime / cameraSmoothTime));

			cameraAnchor.eulerAngles = new Vector3(curRotation.y, curRotation.x, 0);
		}

		protected override void ResetPage()
		{
			if (!playerModelSetup) 
				SetupPlayerModel();

			UpdateStatDisplay();
			UpdateSprites();

			ResetPlayerModel();
		}

		private void UpdateStatDisplay()
		{
			healthDisplay.text = Player.Instance.curMaxHealth.ToString();
			manaDisplay.text = Player.Instance.curMaxMana.ToString();
			enduranceDisplay.text = Player.Instance.totalEnduranceContainers.ToString();

			defenseDisplay.text = Player.Instance.curDefense.ToString();
			atkDisplay.text = Player.Instance.curPhysicalDamage.ToString();
			mgaDisplay.text = Player.Instance.curMagicalDamage.ToString();

			skillpointDisplay.text = Player.AvailableBaseSkillPoints + " | " + Player.AvailableSpecialSkillPoints;
		}
		private void UpdateSprites()
		{

		}
		private void ResetPlayerModel()
		{
			levelDisplay.text = (Player.Instance.EntitieLevel + 1).ToString();
			targetRotation = curRotation = cameraStandardRotation;
			cameraAnchor.eulerAngles = new Vector3(curRotation.y, curRotation.x, 0);
		}
		private void SetupPlayerModel()
		{
			playerModelSetup = true;
			RenderTexture targetTexture = new RenderTexture((int)modelDisplay.rectTransform.rect.width, (int)modelDisplay.rectTransform.rect.height, 1);

			//Setup Playermodel
			playerModel = Instantiate(Player.Instance.modelTransform, Storage.EntitieStorage);
			SetLayerForAllChildren(playerModel);
			playerModel.transform.position = Vector3.zero;
			playerModel.transform.localEulerAngles = new Vector3(0, 180, 0);

			//Config the animator
			Animator animator = playerModel.GetComponent<Animator>();
			animator.updateMode = AnimatorUpdateMode.UnscaledTime;

			//Setup camera
			cameraAnchor = new GameObject("PlayerModelDisplayCameraAnchor").transform;
			cameraAnchor.SetParent(Storage.EntitieStorage);
			modelRenderer = Instantiate(modelRendererCameraPrefab, cameraAnchor);
			modelRenderer.transform.localPosition = new Vector3(cameraToModelDistance.x, cameraToModelDistance.y, -cameraToModelDistance.z);
			modelRenderer.targetTexture = targetTexture;
			modelRenderer.Render();

			modelDisplay.texture = targetTexture;

			void SetLayerForAllChildren(Transform parent)
			{
				parent.gameObject.layer = PLAYER_MODEL_DISPLAY_LAYER;
				
				//Apply to children
				int c = parent.childCount;
				for (int i = 0; i < c; i++)
				{
					//Apply to children of child
					if (parent.GetChild(i).childCount > 0)
					{
						SetLayerForAllChildren(parent.GetChild(i));
						continue;
					}
					parent.GetChild(i).gameObject.layer = PLAYER_MODEL_DISPLAY_LAYER;
				}
			}
		}
	}
}