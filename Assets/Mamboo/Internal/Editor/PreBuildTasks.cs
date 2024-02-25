#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PreBuildTasks : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        RunLunarTasks();
    }

    private void RunLunarTasks()
    {
#if lunar_debug_enabled
        LunarConsoleEditorInternal.Installer.EnablePlugin();
#else
        LunarConsoleEditorInternal.Installer.DisablePlugin();
#endif
    }
}
#endif
