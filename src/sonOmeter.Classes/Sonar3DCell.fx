float4x4 WorldViewProj : WorldViewProjection;
float4 col : COLOR;
float4 backCol : COLOR;
float depth[5];
float depthWall[3];
float heightFactor;

struct VScell_IN
{
	float3 pos : POSITION;
	float cell_index : BLENDWEIGHT; // cell position
};

struct VSwall_IN
{
	float3 pos : POSITION;
	float4 col : COLOR;
	float cell_index : BLENDWEIGHT; // cell position
	float z2 : POINTSIZE; // second depth of corresponding rectangle
};

struct VSwall_OUT
{
	float4 pos : POSITION;
	float4 col : COLOR;
};

float4 VScell(VScell_IN input) : POSITION
{
	input.pos.z = depth[(int)input.cell_index] * heightFactor;
	
	return mul(float4(input.pos.xyz, 1.0f), WorldViewProj);
}

float4 PScell() : COLOR {
	return col;
}

float4 PScellGrid() : COLOR {
	return float4(1.0f, 1.0f, 1.0f, 1.0f);
}

VSwall_OUT VSwall(VSwall_IN input)
{
	VSwall_OUT output = (VSwall_OUT)0;
	int index = (int)input.cell_index;

	if (index < 2)
	{
		if (input.pos.z < depthWall[2])
			input.pos.z = depthWall[2];
	}
	else
	{
		index -= 2;
	}
	
	if (input.pos.z > depthWall[index])
		input.pos.z = depthWall[index] * heightFactor;
	else
		input.pos.z *= heightFactor;
	
	output.pos = mul(float4(input.pos.xyz, 1.0f), WorldViewProj);
	output.col = input.col;
	
	return output;
}

technique technique0 {
	pass p0 {
		VertexShader = compile vs_2_0 VScell();
		PixelShader = compile ps_2_0 PScell();
	}
	pass p1 {
		VertexShader = compile vs_2_0 VScell();
		PixelShader = compile ps_2_0 PScellGrid();
	}
	pass p2 {
		VertexShader = compile vs_2_0 VSwall();
	}
}