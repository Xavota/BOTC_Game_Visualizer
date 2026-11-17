using System.Collections.Generic;
using UnityEngine;

public class ReferenceSheetViewer : MonoBehaviour
{
  public event TokensManager.VoidDelegate OnClosed;
  public event TokensManager.VoidDelegate OnDownload;

  [SerializeField] private ReferenceSheet normalCharacters;
  [SerializeField] private ReferenceSheet travelerCharacters;

  public void Init(string name, string author, Dictionary<string, List<string>> teams)
  {
    normalCharacters?.Init(name, author, teams, false);
    travelerCharacters?.Init(name, author, teams, true);
  }

  public void Close()
  {
    OnClosed?.Invoke();
  }

  public void Download()
  {
    OnDownload?.Invoke();
  }
}