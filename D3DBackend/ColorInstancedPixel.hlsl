struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float3 normal : NORMAL;
	float3 viewDirection : VIEWDIRECTION;
};

float4 main(PixelInputType input) : SV_TARGET
{	
	float3 lightDirection = float3(-0.816497f, -0.408248, -0.408248f);		// MUST BE NORMALIZED!
	float3 ambientColor = float3(0.2f, 0.2f, 0.2f);
	float3 diffuseColor = float3(0.8f, 0.8f, 0.8f);
	
    float diffuseIntensity;
    float3 specularIntensity;	
	float4 color = input.color;
    float3 specular;

	input.normal.xyz = normalize(input.normal.xyz);

    diffuseIntensity = saturate(dot(input.normal.xyz, lightDirection));
	specularIntensity = normalize(lightDirection - 2 * diffuseIntensity * input.normal.xyz);
    specular = diffuseIntensity * pow(saturate(dot(specularIntensity, normalize(input.viewDirection))), 4);

	color *= float4(saturate(diffuseColor * diffuseIntensity + ambientColor), 1.0f);
    color = saturate(color + float4(specular, 0.0f));
	
	return color;
}