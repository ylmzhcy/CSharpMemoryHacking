using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication11
{
    class Fonks
    {
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
        private static Process[] proc;
        public static int pID;
        private static IntPtr pHandle;
        public IntPtr deneme;
        public string message;

        const int zombieBase = 0x0729670;
        const int of1 = 0x868;
        const int of2 = 0x5578;

        public const int PROCESS_VM_READ = 0x10;

        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string text);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            ProcessAccessFlags processAccess,
            bool bInheritHandle,
            int processId
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out float buffer,
            UInt32 size,
            IntPtr lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out UInt32 buffer,
            UInt32 size,
            IntPtr lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(
            IntPtr hProcess, 
            IntPtr lpBaseAddress,
            [In, Out] byte[] buffer, 
            UInt32 size, 
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int nSize,
            out int lpNumberOfBytesWritten);

        [DllImport("user32.dll", EntryPoint = "FindWindowA")]//The FindWindow function retrieves a handle to the top-level window whose class name and window name match the specified strings.
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]//The GetWindowThreadProcessId function retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
        public static extern Int32 GetWindowThreadProcessId(IntPtr hwnd, ref Int32 lpdwProcessId);

        public int WriteLong(int Address, int Value)
        {
            int ret;
            try
            {
                WriteProcessMemory(pHandle, (IntPtr)Address, BitConverter.GetBytes(Value), 4, out ret);
            }
            catch (Exception)
            {
                ret = 0;
            }
            return ret;
        }


        public int ReadLong(int Address)
        {
            UInt32 ret;
            try
            {
                ReadProcessMemory(pHandle, (IntPtr)Address, out ret, (UInt32)4, new IntPtr());
            }
            catch (Exception)
            {
                ret = 0;
            }
            return (int)ret;
        }

        public void ConnectMe(string text1)
        {
            pID = 0;
            IntPtr _hwnd = FindWindow(null, text1);
            if (_hwnd == IntPtr.Zero || text1 == "")
            {
                message = "ForumDC \nOyun Penceresi Bulunamadı!!";
                return;
            }

            GetWindowThreadProcessId(_hwnd, ref pID);
            if (pID == 0)
            {
                message= "ForumDC \nProgramı Yönetici Çalıştır !!";
                return;
            }

            pHandle = OpenProcess(ProcessAccessFlags.All, false, pID);
            //MessageBox.Show(KOHANDLE.ToString());
            if(pHandle != IntPtr.Zero)
            {
                message = "ForumDC \nBağlantı Sağlandı ! İyi Oyunlar :)";

            }

        }

        public void FindProcessOpenH()
        {
            proc = Process.GetProcessesByName("PlantsVsZombies");
            pID = proc[0].Id;
            pHandle = OpenProcess(ProcessAccessFlags.All, false, pID);
            deneme = pHandle;
            //Debug.Print(pHandle.ToString());
        }

        public int GetSunCount()
        {
            return ReadLong(ReadLong(ReadLong(zombieBase) + of1) + of2);
        }

        public void ChangeSunCount(int count)
        {
            WriteLong(ReadLong(ReadLong(zombieBase) + of1) + of2, count);
        }

    }
}
