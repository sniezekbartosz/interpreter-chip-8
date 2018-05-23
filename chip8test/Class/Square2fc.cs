using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8test.Class
{
    public class Square2fc
    {
        private Vert2fc[] leftTriangles;
        private Vert2fc[] rightTriangles;
        public Square2fc()
        {
            leftTriangles = new Vert2fc[3];
            rightTriangles = new Vert2fc[3];
        }

        public void SetColor(float r, float g, float b)
        {
            for(int i = 0; i< 3; ++i)
            {
                leftTriangles[i].SetColor(r, g, b);
                rightTriangles[i].SetColor(r, g, b);
            }
            
        }
        public void setLefts(Vert2fc ul, Vert2fc dl, Vert2fc dr)
        {
            leftTriangles[0] = ul;
            leftTriangles[1] = dl;
            leftTriangles[2] = dr;
        }
        public void setRights(Vert2fc ul, Vert2fc dr, Vert2fc ur)
        {
            rightTriangles[0] = ul;
            rightTriangles[1] = dr;
            rightTriangles[2] = ur;
        }
        public List<float> GetFloats()
        {
            List<float> floats = new List<float>();
            for(int triangleIndex = 0; triangleIndex < leftTriangles.Length; ++triangleIndex)
            {
                for(int floatIndex = 0; floatIndex < 2; ++floatIndex)
                {
                    floats.Add(leftTriangles[triangleIndex].vertices[floatIndex]);
                }
                for (int floatIndex = 0; floatIndex < 2; ++floatIndex)
                {
                    floats.Add(rightTriangles[triangleIndex].vertices[floatIndex]);
                }
            }
            return floats;
        }
    }
}
