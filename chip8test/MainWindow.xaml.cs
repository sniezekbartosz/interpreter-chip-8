using chip8test.Class;
using chip8test.Class.OpenGLEmu;
using Microsoft.Win32;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chip8test
{
    public delegate void EmulateCycle();
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Chip8 chip;
        private int code;
        private Chip8EmulatedDisplay disp;
        private float[] _ArrayPosition;
        private float[] _ArrayColor;
        private bool[] dp = new bool[32 * 64];
        Random rand = new Random();
        private int gameWidth, gameHeight;
        EmulateCycle cycle;


        public MainWindow()
        {
            InitializeComponent();
            this.MinWidth = 70;
            this.MinHeight = 80;
            chip = new Chip8();
            gameHeight = 32;
            gameWidth = 64;
            disp = new Chip8EmulatedDisplay(64, 32);
            _ArrayPosition = disp.getArray();
            //_ArrayPosition = new float[]{ 0.0f, 0.0f, 0.1f, 0.1f, 0.0f, 0.1f };
            _ArrayColor = new float[32*64*18];
            cycle = new EmulateCycle(Nop);
            var myTextBlock = (System.Windows.Controls.Label)this.FindName("opcodeLabel");
            myTextBlock.Content = _ArrayPosition.Length;
        }

        private void Nop()
        {

        }

        private void HostControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void GlControl_ContextCreated(object sender, OpenGL.GlControlEventArgs e)
        {

            Gl.MatrixMode(MatrixMode.Projection);
            Gl.LoadIdentity();
            Gl.Ortho(0.0, 1.0f, 0.0, 1.0, 0.0, 1.0);

            Gl.MatrixMode(MatrixMode.Modelview);
            Gl.LoadIdentity();
        }

        private void GlControl_Render(object sender, OpenGL.GlControlEventArgs e)
        {
            var senderControl = sender as GlControl;
            senderControl.Animation = true;
            senderControl.AnimationTime = 1;

            int vpx = 0;
            int vpy = 0;
            int vpw = senderControl.ClientSize.Width;
            int vph = senderControl.ClientSize.Height;
            Gl.Viewport(vpx, vpy, vpw, vph);
            Gl.ClearColor(0.39f, 0.58f, 0.92f, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            cycle();
            if (chip.draw)
            {
                Draw(chip.display);
            }

            if (Gl.CurrentVersion >= Gl.Version_110)
            {
                // Old school OpenGL 1.1
                // Setup & enable client states to specify vertex arrays, and use Gl.DrawArrays instead of Gl.Begin/End paradigm
                using (MemoryLock vertexArrayLock = new MemoryLock(_ArrayPosition))
                using (MemoryLock vertexColorLock = new MemoryLock(_ArrayColor))
                {
                    // Note: the use of MemoryLock objects is necessary to pin vertex arrays since they can be reallocated by GC
                    // at any time between the Gl.VertexPointer execution and the Gl.DrawArrays execution

                    Gl.VertexPointer(2, VertexPointerType.Float, 0, vertexArrayLock.Address);
                    Gl.EnableClientState(EnableCap.VertexArray);

                    Gl.ColorPointer(3, ColorPointerType.Float, 0, vertexColorLock.Address);
                    Gl.EnableClientState(EnableCap.ColorArray);

                    Gl.DrawArrays(PrimitiveType.Triangles, 0, _ArrayPosition.Length / 2);

                }
            }


        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            //chip.Reset();
            byte[] fileArray;
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() != null)
            {
                string fileName = dialog.FileName;
                fileArray = File.ReadAllBytes(fileName);
                chip.Reset();
                Draw(new bool[64*32]);
                chip.LoadFile(fileArray);
                cycle -= Nop;
                cycle = new EmulateCycle(chip.Tick);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Set()
        {
            throw new NotImplementedException();
        }

        public void Debug(int code)
        {
            this.Dispatcher.Invoke(() =>
            {
                System.Windows.Controls.Label op = (System.Windows.Controls.Label)this.FindName("opcodeLabel");
                op.Content = "opcode: " + code.ToString("X4");

            });
        }

        private void GlControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            System.Windows.Controls.Label op = (System.Windows.Controls.Label)this.FindName("stateLabel");
            op.Content = e.KeyCode.ToString();
            if (e.KeyCode == Keys.D1)
            {
                chip.key[0x1] = true;
            }

            if (e.KeyCode == Keys.D2)
            {
                chip.key[0x2] = true;
            }

            if (e.KeyCode == Keys.D3)
            {
                chip.key[0x3] = true;
            }

            if (e.KeyCode == Keys.D4)
            {
                chip.key[0xc] = true;
            }

            if (e.KeyCode == Keys.Q)
            {
                chip.key[0x4] = true;
            }

            if (e.KeyCode == Keys.W)
            {
                chip.key[0x5] = true;
            }

            if (e.KeyCode == Keys.E)
            {
                chip.key[0x6] = true;
            }

            if (e.KeyCode == Keys.R)
            {
                chip.key[0xd] = true;
            }

            if (e.KeyCode == Keys.A)
            {
                chip.key[0x7] = true;
            }

            if (e.KeyCode == Keys.S)
            {
                chip.key[0x8] = true;
            }

            if (e.KeyCode == Keys.D)
            {
                chip.key[0x9] = true;
            }

            if (e.KeyCode == Keys.F)
            {
                chip.key[0xe] = true;
            }

            if (e.KeyCode == Keys.Z)
            {
                chip.key[0xa] = true;
            }

            if (e.KeyCode == Keys.X)
            {
                chip.key[0x0] = true;
            }

            if (e.KeyCode == Keys.C)
            {
                chip.key[0xb] = true;
            }

            if (e.KeyCode == Keys.V)
            {
                chip.key[0xf] = true;
            }
        }
        private void GlControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1)
            {
                chip.key[0x1] = false;
            }

            if (e.KeyCode == Keys.D2)
            {
                chip.key[0x2] = false;
            }

            if (e.KeyCode == Keys.D3)
            {
                chip.key[0x3] = false;
            }

            if (e.KeyCode == Keys.D4)
            {
                chip.key[0xc] = false;
            }

            if (e.KeyCode == Keys.Q)
            {
                chip.key[0x4] = false;
            }

            if (e.KeyCode == Keys.W)
            {
                chip.key[0x5] = false;
            }

            if (e.KeyCode == Keys.E)
            {
                chip.key[0x6] = false;
            }

            if (e.KeyCode == Keys.R)
            {
                chip.key[0xd] = false;
            }

            if (e.KeyCode == Keys.A)
            {
                chip.key[0x7] = false;
            }

            if (e.KeyCode == Keys.S)
            {
                chip.key[0x8] = false;
            }

            if (e.KeyCode == Keys.D)
            {
                chip.key[0x9] = false;
            }

            if (e.KeyCode == Keys.F)
            {
                chip.key[0xe] = false;
            }

            if (e.KeyCode == Keys.Z)
            {
                chip.key[0xa] = false;
            }

            if (e.KeyCode == Keys.X)
            {
                chip.key[0x0] = false;
            }

            if (e.KeyCode == Keys.C)
            {
                chip.key[0xb] = false;
            }

            if (e.KeyCode == Keys.V)
            {
                chip.key[0xf] = false;
            }
        }

        public void Draw(bool[] gfx)
        {
            int size = 32 * 64;
            for (int i = 0; i < 32; ++i)
            {
                for (int j = 0; j < 64; ++j)
                {
                    var index = 18 * ((i * 64) + j);
                    if (gfx[(i*64) + j])
                    {
                        for(int z = 0; z < 18; ++z)
                        {
                            _ArrayColor[index + z] = 1.0f;
                        }
                    }
                    else
                    {
                        for (int z = 0; z < 18; ++z)
                        {
                            

                            if ((z + 1) % 3 == 1)
                            {
                                _ArrayColor[index + z] = 0.047f;
                            }
                            else if ((z + 1) % 3 == 2)
                            {
                                _ArrayColor[index + z] = 0.051f;
                            }
                            else
                            {
                                _ArrayColor[index + z] = 0.192f;
                            }
                        }
                    }
                }
                    

            }
        }
    }
}
