using UnityEngine;

namespace Genetics.Enviromental
{
    public class BaseFood : MonoBehaviour
    {
        [SerializeField] private SphereCollider _collider;
        [SerializeField] private Rigidbody _rb;

        protected virtual void Awake()
        {
            if (!TryGetComponent<SphereCollider>(out _collider))
                _collider = gameObject.AddComponent<SphereCollider>();
            _collider.isTrigger = false;
            if (!TryGetComponent<Rigidbody>(out _rb))
                _rb = gameObject.AddComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}