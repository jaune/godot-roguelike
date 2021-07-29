using Godot;
using System.Text.Json;

public static class JsonDocumentLoader {
  public static JsonDocument Load (string path) {
  var file = new File();

  if (file.Open(path, File.ModeFlags.Read) != Error.Ok) {
    throw new System.Exception($"Unable to read {path}");
  }

  var data = file.GetAsText();
  file.Close();

  return JsonDocument.Parse(data) ?? throw new System.Exception();
  }
}
