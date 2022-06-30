using System.Collections;
using PocketUniverse.GM_States;
using UnityEngine;

namespace PocketUniverse
{
    public class GameManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private int simulationIterations;
        [SerializeField] private float trajectorySimulationDeltaTime = 0.02f;
        [SerializeField] private int computationsInOneFrame = 10;
        [Space]
        [SerializeField] private float gravitationalConstant = 0.004f;
        [SerializeField] private CelestialBody centralBody;

        private PhysicSimulation physSimScript;

        #endregion

        #region States

        private GM_BaseState currentState;

        public readonly GM_BaseState awakeState = new GM_AwakeState();
        public readonly GM_BaseState simulationState = new GM_SimulationState();
        public readonly GM_BaseState pauseState = new GM_PauseState();

        #endregion

        private void Awake()
        {
            InitFields();
            //TODO: Load bodies on the scene from the file
            InitPreloadedBodies();

            MoveToState(pauseState);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                if (currentState == pauseState)
                {
                    MoveToState(simulationState);
                }
                else if (currentState == simulationState)
                {
                    MoveToState(pauseState);
                }
            }
        }

        private void FixedUpdate()
        {
            currentState.OnUpdateState(this, physSimScript);
        }

        #region Initialization

        private void InitFields()
        {
            physSimScript = new PhysicSimulation(simulationIterations, gravitationalConstant, Time.fixedDeltaTime,
                trajectorySimulationDeltaTime, computationsInOneFrame);
            
            physSimScript.SetCentralBody(centralBody);
        }

        private void InitPreloadedBodies()
        {
            CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();

            if (bodies == null) return;
            
            foreach (var body in bodies)
            {
                body.InitCelestialBody();
                physSimScript.AddBody(body);
            }
        }

        #endregion

        public void MoveToState(GM_BaseState nextState)
        {
            currentState?.OnExitState(this, physSimScript);

            currentState = nextState;
            currentState.OnEnterState(this, physSimScript);
        }
    }
}
