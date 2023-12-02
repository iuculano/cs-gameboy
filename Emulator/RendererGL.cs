using System;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace axGB.Emulator
{
    public class RendererGL : IRenderer
    {
        private GL gl;

        private uint vertexFormat;
        private uint vertexBuffer;
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

            // Fullscreen quad
            float[] vertices = new float[]
            {
                // Positions   Texture coords
                -1.0f, -1.0f,  0.0f, 0.0f, // Bottom left
                -1.0f,  1.0f,  0.0f, 1.0f, // Top left
                 1.0f,  1.0f,  1.0f, 1.0f, // Top right
                 1.0f, -1.0f,  1.0f, 0.0f, // Bottom right
            };

            // glNamedBufferData() handles the upload to the vertex buffer
            vertexBuffer = gl.CreateBuffer();
            gl.NamedBufferData<float>(vertexBuffer, (nuint)(sizeof(float) * vertices.Length), vertices, GLEnum.StaticDraw);

            // Each vertex is 4 floats, 2 for position, 2 for texture coordinates
            vertexFormat = gl.CreateVertexArray();
            gl.VertexArrayVertexBuffer(vertexFormat, 0, vertexBuffer, 0, 4 * sizeof(float));

            gl.EnableVertexArrayAttrib(vertexFormat, 0);
            gl.EnableVertexArrayAttrib(vertexFormat, 1);

            gl.VertexArrayAttribFormat(vertexFormat, 0, 2, GLEnum.Float, false, 0);
            gl.VertexArrayAttribFormat(vertexFormat, 1, 2, GLEnum.Float, false, 2 * sizeof(float));

            gl.VertexArrayAttribBinding(vertexFormat, 0, 0);
            gl.VertexArrayAttribBinding(vertexFormat, 1, 0);

            // Texture
            texture = gl.CreateTexture(GLEnum.Texture2D);
            gl.TextureStorage2D(texture, 1, GLEnum.Rgb8, 160, 144);
            gl.TextureParameter(texture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TextureParameter(texture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            var vertexShaderSource = @"
                # version 460 core

                layout(location = 0) in vec2 position;
                layout(location = 1) in vec2 texCoord;

                out vec2 frag_texCoords;

                void main()
                {
                    gl_Position    = vec4(position.x, position.y, 0.0, 1.0);
                    frag_texCoords = texCoord;
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
                # version 460 core

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

            // Technically there's no need to clear because we're guaranteed to write the whole screen
            gl.Clear(ClearBufferMask.ColorBufferBit);
            
            // Actually all this can happen in init...
            gl.BindVertexArray(vertexFormat);
            gl.UseProgram(shaderProgram);
            gl.BindTextureUnit(0, texture);

            gl.DrawArrays(GLEnum.TriangleFan, 0, 4);
        }
    }
}
