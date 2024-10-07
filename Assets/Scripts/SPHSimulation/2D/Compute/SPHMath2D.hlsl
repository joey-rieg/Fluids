// Collection of functions for SPH calculations in 2D

static const float PI = 3.14159265f;

float Poly6Kernel(float distance, float radius)
{
    // Shows warning of potentially uninitialized variable
    // TODO: figure out a better way than how Spiky Kernel is implemented
    if (distance >= radius)
        return 0;
    
    float volume = PI * pow(radius, 8) / 4;
    float value = max(0, radius * radius - distance * distance);
    
    return value * value * value / volume;    
}

float SpikyKernel(float distance, float radius)
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

float SpikyKernelDerivative(float distance, float radius)
{
    if (distance >= radius)
        return 0;
    
    float value = radius - distance;
        
    return -value * (12 / (PI * pow(radius, 4)));
}