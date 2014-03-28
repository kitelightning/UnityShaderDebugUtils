using EasyHook;
using System;
using System.Collections.Generic;
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

        #region D3DCompileFromFile
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate HRESULT DD3DCompileFromFile([MarshalAs(UnmanagedType.LPWStr), In] String pFileName,
                                             [In, Optional, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(D3D_SHADER_MACROMarshaler))]
                                             D3D_SHADER_MACRO[] pDefines,
                                             [In, Optional] IntPtr pInclude,
                                             [MarshalAs(UnmanagedType.LPStr), In] String pEntrypoint,
                                             [MarshalAs(UnmanagedType.LPStr), In] String pTarget,
                                             [In] uint Flags1,
                                             [In] uint Flags2,
                                             [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob ppCode,
                                             [MarshalAs(UnmanagedType.Interface), Out, Optional] out ID3DBlob ppErrorMsgs);

        [DllImport("d3dcompiler_47.dll", EntryPoint = "D3DCompileFromFile", CallingConvention = CallingConvention.StdCall,
            PreserveSig = true, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT D3DCompileFromFile([MarshalAs(UnmanagedType.LPWStr), In] String pFileName,
                                                        [In, Optional, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(D3D_SHADER_MACROMarshaler))]
                                                        D3D_SHADER_MACRO[] pDefines,
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
                Console.WriteLine("D3DCompileFromFile_Hooked writing from client side");
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
        #endregion 

        #region D3DCompile
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate int DD3DCompile([In] IntPtr pSrcData,
                                 [In] uint srcDataSize,
                                 [MarshalAs(UnmanagedType.LPStr), In, Optional] String pSourceName,
                                 [In, Optional, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(D3D_SHADER_MACROMarshaler))]
                                 D3D_SHADER_MACRO[] pDefines,
                                 [In, Optional] IntPtr pInclude,
                                 [MarshalAs(UnmanagedType.LPStr), In] String pEntryPoint,
                                 [MarshalAs(UnmanagedType.LPStr), In] String pTarget,
                                 [In] uint flags1,
                                 [In] uint flags2,
                                 [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob ppCode,
                                 [MarshalAs(UnmanagedType.Interface), Out, Optional] out ID3DBlob ppErrorMsgs);

        [DllImport("d3dcompiler_47.dll", EntryPoint = "D3DCompile", CallingConvention = CallingConvention.StdCall,
            PreserveSig = true, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int D3DCompile([In] IntPtr pSrcData,
                                            [In] uint srcDataSize,
                                            [MarshalAs(UnmanagedType.LPStr), In, Optional] String pSourceName,
                                            [In, Optional, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(D3D_SHADER_MACROMarshaler))]
                                            D3D_SHADER_MACRO[] pDefines,
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
        #endregion

        #region D3DCompile2
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate int DD3DCompile2([In] IntPtr pSrcData,
                                  [In] uint srcDataSize,
                                  [MarshalAs(UnmanagedType.LPStr), In, Optional] String pSourceName,
                                  [In, Optional, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(D3D_SHADER_MACROMarshaler))]
                                  D3D_SHADER_MACRO[] pDefines,
                                  [In, Optional] IntPtr pInclude,
                                  [MarshalAs(UnmanagedType.LPStr), In] String pEntryPoint,
                                  [MarshalAs(UnmanagedType.LPStr), In] String pTarget,
                                  [In] uint flags1,
                                  [In] uint flags2,
                                  [In] uint SecondaryDataFlags,
                                  [In] IntPtr pSecondaryData,
                                  [In] uint SecondaryDataSize,
                                  [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob ppCode,
                                  [MarshalAs(UnmanagedType.Interface), Out, Optional] out ID3DBlob ppErrorMsgs);                                  

        [DllImport("d3dcompiler_47.dll", EntryPoint = "D3DCompile2", CallingConvention = CallingConvention.StdCall,
            PreserveSig = true, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int D3DCompile2([In] IntPtr pSrcData,
                                             [In] uint srcDataSize,
                                             [MarshalAs(UnmanagedType.LPStr), In, Optional] String pSourceName,
                                             [In, Optional, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(D3D_SHADER_MACROMarshaler))]
                                             D3D_SHADER_MACRO[] pDefines,
                                             [In, Optional] IntPtr pInclude,
                                             [MarshalAs(UnmanagedType.LPStr), In] String pEntryPoint,
                                             [MarshalAs(UnmanagedType.LPStr), In] String pTarget,
                                             [In] uint flags1,
                                             [In] uint flags2,
                                             [In] uint SecondaryDataFlags,
                                             [In] IntPtr pSecondaryData,
                                             [In] uint SecondaryDataSize,
                                             [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob ppCode,
                                             [MarshalAs(UnmanagedType.Interface), Out, Optional] out ID3DBlob ppErrorMsgs);

        static int D3DCompile2_Hooked(IntPtr pSrcData,
                                      uint srcDataSize,
                                      String pSourceName,
                                      D3D_SHADER_MACRO[] pDefines,
                                      IntPtr pInclude,
                                      String pEntryPoint,
                                      String pTarget,
                                      uint flags1,
                                      uint flags2,
                                      uint SecondaryDataFlags,
                                      IntPtr pSecondaryData,
                                      uint SecondaryDataSize,
                                      out ID3DBlob ppCode,
                                      out ID3DBlob ppErrorMsgs)
        {
            try
            {
                Main This = (Main)HookRuntimeInfo.Callback;
                This.Interface.WriteConsole("D3DCompile2_Hooked");
            }
            catch
            {
            }

            // call original API...
            return D3DCompile2(pSrcData,
                               srcDataSize,
                               pSourceName,
                               pDefines,
                               pInclude,
                               pEntryPoint,
                               pTarget,
                               flags1,
                               flags2,
                               SecondaryDataFlags,
                               pSecondaryData,
                               SecondaryDataSize,
                               out ppCode,
                               out ppErrorMsgs);
        }
        #endregion

        public static string ID3DBlobToString(ID3DBlob blob)
        {
            if (blob == null)
            {
                return string.Empty;
            }

            string str = Marshal.PtrToStringAnsi(blob.GetBufferPointer(), (int) blob.GetBufferSize());

            return str;
        }

        public class D3D_SHADER_MACROMarshaler : ICustomMarshaler
        {
            private Dictionary<IntPtr, object> managedObjects = new Dictionary<IntPtr, object>();
            readonly int D3D_SHADER_MACRO_BYTESIZE = Marshal.SizeOf(typeof(D3D_SHADER_MACRO));

            //DX expects D3D_SHADER_MACRO[] to be null terminated
            private static D3D_SHADER_MACRO[] PrepareMacros(D3D_SHADER_MACRO[] macros)
            {
                if (macros == null)
                {
                    return null;
                }
                if (macros.Length == 0)
                {
                    return null;
                }
                if ((macros[macros.Length - 1].Name == null) && (macros[macros.Length - 1].Definition == null))
                {
                    return macros;
                }
                D3D_SHADER_MACRO[] destinationArray = new D3D_SHADER_MACRO[macros.Length + 1];
                Array.Copy(macros, destinationArray, macros.Length);
                destinationArray[macros.Length] = new D3D_SHADER_MACRO() { Name = null, Definition = null };
                return destinationArray;
            }

            private IntPtr MarshalArrayWithAlloc(D3D_SHADER_MACRO[] shaderMacroArray)
            {
                int arrayByteSize = D3D_SHADER_MACRO_BYTESIZE * shaderMacroArray.Length;
                IntPtr pNativeShaderMacroArray = Marshal.AllocHGlobal(arrayByteSize);
                
                IntPtr pCurShaderMacro = pNativeShaderMacroArray;
                for (int i = 0; i < shaderMacroArray.Length; i++, pCurShaderMacro += (D3D_SHADER_MACRO_BYTESIZE))
                {
                    Marshal.StructureToPtr(shaderMacroArray[i], pCurShaderMacro, false);
                }

                return pNativeShaderMacroArray;
            }

            public IntPtr MarshalManagedToNative(object managedObj)
            {
                if (managedObj == null)
                    return IntPtr.Zero;

                if (!(managedObj is D3D_SHADER_MACRO[]))
                    throw new MarshalDirectiveException("D3D_SHADER_MACROMarshaler must be used on a D3D_SHADER_MACRO[] object.");

                var nullTerminatedShaderMacroArray = PrepareMacros((D3D_SHADER_MACRO[])managedObj);

                return MarshalArrayWithAlloc(nullTerminatedShaderMacroArray);
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                if (pNativeData == IntPtr.Zero)
                {
                    return null;
                }

                //NOTE: ikrimae: If it's an [in,out] paremeter, there may be some issues. Investigate later.
                //               Not needed now b/c D3DCompile funcs are only [in] for D3D_SHADER_MACRO

                var shaderMacroList = new List<D3D_SHADER_MACRO>();

                IntPtr pCurShaderMacro = pNativeData;
                while (true)
                {
                    D3D_SHADER_MACRO curShaderMacro = (D3D_SHADER_MACRO) Marshal.PtrToStructure(pCurShaderMacro, typeof(D3D_SHADER_MACRO));
                    shaderMacroList.Add(curShaderMacro);                        

                    if (curShaderMacro.Name == null && curShaderMacro.Definition == null)
                    {
                        pCurShaderMacro = IntPtr.Zero;
                        break;
                    }
                    //TODO: ikrimae: Should probably do an error check if someone doesn't pass a null terminated array.
                    //               Have to look at the D3D documentation to see if Name/Definition parameters can be null

                    pCurShaderMacro += D3D_SHADER_MACRO_BYTESIZE;
                }
                
                return shaderMacroList.ToArray();
            }

            public void CleanUpNativeData(IntPtr pNativeData)
            {
                Marshal.FreeHGlobal(pNativeData);
            }

            public void CleanUpManagedData(object managedObj)
            {
            }

            public int GetNativeDataSize()
            {
                return Marshal.SizeOf(typeof(IntPtr));
            }

            public static ICustomMarshaler GetInstance(string cookie)
            {
                return new D3D_SHADER_MACROMarshaler();
            }
        }



    }
}
