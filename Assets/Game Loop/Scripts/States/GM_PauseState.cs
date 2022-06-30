namespace PocketUniverse.GM_States
{
    public class GM_PauseState : GM_BaseState
    {
        public override void OnEnterState(GameManager controller, PhysicSimulation physSimScript)
        {
            // Update trajectories
            //physSimScript.UpdateTrajectoriesOneFrame();
            StartTrajectorySimulation(controller, physSimScript);
            
            // Enable trajectories
            physSimScript.EnableTrajectories(true);
        }

        public override void OnUpdateState(GameManager controller, PhysicSimulation physSimScript)
        {
            
        }

        public override void OnExitState(GameManager controller, PhysicSimulation physSimScript)
        {
            // Disable trajectories
            physSimScript.EnableTrajectories(false);
        }
        
        private void StartTrajectorySimulation(GameManager controller, PhysicSimulation physSimScript)
        {
            if (physSimScript.isUpdatingTrajectory)
            {
                controller.StopCoroutine(physSimScript.UpdateTrajectories());
            }

            controller.StartCoroutine(physSimScript.UpdateTrajectories());
        }
    }
}
