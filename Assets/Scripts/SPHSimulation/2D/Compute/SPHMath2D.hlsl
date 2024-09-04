// Collection of functions for SPH calculations in 2D

static const float PI = 3.14159265f;

float Poly6Kernel(float distance, float radius)
{
    float influence = 0;
    float volume = PI * pow(radius, 8) / 4;
        
    if (volume > 0.0)
    {
        float value = max(0, radius * radius - distance * distance);
    
        influence = value * value * value / volume;
    }
    
    return influence;
}