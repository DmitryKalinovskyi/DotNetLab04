using System;
using System.ComponentModel;

namespace ChatLibrary.Helpers
{
    public class IdHelper
    {
        public static Guid MungeTwoGuids(Guid guid1, Guid guid2)
        {
            const int BYTECOUNT = 16;
            byte[] destByte = new byte[BYTECOUNT];
            byte[] guid1Byte = guid1.ToByteArray();
            byte[] guid2Byte = guid2.ToByteArray();

            for (int i = 0; i < BYTECOUNT; i++)
            {
                destByte[i] = (byte)(guid1Byte[i] ^ guid2Byte[i]);
            }
            return new Guid(destByte);
        }

        public static Guid MungeTwoInt(int id1, int id2)
        {
            byte[] bytes = new byte[16];

            if(id1 > id2)
            {
                int tmp = id1;
                id1 = id2;
                id2 = tmp;
            }

            // Convert the integers to bytes
            byte[] id1Bytes = BitConverter.GetBytes(id1);
            byte[] id2Bytes = BitConverter.GetBytes(id2);

            // Copy the bytes into the result array
            Array.Copy(id1Bytes, 0, bytes, 0, id1Bytes.Length);
            Array.Copy(id2Bytes, 0, bytes, id1Bytes.Length, id2Bytes.Length);

            return new Guid(bytes);
        }
    }
}
