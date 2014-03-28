using EasyHook;
using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

namespace D3DCompileInjector
{
    public class D3DCompileInterface : MarshalByRefObject
    {
        public void IsInstalled(Int32 InClientPID)
        {
            Console.WriteLine("D3DCompileInterface has been installed in target {0}.\r\n", InClientPID);
            return;
        }

        public void WriteConsole(String Write)
        {
            Console.WriteLine(Write);
        }

        public void ReportException(Exception InInfo)
        {
            Console.WriteLine("The target process has reported an error:\r\n" + InInfo.ToString());
        }

        public void Ping()
        {
        }
    }

    class D3DCompileHost
    {
        static string ChannelName = null;

        static void Main(string[] args)
        {
            try
            {
                int unityPid;

                try
                {
                    Config.Register("D3DCompile Debug Flag Injector",
                                    "D3DCompileInjectorHost.exe",
                                    "D3DCompileInjectedDll.dll");
                }
                catch (ApplicationException)
                {
                    MessageBox.Show("Must run as admin!", "Permission denied...", MessageBoxButtons.OK);

                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }

                //while (true)
                //{
                //    Process[] procs = Process.GetProcessesByName("TestD3DPInvoke");

                //    if (procs.Length == 0)
                //    {
                //        //Thread.Sleep(500);
                //        continue;
                //    }

                //    unityPid = procs[0].Id;
                //    break;
                //}


                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files (x86)\Unity4.3\Editor\Data\Tools\CgBatch.exe");
                    startInfo.Arguments = @"""Temp/CgBatchInput.shader"" ""Assets/VSM"" ""C:/Program Files (x86)/Unity4.3/Editor/Data/CGIncludes"" ""Temp/CgBatchOutput.shader"" ""-d3d11_9x""";
                    startInfo.WorkingDirectory = @"C:\Users\ikrima\src\KnL\Kiten\StaticShadowMap";
                    startInfo.UseShellExecute = false;

                    var cgBatchProcess = Process.Start(startInfo);
                    unityPid = cgBatchProcess.Id;
                }


                RemoteHooking.IpcCreateServer<D3DCompileInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);

                RemoteHooking.Inject(
                    unityPid,
                    InjectionOptions.DoNotRequireStrongName,
                    "D3DCompileInjectedDll.dll",
                    "D3DCompileInjectedDll.dll",
                    ChannelName);

                Console.ReadLine();
            }
            catch (Exception ExtInfo)
            {
                Console.WriteLine("There was an error while connecting to target:\r\n{0}", ExtInfo.ToString());
            }            
        }
    }
}
