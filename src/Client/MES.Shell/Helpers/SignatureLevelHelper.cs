namespace MES.Shell.Helpers;

/// <summary>
/// 签核级别比较工具：L1（操作员）&lt; L2（QA）&lt; L3（工程）&lt; L4（主管）。
/// </summary>
public static class SignatureLevelHelper
{
    private static readonly Dictionary<string, int> LevelOrder = new(
        StringComparer.OrdinalIgnoreCase)
    {
        { "L0", 0 },
        { "L1", 1 },
        { "L2", 2 },
        { "L3", 3 },
        { "L4", 4 },
    };

    /// <summary>
    /// 获取级别的数值排序，级别不存在返回 -1。
    /// </summary>
    public static int GetLevelOrder(string level)
    {
        if (string.IsNullOrWhiteSpace(level))
            return -1;

        return LevelOrder.TryGetValue(level.Trim(), out var order) ? order : -1;
    }

    /// <summary>
    /// 判断实际级别是否满足或超过所需级别。
    /// </summary>
    public static bool IsLevelSufficient(string actualLevel, string requiredLevel)
    {
        var actual = GetLevelOrder(actualLevel);
        var required = GetLevelOrder(requiredLevel);

        if (required < 0)
            return true; // 未定义所需级别，默认放行
        if (actual < 0)
            return false; // 实际级别无效

        return actual >= required;
    }

    /// <summary>
    /// 比较两个级别：返回 &gt;0 表示 levelA &gt; levelB，=0 表示相等，&lt;0 表示 levelA &lt; levelB。
    /// </summary>
    public static int CompareLevels(string levelA, string levelB) =>
        GetLevelOrder(levelA) - GetLevelOrder(levelB);
}
