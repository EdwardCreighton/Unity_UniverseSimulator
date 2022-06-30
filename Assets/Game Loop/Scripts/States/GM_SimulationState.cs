using UnityEngine;

namespace PocketUniverse.GM_States
{
    public class GM_SimulationState : GM_BaseState
    {
        public override void OnEnterState(GameManager controller, PhysicSimulation physSimScript)
        {
            
        }

        public override void OnUpdateState(GameManager controller, PhysicSimulation physSimScript)
        {
            physSimScript.SetDeltaTime(Time.fixedDeltaTime);
            physSimScript.RunTimeSimulation();
        }

        public override void OnExitState(GameManager controller, PhysicSimulation physSimScript)
        {
            
        }
    }
}
