using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TFrameWork
{

    /// <summary>
    /// Mono帧更新管理器
    /// </summary>
    public class MonoUpdate : AutoMonoMgr<MonoUpdate>
    {
        private event UnityAction updateEvent;
        private event UnityAction fixUpdateEvent;
        private event UnityAction lateUpdateEvent;

        public void AddUpdate(UnityAction action)
        {
            updateEvent += action;
        }
        public void RemoveUpdate(UnityAction action)
        {
            updateEvent -= action;
        }
        public void AddFixUpdate(UnityAction action)
        {
            fixUpdateEvent += action;
        }
        public void RemoveFixUpdate(UnityAction action)
        {
            fixUpdateEvent -= action;
        }
        public void AddLateUpdate(UnityAction action)
        {
            lateUpdateEvent += action;
        }
        public void RemoveLateUpdate(UnityAction action)
        {
            lateUpdateEvent -= action;
        }

        private void Update()
        {
            updateEvent?.Invoke();
        }

        private void FixedUpdate()
        {
            fixUpdateEvent?.Invoke();
        }

        private void LateUpdate()
        {
            lateUpdateEvent?.Invoke();
        }
    }
}
