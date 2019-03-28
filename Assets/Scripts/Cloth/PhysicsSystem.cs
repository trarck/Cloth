using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth.Verlet
{
    /// <summary>
    /// 重力和弹簧系数要在一个量级上。
    /// 为了提升性能，在总的循环次数不变的性况下，要提高重力和弹簧系统的值，来达到模拟的效果。
    /// 比如，实际中，重力加速度为9.8的，经过1秒后，质点向下的速度为9.8。
    ///       当步长为0.01的时候，1秒钟要模拟10000次，才能和实际相符。
    /// K是弹簧系统,m质量，g重力加速度,dt时间间隔，times执行次数。
    /// 由于K是弹簧系统基本不会调整量级。如果要减少模拟次数，就要增加重力加速度。
    /// 但是重力加速度增加时，步长为0.01又太大了。所以整个模拟次数不能无限减少。
    ///     K       m       g       dt        times
    ///     50      1     0.0098    0.01       10000      
    ///     50     0.1     0.98    0.01       1000
    /// 
    /// </summary>
    public class PhysicsSystem
    {
        //所有质点
        protected List<Particle> m_Particles = new List<Particle>();
        //所有弹簧
        protected List<Spring> m_Springs = new List<Spring>();
        //重力加速度
        protected Vector3 m_GravityAcceleration=new Vector3(0, -0.981f, 0);
        //空气阻力
        protected float m_AirDamping = 0.125f;
        //模拟的步长
        float m_StepInterval=0.01f;
        //步长的平方
        float m_DoubleStepInterval= 0.0001f;
        //花费的时间
        float m_Elapsed = 0;

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
            ComputeForces();
            //IntegrateVerlet();
            IntegrateEuler();
            //Collides();
        }

        /// <summary>
        /// 计算质点的受力
        /// </summary>
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
                    particle.AddForce(-m_AirDamping * particle.velocity * particle.mass);
                }
            }

            //calc spring force
            for (int i = 0, l = m_Springs.Count; i < l; ++i)
            {
                m_Springs[i].ApplyForce(i==0);
            }
        }

        /// <summary>
        /// 使用verlet计算质点的位置
        /// </summary>
        protected void IntegrateVerlet()
        {
            Vector3 lastPosition;
            Particle particle = null;
            
            for (int i = 0, l = m_Particles.Count; i < l; ++i)
            {
                particle = m_Particles[i];
                lastPosition = particle.position;
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

        /// <summary>
        /// 使用欧拉计算质点位置。
        /// 这里做了改进可以支持大步长。
        /// </summary>
        protected void IntegrateEuler()
        {
            Particle particle = null;
            for (int i = 0, l = m_Particles.Count; i < l; ++i)
            {
                particle = m_Particles[i];
                float deltaTimeMass = m_StepInterval / particle.mass;
                particle.velocity = particle.velocity+ particle.resultantForce * deltaTimeMass;
                //原算法使用旧的速度。但实际测试会出问题。改为当前速度。
                particle.position = particle.position+ particle.velocity * m_StepInterval;
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
