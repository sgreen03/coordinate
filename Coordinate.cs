using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepositionMaterial
{
    class Coordinate
    {
        float X, Y, Z;

        public float getX()
        {
            return this.X;
        }


        public float getY()
        {
            return this.Y;
        }

        
        public float getZ()
        {
            return this.Z;
        }

        public void setX(float X)
        {
            this.X = X;
        }

        public void setY(float Y)
        {
            this.Y = Y;
        }

        public void setZ(float Z)
        {
            this.Z = Z;
        }

        public String printCoordinate()
        {
            String line;

            line = "X" + this.X.ToString("N4") + ",Y" + this.Y.ToString("N4") + ",Z" + this.Z.ToString("N4");

            return line;
        }


        
    }
}
