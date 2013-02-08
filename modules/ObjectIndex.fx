float4x4 tWVP:WORLDVIEWPROJECTION;

texture Tex <string uiname="Texture";>;
sampler Samp = sampler_state {Texture   = (Tex); MipFilter = LINEAR; MinFilter = LINEAR; MagFilter = LINEAR;};

struct vs2ps{float4 Pos:POSITION;float2 TexCd : TEXCOORD0;};

vs2ps VS(float4 p:POSITION0, float4 TexCd : TEXCOORD0){
    vs2ps Out=(vs2ps)0;
    Out.Pos = mul(p,tWVP);
	Out.TexCd = TexCd;
    return Out;
}

float Index;
float4 PS_ID(vs2ps In):COLOR{
	float4 indexColor = (Index * (tex2D(Samp, In.TexCd).a > 0))/ 255.;
	return indexColor;
}

technique T_ID{
    pass P0{AlphaBlendEnable = false; VertexShader=compile vs_3_0 VS();PixelShader=compile ps_3_0 PS_ID();}
}
