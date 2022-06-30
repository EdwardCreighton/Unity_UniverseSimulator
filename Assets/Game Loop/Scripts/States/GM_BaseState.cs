namespace PocketUniverse.GM_States
{
    public abstract class GM_BaseState
    {
        public abstract void OnEnterState(GameManager controller, PhysicSimulation physSimScript);
        public abstract void OnUpdateState(GameManager controller, PhysicSimulation physSimScript);
        public abstract void OnExitState(GameManager controller, PhysicSimulation physSimScript);
    }
}
