$input v_color0, v_texcoord0

#include "bgfx_shader.bsh"

SAMPLER2D(u_texture, 0);

void main() {
    gl_FragColor = v_color0 * texture2D(u_texture, v_texcoord0.xy);
} 