using System;
using System.Text;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace axGB
{
    public class RendererGL : IRenderer
    {
        private readonly GL gl;

        private uint vertexFormat;
        private uint shaderProgram;
        private uint texture;

        public RendererGL(IWindow window)
        {
            gl = GL.GetApi(window);
            Initialize(window);
        }

#if DEBUG
        private void DebugMessageCallback(GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint message, nint userParam)
        {
            string output;

            unsafe
            {
                output = Encoding.ASCII.GetString((byte*)message.ToPointer(), length);
            }

            Console.WriteLine($"{severity.ToString().PadRight(25)} | {output}");
        }
#endif

        public void Initialize(IWindow windowHandle)
        {
#if DEBUG
            DebugProc proc = DebugMessageCallback;
            gl.Enable(GLEnum.DebugOutput);
            gl.DebugMessageCallback<nint>(proc, null);
#endif

            // Apparently this still needs to be bound even without a VBO
            vertexFormat = gl.CreateVertexArray();
            gl.BindVertexArray(vertexFormat);

            // Texture
            texture = gl.CreateTexture(GLEnum.Texture2D);
            gl.TextureStorage2D(texture, 1, GLEnum.Rgb8, 160, 144);
            gl.TextureParameter(texture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TextureParameter(texture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            gl.BindTextureUnit(0, texture);

            var vertexShaderSource = @"
                #version 420 core

                struct Vertex {
                    vec2 position;
                    vec2 uv;
                };

                const Vertex vertices[4] =
                {
                    { vec2(-1.0, -1.0), vec2(0.0, 0.0) }, // Bottom left
                    { vec2(-1.0,  1.0), vec2(0.0, 1.0) }, // Top left
                    { vec2( 1.0,  1.0), vec2(1.0, 1.0) }, // Top right
                    { vec2( 1.0, -1.0), vec2(1.0, 0.0) }  // Bottom right
                };

                out vec2 frag_texCoords;  

                void main()
                {
                    gl_Position    = vec4(vertices[gl_VertexID].position, 0.0, 1.0);
                    frag_texCoords = vertices[gl_VertexID].uv;
                }
            ";

            var vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, vertexShaderSource);
            gl.CompileShader(vertexShader);

            // Try to get some useful information if it failed to compile
            gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vsStatus);
            if (vsStatus != (int)GLEnum.True)
            {
                throw new Exception(gl.GetShaderInfoLog(vertexShader));
            }

            var pixelShaderSource = @"
                # version 420 core

                uniform sampler2D textureSampler;

                in  vec2 frag_texCoords; 
                out vec4 out_color;

                void main()
                {
                    // y at the bottom in GL, need to flip the coordinate or the screen will be upside down
                    out_color = texture(textureSampler, vec2(frag_texCoords.x, frag_texCoords.y * -1.0f));
                }
            ";

            var pixelShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(pixelShader, pixelShaderSource);
            gl.CompileShader(pixelShader);

            gl.GetShader(pixelShader, ShaderParameterName.CompileStatus, out int psStatus);
            if (psStatus != (int)GLEnum.True)
            {
                throw new Exception(gl.GetShaderInfoLog(pixelShader));
            }
                
            shaderProgram = gl.CreateProgram();
            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, pixelShader);
            gl.LinkProgram(shaderProgram);
            gl.UseProgram(shaderProgram);

            // Apparently we can just get rid of the shaders once the resultant program is linked?
            gl.DetachShader(shaderProgram, vertexShader);
            gl.DetachShader(shaderProgram, pixelShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(pixelShader);
        }

        public void Render(ReadOnlySpan<uint> data)
        {
            // Dumb hack - the Color class apparently is backwards, and this needs to be BGRA
            gl.TextureSubImage2D<uint>(texture, 0, 0, 0, 160, 144, GLEnum.Bgra, GLEnum.UnsignedByte, data);
            gl.DrawArrays(GLEnum.TriangleFan, 0, 4);
        }
    }
}
