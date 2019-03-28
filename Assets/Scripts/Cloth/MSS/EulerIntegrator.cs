
using UnityEngine;

namespace Cloth.MSS
{
    public class EulerIntegrator : IIntegrator
    {
        float m_StepInterval=0.01f;
        float m_DoubleStepInterval=0.0001f;

        public EulerIntegrator(float stepInterval)
        {
            m_StepInterval = stepInterval;
            m_DoubleStepInterval = stepInterval * stepInterval;
        }

        public void Integrate(PhysicsSystem physicsSystem)
        {
            Particle particle = null;
            for (int i = 0, l = physicsSystem.particles.Count; i < l; ++i)
            {
                particle = physicsSystem.particles[i];
                float deltaTimeMass = m_StepInterval / particle.mass;
                particle.velocity = particle.velocity + particle.resultantForce * deltaTimeMass;
                //原算法使用旧的速度。但实际测试会出问题。改为当前速度。
                particle.position = particle.position + particle.velocity * m_StepInterval;
            }
        }
    }
}
