using System;
using Genetics;
using UnityEngine;

namespace ProceduralCreature
{
    public class BodyTriggerDetector : MonoBehaviour
    {
        private bool _initialized = false;
        private CreatureContainer _container;
        private SphereCollider _collider;

        private void Awake()
        {
            if(!GetComponent<SphereCollider>())
            {
               _collider = gameObject.AddComponent<SphereCollider>();
               _collider.isTrigger = true;
            }
        }

        public void Init(CreatureContainer container, float perception)
        {
            _container = container;
            _initialized = true;
            _collider.radius = perception;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(!_initialized) return;
            _container.onTriggerEnterEvent?.Invoke(other);
        }
    }
}