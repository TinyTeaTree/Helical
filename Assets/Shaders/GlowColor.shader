Shader "Custom/GlowColor"
{
    Properties
    {
        _GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 2
        _EmissionStrength ("Emission Strength", Range(0, 5)) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "RenderPipeline"="UniversalPipeline"
        }
        
        LOD 100
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _GlowColor;
                float _GlowIntensity;
                float _EmissionStrength;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = vertexInput.positionCS;
                output.uv = input.uv;
                output.normalWS = normalInput.normalWS;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Calculate fresnel effect for edge glow
                float3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionCS);
                float fresnel = 1.0 - saturate(dot(input.normalWS, viewDirWS));
                fresnel = pow(fresnel, 2.0); // Adjust power for glow falloff
                
                // Combine base glow with fresnel edge glow
                float3 glow = _GlowColor.rgb * _GlowIntensity;
                glow += _GlowColor.rgb * fresnel * _EmissionStrength;
                
                return half4(glow, 1.0);
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

