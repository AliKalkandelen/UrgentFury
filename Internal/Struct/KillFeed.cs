using UnityEngine;

public struct KillFeed  {
    public string m_Killer {get; set;}
    public string m_HowKill { get; set; }
    public string m_Killed { get; set; }
    public int m_GunID { get; set; }
    public Color m_KillerColor { get; set; }
    public Color m_KilledColor { get; set; }
    public float m_Timer { get; set; }

    public KillFeed(string t_Killer, string t_Killed,string t_HowKill, int t_GunID, Color t_DeltaC, Color t_ReconC, float t_Timer)
    {
        m_Killer = t_Killer;
        m_HowKill = t_HowKill;
        m_Killed = t_Killed;
        m_GunID = t_GunID;
        m_KillerColor = t_DeltaC;
        m_KilledColor = t_ReconC;
        m_Timer = t_Timer;
    }
}