using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
namespace PointerDotNet
{
    public static class PointerDotNet
    {
        public static IntPtr Add(this IntPtr ptr, IntPtr add)
        {
            return (IntPtr)((long)ptr + (long)add);


        }
        public static IntPtr hProc(this Process pro)
        {
           
            return WinApi.GethProc(WinApi.ProcessAccessFlags.All, false, pro.Id);
        }
        public static class WinApi
        {
            private static List<IntPtr> registerdopenprocess = new List<IntPtr>();
            private static List<long> registerdopenprocessid = new List<long>();
            private static Dictionary<long, IntPtr> registerdopenprocessidtoprocesshproc = new Dictionary<long, IntPtr>();
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(
       ProcessAccessFlags processAccess,
       bool bInheritHandle,
       int processId
  );
            public static IntPtr GethProc(ProcessAccessFlags processAccess,
       bool bInheritHandle,
       int processId)
            {
                
                if (registerdopenprocessid.Contains(processId))
                {
                    return registerdopenprocessidtoprocesshproc[processId];
                }
                else
                {
                    var a = OpenProcess(processAccess, bInheritHandle, processId);
                    registerdopenprocess.Add(a);
                    registerdopenprocessidtoprocesshproc.Add(processId, a);
                    registerdopenprocessid.Add(processId);
                    return a;
                }
            }
            [Flags]
            public enum ProcessAccessFlags : uint
            {
                All = 0x001F0FFF,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VirtualMemoryOperation = 0x00000008,
                VirtualMemoryRead = 0x00000010,
                VirtualMemoryWrite = 0x00000020,
                DuplicateHandle = 0x00000040,
                CreateProcess = 0x000000080,
                SetQuota = 0x00000100,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                QueryLimitedInformation = 0x00001000,
                Synchronize = 0x00100000
            }
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(
    IntPtr hProcess,
    IntPtr lpBaseAddress,
    byte[] lpBuffer,
    Int32 nSize,
    out int lpNumberOfBytesRead);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool WriteProcessMemory(
IntPtr hProcess,
IntPtr lpBaseAddress,
byte[] lpBuffer,
Int32 nSize,
out IntPtr lpNumberOfBytesWritten);
        }
    }


    public class Pointer
    {
        public IntPtr hProc;
        public IntPtr BaseAddress;
        public IntPtr PointerAddress = IntPtr.Zero;
        public IntPtr[] Offset;
        public Pointer(IntPtr hProc, IntPtr BaseAddress, IntPtr[] Offset)
        {

            this.hProc = hProc;
            this.BaseAddress = BaseAddress;
            this.Offset = Offset;
        }

        public IntPtr GetAddress()
        {
            IntPtr temp;
            int bytesRead;
            {
                byte[] buffer = new byte[32];
                PointerDotNet.WinApi.ReadProcessMemory(hProc, BaseAddress, buffer, buffer.Length, out bytesRead);
                temp = (IntPtr)BitConverter.ToInt64(buffer, 0);
            }
            int temp2 = Offset.Length;
            int i = 0;
            while (temp2 > 1)
            {
                byte[] buffer3 = new byte[32];
                PointerDotNet.WinApi.ReadProcessMemory(hProc, temp.Add(Offset[i]), buffer3, buffer3.Length, out bytesRead);
                temp = (IntPtr)BitConverter.ToInt64(buffer3, 0);
                i++;
                temp2--;
            }
            return temp.Add(Offset[i]);
        }
        private IntPtr getptraddr()
        {
            if (PointerAddress == IntPtr.Zero)
            {
                PointerAddress = GetAddress();
            }
            return PointerAddress;
        }
        public int GetValue()
        {
            return (int)(GetValueT<int>());
        }
        public int Getint()
        {
            return GetValueT<int>();
        }


        public long GetLong()
        {
            return GetValueT<long>();
        }


        public float GetFloat()
        {
            return GetValueT<float>();
        }


        public double GetDouble()
        {
            return GetValueT<double>();
        }


        public bool GetBool()
        {
            return GetValueT<bool>();
        }


        public char GetChar()
        {
            return GetValueT<char>();
        }


        public string GetString()
        {
            return GetValueT<string>();
        }


        public short GetShort()
        {
            return GetValueT<short>();
        }


        public ushort GetUShort()
        {
            return GetValueT<ushort>();
        }


        public uint GetUInt()
        {
            return GetValueT<uint>();
        }


        public ulong GetULong()
        {
            return GetValueT<ulong>();
        }

        public T GetValueT<T>()
        {
            
            IntPtr PtrAddr = getptraddr();

            byte[] buffer = new byte[32];
            int bytesRead;
            PointerDotNet.WinApi.ReadProcessMemory(hProc, PtrAddr, buffer, buffer.Length, out bytesRead);
            if (typeof(T) == typeof(int))
            {
                return (T)(object)BitConverter.ToInt32(buffer, 0);
            }
            if (typeof(T) == typeof(float))
            {
                return (T)(object)BitConverter.ToSingle(buffer, 0);
            }
            if (typeof(T) == typeof(double))
            {
                return (T)(object)BitConverter.ToDouble(buffer, 0);
            }
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)BitConverter.ToBoolean(buffer, 0);
            }
            if (typeof(T) == typeof(char))
            {
                return (T)(object)BitConverter.ToChar(buffer, 0);
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)BitConverter.ToString(buffer, 0);
            }
            if (typeof(T) == typeof(short))
            {
                return (T)(object)BitConverter.ToInt16(buffer, 0);
            }
            if (typeof(T) == typeof(long))
            {
                return (T)(object)BitConverter.ToInt64(buffer, 0);
            }
            if (typeof(T) == typeof(ushort))
            {
                return (T)(object)BitConverter.ToUInt16(buffer, 0);
            }
            if (typeof(T) == typeof(uint))
            {
                return (T)(object)BitConverter.ToUInt32(buffer, 0);
            }
            if (typeof(T) == typeof(ulong))
            {
                return (T)(object)BitConverter.ToUInt64(buffer, 0);
            }
            throw new NotSupportedException(typeof(T).Name + " is not suported.");
        }
        public static void SetValue<T>(IntPtr ptr, IntPtr hProc, T value, Encoding encoding)
        {
            IntPtr aaa;
            if (typeof(T) == typeof(int))
            {
                byte[] newver = BitConverter.GetBytes((int)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(float))
            {
                byte[] newver = BitConverter.GetBytes((float)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(double))
            {

                byte[] newver = BitConverter.GetBytes((double)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(bool))
            {
                byte[] newver = BitConverter.GetBytes((bool)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(char))
            {
                byte[] newver = BitConverter.GetBytes((char)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(string))
            {
                byte[] newver = encoding.GetBytes((string)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(short))
            {
                byte[] newver = BitConverter.GetBytes((short)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(long))
            {
                byte[] newver = BitConverter.GetBytes((long)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(ushort))
            {
                byte[] newver = BitConverter.GetBytes((ushort)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(uint))
            {
                byte[] newver = BitConverter.GetBytes((uint)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }
            if (typeof(T) == typeof(ulong))
            {
                byte[] newver = BitConverter.GetBytes((ulong)(object)value);
                PointerDotNet.WinApi.WriteProcessMemory(hProc, ptr, newver, newver.Length, out aaa);
                return;
            }

            else
                throw new NotSupportedException(typeof(T).Name + " is not suported.");

        }
        public static implicit operator IntPtr(Pointer val)
        {
            return val.getptraddr();
        }
        public static implicit operator int(Pointer val)
        {
            return val.GetValueT<int>();
        }

        public static implicit operator long(Pointer val)
        {
            return val.GetValueT<long>();
        }

        public static implicit operator float(Pointer val)
        {
            return val.GetValueT<float>();
        }

        public static implicit operator double(Pointer val)
        {
            return val.GetValueT<double>();
        }

        public static implicit operator bool(Pointer val)
        {
            return val.GetValueT<bool>();
        }

        public static implicit operator char(Pointer val)
        {
            return val.GetValueT<char>();
        }

        public static implicit operator string(Pointer val)
        {
            return val.GetValueT<string>();
        }

        public static implicit operator short(Pointer val)
        {
            return val.GetValueT<short>();
        }

        public static implicit operator ushort(Pointer val)
        {
            return val.GetValueT<ushort>();
        }

        public static implicit operator uint(Pointer val)
        {
            return val.GetValueT<uint>();
        }

        public static implicit operator ulong(Pointer val)
        {
            return val.GetValueT<ulong>();
        }
    }

}
