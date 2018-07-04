using System;
using System.Text.RegularExpressions;

namespace FirebirdDbComparer.Compare
{
    public enum TargetVersion
    {
        Version25,
        Version30,
    }

    public static class TargetVersionExt
    {
        public static string VersionSuffix(this TargetVersion targetVersion)
        {
            return Regex.Match(targetVersion.ToString(), @"(\d+)$", RegexOptions.CultureInvariant).Groups[1].Value;
        }

        public static bool AtLeast25(this TargetVersion targetVersion)
        {
            return targetVersion == TargetVersion.Version25
                || targetVersion == TargetVersion.Version30;
        }

        public static bool AtLeast30(this TargetVersion targetVersion)
        {
            return targetVersion == TargetVersion.Version30;
        }
    }
}
