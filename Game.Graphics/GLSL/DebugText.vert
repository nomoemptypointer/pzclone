#version 450

layout(location = 0) in vec2 inPos;    // vertex position in pixels
layout(location = 1) in vec2 inUV;     // texture coordinates

layout(set = 0, binding = 0) uniform Proj
{
    mat4 proj;   // orthographic projection
};

layout(location = 0) out vec2 fsUV;

void main()
{
    fsUV = inUV;
    gl_Position = proj * vec4(inPos, 0.0, 1.0);
}