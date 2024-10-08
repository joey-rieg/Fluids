Shader "SPH/SPHParticle2D"
{
    Properties
    {
        MaxVelocity ("MaxVelocity", Float) = 100.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"= "Transparent" }
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
            StructuredBuffer<float2> Velocities;
            Texture2D<float4> GradientTexture;
            SamplerState linear_clamp_sampler;
            float4x4 LocalTransform;
            float Scale;
            float MaxVelocity;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 color: TEXCOORD1;
            };

            v2f vert (appdata_base v, uint svInstanceID : SV_InstanceID)
            {
                InitIndirectDrawArgs(0);
                v2f o;
                //uint cmdID = GetCommandID(0);
                uint instanceID = GetIndirectInstanceID(svInstanceID);
                int instanceCount = GetIndirectInstanceCount();

                float3 wPos = float3(Positions[instanceID].xy, 0) + mul(LocalTransform, v.vertex * Scale);
                float velocity = length(Velocities[instanceID]);
                float velT = saturate(velocity / MaxVelocity);
                
                o.vertex = mul(UNITY_MATRIX_VP, float4(wPos, 1));
                //o.color = float4(lerp(0, 1, instanceID / (float)instanceCount), 0, 1, 1);
                o.color = GradientTexture.SampleLevel(linear_clamp_sampler, float2(velT, 0.5), 0);
                o.uv = v.texcoord;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 centeredUVs = (i.uv - 0.5) * 2.0;
                float dst = sqrt(dot(centeredUVs, centeredUVs));
                float alpha = step(dst, 1);
                
                return float4(i.color, alpha);
            }
            ENDCG
        }
    }
}
