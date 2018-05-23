
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chip8test.Class;

namespace chip8test
{
    public class Chip8
    {
        private byte[] Memory = new byte[4096];
        private ushort PC = 0x200;
        private ushort I = 0;
        public byte[] VRegs = new byte[16];
        public bool[] key = new bool[16];
        private ushort[] stack = new ushort[16];
        private int stackPointer = 0;
        private int delayTimer = 60;
        private int soundTimer = 60;
        private OpCode opcode;
        public bool[] display = new bool[32 * 64];
        public bool draw = false;

        Random random = new Random();

        public Chip8()
        {
            LoadFontsToMemory();
        }

        public void Reset()
        {
            PC = 0x200;
            I = 0;
            stackPointer = 0;
            display = new bool[32 * 64];
            VRegs = new byte[16];
            key = new bool[16];
            draw = false;
            delayTimer = 60;
            soundTimer = 60;
            for (int i = 0; i < 4096; ++i)
            {
                Memory[i] = 0;
            }
            LoadFontsToMemory();

        }

        public void Tick()
        {
            draw = false;
            opcode.Fetch(Memory[PC], Memory[PC + 1]);
            HandleOpcode(opcode);
            if (delayTimer > 0)
                --delayTimer;
            if (soundTimer > 0)
            {
                if (soundTimer == 1)
                    Console.Beep();
                --soundTimer;
            }
        }
        private void HandleOpcode(OpCode code)
        {
            switch (code.code & 0xF000)
            {
                case 0x0000:
                    switch (code.code & 0x000F)
                    {
                        case 0x0000:
                            ClearDisplay();
                            PC_I();
                            draw = true;
                            break;
                        case 0x000E:
                            PC = stack[stackPointer--];
                            PC_I();
                            break;
                        default:
                            PC_I();
                            break;
                    }
                    break;

                case 0x1000:
                    PC = (ushort)code.Addr();
                    break;

                case 0x2000:
                    stack[++stackPointer] = PC;
                    PC = code.Addr();
                    break;

                case 0x3000:
                    if (VRegs[code.X()] == code.KK())
                    {
                        PC_I();
                    }
                    PC_I();
                    break;

                case 0x4000:
                    if (VRegs[code.X()] != code.KK())
                    {
                        PC_I();
                    }
                    PC_I();
                    break;

                case 0x5000:
                    if (VRegs[code.Y()] == VRegs[code.X()])
                    {
                        PC_I();
                    }
                    PC_I();
                    break;

                case 0x6000:
                    VRegs[code.X()] = (byte)code.KK();
                    PC_I();
                    break;

                case 0x7000:
                    VRegs[code.X()] += (byte)code.KK();
                    PC_I();
                    break;

                case 0x8000:
                    switch (code.code & 0x000F)
                    {
                        case 0x0000:
                            VRegs[code.X()] = VRegs[code.Y()];
                            PC_I();
                            break;

                        case 0x0001:
                            VRegs[code.X()] |= VRegs[code.Y()];
                            PC_I();
                            break;

                        case 0x0002:
                            VRegs[code.X()] &= VRegs[code.Y()];
                            PC_I();
                            break;

                        case 0x0003:
                            VRegs[code.X()] ^= VRegs[code.Y()];
                            PC_I();
                            break;

                        case 0x0004:
                            VRegs[0xF] = (byte)( (VRegs[code.X()] + VRegs[code.Y()]) > 0xFF ? 1 : 0);
                            VRegs[code.X()] += VRegs[code.Y()];
                            PC_I();
                            break;

                        case 0x0005:
                            VRegs[0xF] = (byte)(VRegs[code.X()] > VRegs[code.Y()] ? 1 : 0);
                            VRegs[code.X()] -= VRegs[code.Y()];
                            PC_I();
                            break;

                        case 0x0006:
                            VRegs[0xf] = (byte)(VRegs[code.X()] & 0x1);
                            VRegs[code.X()] /= 2;
                            PC_I();
                            break;

                        case 0x0007:
                            VRegs[0xF] = (byte)(VRegs[code.Y()] > VRegs[code.X()] ? 1 : 0);
                            VRegs[code.X()] = (byte)(VRegs[code.Y()] - VRegs[code.X()]);
                            PC_I();
                            break;

                        case 0x000E:
                            VRegs[0xf] = (byte)((VRegs[code.X()] & 0x80) != 0 ? 1 : 0);
                            VRegs[code.X()] *= 2;
                            PC_I();
                            break;
                    }

                    break;

                case 0x9000:
                    if (VRegs[code.X()] != VRegs[code.Y()])
                    {
                        PC_I();
                    }
                    PC_I();
                    break;

                case 0xA000:
                    I = code.Addr();
                    PC_I();
                    break;

                case 0xB000:
                    PC = (ushort)(code.Addr() + VRegs[0]);
                    //PC_I();
                    break;

                case 0xC000:
                    VRegs[code.X()] = (byte)(random.Next(255) & code.KK());
                    PC_I();
                    break;

                case 0xD000:
                    VRegs[0xf] = 0;
                    ushort x = VRegs[code.X()];
                    ushort y = VRegs[code.Y()];
                    ushort nibl = code.N();
                    ushort xj, yi;
                    xj = yi = 0;
                    byte pixel;

                    for (int i = 0; i < nibl; ++i)
                    {
                        pixel = Memory[I + i];
                        for (int j = 0; j < 8; ++j)
                        {
                            if ((pixel & (0x80 >> j)) != 0)
                            {
                                //if(x+j >= 0 && x+j< 64 && y+i >= 0 && y+i < 32)
                                //{
                                    ushort pos = (ushort)(((x + j) + ((y + i) * 64)) % 2048);
                                    if (display[pos] == true)
                                    {
                                        VRegs[0xf] = 1;
                                    }
                                    display[pos] ^= true;
                                //}
                                
                                //VRegs[0xf] = display[pos] == true ? (byte)1 : (byte)0;
                                
                            }
                        }
                    }
                    draw = true;
                    PC_I();
                    break;

                case 0xE000:
                    switch (code.code & 0x00FF)
                    {
                        case 0x009E:
                            if (key[VRegs[code.X()]])
                            {
                                PC_I();
                            }
                            PC_I();
                            break;

                        case 0x00A1:
                            if (!key[VRegs[code.X()]])
                            {
                                PC_I();
                            }
                            PC_I();
                            break;
                    }
                    //PC_I();
                    break;

                case 0xF000:
                    switch (code.code & 0x00FF)
                    {
                        case 0x0007:
                            VRegs[code.X()] = (byte)delayTimer;
                            PC_I();
                            break;

                        case 0x000A:
                            for (int i = 0x0; i < 0xf; ++i ) {
                                if(key[i])
                                {
                                    VRegs[code.X()] = (byte)i;
                                    PC_I();
                                    break;
                                }
                            }

                            break;

                        case 0x0015:
                            delayTimer = VRegs[code.X()];
                            PC_I();
                            break;

                        case 0x0018:
                            soundTimer = VRegs[code.X()];
                            PC_I();
                            break;

                        case 0x001E:
                            I += VRegs[code.X()];
                            PC_I();
                            break;

                        case 0x0029:
                            I = (ushort)(VRegs[code.X()] * 5);
                            PC_I();
                            break;

                        case 0x0033:
                            var value = VRegs[code.X()];
                            Memory[I + 2] = (byte)((value % 100) % 10);
                            Memory[I + 1] = (byte)((value / 10) % 10);
                            Memory[I] = (byte)(value / 100);
                            PC_I();
                            break;

                        case 0x0055:
                            for (int i = 0; i < code.X(); ++i)
                            {
                                Memory[I + i] = VRegs[i];
                            }
                            I += (ushort)(code.X() + 1);
                            PC_I();
                            break;

                        case 0x0065:
                            for (int i = 0; i < code.X(); ++i)
                            {
                                VRegs[i] = Memory[I + i];
                            }
                            I += (ushort)(code.X() + 1);
                            PC_I();
                            break;
                    }
                    break;

                default:
                    Console.WriteLine($"{code.code} not implemented yet.");
                    break;
            }
        }

        private void ClearDisplay()
        {
            for (int i = 0; i < 64*32; ++i)
            {
                display[i] = false;
            }
            draw = false;
        }

        private void LoadFontsToMemory()
        {
            var fontSize = Fonts.fonts.Count();
            for (int i = 0; i < fontSize; ++i)
            {
                Memory[i] = Fonts.fonts[i];
            }
        }
        public void LoadFile(byte[] file)
        {
            foreach (var info in file)
            {
                Memory[PC++] = info;
            }
            PC = 0x200;
        }
        private void PC_I()
        {
            PC += 2;
        }

    }
}
