Shader "SPH/DensityParticleShader"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"

            StructuredBuffer<float2> Positions;
            float Scale;
            float2 KernelCenter;
            float KernelRadius;


            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;  
                float3 color : COLOR0;
            };

            v2f vert (appdata_base v, uint svInstanceID : SV_InstanceID)
            {
                InitIndirectDrawArgs(0);

                v2f o;
                uint instanceID = GetIndirectInstanceID(svInstanceID);
                int instanceCount = GetIndirectInstanceCount();

                o.vertex = mul(UNITY_MATRIX_VP, float4(v.vertex * Scale  + Positions[instanceID], 0, 1));
                o.uv = v.texcoord;

                float2 c2p = KernelCenter - Positions[instanceID];
                o.color = sqrt(dot(c2p, c2p)) < KernelRadius ? float3(0, 0, 1) : float3(.3,.3,.3);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 centeredUVs = (i.uv.xy - 0.5) * 2.0;
                float dst = sqrt(dot(centeredUVs, centeredUVs));
                float alpha = step(dst, 1);

                return float4(i.color.xyz, alpha);
            }
            ENDCG
        }
    }
}
