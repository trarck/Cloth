using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth
{
    public class Particle
    {
        //质量
        protected float m_Mass=1.0f;
        //位置
        protected Vector3 m_Position=Vector3.zero;
        //上个位置
        protected Vector3 m_PreviousPosition = Vector3.zero;
        //速度
        protected Vector3 m_Velocity = Vector3.zero;
        //合力
        protected Vector3 m_ResultantForce = Vector3.zero;

        protected bool m_Active=true;

        public Particle()
        {

        }

        public Particle(float mass)
        {
            m_Mass = mass;
            m_Active = true;
        }

        public Particle(float mass,bool active)
        {
            m_Mass = mass;
            m_Active = active;
        }

        public void AddForce(Vector3 force)
        {
            m_ResultantForce += force;
        }

        public void ResetResultantForce()
        {
            m_ResultantForce = Vector3.zero;
        }

        public float mass
        {
            get { return m_Mass; }
            set { m_Mass = value; }
        }

        public Vector3 position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public Vector3 previousPosition
        {
            get { return m_PreviousPosition; }
            set { m_PreviousPosition = value; }
        }

        public Vector3 velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }

        public Vector3 resultantForce
        {
            get { return m_ResultantForce; }
            set { m_ResultantForce = value; }
        }

        public bool isActive
        {
            get { return m_Active; }
            set { m_Active = value; }
        }
    }
}