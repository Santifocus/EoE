using System.Collections.Generic;
using UnityEngine;

namespace EoE
{
	public interface IPoolableObject<T> where T : MonoBehaviour, IPoolableObject<T>
	{
		PoolableObject<T> SelfPool { get; set; }
		void ReturnToPool();
		void Created();
	}
	public class PoolableObject<T> where T : MonoBehaviour, IPoolableObject<T>
	{
		private uint poolSize;
		public uint PoolSize { get => poolSize; set => IncreasePoolsize(value); }
		public List<T> activeObjects => ActiveObjects;

		private T ObjectToPool;
		private Transform PoolParent;
		private bool ReAdjustPoolsize;
		private List<T> ActiveObjects;
		private Queue<T> InActiveObjects;

		public PoolableObject(uint PoolSize, bool ReadjustPoolsize, T ObjectToPool, Transform PoolParent)
		{
			this.ObjectToPool = ObjectToPool;
			this.PoolParent = PoolParent;
			this.ReAdjustPoolsize = ReadjustPoolsize;

			ActiveObjects = new List<T>((int)PoolSize);
			InActiveObjects = new Queue<T>((int)PoolSize);

			IncreasePoolsize(PoolSize);
		}

		private void IncreasePoolsize(uint newPoolSize)
		{
			uint dif = newPoolSize - poolSize;
			if (dif <= 0)
				return;

			poolSize = newPoolSize;
			ActiveObjects.Capacity = (int)newPoolSize;

			for (uint i = 0; i < dif; i++)
			{
				T newObject = Object.Instantiate(ObjectToPool, PoolParent);
				newObject.gameObject.SetActive(false);
				newObject.SelfPool = this;

				newObject.Created();
				InActiveObjects.Enqueue(newObject);
			}
		}

		public T GetPoolObject()
		{
			if (InActiveObjects.Count == 0)
			{
				if (ReAdjustPoolsize)
				{
					IncreasePoolsize(poolSize + 1);
				}
				else
					return null;
			}

			T ret = InActiveObjects.Dequeue();
			ActiveObjects.Add(ret);
			ret.gameObject.SetActive(true);

			return ret;
		}
		public List<T> GetMultipleObjects(uint amount)
		{
			if (InActiveObjects.Count < amount)
			{
				if (ReAdjustPoolsize)
				{
					IncreasePoolsize(poolSize + (amount - (uint)InActiveObjects.Count));
				}
				else
					return null;
			}

			List<T> ret = new List<T>((int)amount);
			for (int i = 0; i < amount; i++)
			{
				T obj = InActiveObjects.Dequeue();
				obj.gameObject.SetActive(true);
				ret.Add(obj);
			}

			return ret;
		}
		public void ReturnPoolObject(T toQueue)
		{
			ActiveObjects.Remove(toQueue);
			toQueue.gameObject.SetActive(false);
			InActiveObjects.Enqueue(toQueue);
		}
	}
}