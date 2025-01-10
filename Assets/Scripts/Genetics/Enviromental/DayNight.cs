using Genetics.Enviromental;
using UnityEngine;

namespace Genetics
{
    public class DayNight : MonoBehaviour
    {
        [SerializeField] private float _secondsPerDay = 120f;
        [SerializeField] private float _secondsPerNight = 120f;

        public float DaysPassed { get; private set; }
        public float SecondsPassed { get; private set; }
        private float _totalCycleDuration;
        private float _currentCycleTime;

        [Header("Light Settings")] [SerializeField]
        private Light _directionalLight;

        [SerializeField] private float _initialXRotation = -5f;
        [Range(0f, 1f)] [SerializeField] private float _minLightIntensity = 0.1f;
        [Range(0f, 1f)] [SerializeField] private float _maxLightIntensity = 1f;

        private float _lightIntensity;

        public float CurrentHour { get; private set; }

        private void Start()
        {
            _totalCycleDuration = _secondsPerDay + _secondsPerNight;
            _currentCycleTime = 0f;
            DaysPassed = 0f;
            SecondsPassed = 0f;
            GameManager.Instance.FoodSpawner.NewDay();
            GameManager.Instance.EnviromentSpawner.Spawn();
        }

        private void Update()
        {
            UpdateCycle();
            UpdateLightIntensity();
            UpdateLightRotation();
        }

        private void UpdateCycle()
        {
            _currentCycleTime += Time.deltaTime;
            SecondsPassed += Time.deltaTime;

            if (_currentCycleTime >= _totalCycleDuration)
            {
                _currentCycleTime = 0f;
                DaysPassed++;
                GameManager.Instance.FoodSpawner.NewDay();
                GameManager.Instance.EnviromentSpawner.Spawn();
            }

            CurrentHour = (24f * _currentCycleTime) / _totalCycleDuration;
        }

        private void UpdateLightIntensity()
        {
            float cycleFraction = _currentCycleTime / _totalCycleDuration;

            if (cycleFraction < _secondsPerDay / _totalCycleDuration)
            {
                float dayFraction = cycleFraction / (_secondsPerDay / _totalCycleDuration);
                _lightIntensity = Mathf.Lerp(_minLightIntensity, _maxLightIntensity, dayFraction);
            }
            else
            {
                float nightFraction = (cycleFraction - _secondsPerDay / _totalCycleDuration) /
                                      (_secondsPerNight / _totalCycleDuration);
                _lightIntensity = Mathf.Lerp(_maxLightIntensity, _minLightIntensity, nightFraction);
            }

            if (_directionalLight != null)
            {
                _directionalLight.intensity = _lightIntensity;
            }
        }

        private void UpdateLightRotation()
        {
            if (_directionalLight == null) return;

            float cycleFraction = _currentCycleTime / _totalCycleDuration;
            float xRotation = Mathf.Lerp(_initialXRotation, _initialXRotation + 360f, cycleFraction);

            _directionalLight.transform.rotation = Quaternion.Euler(xRotation, -30f, 0f);
        }
    }
}