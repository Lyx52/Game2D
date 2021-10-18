#version 450
layout(location = 0) in vec2 position;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 uvCoord;
layout(location = 3) in float texIndex;

layout(binding = 1) uniform Matrices {
    mat4 viewProjection;
} block_matrices;

out vec4 pass_Color;
out vec2 pass_uvCoord;
out float pass_texIndex;

void main(void)
{
    gl_Position = block_matrices.viewProjection * vec4(position.xy, 1.0, 1.0);
    pass_Color = color;
    pass_uvCoord = uvCoord;
    pass_texIndex = texIndex;
}