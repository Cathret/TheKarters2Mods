using UnityEngine;

namespace TheKarters2Mods.Patches;

public class AFastFall
{
    protected static float ms_fastFallRate = 200f;
    protected static float ms_minimumJumpTimeBeforeFastFall = 0.5f;

    protected static bool IsAllowedToFastFall(PixelKartPhysics _kartPhysics)
    {
        float timeSinceStartedJump = Time.time - _kartPhysics.fPhysicsKartJumped_InUnityTime;
        return !_kartPhysics.bWasGrounded && !_kartPhysics.bIsDrifting && timeSinceStartedJump >= ms_minimumJumpTimeBeforeFastFall;
    }
}