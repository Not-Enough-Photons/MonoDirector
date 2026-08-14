namespace NEP.MonoDirector.Content;

public struct PalletVersion
{
    public PalletVersion(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    public static bool operator == (PalletVersion lhs, PalletVersion rhs)
    {
        return (lhs.Major == rhs.Major) && (lhs.Minor == rhs.Minor) && (lhs.Patch == rhs.Patch);
    }

    public static bool operator != (PalletVersion lhs, PalletVersion rhs)
    {
        return (lhs.Major != rhs.Major) || (lhs.Minor != rhs.Minor) || (lhs.Patch != rhs.Patch);
    }

    public static bool operator > (PalletVersion lhs, PalletVersion rhs)
    {
        return (lhs.Major > rhs.Major) || (lhs.Minor > rhs.Minor) || (lhs.Patch > rhs.Patch);
    }
    
    public static bool operator < (PalletVersion lhs, PalletVersion rhs)
    {
        return (lhs.Major < rhs.Major) || (lhs.Minor < rhs.Minor) || (lhs.Patch < rhs.Patch);
    }

    public readonly int Major;
    public readonly int Minor;
    public readonly int Patch;
}
