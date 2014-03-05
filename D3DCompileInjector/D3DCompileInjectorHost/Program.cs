using EasyHook;
using System;
using System.Diagnostics;
using System.Runtime.Remoting;
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

    class Program
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
                                    "D3DCompileInjectedDll.dll", 
                                    "D3DCompileInjectorHost.exe");
                }
                catch (ApplicationException)
                {
                    MessageBox.Show("This is an administrative task!", "Permission denied...", MessageBoxButtons.OK);

                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }

                Process[] procs = Process.GetProcessesByName("Unity");

                unityPid = procs[0].Id;


                RemoteHooking.IpcCreateServer<D3DCompileInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);

                RemoteHooking.Inject(
                    unityPid, 
                    InjectionOptions.Default, 
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
