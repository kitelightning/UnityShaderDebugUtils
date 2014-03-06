using EasyHook;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace D3DCompileInjector
{
    public partial class Main : EasyHook.IEntryPoint
    {
        D3DCompileInterface Interface;
        //LocalHook D3DCompileHook;
        LocalHook D3DCompileFromFileHook;

        public Main(RemoteHooking.IContext InContext, String InChannelName)
        {
            Interface = RemoteHooking.IpcConnectClient<D3DCompileInterface>(InChannelName);
            Interface.WriteConsole("Dll successfully injected.");
        }

        public void Run(RemoteHooking.IContext InContext, String InChannelName)
        {
            // install hook...
            try
            {
                //D3DCompileHook = LocalHook.Create(
                //    LocalHook.GetProcAddress("D3Dcompiler_47.dll", "D3DCompile"),
                //    new DD3DCompile(D3DCompile_Hooked),
                //    this);

                //D3DCompileHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                D3DCompileFromFileHook = LocalHook.Create(
                    LocalHook.GetProcAddress("D3Dcompiler_47.dll", "D3DCompileFromFile"),
                    new DD3DCompileFromFile(D3DCompileFromFile_Hooked),
                    this);

                D3DCompileFromFileHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            }
            catch (Exception ExtInfo)
            {
                Interface.ReportException(ExtInfo);

                return;
            }

            Interface.IsInstalled(RemoteHooking.GetCurrentProcessId());

            RemoteHooking.WakeUpProcess();

            // wait for host process termination...
            try
            {
                while (true)
                {
                    Thread.Sleep(500);

                    Interface.Ping();
                }
            }
            catch
            {
                // Ping() will raise an exception if host is unreachable
            }

            //Device dev;
            //dev = new Device(new Direct3D(), 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 });

            //IntPtr addy = dev.ComPointer;

            //addy = (IntPtr)Marshal.ReadInt32(addy);

            //addy = (IntPtr)((int)addy + 0xA8);
            //addy = (IntPtr)Marshal.ReadInt32(addy);

            //EndSceneHooker = LocalHook.Create((IntPtr)addy, new DEndScene(EndSceneHook), this);
            //EndSceneHooker.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

            //while (true)
            //{
            //}
        }
    }
}
