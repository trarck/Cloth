using System;
using System.Collections.Generic;
using UnityEngine;
using Cloth.Verlet;
using UnityEditor;

namespace Cloth
{
    public class ClothSimulation:MonoBehaviour
    {
        protected int m_Column=20;
        protected int m_Row=20;
        protected Vector2 m_GridSize=new Vector2(4f,4f);

        List<Particle> m_Particles;
        List<Spring> m_Springs;

        float m_StructStiffness = 50.75f;
        float m_StructDamping = 0.25f;

        float m_ShearStiffness = 50.75f;
        float m_ShearDamping = 0.25f;

        float m_BledStiffness = 50.95f;
        float m_BledDamping = 0.25f;

        float m_Mass = 10f;

        VerletSystem m_System;

        [SerializeField]
        GameObject m_PointPrefab;
        List<Transform> m_Points=new List<Transform>();

        long startTime = 0;
        int frame;

        public void Init()
        {
            SetupGrids();
            SetUpPoints();

            m_System = new VerletSystem();
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
            float particleMass = 1f;// m_Mass/(m_Column*m_Row);
            for (int j = 0; j <= m_Column; ++j)
            {
                for(int i = 0; i <= m_Row; ++i)
                {
                    Particle particle = new Particle();
                    particle.position=new Vector3(((((float)i)/m_Column) *2 -1) *m_GridSize.x*0.5f, m_GridSize.x+1, (((float)j)/ m_Row)*m_GridSize.y);
                    particle.previousPosition = particle.position;
                    particle.mass = particleMass*(j+1);
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

        private void OnGUI()
        {
            if (GUI.Button(new Rect(20, 200, 100, 40), "Step"))
            {
                if (m_System != null)
                {
                    m_System.Step();
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (m_Springs == null) return;

            foreach(Spring spring in m_Springs)
            {
                Vector3 middle=Vector3.zero;

                switch (spring.type)
                {
                    case SpringType.Struct:
                        Gizmos.color = Color.red;
                        middle = (spring.particleA.position + spring.particleB.position) * 0.5f+new Vector3(0,0.2f,0);
                        break;
                    case SpringType.Shear:
                        Gizmos.color = Color.green;
                        middle = (spring.particleA.position + spring.particleB.position) * 0.5f + new Vector3(0, 0.4f, 0);
                        break;
                    case SpringType.Blend:
                        Gizmos.color = Color.blue;
                        middle = (spring.particleA.position + spring.particleB.position) * 0.5f + new Vector3(0, 0.8f, 0);
                        break;
                }

                //Gizmos.DrawLine(spring.particleA.position, middle);
                //Gizmos.DrawLine(middle, spring.particleB.position);
                Gizmos.DrawLine(spring.particleA.position, spring.particleB.position);
            }
        }

        void SetUpPoints()
        {
            m_Points.Clear();
            for (int i = 0, l = m_Particles.Count; i < l; ++i)
            {
                GameObject pointObj = GameObject.Instantiate(m_PointPrefab, transform);
                Transform pointTransform = pointObj.transform;
                pointTransform.localPosition = m_Particles[i].position;
                m_Points.Add(pointTransform);
            }
            m_Points[0].GetChild(0).localScale = new Vector3(0.1f, 0.1f, 0.1f);
            m_Points[m_Column].GetChild(0).localScale = new Vector3(0.1f, 0.1f, 0.1f);
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

        private void Start()
        {
            Init();
            startTime= System.DateTime.Now.Ticks;

        }

        private void Update()
        {
            for (int i = 0, l = m_Particles.Count; i < l; ++i)
            {
                m_Points[i].localPosition = m_Particles[i].position;
            }

            long current= System.DateTime.Now.Ticks;
            if (current - startTime > 10000000)
            {
                startTime = current;
                Debug.LogFormat("{0},{1},{2}",m_System.stepElapsed/10, frame,m_System.stepFrame);
                m_System.stepElapsed = 0;
                m_System.stepFrame = 0;
                frame = 0;
            }
        }

        private void FixedUpdate()
        {
            //Debug.Log(Time.fixedDeltaTime);
            ++frame;
            if (m_System != null)
            {
                //m_System.Step();
                m_System.Update(Time.fixedDeltaTime*20);
            }
        }
    }

   
}
