using EasyHook;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace D3DCompileInjector
{
    public class Main : EasyHook.IEntryPoint
    {
        //{ "key": "pSrcData", "type": "LPCVOID" },
        //                        { "key": "SrcDataSize", "type": "SIZE_T" },
        //                        { "key": "pSourceName", "type": "LPCSTR" },
        //                        { "key": "pDefines", "type": "LPVOID" },
        //                        { "key": "pInclude", "type": "LPVOID" },
        //                        { "key": "pEntrypoint", "type": "LPCSTR" },
        //                        { "key": "pTarget", "type": "LPCSTR" },
        //                        { "key": "Flags1", "type": "UINT" },
        //                        { "key": "Flags2", "type": "UINT" },
        //                        { "key": "ppCode", "type": "ID3DBlob", "flags": ["out"] },
        //                        { "key": "ppErrorMsgs", "type": "ID3DBlob", "flags": ["out"] }
        [ ComImport, InterfaceType ( ComInterfaceType.InterfaceIsIUnknown ) , Guid ( "8BA5FB08-5195-40E2-AC58-0D989C3A0102" ) ] 
        public interface ID3DBlob { 
            [ PreserveSig ]
            IntPtr GetBufferPointer ( ) ; 
            [ PreserveSig ] 
            ulong   GetBufferSize ( ) ; 
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        public struct D3D_SHADER_MACRO
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string Name;
            
            [MarshalAs(UnmanagedType.LPStr)]
            public string Definition;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate int DD3DCompile([In] IntPtr pSrcData,
                                    [In] uint srcDataSize, 
                                    [MarshalAs(UnmanagedType.LPStr), In, Optional] String pSourceName, 
                                    [In, Optional]D3D_SHADER_MACRO[] pDefines, 
                                    [In, Optional] IntPtr pInclude,
                                    [MarshalAs(UnmanagedType.LPStr), In] String pEntryPoint, 
                                    [MarshalAs(UnmanagedType.LPStr), In] String pTarget, 
                                    [In] uint flags1, 
                                    [In] uint flags2, 
                                    [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob ppCode,
                                    [MarshalAs(UnmanagedType.Interface), Out, Optional] out ID3DBlob ppErrorMsgs);

        [DllImport("d3dcompiler_47.dll", EntryPoint = "D3DCompile", CallingConvention = CallingConvention.StdCall, PreserveSig = true, SetLastError = true)]
        public static extern int D3DCompile([In] IntPtr pSrcData,
                                            [In] uint srcDataSize,
                                            [MarshalAs(UnmanagedType.LPStr), In, Optional] String pSourceName,
                                            [In, Optional]D3D_SHADER_MACRO[] pDefines,
                                            [In, Optional] IntPtr pInclude,
                                            [MarshalAs(UnmanagedType.LPStr), In] String pEntryPoint,
                                            [MarshalAs(UnmanagedType.LPStr), In] String pTarget,
                                            [In] uint flags1,
                                            [In] uint flags2,
                                            [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob ppCode,
                                            [MarshalAs(UnmanagedType.Interface), Out, Optional] out ID3DBlob ppErrorMsgs);

        static int D3DCompile_Hooked(IntPtr pSrcData,
                                       uint srcDataSize,
                                       String pSourceName,
                                       D3D_SHADER_MACRO[] pDefines,
                                       IntPtr pInclude,
                                       String pEntryPoint,
                                       String pTarget,
                                       uint flags1,
                                       uint flags2,
                                       out ID3DBlob ppCode,
                                       out ID3DBlob ppErrorMsgs)
        {
            try
            {
                Main This = (Main)HookRuntimeInfo.Callback;
                This.Interface.WriteConsole("D3DCompile_Hooked");
            }
            catch
            {
            }

            // call original API...
            return D3DCompile(pSrcData,
                              srcDataSize,
                              pSourceName,
                              pDefines,
                              pInclude,
                              pEntryPoint,
                              pTarget,
                              flags1,
                              flags2,
                              out ppCode,
                              out ppErrorMsgs);
        }

        D3DCompileInterface Interface;
        LocalHook D3DCompileHooker;

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
                D3DCompileHooker = LocalHook.Create(
                    LocalHook.GetProcAddress("D3Dcompiler_47.dll", "D3DCompile"),
                    new DD3DCompile(D3DCompile_Hooked),
                    this);

                D3DCompileHooker.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
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
