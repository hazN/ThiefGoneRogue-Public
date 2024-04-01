using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        private GameObject user;
        private IEnumerable<GameObject> targets;
        private Vector3 OriginalTarget;
        private Transform targetedPoint;
        private bool isCanceled = false;

        public string Name => throw new NotImplementedException();

        public AbilityData(GameObject user)
        {
            this.user = user;
        }

        public IEnumerable<GameObject> GetTargets()
        {
            return targets;
        }
        public void SetTargets(IEnumerable<GameObject> targets)
        {
            this.targets = targets;
        }
        public GameObject GetUser()
        {
            return user;
        }

        public void SetTargetedPoint(Transform targetedPoint)
        {
            this.targetedPoint = targetedPoint;
        }
        public Transform GetTargetedPoint()
        {
            return targetedPoint;
        }
        public void SetOriginalTarget(Vector3 originalTarget)
        {
            this.OriginalTarget = originalTarget;
        }
        public Vector3 GetOriginalTarget()
        {
            return OriginalTarget;
        }
        public void StartCoroutine(IEnumerator coroutine)
        {
            user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public void Cancel()
        {
            isCanceled = true;
        }
        public bool IsCanceled()
        {
            return isCanceled;
        }

        public Quaternion GetAimDirection()
        {
            return Quaternion.LookRotation(targetedPoint.position - user.transform.position);
        }
    }
}