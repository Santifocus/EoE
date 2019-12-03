using TMPro;
using UnityEngine;

namespace EoE
{
	public class DamageNumber : MonoBehaviour, IPoolableObject<DamageNumber>
	{
		private const float PERLIN_NOISE_ZOOM = 60;
		private const float MAX_NOISE_MARCH_DISTANCE = 200;
		private static bool PerlinNoiseScrollSetup = false;
		private static int MarchDirectionX = 1;
		private static int MarchDirectionY = 1;
		private static int MarchDirectionZ = 1;
		private static Vector2 PerlinNoiseScrollX;
		private static Vector2 PerlinNoiseScrollY;
		private static Vector2 PerlinNoiseScrollZ;

		public PoolableObject<DamageNumber> SelfPool { get; set; }
		public TextMeshPro display = default;
		[SerializeField] private SpriteRenderer critIcon = default;
		private float lifeTime;

		private Gradient changeGradient;
		private Vector3 velocity;

		public void BeginDisplay(Vector3 velocity, Gradient changeGradient, string displayedText, bool wasCritcal)
		{
			if (!PerlinNoiseScrollSetup)
				SetupPerlinNoiseScroll();

			lifeTime = 0;
			this.changeGradient = changeGradient;
			transform.localEulerAngles = new Vector2(PlayerCameraController.CurRotation.y, PlayerCameraController.CurRotation.x);

			Color curColor = GetCurrentColor(0);

			display.text = displayedText;
			display.color = curColor;
			display.ForceMeshUpdate();

			this.velocity = velocity + GetPseudoRandomVelocity();

			if (wasCritcal)
			{
				critIcon.color = curColor;
				critIcon.gameObject.SetActive(true);
				critIcon.transform.localPosition = new Vector3(display.textBounds.extents.x + 0.25f, 0, 0);
			}
			else
			{
				critIcon.gameObject.SetActive(false);
			}
		}

		private void SetupPerlinNoiseScroll()
		{
			PerlinNoiseScrollSetup = true;
			PerlinNoiseScrollX = new Vector2((Random.value - 0.5f) * MAX_NOISE_MARCH_DISTANCE * 1.5f, (Random.value - 0.5f) * MAX_NOISE_MARCH_DISTANCE * 1.5f);
			PerlinNoiseScrollY = new Vector2((Random.value - 0.5f) * MAX_NOISE_MARCH_DISTANCE * 1.5f, (Random.value - 0.5f) * MAX_NOISE_MARCH_DISTANCE * 1.5f);
			PerlinNoiseScrollZ = new Vector2((Random.value - 0.5f) * MAX_NOISE_MARCH_DISTANCE * 1.5f, (Random.value - 0.5f) * MAX_NOISE_MARCH_DISTANCE * 1.5f);
		}

		private Vector3 GetPseudoRandomVelocity()
		{
			UpdateNoiseMarching();

			float xForce = (Mathf.PerlinNoise(PerlinNoiseScrollX.x, PerlinNoiseScrollX.y) - 0.5f) * 2;
			float yForce = (Mathf.PerlinNoise(PerlinNoiseScrollY.x, PerlinNoiseScrollY.y) - 0.5f) * 2;
			float zForce = (Mathf.PerlinNoise(PerlinNoiseScrollZ.x, PerlinNoiseScrollZ.y) - 0.5f) * 2;
			Vector3 outer = new Vector3(xForce, yForce, zForce).normalized * GameController.CurrentGameSettings.DamageNumberRandomMovementPower;

			return outer;
			//Local functions
			void UpdateNoiseMarching()
			{
				PerlinNoiseScrollX.x += 1f / PERLIN_NOISE_ZOOM * MarchDirectionX;
				PerlinNoiseScrollY.x += 1f / PERLIN_NOISE_ZOOM * MarchDirectionY;
				PerlinNoiseScrollZ.x += 1f / PERLIN_NOISE_ZOOM * MarchDirectionZ;

				CheckMarchDirection(PerlinNoiseScrollX.x, ref MarchDirectionX);
				CheckMarchDirection(PerlinNoiseScrollY.x, ref MarchDirectionY);
				CheckMarchDirection(PerlinNoiseScrollZ.x, ref MarchDirectionZ);
			}
			void CheckMarchDirection(float marchPoint, ref int direction)
			{
				if (marchPoint * direction > MAX_NOISE_MARCH_DISTANCE)
					direction *= -1;
			}
		}

		private void Update()
		{
			lifeTime += Time.deltaTime;
			float lifePoint = lifeTime / GameController.CurrentGameSettings.DamageNumberLifeTime;

			Color curColor = GetCurrentColor(lifePoint);
			display.color = curColor;
			critIcon.color = curColor;

			transform.position += velocity * Time.deltaTime;
			transform.localEulerAngles = new Vector2(PlayerCameraController.CurRotation.y, PlayerCameraController.CurRotation.x);
			if (lifePoint > 1)
				ReturnToPool();
		}
		private Color GetCurrentColor(float lifePoint)
		{
			return changeGradient.Evaluate(lifePoint);
		}
		public void ReturnToPool()
		{
			SelfPool.ReturnPoolObject(this);
		}
	}
}