using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8test.Class
{
    public struct OpCode
    {
        public ushort code;
        public void Fetch(byte left, byte right)
        {
            code = (ushort)(left << 8 | right);
        }
        public ushort X()
        {
            return (ushort)((code & 0x0F00) >> 8);
        }
        public ushort Y()
        {
            return (ushort)((code & 0x00F0) >> 4);
        }
        public ushort N()
        {
            return (ushort)(code & 0x000F);
        }
        public ushort KK()
        {
            return (ushort)(code & 0x00FF);
        }
        public ushort Addr()
        {
            return (ushort)(code & 0x0FFF);
        }
    }
}
