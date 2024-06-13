namespace SFA.DAS.ProviderCommitments.Web.Helpers;

public static class FlagExtensions
{
    public static ulong PipeDelimitedToFlags<T>(this string pipedelimitedlist) where T: Enum
    {
        if(string.IsNullOrEmpty(pipedelimitedlist))
        {
            return (ulong)0;
        }

        var list = pipedelimitedlist.Split('|').ToList();
        return list.ToFlags<T>();
    }

    public static ulong ToFlags<T>(this List<string> list) where T : Enum
    {
        var flags = (ulong)0;
        foreach (var item in list)
        {
            flags |= (ulong)Enum.Parse(typeof(T), item, true);
        }
        return flags;
    }

    public static bool IsFlagSet<T>(this ulong flags, T flag) where T : Enum
    {
        var checkFlag = (ulong)(object)flag;
        var andFlag = flags & checkFlag;

        if(andFlag != 0)
        {
            return true;
        }

        return false;
    }
}
