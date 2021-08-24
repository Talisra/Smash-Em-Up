using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PerksDialog : WarningDialog
{
    public List<PerkPanel> panels = new List<PerkPanel>();

    public void AddPerksToShow(List<Perk> perks)
    {
        for (int i = 0; i < perks.Count; i++)
        {
            panels[i].InitPerk(perks[i]);
            panels[i].gameObject.SetActive(true);
            if (i >= panels.Count-1)
                return;
        }
        foreach(PerkPanel p in panels)
        {
            if (p.perk == null)
            {
                p.gameObject.SetActive(false);
            }
        }
    }

    private void Clear()
    {
        foreach(PerkPanel panel in panels)
        {
            panel.gameObject.SetActive(false);
        }
        PerksManager.Instance.perksToShow.Clear();
    }

    public void Call()
    {
        this.gameObject.SetActive(true);
    }
    public override void OK()
    {
        Clear();
        GameProfile.Instance.ShowTutorialForPlayer(1);
        this.gameObject.SetActive(false);
    }
}
