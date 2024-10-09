using UnityEngine;

[CreateAssetMenu(fileName = "SPHSystemProperties", menuName = "SPH/SystemProperties")]
public class SPHSystemProperties : ScriptableObject
{
  [Range(0.0f, 1.0f)]
  [TooltipAttribute("Controls how fast time moves on")]
  public float TimeScale = 1.0f;

  [Range(1, 60)]
  [Tooltip("Controls how many simulation steps are calcualted per frame")]
  public int IterationsPerFrame = 4;

  [Tooltip("Gravity acceleration. 3rd dimension is ignored for 2D simulation.")]
  public Vector3 Gravity = new Vector3(0, -9.81f, 0);

  [Tooltip("Dampening factor when particles hit an object")]
  public float BoundaryDampening = 0.05f;

  [Tooltip("Influence radius")]
  public float KernelRadius = 1.0f;

  [Tooltip("Target density the fluid wants to reach")]
  public float TargetDensity = 1.0f;

  [Tooltip("Factor to handle pressure impact to reach target density")]
  public float PressureMultiplier = 1.0f;

  [Tooltip("Factor to modify near pressure")]
  public float NearPressureMultiplier = 1.0f;

  [Tooltip("Influence of viscosity on the fluid")]
  public float ViscosityMultiplier = 0.5f;
}
