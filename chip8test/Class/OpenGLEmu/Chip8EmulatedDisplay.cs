using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8test.Class.OpenGLEmu
{
    public class Chip8EmulatedDisplay
    {
        private int width, height;
        private Square2fc[] squares;
        private float widthSeed;
        public IList<float> Vertices = new List<float>();
        private float heightSeed;
        int size;

        public Chip8EmulatedDisplay(int width, int height)
        {
            this.width = width;
            this.height = height;
            size = width * height;
            widthSeed = 1 / (float)width;
            heightSeed = 1 / (float)height;
            SeedVertices();
            
        }

        private void SeedVertices()
        {
            for(float i = 1 ; i > 0; i -= heightSeed)
            {
                for(float j = 0; j < 1; j += widthSeed)
                {
                    float x1 = j + widthSeed;
                    float y1 = i - heightSeed;

                    /************ 1st triangle **************/

                    Vertices.Add(j);
                    Vertices.Add(i);

                    Vertices.Add(j);
                    Vertices.Add(y1);

                    Vertices.Add(x1);
                    Vertices.Add(y1);

                    /************ 2nd triangle *************/
                    Vertices.Add(j);
                    Vertices.Add(i);

                    Vertices.Add(x1);
                    Vertices.Add(y1);

                    Vertices.Add(x1);
                    Vertices.Add(i);


                }
            }
        }

        public float[] getArray()
        {
            return Vertices.ToArray();
        }

    }
}
