using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Nebula_V1
{
    internal class RPC
    {

        public static uint g_entity = 0x16b9f20;
        public static uint G_SetMoel = 0x2774a4;
        public static uint entitySize = 0x31c;
        private static uint function_address = 0x7aa050;

        public static int Call(uint func_address, params object[] parameters)
        {
            int length = parameters.Length;
            int index = 0;
            uint num3 = 0;
            uint num4 = 0;
            uint num5 = 0;
            uint num6 = 0;
            while (index < length)
            {
                if (parameters[index] is int)
                {
                    Form1.PS32.Extension.WriteInt32(0x10020000 + (num3 * 4), (int)parameters[index]);

                    num3++;
                }
                else if (parameters[index] is uint)
                {
                    Form1.PS32.Extension.WriteUInt32(0x10020000 + (num3 * 4), (uint)parameters[index]);

                    num3++;
                }
                else
                {
                    uint num7;
                    if (parameters[index] is string)
                    {
                        num7 = 0x10022000 + (num4 * 0x400);
                        Form1.PS32.Extension.WriteString(num7, Convert.ToString(parameters[index]));

                        Form1.PS32.Extension.WriteUInt32(0x10020000 + (num3 * 4), num7);

                        num3++;
                        num4++;
                    }
                    else if (parameters[index] is float)
                    {
                        Form1.PS32.Extension.WriteFloat(0x10020024 + (num5 * 4), (float)parameters[index]);

                        num5++;
                    }
                    else if (parameters[index] is float[])
                    {
                        float[] input = (float[])parameters[index];
                        num7 = 0x10021000 + (num6 * 4);
                        Form1.WriteSingle(num7, input);
                        Form1.PS32.Extension.WriteUInt32(0x10020000 + (num3 * 4), num7);

                        num3++;
                        num6 += (uint)input.Length;
                    }
                }
                index++;
            }

            Form1.PS32.Extension.WriteUInt32(0x1002004c, func_address);
            Thread.Sleep(20);
            return Form1.PS32.Extension.ReadInt32(0x10020050);
        }

        public static int Init()
        {
            function_address = 0x7aa050;
            BISOON();
            return 0;
        }

        public static void iPrintln(int client, string txt)
        {
            SV_GameSendServerCommand(client, "O \"" + txt + "\"");
        }

        public static void iPrintlnBold(int client, string txt)
        {
            SV_GameSendServerCommand(client, "< \"" + txt + "\"");
        }

        public static void BISOON()
        {
            if (Form1.PS32.GetBytes(function_address, 4) == new byte[] { 0xf8, 0x21, 0xff, 0x91 })
            {
                MessageBox.Show("Already Enabled");
            }
            else
            {
                Form1.PS32.SetMemory(function_address, new byte[] { 0x4e, 0x80, 0, 0x20 });
                Thread.Sleep(20);
                byte[] buffer = new byte[] {
                0x7c, 8, 2, 0xa6, 0xf8, 1, 0, 0x80, 60, 0x60, 0x10, 2, 0x81, 0x83, 0, 0x4c,
                0x2c, 12, 0, 0, 0x41, 130, 0, 100, 0x80, 0x83, 0, 4, 0x80, 0xa3, 0, 8,
                0x80, 0xc3, 0, 12, 0x80, 0xe3, 0, 0x10, 0x81, 3, 0, 20, 0x81, 0x23, 0, 0x18,
                0x81, 0x43, 0, 0x1c, 0x81, 0x63, 0, 0x20, 0xc0, 0x23, 0, 0x24, 0xc0, 0x43, 0, 40,
                0xc0, 0x63, 0, 0x2c, 0xc0, 0x83, 0, 0x30, 0xc0, 0xa3, 0, 0x34, 0xc0, 0xc3, 0, 0x38,
                0xc0, 0xe3, 0, 60, 0xc1, 3, 0, 0x40, 0xc1, 0x23, 0, 0x48, 0x80, 0x63, 0, 0,
                0x7d, 0x89, 3, 0xa6, 0x4e, 0x80, 4, 0x21, 60, 0x80, 0x10, 2, 0x38, 160, 0, 0,
                0x90, 0xa4, 0, 0x4c, 0x90, 100, 0, 80, 0xe8, 1, 0, 0x80, 0x7c, 8, 3, 0xa6,
                0x38, 0x21, 0, 0x70, 0x4e, 0x80, 0, 0x20
             };
                Form1.PS32.SetMemory(function_address + 4, buffer);
                Form1.PS32.SetMemory(0x10020000, new byte[0x2854]);
                Form1.PS32.SetMemory(function_address, new byte[] { 0xf8, 0x21, 0xff, 0x91 });
            }
        }

        public static void SV_GameSendServerCommand(int client, string command)
        {
            Call(0x349f6c, new object[] { client, 0, command });
        }
        public static void G_SetModel(int Client, string Model)
        {
            RPC.Call(G_SetMoel, new object[] { g_entity + ((uint)(entitySize * Client)), Model });
        }
        public static void SvKick(int Client, string Reason)
        {
            RPC.SV_GameSendServerCommand(Client, "5 \"\n" + Reason + "\"");
        }
        public static void KickClient(int Client)
        {
            RPC.cbuf_addtext("clientKick " + Client);
        }
        public static void BlurClient(int Client, bool On)
        {
            if (On)
            {
                SV_GameSendServerCommand(Client, "d 100000000 20");

            }
            else
            {
                SV_GameSendServerCommand(Client, "d 0 0");

            }
        }
        public static void PS3Freeze(int Client)
        {
            iPrintlnBold(Client, "^1Warning: ^3Your PS3 Will be froze about 3 sec by using ^1BISOON's ^3 Tool");
            Thread.Sleep(2500);
            SV_GameSendServerCommand(Client, "^ 6 90 ");
        }
        public static void Player_Die(int Killer, int Victim, int meansOfDeath = 18, int iWeapon = 0)
        {
            UInt32 Attacker = G_Entity(Killer);
            UInt32 Inflictor = G_Entity(Victim);
            RPC.Call(0x001FD370, Inflictor, Attacker, Attacker, 0xFF, meansOfDeath, iWeapon, 0xD0300AD4C);
            Thread.Sleep(100);

        }
        public static UInt32 G_Entity(int entityIndex, UInt32 Mod = 0x00)
        {
            return (0x0016B9F20 + (UInt32)Mod) + ((UInt32)entityIndex * 0x31C);
        }
        public static void cbuf_addtext(string Command)
        {
            RPC.Call(0x313c18, 0, Command);
        }
        public static void Vision(int Client, string vision)
        {
            RPC.SV_GameSendServerCommand(Client, "2 1060 \"" + vision + "\"");
        }
        public static void Clone(int Client)
        {
            Form1.PS32.Extension.WriteBytes(0x1F63C4, new byte[] { 0x38, 0x60, 0x0, 0x0 });
            Form1.PS32.Extension.WriteInt16(0x1F63C4 + 2, (short)Client);
            RPC.Call(0x1F6388, Client);
        }
        public static void Fov(int fov)
        {
            SV_GameSendServerCommand(-1, "^ 5 " + fov);
        }
        public static void KillCamDuration(int Duration)
        {
            RPC.cbuf_addtext("set scr_killcam_time " + Duration);
        }
        public static void G_InitalizeAmmo(int Client)
        {
            RPC.Call(0x1E6698, 0x16B9F20 + 0x31C * (uint)Client);
        }
        public static void Add_Ammo(int Client, int Ammo)
        {
            RPC.Call(0x2089A8, 0x16B9F20 + 0x31C * (uint)Client, ReturnHeldWeapon(Client), Ammo, Ammo, Ammo, Ammo);
        }
        public static int ReturnHeldWeapon(int Client)
        {
            return Form1.PS32.Extension.ReadByte(0x17810E3 + 0x5808 * (uint)Client);
        }

        public static void playSound(Int32 clientIndex, String soundName)
        {
            Int32 soundIndex = RPC.Call(0x4f45bc, soundName);
            RPC.SV_GameSendServerCommand(clientIndex, "B " + soundIndex);
        }
        public static String char_to_wchar(String text)
        {
            String wchar = text;
            for (Int32 i = 0; i < text.Length; i++)
            {
                wchar = wchar.Insert(i * 2, "\0");
            }
            return wchar;
        }

        public static String doKeyboard(Int32 KeyboardType = 2, String Title = "Title", String PresetText = "", Int32 MaxLength = 20)
        {
            Form1.PS32.Extension.WriteByte(0x467507, (Byte)KeyboardType);
            RPC.Call(0x46710C, 0xD67E980, char_to_wchar(Title), char_to_wchar(PresetText), MaxLength, 0xD57560);
            while (Form1.PS32.Extension.ReadInt32(0xD82140) != 0)
                continue;
            return Form1.PS32.Extension.ReadString(0x03090E22);
        }
        public static int precacheShader(string Shader)
        {
            Form1.PS32.Extension.WriteInt32(0x1608100 + 0x40, 1);
            return RPC.Call(0x275e80, Shader);
        }
        public static void G_EntAttach(int entityIndex, string modelName, string tagName)
        {
            Int32 tag = RPC.Call(0x48E718, tagName);
            RPC.Call(0x27769c, G_Entity(entityIndex), modelName, tag, 0);
        }
        public static void AddEvent(Int32 clientIndex, Int32 Event, Object eventParams)
        {
            RPC.Call(0x2797b0, G_Entity(clientIndex), Event, eventParams);
        }
        public static void playRumble(Int32 clientIndex, Int32 RumbleIndex)
        {
            if (RumbleIndex != 0)
            { AddEvent(clientIndex, 0x70, RumbleIndex); }
        }
    }
}

