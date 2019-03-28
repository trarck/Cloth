using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth.MSS
{
    public enum SpringType
    {
        Struct,
        Shear,
        Blend
    }

    public class Spring
    {
        //A点
        protected Particle m_ParticleA;
        //B点
        protected Particle m_ParticleB;
        //弹性系数。Ks
        protected float m_Stiffness;
        //阻尼系数。Kd
        protected float m_Damping;
        //静止长度。
        protected float m_RestLength;

        public SpringType type;

        /// <summary>
        /// 参考http://www.cnblogs.com/shushen/p/5473264.html
        /// 这里使用A-B,参考使用B-A
        /// </summary>
        public void ApplyForce(bool debug=false)
        {
            //get the direction vector
            Vector3 direction = m_ParticleA.position - m_ParticleB.position;
            //check for zero vector
            if (direction != Vector3.zero)
            {
                //get length
                float currentLength = direction.magnitude;

                //normalize
                direction = direction.normalized;
                //spring force
                float springForce = -m_Stiffness * (currentLength - m_RestLength);
                //spring damping force
                Vector3 deltaVelocity = m_ParticleA.velocity - m_ParticleB.velocity;
                float dampingForce = -m_Damping * Vector3.Dot(deltaVelocity, direction);
                Vector3 force = (springForce + dampingForce) * direction;
                //if (debug)
                //{
                //    //Debug.Log("[force]:" + force.x + "," + force.y + "," + force.z
                //    //    + "[term]:" + springForce + "," + dampingForce
                //    //    + "[len]:" + currentLength + "," + m_RestLength
                //    //    + "[PosA]:" + m_ParticleA.position.x + "," + m_ParticleA.position.y + "," + m_ParticleA.position.z
                //    //    + "[PosB]:" + m_ParticleB.position.x + "," + m_ParticleB.position.y + "," + m_ParticleB.position.z
                //    //    + "[Va]:" + m_ParticleA.velocity.x + "," + m_ParticleA.velocity.y + "," + m_ParticleA.velocity.z
                //    //    + "[Vb]:" + m_ParticleB.velocity.x + "," + m_ParticleB.velocity.y + "," + m_ParticleB.velocity.z
                //    //);
                //}
                //apply the equal and opposite forces to the objects
                if (m_ParticleA.isActive)
                {
                    m_ParticleA.AddForce(force);
                }

                if (m_ParticleB.isActive)
                {
                    m_ParticleB.AddForce(-force);
                }
            }
        }

        public Particle particleA
        {
            get { return m_ParticleA; }
            set { m_ParticleA = value; }
        }

        public Particle particleB
        {
            get { return m_ParticleB; }
            set { m_ParticleB = value; }
        }

        public float stiffness
        {
            get { return m_Stiffness; }
            set { m_Stiffness = value; }
        }

        public float damping
        {
            get { return m_Damping; }
            set { m_Damping = value; }
        }

        public float restLength
        {
            get { return m_Stiffness; }
            set { m_RestLength = value; }
        }
    }
}