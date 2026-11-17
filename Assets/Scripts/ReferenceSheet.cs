using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReferenceSheet : MonoBehaviour
{
  [SerializeField] TMP_Text scriptName;
  [SerializeField] TMP_Text scriptAuthor;

  [SerializeField] GameObject normalCharacters;
  [SerializeField] GameObject travellersCharacters;

  [SerializeField] List<RawImage> townsfolksIcons = new();
  [SerializeField] List<TMP_Text> townsfolksNames = new();
  [SerializeField] List<TMP_Text> townsfolksAbilities = new();

  [SerializeField] List<RawImage> outsidersIcons = new();
  [SerializeField] List<TMP_Text> outsidersNames = new();
  [SerializeField] List<TMP_Text> outsidersAbilities = new();

  [SerializeField] List<RawImage> minionsIcons = new();
  [SerializeField] List<TMP_Text> minionsNames = new();
  [SerializeField] List<TMP_Text> minionsAbilities = new();

  [SerializeField] List<RawImage> demonsIcons = new();
  [SerializeField] List<TMP_Text> demonsNames = new();
  [SerializeField] List<TMP_Text> demonsAbilities = new();

  [SerializeField] List<RawImage> travellersIcons = new();
  [SerializeField] List<TMP_Text> travellersNames = new();
  [SerializeField] List<TMP_Text> travellersAbilities = new();


  public void Init(string name, string author, Dictionary<string, List<string>> teams, bool travellers = false)
  {
    HideAllRoles();

    if(scriptName != null) scriptName.text = name;
    if(scriptAuthor != null) scriptAuthor.text = "by " + author;

    normalCharacters?.SetActive(!travellers);
    travellersCharacters?.SetActive(travellers);

    foreach (var t in teams)
    {
      for (int i = 0; i < t.Value.Count; ++i)
      {
        var r = t.Value[i];

        var roleImg = TokensManager.Instance.GetRoleImage(r);

        var roleData = TokensManager.Instance.GetRoleData(r);
        string rename = TokensManager.Instance.GetRename(r);
        string roleName = roleData?.name ?? "";
        string roleAbility = roleData?.ability ?? "";

        if(t.Key == "townsfolk")
        {
          SetTownsfolkInfo(i, roleImg, roleName, roleAbility, rename);
        }
        else if(t.Key == "outsider")
        {
          SetOutsiderInfo(i, roleImg, roleName, roleAbility, rename);
        }
        else if(t.Key == "minion")
        {
          SetMinionInfo(i, roleImg, roleName, roleAbility, rename);
        }
        else if(t.Key == "demon")
        {
          SetDemonInfo(i, roleImg, roleName, roleAbility, rename);
        }
        else if(t.Key == "traveler")
        {
          SetTravellerInfo(i, roleImg, roleName, roleAbility, rename);
        }
      }
    }
  }

  private void HideAllRoles()
  {
    foreach (var o in townsfolksIcons)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in townsfolksNames)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in townsfolksAbilities)
    {
      o.gameObject.SetActive(false);
    }

    foreach(var o in outsidersIcons)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in outsidersNames)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in outsidersAbilities)
    {
      o.gameObject.SetActive(false);
    }

    foreach(var o in minionsIcons)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in minionsNames)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in minionsAbilities)
    {
      o.gameObject.SetActive(false);
    }

    foreach(var o in demonsIcons)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in demonsNames)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in demonsAbilities)
    {
      o.gameObject.SetActive(false);
    }

    foreach(var o in travellersIcons)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in travellersNames)
    {
      o.gameObject.SetActive(false);
    }
    foreach(var o in travellersAbilities)
    {
      o.gameObject.SetActive(false);
    }
  }

  private void SetTownsfolkInfo(int i, Texture2D img, string name, string ability, string rename = null)
  {
    if(img != null && i < townsfolksIcons.Count && townsfolksIcons[i] != null)
    {
      townsfolksIcons[i].gameObject.SetActive(true);
      townsfolksIcons[i].texture = img;
    }
    if(i < townsfolksNames.Count && townsfolksNames[i] != null)
    {
      townsfolksNames[i].gameObject.SetActive(true);
      townsfolksNames[i].text = rename != null ? rename + " (" + name + ")" : name;
    }
    if(i < townsfolksAbilities.Count && townsfolksAbilities[i] != null)
    {
      townsfolksAbilities[i].gameObject.SetActive(true);
      townsfolksAbilities[i].text = ability;
    }
  }

  private void SetOutsiderInfo(int i, Texture2D img, string name, string ability, string rename = null)
  {
    if(img != null && i < outsidersIcons.Count && outsidersIcons[i] != null)
    {
      outsidersIcons[i].gameObject.SetActive(true);
      outsidersIcons[i].texture = img;
    }
    if((name?? "") != "" && i < outsidersNames.Count && outsidersNames[i] != null)
    {
      outsidersNames[i].gameObject.SetActive(true);
      outsidersNames[i].text = rename != null ? rename + " (" + name + ")" : name;
    }
    if((ability ?? "") != "" && i < outsidersAbilities.Count && outsidersAbilities[i] != null)
    {
      outsidersAbilities[i].gameObject.SetActive(true);
      outsidersAbilities[i].text = ability;
    }
  }

  private void SetMinionInfo(int i, Texture2D img, string name, string ability, string rename = null)
  {
    if(img != null && i < minionsIcons.Count && minionsIcons[i] != null)
    {
      minionsIcons[i].gameObject.SetActive(true);
      minionsIcons[i].texture = img;
    }
    if((name ?? "") != "" && i < minionsNames.Count && minionsNames[i] != null)
    {
      minionsNames[i].gameObject.SetActive(true);
      minionsNames[i].text = rename != null ? rename + " (" + name + ")" : name;
    }
    if((ability ?? "") != "" && i < minionsAbilities.Count && minionsAbilities[i] != null)
    {
      minionsAbilities[i].gameObject.SetActive(true);
      minionsAbilities[i].text = ability;
    }
  }

  private void SetDemonInfo(int i, Texture2D img, string name, string ability, string rename = null)
  {
    if(img != null && i < demonsIcons.Count && demonsIcons[i] != null)
    {
      demonsIcons[i].gameObject.SetActive(true);
      demonsIcons[i].texture = img;
    }
    if((name ?? "") != "" && i < demonsNames.Count && demonsNames[i] != null)
    {
      demonsNames[i].gameObject.SetActive(true);
      demonsNames[i].text = rename != null ? rename + " (" + name + ")" : name;
    }
    if((ability ?? "") != "" && i < demonsAbilities.Count && demonsAbilities[i] != null)
    {
      demonsAbilities[i].gameObject.SetActive(true);
      demonsAbilities[i].text = ability;
    }
  }

  private void SetTravellerInfo(int i, Texture2D img, string name, string ability, string rename = null)
  {
    if(img != null && i < travellersIcons.Count && travellersIcons[i] != null)
    {
      travellersIcons[i].gameObject.SetActive(true);
      travellersIcons[i].texture = img;
    }
    if((name ?? "") != "" && i < travellersNames.Count && travellersNames[i] != null)
    {
      travellersNames[i].gameObject.SetActive(true);
      travellersNames[i].text = rename != null ? rename + " (" + name + ")" : name;
    }
    if((ability ?? "") != "" && i < travellersAbilities.Count && travellersAbilities[i] != null)
    {
      travellersAbilities[i].gameObject.SetActive(true);
      travellersAbilities[i].text = ability;
    }
  }
}