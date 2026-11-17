using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
  private bool hiddenInfo = false;

  [SerializeField] private GameObject nightPhaseRow;
  [SerializeField] private RawImage editionImg;

  [Header("Player Statistics")]
  [SerializeField] private TMP_Text playerCountText;
  [SerializeField] private TMP_Text alivePlayerCountText;
  [SerializeField] private TMP_Text deadVoteCountText;

  [SerializeField] private TMP_Text townsfolkCountText;
  [SerializeField] private TMP_Text townsfolkExtraText;
  [SerializeField] private TMP_Text outsiderCountText;
  [SerializeField] private TMP_Text outsiderExtraText;
  [SerializeField] private TMP_Text minionCountText;
  [SerializeField] private TMP_Text minionExtraText;
  [SerializeField] private TMP_Text demonCountText;
  [SerializeField] private TMP_Text demonExtraText;
  [SerializeField] private TMP_Text travellerCountText;
  [SerializeField] private TMP_Text travellerExtraText;
  [SerializeField] private GameObject travellerCountObj;

  public void SetNightPhaseState(bool state)
  {
    if(nightPhaseRow != null) nightPhaseRow.SetActive(state);
  }

  public void SetInfoVisibility(bool state)
  {
    hiddenInfo = !state;
    if(townsfolkExtraText != null)
    {
      townsfolkExtraText.gameObject.SetActive(!hiddenInfo && townsfolkExtraText.text.Length != 0);
    }
    if(outsiderExtraText != null)
    {
      outsiderExtraText.gameObject.SetActive(!hiddenInfo && outsiderExtraText.text.Length != 0);
    }
    if(minionExtraText != null)
    {
      minionExtraText.gameObject.SetActive(!hiddenInfo && minionExtraText.text.Length != 0);
    }
    if(demonExtraText != null)
    {
      demonExtraText.gameObject.SetActive(!hiddenInfo && demonExtraText.text.Length != 0);
    }
    if(travellerExtraText != null)
    {
      travellerExtraText.gameObject.SetActive(!hiddenInfo && travellerExtraText.text.Length != 0);
    }
  }

  public void SetPlayersCount(int playerCount, int deadCount, int deadVoteCount,
    Dictionary<string, int> teamsCountDict, Dictionary<string, List<(string, int)>> teamsCountModDict)
  {
    if(playerCountText != null) playerCountText.text = playerCount.ToString();
    if(alivePlayerCountText != null) alivePlayerCountText.text = (playerCount - deadCount).ToString();
    if(deadVoteCountText != null) deadVoteCountText.text = deadVoteCount.ToString();

    if(townsfolkCountText != null) townsfolkCountText.text =
        teamsCountDict.GetValueOrDefault("townsfolk", 0).ToString();
    if(townsfolkExtraText != null)
    {
      var mods = teamsCountModDict.GetValueOrDefault("townsfolk", null);
      townsfolkExtraText.text = GetModsText(mods);
      townsfolkExtraText.gameObject.SetActive(!hiddenInfo && mods != null && mods.Count > 0);
    }

    if(outsiderCountText != null) outsiderCountText.text =
        teamsCountDict.GetValueOrDefault("outsider", 0).ToString();
    if(outsiderExtraText != null)
    {
      var mods = teamsCountModDict.GetValueOrDefault("outsider", null);
      outsiderExtraText.text = GetModsText(mods);
      outsiderExtraText.gameObject.SetActive(!hiddenInfo && mods != null && mods.Count > 0);
    }

    if(minionCountText != null) minionCountText.text =
        teamsCountDict.GetValueOrDefault("minion", 0).ToString();
    if(minionExtraText != null)
    {
      var mods = teamsCountModDict.GetValueOrDefault("minion", null);
      minionExtraText.text = GetModsText(mods);
      minionExtraText.gameObject.SetActive(!hiddenInfo && mods != null && mods.Count > 0);
    }

    if(demonCountText != null) demonCountText.text =
        teamsCountDict.GetValueOrDefault("demon", 0).ToString();
    if(demonExtraText != null)
    {
      var mods = teamsCountModDict.GetValueOrDefault("demon", null);
      demonExtraText.text = GetModsText(mods);
      demonExtraText.gameObject.SetActive(!hiddenInfo && mods != null && mods.Count > 0);
    }

    int travellerCount = teamsCountDict.GetValueOrDefault("traveler", 0);
    if(travellerCountObj != null) travellerCountObj.SetActive(travellerCount > 0);

    if(travellerCountText != null) travellerCountText.text =
        travellerCount.ToString();
    if(travellerExtraText != null)
    {
      var mods = teamsCountModDict.GetValueOrDefault("traveller", null);
      travellerExtraText.text = GetModsText(mods);
      travellerExtraText.gameObject.SetActive(!hiddenInfo && mods != null && mods.Count > 0);
    }
  }

  private string GetModsText(List<(string, int)> mods)
  {
    if(mods == null || mods.Count == 0) return "";

    string r = "";
    foreach (var mod in mods)
    {
      if(mod.Item1 == "set") continue;
      r += mod.Item1 == "add" ? "+" : (mod.Item1 == "sub" ? "-" : (mod.Item1 == "aos" ? "+-" : mod.Item1));
      r += mod.Item2.ToSafeString() + ";";
    }
    int setCount = mods.Count(x => x.Item1 == "set");
    foreach(var mod in mods)
    {
      if(mod.Item1 != "set") continue;
      if (r.Length == 0 && setCount == 1)
      {
        r += mod.Item2.ToSafeString() + ";";
        break;
      }

      r += "set " + mod.Item2.ToSafeString() + ";";
    }

    if (r.Length > 0)
    {
      r = r.Substring(0, r.Length - 1);
    }

    return " (" + r + ")";
  }


  public void ChangeEditionImg(Texture2D img)
  {
    if(editionImg == null || img == null || img.height <= 0) return;

    AspectRatioFitter fitter = editionImg.gameObject.GetComponent<AspectRatioFitter>();
    if(fitter == null) return;

    float aspect = (float)img.width / img.height;
    fitter.aspectRatio = aspect;

    editionImg.texture = img;
  }
}