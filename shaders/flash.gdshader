shader_type canvas_item;

uniform vec4 ColorParameter : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float Progress : hint_range(0.0, 1.0) = 0.0;

void fragment() {
	vec4 base_color = texture(TEXTURE, UV) * COLOR;
	vec3 flash_color = mix(base_color.rgb, ColorParameter.rgb, Progress);
	COLOR = vec4(flash_color, base_color.a);
}
