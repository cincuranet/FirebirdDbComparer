namespace FirebirdDbComparer.Compare;

public enum TargetVersion
{
    Version25 = 25,
    Version30 = 30,
    Version40 = 40,
}

public static class TargetVersionExt
{
    public static string VersionSuffix(this TargetVersion tv)
    {
        return ((int)tv).ToString();
    }

    public static bool AtLeast(this TargetVersion tv, TargetVersion targetVersion)
    {
        return tv >= targetVersion;
    }

    public static bool AtMost(this TargetVersion tv, TargetVersion targetVersion)
    {
        return tv <= targetVersion;
    }
}
