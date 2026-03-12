using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public class ObjectPool<T> where T : MonoBehaviour, IPoolableObject
    {
        private readonly Stack<T> objects = new Stack<T>();
        private readonly T objectPrefab;
        private readonly Transform container;

        public ObjectPool(T prefab, Transform containerTransform, int initialSize = 10)
        {
            objectPrefab = prefab;
            container = containerTransform;
            initialSize = Math.Max(0, initialSize);
            for (int i = 0; i < initialSize; ++i)
            {
                InstantiateObject();
            }
        }

        private void InstantiateObject()
        {
            var newObj = GameObject.Instantiate(objectPrefab, container);
            newObj.gameObject.SetActive(false);
            objects.Push(newObj);
        }

        public T GetObject()
        {
            if (objects.Count == 0)
            {
                InstantiateObject();
            }
            var obj = objects.Pop();
            obj.transform.SetParent(null);
            obj.OnActivate();
            return obj;
        }

        public void ReleaseObject(T obj)
        {
            if (obj == null)
            {
                return;
            }
            objects.Push(obj);
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(container);
        }
    }
}