using UnityEngine;

namespace Cloth.MSS
{
    public class AirDrag:IExternalForceGenerator
    {
        protected float m_Coefficient = 0.125f;

        public AirDrag()
        {

        }

        public AirDrag(float coefficient)
        {
            m_Coefficient = coefficient;
        }

        public void Apply(Particle particle)
        {
            particle.AddForce(-m_Coefficient * particle.velocity*particle.area);
        }
    }
}
