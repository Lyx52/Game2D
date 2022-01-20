#version 450
in vec4 pass_Color;
in vec2 pass_uvCoord;
in float pass_texIndex;

out vec4 outputColor;
layout(binding = 0) uniform sampler2D textures[32];

vec4 GetTexture(in int textureIndex) {
    switch(textureIndex) {
        case 0: return pass_Color * texture(textures[0], pass_uvCoord);
        case 1: return pass_Color * texture(textures[1], pass_uvCoord);
        case 2: return pass_Color * texture(textures[2], pass_uvCoord);
        case 3: return pass_Color * texture(textures[3], pass_uvCoord);
        case 4: return pass_Color * texture(textures[4], pass_uvCoord);
        case 5: return pass_Color * texture(textures[5], pass_uvCoord);
        case 6: return pass_Color * texture(textures[6], pass_uvCoord);
        case 7: return pass_Color * texture(textures[7], pass_uvCoord);
        case 8: return pass_Color * texture(textures[8], pass_uvCoord);
        case 9: return pass_Color * texture(textures[9], pass_uvCoord);
        case 10: return pass_Color * texture(textures[10], pass_uvCoord);
        case 11: return pass_Color * texture(textures[11], pass_uvCoord);
        case 12: return pass_Color * texture(textures[12], pass_uvCoord);
        case 13: return pass_Color * texture(textures[13], pass_uvCoord);
        case 14: return pass_Color * texture(textures[14], pass_uvCoord);
        case 15: return pass_Color * texture(textures[15], pass_uvCoord);
        case 16: return pass_Color * texture(textures[16], pass_uvCoord);
        case 17: return pass_Color * texture(textures[17], pass_uvCoord);
        case 18: return pass_Color * texture(textures[18], pass_uvCoord);
        case 19: return pass_Color * texture(textures[19], pass_uvCoord);
        case 20: return pass_Color * texture(textures[20], pass_uvCoord);
        case 21: return pass_Color * texture(textures[21], pass_uvCoord);
        case 22: return pass_Color * texture(textures[22], pass_uvCoord);
        case 23: return pass_Color * texture(textures[23], pass_uvCoord);
        case 24: return pass_Color * texture(textures[24], pass_uvCoord);
        case 25: return pass_Color * texture(textures[25], pass_uvCoord);
        case 26: return pass_Color * texture(textures[26], pass_uvCoord);
        case 27: return pass_Color * texture(textures[27], pass_uvCoord);
        case 28: return pass_Color * texture(textures[28], pass_uvCoord);
        case 29: return pass_Color * texture(textures[29], pass_uvCoord);
        case 30: return pass_Color * texture(textures[30], pass_uvCoord);
        case 31: return pass_Color * texture(textures[31], pass_uvCoord);
        default: return pass_Color; // Tex dosnt exist!
    }
}
void main(void)
{
    outputColor = GetTexture(int(pass_texIndex));
}