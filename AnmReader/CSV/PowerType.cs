﻿namespace BrawlhallaANMReader.ANM.CSV
{
    [Serializable()]
    public class PowerType
    {
        [CsvIgnore]
        public static IList<PowerType>? PowerTypes { get; set; } = default;
        public string PowerName { get; set; } = default!;
        public string PowerID { get; set; } = default!;
        public string OrderID { get; set; } = default!;
        public string DevNotes { get; set; } = default!;
        public string MissionTags { get; set; } = default!;
        public string Priority { get; set; } = default!;
        public string CastSound { get; set; } = default!;
        public string HitSoundLight { get; set; } = default!;
        public string HitSoundHeavy { get; set; } = default!;
        public string CastSoundEvent { get; set; } = default!;
        public string HitSoundEvent { get; set; } = default!;
        public string TargetMethod { get; set; } = default!;
        public string ParentItem { get; set; } = default!;
        public string OriginPower { get; set; } = default!;
        public string IsAirPower { get; set; } = default!;
        public string IsSignature { get; set; } = default!;
        public string IsAntiair { get; set; } = default!;
        public string AoERadiusX { get; set; } = default!;
        public string AoERadiusY { get; set; } = default!;
        public string CenterOffsetX { get; set; } = default!;
        public string CenterOffsetY { get; set; } = default!;
        public string CastImpulseX { get; set; } = default!;
        public string CastImpulseY { get; set; } = default!;
        public string FireImpulseX { get; set; } = default!;
        public string FireImpulseY { get; set; } = default!;
        public string FireImpulseMaxX { get; set; } = default!;
        public string ImpulseMaxOnDCOnly { get; set; } = default!;
        public string SpeedLimit { get; set; } = default!;
        public string SpeedLimitY { get; set; } = default!;
        public string SpeedLimitAttack { get; set; } = default!;
        public string SpeedLimitBackward { get; set; } = default!;
        public string SpeedLimitAttackBackward { get; set; } = default!;
        public string SelfImpulseOnHit { get; set; } = default!;
        public string EndOnHit { get; set; } = default!;
        public string CancelGravity { get; set; } = default!;
        public string WallCancel { get; set; } = default!;
        public string AllowMove { get; set; } = default!;
        public string AllowRecoverMove { get; set; } = default!;
        public string AllowJumpDuringRecover { get; set; } = default!;
        public string AllowLeaveGround { get; set; } = default!;
        public string AllowHitOnZeroDamage { get; set; } = default!;
        public string AccelMult { get; set; } = default!;
        public string BackwardAccelMult { get; set; } = default!;
        public string TurnOffDampening { get; set; } = default!;
        public string KeepGroundFriction { get; set; } = default!;
        public string IgnoreGroundRestrict { get; set; } = default!;
        public string DoNotBounceOffNoSlideCeiling { get; set; } = default!;
        public string NoSlideCeilingBuffer { get; set; } = default!;
        public string CastAnim { get; set; } = default!;
        public string Hurtbox { get; set; } = default!;
        public string CastTime { get; set; } = default!;
        public string FixedRecoverTime { get; set; } = default!;
        public string RecoverTime { get; set; } = default!;
        public string AntigravTime { get; set; } = default!;
        public string GCancelTime { get; set; } = default!;
        public string IgnoreForcedFallTime { get; set; } = default!;
        public string ShowCloudTime { get; set; } = default!;
        public string CooldownTime { get; set; } = default!;
        public string IgnoreCDOverride { get; set; } = default!;
        public string OnHitCooldownTime { get; set; } = default!;
        public string ShakeTime { get; set; } = default!;
        public string DisableShake { get; set; } = default!;
        public string OnlyShakeOnce { get; set; } = default!;
        public string ShakeAllCams { get; set; } = default!;
        public string FixedMinChargeTime { get; set; } = default!;
        public string MinCancelTime { get; set; } = default!;
        public string LoseInvulnTime { get; set; } = default!;
        public string BaseDamage { get; set; } = default!;
        public string VariableImpulse { get; set; } = default!;
        public string FixedImpulse { get; set; } = default!;
        public string MinimumImpulse { get; set; } = default!;
        public string ImpulseOffsetX { get; set; } = default!;
        public string ImpulseOffsetY { get; set; } = default!;
        public string ImpulseOffsetMaxX { get; set; } = default!;
        public string ImpulseToPoint { get; set; } = default!;
        public string ToPointChangeX { get; set; } = default!;
        public string ToPointChangeY { get; set; } = default!;
        public string ToPointChangeDmg { get; set; } = default!;
        public string LockTo45Degrees { get; set; } = default!;
        public string DownwardForceMult { get; set; } = default!;
        public string MirrorImpulseOffset { get; set; } = default!;
        public string MirrorOffsetCenter { get; set; } = default!;
        public string IgnoreStrength { get; set; } = default!;
        public string AcceptInput { get; set; } = default!;
        public string HeldDirOffsets { get; set; } = default!;
        public string DIMaxAngle { get; set; } = default!;
        public string ImpulseOnHeavy { get; set; } = default!;
        public string ItemSpeedDamage { get; set; } = default!;
        public string ItemSpeedImpulse { get; set; } = default!;
        public string AirTimeMultOnly { get; set; } = default!;
        public string IsMultihit { get; set; } = default!;
        public string MinTimeBetweenHits { get; set; } = default!;
        public string InheritAlreadyHit { get; set; } = default!;
        public string InterruptThreshold { get; set; } = default!;
        public string CanDamageEveryone { get; set; } = default!;
        public string CanAssist { get; set; } = default!;
        public string ConsumesWeapon { get; set; } = default!;
        public string FixedStunTime { get; set; } = default!;
        public string HoldHitEnts { get; set; } = default!;
        public string HoldOffsetX { get; set; } = default!;
        public string HoldOffsetY { get; set; } = default!;
        public string UpdateHeldEnts { get; set; } = default!;
        public string GrabInterpolateTime { get; set; } = default!;
        public string GrabAnim { get; set; } = default!;
        public string GrabAnimSpeed { get; set; } = default!;
        public string GrabForceUpdate { get; set; } = default!;
        public string Uninterruptable { get; set; } = default!;
        public string CanChangeDirection { get; set; } = default!;
        public string ComboName { get; set; } = default!;
        public string ComboOverrideIfHit { get; set; } = default!;
        public string ComboOverrideIfRelease { get; set; } = default!;
        public string ComboOverrideIfWall { get; set; } = default!;
        public string ComboOverrideIfButton { get; set; } = default!;
        public string ComboOverrideIfDir { get; set; } = default!;
        public string ComboOverrideIfInterrupt { get; set; } = default!;
        public string IgnoreButtonOnHit { get; set; } = default!;
        public string IgnoreButtonOnMiss { get; set; } = default!;
        public string ComboUseSameTargetPos { get; set; } = default!;
        public string UseCollisionAsTargetPos { get; set; } = default!;
        public string ComboUseTargetAsSource { get; set; } = default!;
        public string ComboUseSameSourcePos { get; set; } = default!;
        public string BGPowerOnFire { get; set; } = default!;
        public string BGCastIdx { get; set; } = default!;
        public string AllowBGInterrupt { get; set; } = default!;
        public string PopulateActivePowerHits { get; set; } = default!;
        public string PopulateBGHits { get; set; } = default!;
        public string ExhaustedVersion { get; set; } = default!;
        public string GCVersion { get; set; } = default!;
        public string MomentumVersion { get; set; } = default!;
        public string TeamTauntPower { get; set; } = default!;
        public string AnimLayer { get; set; } = default!;
        public string FXLayer { get; set; } = default!;
        public string IsWorldCastGfx { get; set; } = default!;
        public string CustomArtCastGfx { get; set; } = default!;
        public string DelayCastGfxToFirstFire { get; set; } = default!;
        public string DelayCastGFXCleanUp { get; set; } = default!;
        public string CastAnimSource { get; set; } = default!;
        public string DoNotSendSync { get; set; } = default!;
        public string IsThrow { get; set; } = default!;
        public string CannotAttackAroundCorners { get; set; } = default!;
        public string ForceHitThroughSoftPlat { get; set; } = default!;
        public string ForceFaceRight { get; set; } = default!;
        public CastGfx CastGfx { get; set; } = new();
        public string CastGfxRotation { get; set; } = default!;
        public string IsWorldFireGfx { get; set; } = default!;
        public string IsAttackFireGfx { get; set; } = default!;
        public string CustomArtFireGfx { get; set; } = default!;
        public string FireAnimSource { get; set; } = default!;
        public FireGfx FireGfx { get; set; } = new();
        public string FireGfxRotation { get; set; } = default!;
        public string IsWorldHitGfx { get; set; } = default!;
        public string OnlyOnceHitGfx { get; set; } = default!;
        public string OwnerFacingHitGfx { get; set; } = default!;
        public string PlayHitGfxBehind { get; set; } = default!;
        public string HitAnimSource { get; set; } = default!;
        public string HitReactAnim { get; set; } = default!;
        public HitGfx HitGfx { get; set; } = new();

        private readonly CsvSerializer<PowerType> _csv = new()
        {
            HasHeader = true
        };

        public PowerType()
        {

        }
        public void Parse(Stream stream)
        {
            PowerTypes = _csv.Deserialize(stream);
        }
        public void Write(FileStream stream)
        {
            _csv.Serialize(stream, PowerTypes ?? throw new NullReferenceException());
        }
    }
}
