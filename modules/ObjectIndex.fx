float4x4 tWVP:WORLDVIEWPROJECTION;

struct vs2ps{float4 Pos:POSITION;float3 PosM:COLOR0;};

vs2ps VS(float4 p:POSITION0){
    vs2ps Out=(vs2ps)0;
    Out.Pos = mul(p,tWVP);
    return Out;
}

///////INDEX==COLOR//////////
float Index;
float4 PS_ID():COLOR{
       return Index/255.;
}
/////////////////////////////

technique T_ID{
    pass P0{AlphaBlendEnable=FALSE;VertexShader=compile vs_3_0 VS();PixelShader=compile ps_3_0 PS_ID();}
}
