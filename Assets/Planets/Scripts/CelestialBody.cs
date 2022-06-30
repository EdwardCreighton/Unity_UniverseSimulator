using UnityEngine;

namespace PocketUniverse
{
    public class CelestialBody : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Vector3 initVelocity;
        [SerializeField] private float density;
        [SerializeField] private float radius;
        [SerializeField] private float mass;

        private Vector3 currentVelocity;
        
        #endregion

        #region Initialization

        public void InitCelestialBody()
        {
            SetMass();

            currentVelocity = initVelocity;
        }
        
        public void Accelerate(Vector3 acceleration)
        {
            currentVelocity += acceleration;
        }

        private void SetMass()
        {
            float volume = 4 * Mathf.PI * radius * radius * radius / 3;
            
            mass = volume * density;
        }

        #endregion

        #region Getters

        public float GetMass()
        {
            return mass;
        }
        
        public Vector3 GetVelocity()
        {
            return currentVelocity;
        }

        #endregion
    }
}
