using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace PocketUniverse
{
    public class PhysicSimulation
    {
        #region Fields

        private int computationIterations;
        
        private float gravitationalConstant;
        private float deltaTime;
        private float trajectorySimDeltaTime;
        private float oneFrameComputations;

        public bool isUpdatingTrajectory { get; private set; }
        private bool drawRelative = true;

        private List<CelestialBody> bodies;
        private Dictionary<CelestialBody, List<Vector3>> positions;
        private Dictionary<CelestialBody, List<Vector3>> velocities;
        private Dictionary<CelestialBody, LineRenderer> lineRenderers;

        private CelestialBody centralBody;

        #endregion

        public PhysicSimulation(int simIter, float gravitationalConstant, float deltaTime, float trajectorySimDt, int oneFrameComputations)
        {
            bodies = new List<CelestialBody>();
            positions = new Dictionary<CelestialBody, List<Vector3>>();
            velocities = new Dictionary<CelestialBody, List<Vector3>>();
            lineRenderers = new Dictionary<CelestialBody, LineRenderer>();

            computationIterations = simIter;
            this.gravitationalConstant = gravitationalConstant;
            this.oneFrameComputations = oneFrameComputations;
            SetDeltaTime(deltaTime);
            SetTrajectorySimDeltaTime(trajectorySimDt);
        }

        #region Lists Interface

        public void AddBody(CelestialBody body)
        {
            bodies.Add(body);
            
            positions.Add(body, new List<Vector3>(computationIterations));
            InitVectorList(positions[body]);
            
            velocities.Add(body, new List<Vector3>(computationIterations));
            InitVectorList(velocities[body]);

            LineRenderer lineRenderer = body.GetComponent<LineRenderer>();
            lineRenderer.positionCount = computationIterations;
            lineRenderers.Add(body, lineRenderer);
        }

        private void InitVectorList(List<Vector3> list)
        {
            for (int i = 0; i < list.Capacity; i++)
            {
                list.Add(Vector3.zero);
            }
        }

        #endregion

        #region Computations

        private Vector3 GetDirection(Vector3 firstBody, Vector3 secondBody)
        {
            return secondBody - firstBody;
        }
        
        private Vector3 ComputeForce(CelestialBody firstBody, CelestialBody secondBody, Vector3 directionTojBody)
        {
            float sqrDistance = directionTojBody.sqrMagnitude;
            directionTojBody.Normalize();

            return directionTojBody * (gravitationalConstant * firstBody.GetMass() *
                secondBody.GetMass() / sqrDistance);
        }

        private Vector3 ComputeFrameAcceleration(CelestialBody body, Vector3 force, float deltaTime)
        {
            return force * (deltaTime / body.GetMass());
        }

        #endregion

        #region Simulation

        public IEnumerator UpdateTrajectories()
        {
            isUpdatingTrajectory = true;
            
            foreach (var body in bodies)
            {
                velocities[body][0] = body.GetVelocity();
                velocities[body][1] = velocities[body][0];
                positions[body][0] = body.transform.position;
                positions[body][1] = positions[body][0];
                
                for (int i = 0; i < lineRenderers[body].positionCount; i++)
                {
                    lineRenderers[body].SetPosition(i, positions[body][0]);
                }
            }

            Vector3 refInitPosition = Vector3.zero;

            if (drawRelative && centralBody)
            {
                refInitPosition = positions[centralBody][0];
            }

            for (int simCounter = 1; simCounter < computationIterations; simCounter++)
            {
                for (int iBody = 0; iBody < bodies.Count; iBody++)
                {
                    for (int jBody = 1 + iBody; jBody < bodies.Count; jBody++)
                    {
                        // Compute this sim frame velocity
                        Vector3 directionTojBody = GetDirection(positions[bodies[iBody]][simCounter],
                            positions[bodies[jBody]][simCounter]);
                        Vector3 force = ComputeForce(bodies[iBody], bodies[jBody], directionTojBody);

                        velocities[bodies[iBody]][simCounter] += ComputeFrameAcceleration(bodies[iBody], force, trajectorySimDeltaTime);
                        velocities[bodies[jBody]][simCounter] += ComputeFrameAcceleration(bodies[jBody], -force, trajectorySimDeltaTime);
                        
                        // Set next sim frame velocity
                        velocities[bodies[iBody]][Mathf.Min(simCounter + 1, computationIterations - 1)] =
                            velocities[bodies[iBody]][simCounter];
                        velocities[bodies[jBody]][Mathf.Min(simCounter + 1, computationIterations - 1)] =
                            velocities[bodies[jBody]][simCounter];
                    }
                    
                    // Compute this frame position
                    positions[bodies[iBody]][simCounter] = positions[bodies[iBody]][simCounter - 1] +
                        velocities[bodies[iBody]][simCounter] * trajectorySimDeltaTime;

                    if (!drawRelative || !centralBody)
                    {
                        lineRenderers[bodies[iBody]].SetPosition(simCounter, positions[bodies[iBody]][simCounter]);
                    }

                    for (int i = simCounter + 1; i < lineRenderers[bodies[iBody]].positionCount; i++)
                    {
                        lineRenderers[bodies[iBody]].SetPosition(i, lineRenderers[bodies[iBody]].GetPosition(simCounter));
                    }

                    // Set next sim frame position
                    positions[bodies[iBody]][Mathf.Min(simCounter + 1, computationIterations - 1)] =
                        positions[bodies[iBody]][simCounter];
                }

                if (simCounter % oneFrameComputations == 0)
                {
                    yield return new WaitForFixedUpdate();
                }
            }

            // Update lineRenderer
            if (drawRelative && centralBody)
            {
                for (int simCounter = 0; simCounter < computationIterations; simCounter++)
                {
                    foreach (var body in bodies)
                    {
                        if (body == centralBody)
                        {
                            lineRenderers[body].SetPosition(simCounter, positions[body][simCounter] - positions[centralBody][simCounter]);
                            continue;
                        }
                        
                        lineRenderers[body].SetPosition(simCounter, positions[body][simCounter] - positions[centralBody][simCounter] + refInitPosition);
                    }
                }
            }

            isUpdatingTrajectory = false;
        }
        
        public void UpdateTrajectoriesOneFrame()
        {
            foreach (var body in bodies)
            {
                velocities[body][0] = body.GetVelocity();
                velocities[body][1] = velocities[body][0];
                positions[body][0] = body.transform.position;
                positions[body][1] = positions[body][0];
                lineRenderers[body].SetPosition(0, positions[body][0]);
            }

            for (int simCounter = 1; simCounter < computationIterations; simCounter++)
            {
                for (int iBody = 0; iBody < bodies.Count; iBody++)
                {
                    for (int jBody = 1 + iBody; jBody < bodies.Count; jBody++)
                    {
                        // Compute this sim frame velocity
                        Vector3 directionTojBody = GetDirection(positions[bodies[iBody]][simCounter],
                            positions[bodies[jBody]][simCounter]);
                        Vector3 force = ComputeForce(bodies[iBody], bodies[jBody], directionTojBody);

                        velocities[bodies[iBody]][simCounter] += ComputeFrameAcceleration(bodies[iBody], force, trajectorySimDeltaTime);
                        velocities[bodies[jBody]][simCounter] += ComputeFrameAcceleration(bodies[jBody], -force, trajectorySimDeltaTime);
                        
                        // Set next sim frame velocity
                        velocities[bodies[iBody]][Mathf.Min(simCounter + 1, computationIterations - 1)] =
                            velocities[bodies[iBody]][simCounter];
                        velocities[bodies[jBody]][Mathf.Min(simCounter + 1, computationIterations - 1)] =
                            velocities[bodies[jBody]][simCounter];
                    }
                    
                    // Compute this frame position
                    positions[bodies[iBody]][simCounter] = positions[bodies[iBody]][simCounter - 1] +
                        velocities[bodies[iBody]][simCounter] * trajectorySimDeltaTime;
                    
                    // Update lineRenderer
                    lineRenderers[bodies[iBody]].SetPosition(simCounter, positions[bodies[iBody]][simCounter]);
                    
                    // Set next sim frame position
                    positions[bodies[iBody]][Mathf.Min(simCounter + 1, computationIterations - 1)] =
                        positions[bodies[iBody]][simCounter];
                }
            }
        }

        public void RunTimeSimulation()
        {
            UpdateVelocities();
        }
        
        private void UpdateVelocities()
        {
            for (int iBody = 0; iBody < bodies.Count; iBody++)
            {
                for (int jBody = 1 + iBody; jBody < bodies.Count; jBody++)
                {
                    Vector3 directionTojBody = GetDirection(bodies[iBody].transform.position, bodies[jBody].transform.position);
                    Vector3 force = ComputeForce(bodies[iBody], bodies[jBody], directionTojBody);
                    
                    bodies[iBody].Accelerate(ComputeFrameAcceleration(bodies[iBody], force, deltaTime));
                    bodies[jBody].Accelerate(ComputeFrameAcceleration(bodies[jBody], -force, deltaTime));
                }

                UpdatePosition(bodies[iBody]);
            }
        }

        private void UpdatePosition(CelestialBody body)
        {
            body.transform.position += body.GetVelocity() * deltaTime;
        }

        #endregion

        #region Prints

        public void PrintBodies()
        {
            foreach (var body in bodies)
            {
                Debug.Log(body.name);
            }
        }

        public void PrintPositions()
        {
            Debug.Log("Positions");
            
            foreach (var pair in positions)
            {
                Debug.Log("==== Body: " + pair.Key.name);
                
                for (int i = 0; i < computationIterations; i++)
                {
                    Debug.Log(pair.Value[i]);
                }
            }
            
            Debug.Log("End of Positions\n");
        }
        
        public void PrintVelocities()
        {
            Debug.Log("Velocities");
            
            foreach (var pair in velocities)
            {
                Debug.Log("==== Body: " + pair.Key.name);
                
                for (int i = 0; i < computationIterations; i++)
                {
                    Debug.Log(pair.Value[i]);
                }
            }
            
            Debug.Log("End of Velocities\n");
        }

        #endregion

        #region Setters

        public void SetDeltaTime(float deltaTime)
        {
            this.deltaTime = deltaTime;
        }

        public void SetTrajectorySimDeltaTime(float simDt)
        {
            trajectorySimDeltaTime = simDt;
        }

        public void EnableTrajectories(bool newState)
        {
            foreach (var body in bodies)
            {
                lineRenderers[body].enabled = newState;
            }
        }

        public void SetCentralBody(CelestialBody centralBody)
        {
            this.centralBody = centralBody;
        }

        #endregion
    }
}
