Shader "Sprites/Scenery"  
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ( "Tint", Color ) = (1, 1, 1, 1)
        _BehindTint ("Behind Tint", Color) = (0, 0, 0, 0.5)
    }

    CGINCLUDE
    struct appdata_t
    {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float2 texcoord : TEXCOORD0;
    };

    struct v2f  
    {
        float4 vertex : SV_POSITION;
        fixed4 color : COLOR;
        half2 texcoord : TEXCOORD0;
    };

    fixed4 _Color;  
    sampler2D _MainTex;

    v2f vert(appdata_t IN)  
    {
        v2f OUT;
        OUT.vertex = UnityObjectToClipPos(IN.vertex);
        OUT.texcoord = IN.texcoord;
        OUT.color = IN.color * _Color;
        return OUT;
    }
    ENDCG

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Stencil
            {
                Ref 4
                Comp NotEqual
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }

        Pass
        {
            Stencil
            {
                Ref 4
                Comp Equal
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            fixed4 _BehindTint;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord);
                return _BehindTint * c.a;
            }
            ENDCG
        }
    }
}