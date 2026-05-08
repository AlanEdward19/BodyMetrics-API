namespace BodyMetricsApi.Features.Athletes.PhysicalAssessments.Shared.ViewModels;

public sealed record CircumferencesViewModel(
    decimal? ShoulderCm,
    decimal? ChestCm,
    decimal? RightArmCm,
    decimal? LeftArmCm,
    decimal? WaistCm,
    decimal? HipCm,
    decimal? RightMidThighCm,
    decimal? LeftMidThighCm,
    decimal? RightCalfCm,
    decimal? LeftCalfCm,
    decimal? RightWristCm,
    decimal? RightKneeCm,
    decimal? RightAnkleCm);

