UnityShaderDebugUtils
=====================

Helper utilities and files for enabling shader debugging

I wanted to use source-level shader debugging (use your favorite GPU debugger: nvidia nsight, renderDoc, Intel GPA) with Unity.  Unfortunately, Unity compiles their shaders offline (without the D3D debug flags) so at most, you can only get the D3D asm/bytecode at runtime during your shader debugging. Not very useful for complex shaders.

So, I started to write an API hook into CGBatch that would pass the necessary debug flags.

We just switched over to the Unreal Engine so I probably won't keep updating this but I figured I'd post it for anyone who wants to use it. 

Here are my notes about the Unity render pipeline:
Unity's Shader Compilation:
	• Unity compiles all shaders & variants at build time
	• Material class constructor takes a shader string as a parameter. This can be read from a file (a file created as an output of CgBatch.exe). 
		○ For detailed explanations on how to use Cgc at runtime to compile a .shader ShaderLab file, check out: http://kylehalladay.com/all/blog/2014/01/12/Runtime-Shader-Compilation-Unity.html & http://forum.unity3d.com/threads/87085-Runtime-shader-compilation
	• CgBatch.exe is the off-line shader compilation tool that the Unity Editor calls to compile from ShaderLab to hlsl/glsl/ARB
	• Everything between CGPROGRAM/ENDCG is pretty much straight CG/HLSL. The 
	• Shader compilation process
		1. Extract CGPROGRAM block from .shader Shaderlab file
		2. Places it in Temp/CgBatchInput.shader
		3. Runs ShaderFixor.exe (not sure what this is yet)
		4. Pass it to CgBatch (C:\Program Files (x86)\Unity4.3\Editor\Data\Tools\CgBatch.exe "Temp/CgBatchInput.shader" "Assets/VSM" "C:/Program Files (x86)/Unity4.3/Editor/Data/CGIncludes" "Temp/CgBatchOutput.shader" "-d3d11_9x" (started in: C:\Users\ikrima\src\KnL\Kiten\StaticShadowMap)
		5. CgBatch runs a preprocessor through the snippet
		6. CGPROGRAM code is compiled to:
			§ Direct3D 9 via Cg (allegedly slated to change to MS HLSL compiler) 
			§ OpenGL via Cg (allegedly slated to be dropped) or hlsl2glsl
			§ OpenGL ES 2.0 via hlsl2glsl + glsl optimizer
			§ Direct3D 11 via MS' HLSL
			§ Flash via Cg + our own d3d9-AGAL converter
			§ Xbox360 via MS' 360 HLSL
			§ PS3 via Sony's Cg
			§ WiiU via something
		7. Take resulting microcode and replace the original CGPROGRAM block with it
			• Binary bytecode is encoded as psuedo-hex (base character 'a', encoding is 'a'+digit. Ex: a => 0x0, i => 0x9, j=>0xa
	• ShaderLab syntax (Passes, Properties, Keywords, etc) are parsed at run time
