using UnityEngine;
using UnityEngine.InputSystem;

namespace PocketUniverse.Input
{
    public class InputData
    {
        public Vector2 look;
        public Vector2 move;
    }
    
    public class InputListener : MonoBehaviour
    {
        #region Fields

        private InputData inputData = new InputData();

        #endregion

        #region Events

        public void OnLook(InputValue value)
        {
            inputData.look = value.Get<Vector2>();
        }

        public void OnMove(InputValue value)
        {
            inputData.move = value.Get<Vector2>();
        }

        #endregion

        #region Getter

        public InputData GetInput()
        {
            return inputData;
        }

        #endregion
    }
}
