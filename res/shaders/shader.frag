#version 450
uniform vec4 colorIn;
in vec4 pass_Color;
in vec2 pass_uvCoord;
in float pass_texIndex;

out vec4 outputColor;
layout(binding = 0) uniform sampler2D textures[32];

void main(void)
{
    // highp int texIndex = int(pass_texIndex);
    //outputColor = pass_Color * texture(textures[pass_texIndex], pass_uvCoord);
    // Todo: Implement integer buffering...
    switch(int(pass_texIndex)) {
        case 0:
            outputColor = pass_Color * texture(textures[0], pass_uvCoord);
            break;
        case 1:
            outputColor = pass_Color * texture(textures[1], pass_uvCoord);
            break;
        case 2:
            outputColor = pass_Color * texture(textures[2], pass_uvCoord);
            break;
        case 3:
            outputColor = pass_Color * texture(textures[3], pass_uvCoord);
            break;
        case 4:
            outputColor = pass_Color * texture(textures[4], pass_uvCoord);
            break;
        case 5:
            outputColor = pass_Color * texture(textures[5], pass_uvCoord);
            break;
        case 6:
            outputColor = pass_Color * texture(textures[6], pass_uvCoord);
            break;
        case 7:
            outputColor = pass_Color * texture(textures[7], pass_uvCoord);
            break;
        case 8:
            outputColor = pass_Color * texture(textures[8], pass_uvCoord);
            break;
        case 9:
            outputColor = pass_Color * texture(textures[9], pass_uvCoord);
            break;
        case 10:
            outputColor = pass_Color * texture(textures[10], pass_uvCoord);
            break;
        case 11:
            outputColor = pass_Color * texture(textures[11], pass_uvCoord);
            break;
        case 12:
            outputColor = pass_Color * texture(textures[12], pass_uvCoord);
            break;
        case 13:
            outputColor = pass_Color * texture(textures[13], pass_uvCoord);
            break;
        case 14:
            outputColor = pass_Color * texture(textures[14], pass_uvCoord);
            break;
        case 15:
            outputColor = pass_Color * texture(textures[15], pass_uvCoord);
            break;
        default:
            outputColor = vec4(1.0, 0, 0.5, 1.0);
            break;
    }
}