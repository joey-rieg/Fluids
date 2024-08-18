using UnityEngine;

[CreateAssetMenu(fileName = "SPHSystemProperties", menuName = "SPH/SystemProperties")]
public class SPHSystemProperties : ScriptableObject
{
  [Range(0.0f, 1.0f)]
  [TooltipAttribute("Controls how fast time moves on")]
  public float TimeScale = 1.0f;

  [Range(1, 60)]
  [Tooltip("Controls how many simulation steps are calcualted per frame")]
  public int IterationsPerFrame = 10;

  [Tooltip("Gravity acceleration. 3rd dimension is ignored for 2D simulation.")]
  public Vector3 Gravity = new Vector3(0, -9.81f, 0);

  [Tooltip("Dampening factor when particles hit an object")]
  public float BoundaryDampening = 0.05f;
}
