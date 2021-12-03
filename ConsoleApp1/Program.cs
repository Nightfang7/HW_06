using System;
using System.IO;
using System.Text;


namespace _HW06
{
    public static class Extensions
    {
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }
    }
    class Program
    {
        private static byte[] m_PacketData;
        //private static byte[] m_UnPacketData;

        private static uint m_Pos;
        static void Main(string[] args)
        {
            m_PacketData = new byte[1024];
            //m_UnPacketData = new byte[1024];
            m_Pos = 4;

            Write(109);
            Write(109.99f);
            Write("Helloooooo!");
            dataLength((int)(m_Pos - 4));
            Console.Write($"Output Byte array(length:{m_Pos-4}): ");
            for (var i = 0; i < m_Pos; i++)
            {
                Console.Write(m_PacketData[i] + ", ");
            }
            _Read(m_PacketData);
            Console.ReadLine();

        }
        private static bool dataLength(int i)
        {
            var bytes = BitConverter.GetBytes(i);
            AdddataLength(bytes);
            return true;
        }
        private static void AdddataLength(byte[] byteData)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }
            byteData.CopyTo(m_PacketData, 0);
        }
        // write an integer into a byte array
        private static bool Write(int i)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);

            _Write(bytes);
            return true;
        }

        // write a float into a byte array
        private static bool Write(float f)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(f);

            _Write(bytes);
            return true;
        }

        // write a string into a byte array
        private static bool Write(string s)
        {
            // convert string to byte array
            var bytes = Encoding.Unicode.GetBytes(s);

            // write byte array length to packet's byte array
            if (Write(bytes.Length) == false)
            {
                return false;
            }

            _Write(bytes);
            return true;
        }

        // write a byte array into packet's byte array
        private static void _Write(byte[] byteData)
        {
            // converter little-endian to network's big-endian
            //byteData.CopyTo(m_UnPacketData, m_Pos);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }
            byteData.CopyTo(m_PacketData, m_Pos);
            m_Pos += (uint)byteData.Length;
        }

        private static byte[] reverseData(byte[] bytes)
        {
            Array.Reverse(bytes);
            return bytes;
        }
        private static int ReadInt(byte[] bytes)
        {
            bytes = reverseData(bytes);
            var i = BitConverter.ToInt32(bytes, 0);
            return i;
        }
        private static float ReadFloat(byte[] bytes)
        {
            bytes = reverseData(bytes);
            var f = BitConverter.ToSingle(bytes, 0);
            return f;
        }
        private static string ReadString(byte[] bytes, int stringLen)
        {
            bytes = reverseData(bytes);
            var s = Encoding.Unicode.GetString(bytes, 0, stringLen);
            return s;
        }
        //read a packeet's byte to unpackage
        private static void _Read(byte[] byteData)
        {
            byte[] datalength = byteData.Slice(0, 4);
            int count = ReadInt(datalength);
            byte[] newByte = byteData.Slice(4, count+4);
            byte[] intData = newByte.Slice(0, 4);
            int intNum = ReadInt(intData);

            byte[] floatData = newByte.Slice(4, 8);
            float fNum = ReadFloat(floatData);
            byte[] stringLenData = newByte.Slice(8, 12);
            int stringLen = ReadInt(stringLenData);
            byte[] stData = newByte.Slice(12, count);
            string uniString = ReadString(stData, stringLen);

            Console.WriteLine("\n拆包Data: ");
            Console.WriteLine(intNum);
            Console.WriteLine(fNum);
            Console.WriteLine(uniString);

        }
    }
}
