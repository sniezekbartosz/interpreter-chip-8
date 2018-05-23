using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8test.Class
{
    public class Vert2fc
    {
        public float[] vertices = new float[2];
        public float[] colors = new float[3];
        public Vert2fc(float x, float y)
        {
            vertices[0] = x;
            vertices[1] = y;
        }
        public void SetColor(float r, float g, float b)
        {
            colors[0] = r;
            colors[1] = g;
            colors[2] = b;
        }
    }
}
