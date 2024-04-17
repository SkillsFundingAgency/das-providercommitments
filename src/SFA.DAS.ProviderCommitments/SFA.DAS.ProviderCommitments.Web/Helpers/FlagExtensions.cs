namespace SFA.DAS.ProviderCommitments.Web.Helpers;

public static class FlagExtensions
{
    public static int PipeDelimitedToFlags<T>(this string pipedelimitedlist) where T: Enum
    {
        if(string.IsNullOrEmpty(pipedelimitedlist))
        {
            return 0;
        }

        var list = pipedelimitedlist.Split('|').ToList();
        return list.ToFlags<T>();
    }

    public static int ToFlags<T>(this List<string> list) where T : Enum
    {
        var flags = 0;
        foreach (var item in list)
        {
            flags |= (int)Enum.Parse(typeof(T), item);
        }
        return flags;
    }

    public static bool IsFlagSet<T>(this int flags, T flag) where T : Enum
    {
        var checkFlag = (int)(object)flag;
        var andFlag = flags & checkFlag;

        if(andFlag != 0)
        {
            return true;
        }

        return false;
    }
}
