using System;
using UnityEngine;

namespace Genetics.Environmental
{
    [RequireComponent(typeof(Collider))]
    public class EnviromentArea : MonoBehaviour
    {
        [SerializeField] private Enviroment AreaEnviroment; // { get; set; }
        [SerializeField] private Color areaColor=Color.green; // { get; set; }
        private BoxCollider _boxCollider;
        private DayNight _dayNight;
        
        private void Start()
        {
            _dayNight = GameManager.Instance.DayNight;
            _boxCollider = GetComponent<BoxCollider>();
            if (_boxCollider != null && !_boxCollider.isTrigger)
                GetComponent<Collider>().isTrigger = true;
        }


        private void LateUpdate()
        {
            AreaEnviroment.UpdateTemperature(_dayNight.CurrentHour);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Objet enter in area: " + other.name);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Objet left the area: " + other.name);
        }

        private void OnDrawGizmos()
        {
            if (_boxCollider == null)
                return;

            Matrix4x4 originalMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = areaColor; 
            Gizmos.DrawWireCube(_boxCollider.center, _boxCollider.size);
            Gizmos.matrix = originalMatrix;
        }
    }
}