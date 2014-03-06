using System.Windows.Forms;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using System;
using System.Runtime.InteropServices;
using D3DCompileInjector;

namespace SimpleTriangle
{
    static class Program
    {
        static void Main()
        {
            TestShaderCompilationPInvoke();

            TestManagedDXDevice();
        }

        private static void TestManagedDXDevice()
        {
            Device device;
            SwapChain swapChain;
            ShaderSignature inputSignature;
            VertexShader vertexShader;
            PixelShader pixelShader;

            var form = new RenderForm("Tutorial 3: Simple Triangle");
            var description = new SwapChainDescription()
            {
                BufferCount = 2,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, description, out device, out swapChain);

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            RenderTargetView renderTarget;
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(device, resource);

            // setting a viewport is required if you want to actually see anything
            var context = device.ImmediateContext;
            var viewport = new Viewport(0.0f, 0.0f, form.ClientSize.Width, form.ClientSize.Height);
            context.OutputMerger.SetTargets(renderTarget);
            context.Rasterizer.SetViewports(viewport);

            // load and compile the vertex shader
            using (var bytecode = ShaderBytecode.CompileFromFile("triangle.fx", "VShader", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                inputSignature = ShaderSignature.GetInputSignature(bytecode);
                vertexShader = new VertexShader(device, bytecode);
            }

            // load and compile the pixel shader
            using (var bytecode = ShaderBytecode.CompileFromFile("triangle.fx", "PShader", "ps_4_0", ShaderFlags.None, EffectFlags.None))
                pixelShader = new PixelShader(device, bytecode);

            // create test vertex data, making sure to rewind the stream afterward
            var vertices = new DataStream(12 * 3, true, true);
            vertices.Write(new Vector3(0.0f, 0.5f, 0.5f));
            vertices.Write(new Vector3(0.5f, -0.5f, 0.5f));
            vertices.Write(new Vector3(-0.5f, -0.5f, 0.5f));
            vertices.Position = 0;

            // create the vertex layout and buffer
            var elements = new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0) };
            var layout = new InputLayout(device, inputSignature, elements);
            var vertexBuffer = new SlimDX.Direct3D11.Buffer(device, vertices, 12 * 3, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            // configure the Input Assembler portion of the pipeline with the vertex data
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 12, 0));

            // set the shaders
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);

            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAltEnter);

            // handle alt+enter ourselves
            form.KeyDown += (o, e) =>
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                    swapChain.IsFullScreen = !swapChain.IsFullScreen;
            };

            // handle form size changes
            form.UserResized += (o, e) =>
            {
                renderTarget.Dispose();

                swapChain.ResizeBuffers(2, 0, 0, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
                using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                    renderTarget = new RenderTargetView(device, resource);

                context.OutputMerger.SetTargets(renderTarget);
            };

            MessagePump.Run(form, () =>
            {
                // clear the render target to a soothing blue
                context.ClearRenderTargetView(renderTarget, new Color4(0.5f, 0.5f, 1.0f));

                // draw the triangle
                context.Draw(3, 0);
                swapChain.Present(0, PresentFlags.None);
            });

            // clean up all resources
            // anything we missed will show up in the debug output
            vertices.Close();
            vertexBuffer.Dispose();
            layout.Dispose();
            inputSignature.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }

        private static void TestShaderCompilationPInvoke()
        {
            
            String shader = "void vs_main(in float4 pos : POSITION, out float4 opos : SV_POSITION) { opos = pos; }";
            D3DCompileInjector.Main.ID3DBlob pCode = null, pMsg = null;

            IntPtr globalShader = Marshal.StringToHGlobalAnsi(shader);
            int lastError = Marshal.GetLastWin32Error();
            HRESULT hr = D3DCompileInjector.Main.D3DCompile(globalShader, 
                                                            (uint)shader.Length, 
                                                            null,
                                                            new D3DCompileInjector.Main.D3D_SHADER_MACRO[2] { 
                                                                new D3DCompileInjector.Main.D3D_SHADER_MACRO { Name = "EXAMPLE_DEFINE", Definition = "1" },
                                                                new D3DCompileInjector.Main.D3D_SHADER_MACRO { Name = null, Definition = null }
                                                            }, 
                                                            IntPtr.Zero, 
                                                            "vs_main", 
                                                            "vs_5_0", 
                                                            0, 
                                                            0, 
                                                            out pCode, 
                                                            out pMsg);
            lastError = Marshal.GetLastWin32Error();

            if (hr.Failed)
            {
                string error = string.Format("{0} : {1}", hr.ToString(), D3DCompileInjector.Main.ID3DBlobToString(pMsg));
                throw new Exception(error);
            }

            IntPtr codePtr = pCode.GetBufferPointer();
            ulong codeSize = pCode.GetBufferSize();

            Console.WriteLine("Code size " + codeSize);

            hr = D3DCompileInjector.Main.D3DCompileFromFile(
                @".\triangle.fx", 
                new D3DCompileInjector.Main.D3D_SHADER_MACRO[2] { 
                    new D3DCompileInjector.Main.D3D_SHADER_MACRO { Name = "EXAMPLE_DEFINE", Definition = "1" },
                    new D3DCompileInjector.Main.D3D_SHADER_MACRO { Name = null, Definition = null }
                },
                IntPtr.Zero,
                "VShader", 
                "vs_5_0", 
                0, 
                0, 
                out pCode, 
                out pMsg);
            lastError = Marshal.GetLastWin32Error();
            if (hr.Failed)
            {
                string error = string.Format("{0} : {1}", hr.ToString(), D3DCompileInjector.Main.ID3DBlobToString(pMsg));
                throw new Exception(error);
            }

            codePtr = pCode.GetBufferPointer();
            codeSize = pCode.GetBufferSize();
            
            
            Console.WriteLine("Code size " + codeSize);
        }
    }
}
