using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class JsonLoader
{
  public static string[] ParseEdition(string json)
  {
    JArray array = JArray.Parse(json);

    if(array.Count == 0) return null;

    if(array[0]["id"] == null     || array[0]["id"].Type != JTokenType.String ||
       array[0]["id"].ToString() != "_meta" ||
       array[0]["name"] == null   || array[0]["name"].Type != JTokenType.String ||
       array[0]["author"] == null || array[0]["author"].Type != JTokenType.String)
      throw new JsonException("Missing or incorrect metadata.");

    List<string> strings = new List<string>();

    strings.Add(array[0]["name"].ToString());
    strings.Add(array[0]["author"].ToString());

    // Skip the first element (metadata)
    for(int i = 1; i < array.Count; i++)
    {
      if(array[i].Type != JTokenType.String)
        throw new JsonException(
            $"Expected string at index {i}"
        );

      strings.Add(array[i].Value<string>());
    }

    return strings.ToArray();
  }
}
