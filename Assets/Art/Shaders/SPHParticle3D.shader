// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SPH/SPHParticle3D"
{
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

            StructuredBuffer<float3> Positions;
            float4x4 LocalTransform;
            float Scale;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
            };

            v2f vert (appdata_base v, uint svInstanceID : SV_InstanceID)
            {
                InitIndirectDrawArgs(0);
                v2f o;
                //uint cmdID = GetCommandID(0);
                uint instanceID = GetIndirectInstanceID(svInstanceID);
                int instanceCount = GetIndirectInstanceCount();

                float3 wPos = Positions[instanceID].xyz + mul(LocalTransform, v.vertex * Scale);

                o.vertex = mul(UNITY_MATRIX_VP, float4(wPos, 1));
                
                o.color = float4(lerp(0, 1, instanceID / (float)instanceCount), 0, 1, 1);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
