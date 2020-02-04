using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Behaivior
{
	public class GameAreaPart : MonoBehaviour
	{
		public static bool DrawOpaqueArea = false;
		private static readonly Color OpaqueDrawColor = new Color(0.5f, 0.5f, 1, 0.6f); 
		public enum AreaShape { Sphere, Box }
		[HideInInspector] public AreaShape areaShape = AreaShape.Sphere;
		[HideInInspector] public float sphereRadius = 50;
		[HideInInspector] public Vector3 boxSize = Vector3.one;
		private Bounds boxBounds;
		private void Start()
		{
			boxBounds = new Bounds(transform.position, boxSize);
		}
		public bool PlayerInPart()
		{
			if (areaShape == AreaShape.Sphere)
			{
				float dist = (transform.position - Player.Instance.transform.position).sqrMagnitude;
				return dist < (sphereRadius * sphereRadius);
			}
			else
			{
				return boxBounds.Contains(Player.Instance.transform.position);
			}
		}
		public void OnDrawGizmosSelected()
		{
			if (DrawOpaqueArea)
				Gizmos.color = OpaqueDrawColor;
			else
				Gizmos.color = Color.blue;
			if (areaShape == AreaShape.Sphere)
			{
				if (DrawOpaqueArea)
					Gizmos.DrawSphere(transform.position, sphereRadius);
				else
					Gizmos.DrawWireSphere(transform.position, sphereRadius);
			}
			else
			{
				if (DrawOpaqueArea)
					Gizmos.DrawCube(transform.position, boxSize);
				else
					Gizmos.DrawWireCube(transform.position, boxSize);
			}
		}
	}
}