using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth.Verlet
{
    public class VerletSystem
    {
        protected List<Particle> m_Particles = new List<Particle>();
        protected List<Spring> m_Springs = new List<Spring>();

        protected Vector3 m_GravityAcceleration=new Vector3(0, -0.0981f, 0);
        protected float m_AirDamping= 0.125f;

        float m_StepInterval=0.02f;
        float m_DoubleStepInterval= 0.0004f;
        float m_Elapsed = 0;

        float m_Factor = 1.0f;

        public long stepElapsed = 0;
        public int stepFrame = 0;

        public void Update(float dt)
        {
            m_Elapsed += dt;
            while (m_Elapsed >= m_StepInterval)
            {
                m_Elapsed -= m_StepInterval;
                Step();
            }
        }

        public void Step()
        {
            ++stepFrame;
            long start = System.DateTime.Now.Ticks;
            ComputeForces();
            //Integrates();
            IntegratesEuler();
            stepElapsed += System.DateTime.Now.Ticks - start;
            //Collides();
        }

        protected void ComputeForces()
        {
            //init gravity and air damping
            Particle particle = null;
            for (int i = 0, l = m_Particles.Count; i < l; ++i)
            {
                particle = m_Particles[i];
                particle.ResetResultantForce();
                if (particle.isActive)
                {
                    particle.AddForce(m_GravityAcceleration * particle.mass);
                    particle.AddForce(-m_AirDamping * particle.velocity);
                }
            }

            //calc spring force
            for (int i = 0, l = m_Springs.Count; i < l; ++i)
            {
                m_Springs[i].ApplyForce(i==0);
            }
        }

        protected void Integrates()
        {
            Vector3 lastPosition;
            Particle particle = null;
            
            for (int i = 0, l = m_Particles.Count; i < l; ++i)
            {
                particle = m_Particles[i];
                lastPosition = particle.position;
                //Debug.Log(particle.resultantForce);
                float doubleDeltaMass = m_DoubleStepInterval / particle.mass;
                particle.position = particle.position * 2 - particle.previousPosition + doubleDeltaMass * particle.resultantForce;
                if (particle.position.y < 0)
                {
                    var p = particle.position;
                    p.y = 0;
                    particle.position=p;
                }
                particle.previousPosition = lastPosition;
                particle.velocity = (particle.position - particle.previousPosition) / m_StepInterval;
            }
        }

        protected void IntegratesEuler()
        {
            
            Particle particle = null;
            
            for (int i = 0, l = m_Particles.Count; i < l; ++i)
            {

                particle = m_Particles[i];
                var oldV = particle.velocity;
                if (i == 1)
                {
                    //Debug.Log("ddd");
                }
                float deltaTimeMass = m_StepInterval / particle.mass;
                particle.velocity = particle.velocity+ particle.resultantForce * deltaTimeMass;

                particle.position = particle.position+ particle.velocity * m_StepInterval;

                //particle.velocity *= m_StepInterval;
            }
        }

        protected void Collides()
        {
            Particle particle = null;
            for (int i = 0, l = m_Particles.Count; i < l; ++i)
            {
                particle = m_Particles[i];
            }
        }

        public List<Particle> particles
        {
            get
            {
                return m_Particles;
            }
            set
            {
                m_Particles = value;
            }
        }

        public List<Spring> springs
        {
            get
            {
                return m_Springs;
            }
            set
            {
                m_Springs = value;
            }
        }

        public float stepInterval
        {
            get {
                return m_StepInterval;
            }
            set
            {
                m_StepInterval = value;
                m_DoubleStepInterval = m_StepInterval * m_StepInterval;
            }
        }
    }
}
