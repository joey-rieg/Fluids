// Collection of functions for SPH calculations in 2D

static const float PI = 3.14159265f;

float Poly6Kernel(float distance, float radius)
{
    float result = 0;
    if (distance < radius)
    {
        float value = max(0, radius * radius - distance * distance);
        float volume = 4 / PI * pow(radius, 8);
        
        result = value * value * value * volume;
    }
    
    return result;
}

float SpikyKernelPow2(float distance, float radius)
{
    float result = 0;
    if (distance < radius)
    {
        float value = radius - distance;
        float volume = 6 / (PI * pow(radius, 4));
        
        result = value * value * volume;
    }

    return result;
}

float SpikyKernelPow2Derivative(float distance, float radius)
{
    float result = 0;
    if (distance < radius)
    {
        float value = radius - distance;
        float volume = (12 / (PI * pow(radius, 4)));
        
        result = -value * volume;
    }
    
    return result;
}

float SpikyKernelPow3(float distance, float radius)
{
    float result = 0;
    if (distance < radius)
    {
        float value = radius - distance;
        float volume = 10 / (PI * pow(radius, 5));
        
        result = value * value * value * volume;                
    }
    
    return result;
}

float SpikyKernelPow3Derivative(float distance, float radius)
{
    float result = 0;
    if (distance < radius)
    {
        float value = radius - distance;
        float volume = 30 / (PI * pow(radius, 5));

        result = -value * value * volume;
    }
    
    return result;
}