
using UnityEngine;

namespace Cloth.MSS
{
    public class VerletIntegrator:IIntegrator
    {
        float m_StepInterval=0.01f;
        float m_DoubleStepInterval=0.0001f;

        public VerletIntegrator(float stepInterval)
        {
            m_StepInterval = stepInterval;
            m_DoubleStepInterval = stepInterval * stepInterval;
        }

        public void Integrate(PhysicsSystem physicsSystem)
        {
            Vector3 lastPosition;
            Particle particle = null;

            for (int i = 0, l = physicsSystem.particles.Count; i < l; ++i)
            {
                particle = physicsSystem.particles[i];
                lastPosition = particle.position;
                float doubleDeltaMass = m_DoubleStepInterval / particle.mass;
                particle.position = particle.position * 2 - particle.previousPosition + doubleDeltaMass * particle.resultantForce;
                if (particle.position.y < 0)
                {
                    var p = particle.position;
                    p.y = 0;
                    particle.position = p;
                }
                particle.previousPosition = lastPosition;
                particle.velocity = (particle.position - particle.previousPosition) / m_StepInterval;
            }
        }
    }
}
