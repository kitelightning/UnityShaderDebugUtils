using EasyHook;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace D3DCompileInjector
{
    public partial class Main : EasyHook.IEntryPoint
    {
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("8BA5FB08-5195-40E2-AC58-0D989C3A0102")]
        public interface ID3DBlob
        {
            [PreserveSig]
            IntPtr GetBufferPointer();
            [PreserveSig]
            ulong GetBufferSize();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct D3D_SHADER_MACRO
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string Name;

            [MarshalAs(UnmanagedType.LPStr)]
            public string Definition;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        delegate HRESULT DD3DCompileFromFile([MarshalAs(UnmanagedType.LPWStr), In] String pFileName,
                                         [In, Optional] D3D_SHADER_MACRO[] pDefines,
                                         [In, Optional] IntPtr pInclude,
                                         [MarshalAs(UnmanagedType.LPStr), In] String pEntrypoint,
                                         [MarshalAs(UnmanagedType.LPStr), In] String pTarget,
                                         [In] uint Flags1,
                                         [In] uint Flags2,
                                         [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob ppCode,
                                         [MarshalAs(UnmanagedType.Interface), Out, Optional] out ID3DBlob ppErrorMsgs);

        [DllImport("d3dcompiler_47.dll", EntryPoint = "D3DCompileFromFile", CallingConvention = CallingConvention.StdCall,
            PreserveSig = true, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern HRESULT D3DCompileFromFile([MarshalAs(UnmanagedType.LPWStr), In] String pFileName,
                                                    [In, Optional] D3D_SHADER_MACRO[] pDefines,
                                                    [In, Optional] IntPtr pInclude,
                                                    [MarshalAs(UnmanagedType.LPStr), In] String pEntrypoint,
                                                    [MarshalAs(UnmanagedType.LPStr), In] String pTarget,
                                                    [In] uint Flags1,
                                                    [In] uint Flags2,
                                                    [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob ppCode,
                                                    [MarshalAs(UnmanagedType.Interface), Out, Optional] out ID3DBlob ppErrorMsgs);

        static HRESULT D3DCompileFromFile_Hooked(String pFileName,
                                                 D3D_SHADER_MACRO[] pDefines,
                                                 IntPtr pInclude,
                                                 String pEntrypoint,
                                                 String pTarget,
                                                 uint Flags1,
                                                 uint Flags2,
                                                 out ID3DBlob ppCode,
                                                 out ID3DBlob ppErrorMsgs)
        {
            try
            {
                Main This = (Main)HookRuntimeInfo.Callback;
                This.Interface.WriteConsole("D3DCompileFromFile_Hooked");
                //Console.WriteLine("D3DCompileFromFile_Hooked writing from client side");
            }
            catch
            {
            }

            return D3DCompileFromFile(pFileName, 
                                      pDefines, 
                                      pInclude, 
                                      pEntrypoint, 
                                      pTarget, 
                                      Flags1, 
                                      Flags2, 
                                      out ppCode, 
                                      out ppErrorMsgs);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int DD3DCompile([In] IntPtr pSrcData,
                                 [In] uint srcDataSize,
                                 [MarshalAs(UnmanagedType.LPStr), In, Optional] String pSourceName,
                                 [In, Optional] D3D_SHADER_MACRO[] pDefines,
                                 [In, Optional] IntPtr pInclude,
                                 [MarshalAs(UnmanagedType.LPStr), In] String pEntryPoint,
                                 [MarshalAs(UnmanagedType.LPStr), In] String pTarget,
                                 [In] uint flags1,
                                 [In] uint flags2,
                                 [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob ppCode,
                                 [MarshalAs(UnmanagedType.Interface), Out, Optional] out ID3DBlob ppErrorMsgs);

        [DllImport("d3dcompiler_47.dll", EntryPoint = "D3DCompile", CallingConvention = CallingConvention.StdCall,
            PreserveSig = true, SetLastError = true)]
        public static extern int D3DCompile([In] IntPtr pSrcData,
                                            [In] uint srcDataSize,
                                            [MarshalAs(UnmanagedType.LPStr), In, Optional] String pSourceName,
                                            [In, Optional] D3D_SHADER_MACRO[] pDefines,
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
                Console.WriteLine("D3DCompile_Hooked writing from client side");
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

        public static string ID3DBlobToString(ID3DBlob blob)
        {
            if (blob == null)
            {
                return string.Empty;
            }

            string str = Marshal.PtrToStringAnsi(blob.GetBufferPointer(), (int) blob.GetBufferSize());
            return str;
        }



    }
}
