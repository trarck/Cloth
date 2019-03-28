using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloth.MSS
{
    public interface IExternalForceGenerator
    {
        void Apply(Particle particle);
    }
}
