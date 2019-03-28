using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Cloth.MSS
{
    public class ClothSimulation
    {
        protected int m_Column=10;
        protected int m_Row=10;
        protected Vector2 m_GridSize=new Vector2(4f,4f);

        List<Particle> m_Particles;
        List<Spring> m_Springs;

        float m_StructStiffness = 50;
        float m_StructDamping =0.25f;

        float m_ShearStiffness = 50;
        float m_ShearDamping = 0.25f;

        float m_BledStiffness = 50;
        float m_BledDamping = 0.25f;

        float m_ParticleMass = 0.1f;

        PhysicsSystem m_System;

        public void Init()
        {
            SetupGrids();
            m_System = new PhysicsSystem();
            m_System.particles = m_Particles;
            m_System.springs = m_Springs;
        }

        public void SetupGrids()
        {
            m_Springs = new List<Spring>();

            //setup particles
            m_Particles = new List<Particle>();
            int u = m_Column + 1;
            int v = m_Row + 1;
            float particleMass = m_ParticleMass;
            for (int j = 0; j <= m_Column; ++j)
            {
                for(int i = 0; i <= m_Row; ++i)
                {
                    Particle particle = new Particle();
                    particle.position=new Vector3(((((float)i)/m_Column) *2 -1) *m_GridSize.x*0.5f, m_GridSize.x+1, (((float)j)/ m_Row)*m_GridSize.y);
                    particle.previousPosition = particle.position;
                    particle.mass = particleMass;// *(1+j*0.01f);
                    m_Particles.Add(particle);  
                }
            }

            m_Particles[0].isActive = false;
            m_Particles[m_Column].isActive = false;

            //setup springs
            //Struct Springs
            // Horizontal
            for (int j = 0; j < v; ++j)
            {
                for(int i = 0; i < m_Row; ++i)
                {
                    m_Springs.Add(CreateSpring(m_Particles[j * u + i], m_Particles[j * u + i + 1], m_StructStiffness, m_StructDamping,SpringType.Struct));
                }
            }
            // Vertical
            for (int i = 0; i < u; ++i)
            {
                for (int j = 0; j < m_Row; ++j)
                {
                    m_Springs.Add(CreateSpring(m_Particles[j * u + i], m_Particles[(j+1) * u + i], m_StructStiffness, m_StructDamping, SpringType.Struct));
                }
            }

            // Shearing Springs
            for (int j = 0; j < m_Row; ++j)
            {
                for (int i = 0; i < m_Column; ++i)
                {
                    m_Springs.Add(CreateSpring(m_Particles[j * u + i], m_Particles[(j + 1) * u + i+1], m_ShearStiffness, m_ShearDamping, SpringType.Shear));
                    m_Springs.Add(CreateSpring(m_Particles[(j+1) * u + i], m_Particles[j * u + i + 1], m_ShearStiffness, m_ShearDamping, SpringType.Shear));
                }
            }

            // Bend Springs
            for (int j = 0; j < v; ++j)
            {
                for (int i = 0; i < u - 2; ++i)
                {
                    m_Springs.Add(CreateSpring(m_Particles[j * u + i], m_Particles[j * u + i+2], m_BledStiffness, m_BledDamping, SpringType.Blend));
                }
                m_Springs.Add(CreateSpring(m_Particles[j * u + u-3], m_Particles[j * u + u-1], m_BledStiffness, m_BledDamping, SpringType.Blend));
            }

            for (int i = 0; i < u; ++i)
            {
                for (int j = 0; j< v - 2; ++j)
                {
                    m_Springs.Add(CreateSpring(m_Particles[j * u + i], m_Particles[(j+2) * u + i], m_BledStiffness, m_BledDamping, SpringType.Blend));
                }
                m_Springs.Add(CreateSpring(m_Particles[(v-3) * u + i], m_Particles[(v-1) * u + i], m_BledStiffness, m_BledDamping, SpringType.Blend));
            }

            Debug.LogFormat("Particles:{0},springs:{1}", m_Particles.Count,m_Springs.Count);
        }


        Spring CreateSpring(Particle a, Particle b, float ks, float kd,SpringType type)
        {
            Spring spring = new Spring();
            spring.particleA = a;
            spring.particleB = b;
            spring.stiffness = ks;
            spring.damping = kd;
            spring.restLength = (a.position - b.position).magnitude;
            spring.type = type;
            return spring;
        }

        public void Update()
        {
            if (m_System != null)
            {
                //m_System.Step();
                m_System.Update(Time.fixedDeltaTime * 4);
            }
        }
    }

   
}
