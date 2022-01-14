#version 450
layout(location = 0) in vec2 position;
layout(location = 1) in vec4 color;

out vec4 pass_Color;
void main(void)
{
    gl_Position = vec4(position.xy, 0.0f, 1.0);
    
    pass_Color = color;
}