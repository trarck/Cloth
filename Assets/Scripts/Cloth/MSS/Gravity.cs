using UnityEngine;

namespace Cloth.MSS
{
    public class Gravity:IExternalForceGenerator
    {
        protected Vector3 m_GravityAcceleration = new Vector3(0, -0.981f, 0);

        public Gravity()
        {

        }

        public Gravity(Vector3 gravityAcceleration)
        {
            m_GravityAcceleration = gravityAcceleration;
        }

        public void Apply(Particle particle)
        {
            particle.AddForce(m_GravityAcceleration * particle.mass);
        }
    }
}
