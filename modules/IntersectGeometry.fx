float2 R;
float2 Rmap;
texture map,points;
sampler sMap=sampler_state{Texture=(map);MipFilter=POINT;MinFilter=POINT;MagFilter=POINT;AddressU=BORDER;AddressV=BORDER;};
sampler sPoints=sampler_state{Texture=(points);MipFilter=POINT;MinFilter=POINT;MagFilter=POINT;AddressU=CLAMP;AddressV=CLAMP;};

float4 p0(float2 x:TEXCOORD0,float2 vp:vpos):color{
    float2 pointXY=tex2D(sPoints,(vp+.5)/R).xy*float2(.5,-.5)+.5+.5/Rmap;
    float4 c=tex2D(sMap,pointXY).a;
    return c;
}
void vs2d(inout float4 vp:POSITION0,inout float2 uv:TEXCOORD0){vp.xy*=2;uv+=.5/R;}
technique IntersectGeometry{pass pp0{AlphaBlendEnable=FALSE;vertexshader=compile vs_3_0 vs2d();pixelshader=compile ps_3_0 p0();}}
