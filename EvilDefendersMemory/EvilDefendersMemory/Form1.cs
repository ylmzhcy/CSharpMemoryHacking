using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EvilDefendersMemory
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static IntPtr pHandle = IntPtr.Zero;
        int ptrValue;
        private Process[] processes;
        private Process process;

        private IntPtr dllPtr;

        //Let's find pointer 
        int basePtr;

        //offsets
        const int offset1 = 0x50;
        const int offset2 = 0x16c;
        const int offset3 = 0x1C;
        const int offset4 = 0x64;
        const int offset5 = 0x00;


        #region MemoryTools
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

        #endregion


        private void GetBasePointerAndHandle(string gameProcName, string moduleDllName)
        {
            processes = Process.GetProcessesByName(gameProcName); //game processes name
            process = processes[0];

            ProcessModuleCollection modules = process.Modules;
            ProcessModule dllBaseAdress = null;

            foreach (ProcessModule i in modules)
            {
                if (i.ModuleName == moduleDllName) //game Dll name
                {
                    dllBaseAdress = i;
                    break;
                }
            }

            dllPtr = dllBaseAdress.BaseAddress;

            pHandle = OpenProcess(ProcessAccessFlags.All, false, process.Id);

            if (pHandle == IntPtr.Zero)
            {
                MessageBox.Show("Handle = 0 \n" +
                    "Please Run Administrator!");
            }
            else
            {
                label3.Text = "Connect \n" +
                    "Succesfull!";
            }

        }

        #region GetPointerAddrValue
        private void GetPointerAddr(Int32 addModuleBaseAddr, int off1)
        {
            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            ptrValue = ReadLong(ReadLong(basePtr) + off1);
        }
        private void GetPointerAddr(Int32 addModuleBaseAddr, int off1, int off2)
        {
            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            ptrValue = ReadLong(ReadLong(ReadLong(basePtr) + off1) + off2);
        }
        private void GetPointerAddr(Int32 addModuleBaseAddr, int off1, int off2, int off3)
        {
            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            ptrValue = ReadLong(ReadLong(ReadLong(ReadLong(basePtr) + off1) + off2) + off3);
        }
        private void GetPointerAddr(Int32 addModuleBaseAddr, int off1, int off2, int off3, int off4)
        {

            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            ptrValue = ReadLong(ReadLong(ReadLong(ReadLong(ReadLong(basePtr) + off1) + off2) + off3) + off4);
        }
        private void GetPointerAddr(Int32 addModuleBaseAddr, int off1, int off2, int off3, int off4, int off5)
        {
            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            ptrValue = ReadLong(ReadLong(ReadLong(ReadLong(ReadLong(ReadLong(basePtr) + off1) + off2) + off3) + off4) + off5);
        }
        #endregion


        #region WritePointerValue
        private void WritePointerValue(Int32 addModuleBaseAddr, int off1,int value)
        {
            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            WriteLong(ReadLong(basePtr) + off1, value);
        }
        private void WritePointerValue(Int32 addModuleBaseAddr, int off1, int off2,int value)
        {
            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            WriteLong(ReadLong(ReadLong(basePtr) + off1) + off2,value);
        }
        private void WritePointerValue(Int32 addModuleBaseAddr, int off1, int off2, int off3, int value)
        {
            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            WriteLong(ReadLong(ReadLong(ReadLong(basePtr) + off1) + off2) + off3, value);
        }
        private void WritePointerValue(Int32 addModuleBaseAddr, int off1, int off2, int off3, int off4, int value)
        {
            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            WriteLong(ReadLong(ReadLong(ReadLong(ReadLong(basePtr) + off1) + off2) + off3) + off4, value);
        }
        private void WritePointerValue(Int32 addModuleBaseAddr, int off1, int off2, int off3, int off4, int off5, int value)
        {
            basePtr = dllPtr.ToInt32() + addModuleBaseAddr;

            WriteLong(ReadLong(ReadLong(ReadLong(ReadLong(ReadLong(basePtr) + off1) + off2) + off3) + off4) + off5, value);
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetPointerAddr(0x001F52C4, offset1, offset2, offset3, offset4);
            lblGetSunCount.Text = ptrValue.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GetBasePointerAndHandle(txtExeName.Text, txtDllName.Text);
        }

        private void txtExeName_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            WritePointerValue(0x001F52C4, offset1, offset2, offset3, offset4, Convert.ToInt32(txtSunCount.Text));
        }
    }
}
