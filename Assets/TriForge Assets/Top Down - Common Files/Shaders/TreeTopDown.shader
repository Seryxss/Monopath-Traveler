Shader "TriForge/Top Down/Tree_URP"
{
    Properties
    {
        _Cutoff("Mask Clip Value", Float) = 0.5
        _MainTex("Base Color", 2D) = "white" {}
        _Normal("Normal", 2D) = "bump" {}
        _MaskMap("Mask Map", 2D) = "white" {}
        _Smoothness("Smoothness", Range(0, 1)) = 0
        _AOIntensity("AO Intensity", Range(0, 1)) = 1
        _MainTextureBrightness("Main Texture Brightness", Range(0, 2)) = 0.5
        [Toggle(_USEGRADIENT_ON)] _UseGradient("UseGradient", Float) = 0
        _DetailBendingScale("Detail Bending Scale", Float) = 5
        _GradientPosition("Gradient Position", Float) = 2.93
        _TopBrightness("Top Brightness", Range(0, 10)) = 3
        [HDR]_TopColor("Top Color", Color) = (0,0,0,0)
        [HDR]_Color("Color", Color) = (0,0,0,0)
        [KeywordEnum(UV2,UV3)] _GradientUV("Gradient UV", Float) = 0
        _NormalIntensity("Normal Intensity", Float) = 1
        _WindDirectionRandomness("Wind Direction Randomness", Range(0, 1)) = 0
        _WindMultiplier("Wind Multiplier", Float) = 1
        [Toggle(_USECOLORVARIANCE_ON)] _UseColorVariance("UseColorVariance", Float) = 0
        _ColorVarianceAmount("Color Variance Amount", Range(0, 1)) = 0.44
        _ColorVariationMask("Color Variation Mask", 2D) = "white" {}
        _DetailBendingIntensity("Detail Bending Intensity", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "TransparentCutout" 
            "Queue" = "AlphaTest" 
            "RenderPipeline" = "UniversalPipeline"
            "DisableBatching" = "True" 
        }
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma shader_feature_local _USECOLORVARIANCE_ON
            #pragma shader_feature_local _USEGRADIENT_ON
            #pragma shader_feature_local _GRADIENTUV_UV2 _GRADIENTUV_UV3
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Normal_ST;
                float4 _MaskMap_ST;
                float4 _Color;
                float4 _TopColor;
                float _Cutoff;
                float _Smoothness;
                float _AOIntensity;
                float _MainTextureBrightness;
                float _DetailBendingScale;
                float _GradientPosition;
                float _TopBrightness;
                float _NormalIntensity;
                float _WindDirectionRandomness;
                float _WindMultiplier;
                float _ColorVarianceAmount;
                float _DetailBendingIntensity;
                
                // Globals
                float3 TD_WindDirection;
                float TD_WindStrength;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_Normal); SAMPLER(sampler_Normal);
            TEXTURE2D(_MaskMap); SAMPLER(sampler_MaskMap);
            TEXTURE2D(_ColorVariationMask); SAMPLER(sampler_ColorVariationMask);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1; // Wind weights
                float4 texcoord2 : TEXCOORD2; // UV2
                float4 texcoord3 : TEXCOORD3; // UV3
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD4;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float2 uv3 : TEXCOORD2;
                float4 tangentWS : TANGENT;
                float3 normalWS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // Noise Functions
            float3 mod3D289(float3 x) { return x - floor(x / 289.0) * 289.0; }
            float4 mod3D289(float4 x) { return x - floor(x / 289.0) * 289.0; }
            float4 permute(float4 x) { return mod3D289((x * 34.0 + 1.0) * x); }
            float4 taylorInvSqrt(float4 r) { return 1.79284291400159 - r * 0.85373472095314; }

            float snoise(float3 v)
            {
                const float2 C = float2(1.0 / 6.0, 1.0 / 3.0);
                float3 i = floor(v + dot(v, C.yyy));
                float3 x0 = v - i + dot(i, C.xxx);
                float3 g = step(x0.yzx, x0.xyz);
                float3 l = 1.0 - g;
                float3 i1 = min(g.xyz, l.zxy);
                float3 i2 = max(g.xyz, l.zxy);
                float3 x1 = x0 - i1 + C.xxx;
                float3 x2 = x0 - i2 + C.yyy;
                float3 x3 = x0 - 0.5;
                i = mod3D289(i);
                float4 p = permute(permute(permute(i.z + float4(0.0, i1.z, i2.z, 1.0)) + i.y + float4(0.0, i1.y, i2.y, 1.0)) + i.x + float4(0.0, i1.x, i2.x, 1.0));
                float4 j = p - 49.0 * floor(p / 49.0);
                float4 x_ = floor(j / 7.0);
                float4 y_ = floor(j - 7.0 * x_);
                float4 x = (x_ * 2.0 + 0.5) / 7.0 - 1.0;
                float4 y = (y_ * 2.0 + 0.5) / 7.0 - 1.0;
                float4 h = 1.0 - abs(x) - abs(y);
                float4 b0 = float4(x.xy, y.xy);
                float4 b1 = float4(x.zw, y.zw);
                float4 s0 = floor(b0) * 2.0 + 1.0;
                float4 s1 = floor(b1) * 2.0 + 1.0;
                float4 sh = -step(h, 0.0);
                float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
                float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
                float3 g0 = float3(a0.xy, h.x);
                float3 g1 = float3(a0.zw, h.y);
                float3 g2 = float3(a1.xy, h.z);
                float3 g3 = float3(a1.zw, h.w);
                float4 norm = taylorInvSqrt(float4(dot(g0, g0), dot(g1, g1), dot(g2, g2), dot(g3, g3)));
                g0 *= norm.x;
                g1 *= norm.y;
                g2 *= norm.z;
                g3 *= norm.w;
                float4 m = max(0.6 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
                m = m * m; m = m * m;
                float4 px = float4(dot(x0, g0), dot(x1, g1), dot(x2, g2), dot(x3, g3));
                return 42.0 * dot(m, px);
            }

            float3 RotateAroundAxis(float3 center, float3 original, float3 u, float angle)
            {
                original -= center;
                float C = cos(angle);
                float S = sin(angle);
                float t = 1 - C;
                float m00 = t * u.x * u.x + C;
                float m01 = t * u.x * u.y - S * u.z;
                float m02 = t * u.x * u.z + S * u.y;
                float m10 = t * u.x * u.y + S * u.z;
                float m11 = t * u.y * u.y + C;
                float m12 = t * u.y * u.z - S * u.x;
                float m20 = t * u.x * u.z - S * u.y;
                float m21 = t * u.y * u.z + S * u.x;
                float m22 = t * u.z * u.z + C;
                float3x3 finalMatrix = float3x3(m00, m01, m02, m10, m11, m12, m20, m21, m22);
                return mul(finalMatrix, original) + center;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float ifLocalVar183 = (TD_WindDirection.x == 0.0) ? 1.0 : 0.0;
                float ifLocalVar186 = (TD_WindDirection.z == 0.0) ? 1.0 : 0.0;
                float3 lerpResult187 = lerp(TD_WindDirection, float3(1,0,0), (ifLocalVar183 * ifLocalVar186));
                float3 worldToObjDir94 = normalize(mul(GetWorldToObjectMatrix(), float4(lerpResult187, 0)).xyz);
                float3 lerpResult173 = lerp(worldToObjDir94, lerpResult187, _WindDirectionRandomness);
                float3 objToWorld214 = mul(GetObjectToWorldMatrix(), float4(float3(0,0,0), 1)).xyz;
                
                float2 panner82 = ((_Time.y * 1.0) * float2(0.12,0) + ((objToWorld214).xz / 50.0));
                float simplePerlin3D80 = snoise(float3(panner82, 0.0) * 3.0) * 0.5 + 0.5;
                
                float3 ase_worldPos = mul(GetObjectToWorldMatrix(), input.positionOS).xyz;
                float2 panner102 = ((_Time.y * 0.6) * float2(-0.1,0.1) + ((ase_worldPos).xz / _DetailBendingScale));
                float simplePerlin3D97 = snoise(float3(panner102, 0.0) * 10.0) * 0.5 + 0.5;
                
                float3 ase_vertex3Pos = input.positionOS.xyz;
                float bendValue = ((-0.4 + (simplePerlin3D80) * 1.0) + (_DetailBendingIntensity * simplePerlin3D97)) * 30.0;
                float3 rotatedValue59 = RotateAroundAxis(float3(0,0,0), ase_vertex3Pos, lerpResult173, radians(bendValue));
                
                // Apply Wind
                input.positionOS.xyz += ((input.texcoord1.y * (rotatedValue59 - ase_vertex3Pos)) * (TD_WindStrength * _WindMultiplier));

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w);
                output.uv = input.texcoord.xy;
                output.uv2 = input.texcoord2.xy;
                output.uv3 = input.texcoord3.xy;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float2 uv_MainTex = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                half4 tex2DNode1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv_MainTex);
                
                // Alpha Clip
                clip(tex2DNode1.a - _Cutoff);

                half4 temp_output_155_0 = tex2DNode1 * _Color;
                half4 temp_output_47_0 = temp_output_155_0 * _MainTextureBrightness;
                
                half3 desaturateInitialColor46 = temp_output_47_0.rgb;
                half desaturateDot46 = dot(desaturateInitialColor46, half3(0.299, 0.587, 0.114));
                half3 desaturateVar46 = lerp(desaturateInitialColor46, desaturateDot46.xxx, 0.25);

                #if defined(_GRADIENTUV_UV2)
                    float staticSwitch166 = input.uv2.y;
                #elif defined(_GRADIENTUV_UV3)
                    float staticSwitch166 = input.uv3.y;
                #else
                    float staticSwitch166 = input.uv2.y;
                #endif

                half4 lerpResult32 = lerp(temp_output_47_0, (half4(desaturateVar46, 0.0) * (_TopColor * _TopBrightness)), saturate(pow(staticSwitch166, _GradientPosition)));
                
                #ifdef _USEGRADIENT_ON
                    half4 staticSwitch170 = lerpResult32;
                #else
                    half4 staticSwitch170 = temp_output_155_0;
                #endif

                half dotResult205 = dot(lerpResult32, half4(0.55, 0.55, 0.55, 0.0));
                float3 objToWorld204 = mul(GetObjectToWorldMatrix(), float4(0,0,0, 1)).xyz;
                half4 colorVarMask = SAMPLE_TEXTURE2D(_ColorVariationMask, sampler_ColorVariationMask, (objToWorld204.xz / 50.0));
                half4 lerpResult210 = lerp(staticSwitch170, ((dotResult205 * 5.0) * colorVarMask), _ColorVarianceAmount);

                #ifdef _USECOLORVARIANCE_ON
                    half4 finalAlbedo = lerpResult210;
                #else
                    half4 finalAlbedo = staticSwitch170;
                #endif

                // Mask Map (Smoothness & AO)
                float2 uv_MaskMap = input.uv * _MaskMap_ST.xy + _MaskMap_ST.zw;
                half4 tex2DNode3 = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, uv_MaskMap);
                half smoothness = tex2DNode3.a * _Smoothness;
                half ao = lerp(1.0, tex2DNode3.g, _AOIntensity);

                // Normal
                float2 uv_Normal = input.uv * _Normal_ST.xy + _Normal_ST.zw;
                half4 normalSample = SAMPLE_TEXTURE2D(_Normal, sampler_Normal, uv_Normal);
                half3 normalTS = UnpackNormalScale(normalSample, _NormalIntensity);

                // Setup URP Lighting
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, cross(input.normalWS, input.tangentWS.xyz) * input.tangentWS.w, input.normalWS));
                inputData.viewDirectionWS = normalize(GetCameraPositionWS() - input.positionWS);
                
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = finalAlbedo.rgb;
                surfaceData.metallic = 0.0;
                surfaceData.smoothness = smoothness;
                surfaceData.occlusion = ao;
                surfaceData.alpha = 1.0;

                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                return color;
            }
            ENDHLSL
        }
    }
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}