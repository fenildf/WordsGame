using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZCC.GameObjectPool {

    /// <summary>继承该接口以支持对象池</summary>
    public interface IPool<T> {

        /// <summary>取出后重置</summary>
        void ResetObject();

        /// <summary>移除并添加到对象池</summary>
        void RemoveAndStore();
    }
        
    /// <summary>对象池异常</summary>
    public class ObjectPoolException : ArgumentException {
        public ObjectPoolException(string message,Exception innerException) : base(message, innerException) { }
        public ObjectPoolException(string message):base(message) { }
        public ObjectPoolException() : base() { }
    }

    /// <summary>对象池</summary>
    public static class ObjectPool<T> where T : class, IPool<T> {

        public static int Count {//当前容量
            get { return ObjectsInPool.Count; }
        }

        private static int MaxCapacity = 10;//最大容量

        private static Stack<T> ObjectsInPool;

        /// <summary>可以手动初始化</summary>
        public static void Init(int maxCapacity) {
            MaxCapacity = maxCapacity;
            ObjectsInPool = new Stack<T>();
        }
        public static void Init() {
            Init(2);
        }

        /// <summary>ctor</summary>
        static ObjectPool () {
            Init();
        }
            

        /// <summary>向对象池中添加游戏对象</summary>
        public static void StoreObject(T _Object) {
            if (ObjectsInPool.Count >= MaxCapacity) {
                MaxCapacity *= 2;//自动扩容
            }
            if (_Object == null) {
                throw new ObjectPoolException("对象池不能添加空对象");
            }
            if (ObjectsInPool.Contains(_Object)) {
                throw new ObjectPoolException("该对象已在对象池中");
            }
            ObjectsInPool.Push(_Object);
        }

        /// <summary>从对象池中取出游戏对象</summary>
        public static bool TryGetObject(out T _Object) {
            if (ObjectsInPool.Count == 0) {
                _Object = null; 
                return false;
            }
            T sourceObject = ObjectsInPool.Pop();
            //防止放入对象池的对象被置空
            if (sourceObject != null) {
                sourceObject.ResetObject();
                _Object = sourceObject;
                return true;
            }
            else {
                return TryGetObject(out _Object);
            }
        }

        /// <summary>在切换场景后手动调用 根据需要手动清空对象池中的对象引用 </summary>
        /// 因为场景切换后引用大部分都会变空
        public static void Clear() {
            ObjectsInPool = new Stack<T>();
        }

    }


}