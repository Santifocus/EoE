using EoE.Combatery;
using EoE.Controlls;
using EoE.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class CharacterMenuMain : CharacterMenuPage
	{
		private const int PLAYER_MODEL_DISPLAY_LAYER = 14;
		[Space(10)]
		[SerializeField] private TextMeshProUGUI levelDisplay = default;
		[SerializeField] private RawImage modelDisplay = default;
		[SerializeField] private Transform modelLight = default;
		[SerializeField] private Camera modelRendererCameraPrefab = default;
		[SerializeField] private Vector3 cameraStandardRotation = new Vector3(30, 180, 0);
		[SerializeField] private Vector3 cameraToModelDistance = new Vector3(0, 2, 8);

		[Space(5)]
		[SerializeField] private Vector2 cameraVerticalAngleClamps = new Vector2(30, -30);
		[SerializeField] private Vector2 cameraRotationSpeed = new Vector2(4, 2);
		[SerializeField] private float cameraSmoothSpeed = 2;

		[Space(10)]
		[SerializeField] private TextMeshProUGUI healthDisplay = default;
		[SerializeField] private TextMeshProUGUI manaDisplay = default;
		[UnityEngine.Serialization.FormerlySerializedAs("enduranceDisplay")] [SerializeField] private TextMeshProUGUI staminaDisplay = default;
		[SerializeField] private TextMeshProUGUI defenseDisplay = default;
		[SerializeField] private TextMeshProUGUI atkDisplay = default;
		[SerializeField] private TextMeshProUGUI mgaDisplay = default;
		[SerializeField] private TextMeshProUGUI skillpointDisplay = default;
		[SerializeField] private TextMeshProUGUI experienceDisplay = default;

		[Space(10)]
		[SerializeField] private int[] weaponHoldpointChildIndexes = default;

		private bool playerModelSetup;
		private Transform cameraAnchor;
		private Camera modelRenderer;
		private Transform playerModel;
		private Transform weaponHoldPoint;
		private Transform playerWeapon;
		private Vector2 curRotation;
		private Vector2 targetRotation;

		private void Update()
		{
			if (!ActivePage || !playerModelSetup)
				return;

			targetRotation += InputController.CameraMove * Time.unscaledDeltaTime * cameraRotationSpeed;

			targetRotation.y = Mathf.Clamp(targetRotation.y, cameraVerticalAngleClamps.x, cameraVerticalAngleClamps.y);
			curRotation = new Vector2(Mathf.LerpAngle(curRotation.x, targetRotation.x, Time.unscaledDeltaTime * cameraSmoothSpeed), Mathf.LerpAngle(curRotation.y, targetRotation.y, Time.unscaledDeltaTime * cameraSmoothSpeed));

			cameraAnchor.eulerAngles = modelLight.eulerAngles = new Vector3(curRotation.y, curRotation.x, 0);
			AnchorWeapon();
		}

		protected override void ResetPage()
		{
			if (!playerModelSetup)
				SetupPlayerModel();

			UpdatePlayerModel();
			UpdateStatDisplay();
			ResetPlayerModel();
		}

		private void UpdateStatDisplay()
		{
			healthDisplay.text = Player.Instance.curMaxHealth.ToString();
			manaDisplay.text = Player.Instance.curMaxMana.ToString();
			staminaDisplay.text = Player.Instance.curMaxStamina.ToString();

			defenseDisplay.text = Player.Instance.curDefense.ToString();
			atkDisplay.text = Player.Instance.curPhysicalDamage.ToString();
			mgaDisplay.text = Player.Instance.curMagicalDamage.ToString();

			skillpointDisplay.text = Player.Instance.AvailableSkillPoints + " | " + Player.Instance.AvailableAtributePoints;
			experienceDisplay.text = Player.Instance.TotalExperience + " / " + Player.Instance.RequiredExperienceForLevel;
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
			playerModel = Instantiate(Player.Instance.modelTransform, Storage.ParticleStorage);
			SetLayerForAllChildren(playerModel);
			playerModel.transform.position = Player.Instance.modelTransform.localPosition;
			playerModel.transform.localEulerAngles = new Vector3(0, 180, 0);

			weaponHoldPoint = playerModel;
			for (int i = 0; i < weaponHoldpointChildIndexes.Length; i++)
				weaponHoldPoint = weaponHoldPoint.GetChild(weaponHoldpointChildIndexes[i]);

			//Config the animator
			Animator animator = playerModel.GetComponent<Animator>();
			animator.updateMode = AnimatorUpdateMode.UnscaledTime;

			//Setup camera
			cameraAnchor = new GameObject("PlayerModelDisplayCameraAnchor").transform;
			cameraAnchor.SetParent(Storage.ParticleStorage);
			modelRenderer = Instantiate(modelRendererCameraPrefab, cameraAnchor);
			modelRenderer.transform.localPosition = new Vector3(cameraToModelDistance.x, cameraToModelDistance.y, -cameraToModelDistance.z);
			modelRenderer.targetTexture = targetTexture;
			modelRenderer.Render();
			modelRenderer.farClipPlane = cameraToModelDistance.z * 2;

			modelDisplay.texture = targetTexture;
		}
		private void UpdatePlayerModel()
		{
			if (playerWeapon)
				Destroy(playerWeapon.gameObject);

			if (WeaponController.Instance)
			{
				playerWeapon = WeaponController.Instance.CloneModel().transform;
				SetLayerForAllChildren(playerWeapon);
			}
			AnchorWeapon();
		}
		private void AnchorWeapon()
		{
			if (!playerWeapon || !WeaponController.Instance)
				return;


			Vector3 worldOffset =	WeaponController.Instance.weaponInfo.WeaponPositionOffset.x * weaponHoldPoint.right +
									WeaponController.Instance.weaponInfo.WeaponPositionOffset.y * weaponHoldPoint.up +
									WeaponController.Instance.weaponInfo.WeaponPositionOffset.z * weaponHoldPoint.forward;

			playerWeapon.transform.position = weaponHoldPoint.position + worldOffset;
			playerWeapon.transform.eulerAngles = weaponHoldPoint.eulerAngles + WeaponController.Instance.weaponInfo.WeaponRotationOffset;
		}
		private void SetLayerForAllChildren(Transform parent)
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
		protected override void DeactivatePage()
		{
			gameObject.SetActive(false);
		}
	}
}