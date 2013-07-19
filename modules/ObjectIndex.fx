float4x4 tWVP:WORLDVIEWPROJECTION;

Texture2D texture2d <string uiname="Texture";>;
SamplerState g_samLinear : IMMUTABLE
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct vs2ps{float4 Pos:POSITION; float4 TexCd : TEXCOORD0;};

vs2ps VS(float4 p:POSITION0, float4 TexCd : TEXCOORD0)
{
    vs2ps Out=(vs2ps)0;
    Out.Pos = mul(p,tWVP);
	Out.TexCd = TexCd;
    return Out;
}

float Index;
float4 PS_ID(vs2ps In):SV_Target
{
	float4 indexColor = (Index * (texture2d.Sample(g_samLinear,In.TexCd.xy).a > 0))/ 10000.;
	return indexColor;
}

technique10 T_ID{
    pass P0
	{
		//AlphaBlendEnable = false;
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS_ID() ) );
	}
}
