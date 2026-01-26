#version 450

layout(set = 0, binding = 1) uniform sampler2D FontTex;

layout(location = 0) in vec2 fsUV;
layout(location = 0) out vec4 outColor;

layout(push_constant) uniform TextColor
{
    vec4 color;
};

void main()
{
    float a = texture(FontTex, fsUV).r; // assuming font alpha in red channel
    outColor = vec4(color.rgb, color.a * a);
}
