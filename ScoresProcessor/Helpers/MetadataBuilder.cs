using Newtonsoft.Json;

namespace ScoresProcessor.Helpers;

public class MetadataBuilder(ScoresConfig config)
{
  public string GenerateMetadataFor(IEnumerable<Result> results)
  {
    var metadata = results.Select(item => new
    {
      Name = item.ScoreName,
      item.Mscz,
      Scores = item.ScoreImages,
    });
    return JsonConvert.SerializeObject(metadata, Formatting.Indented);
  }

  /// <summary>
  /// Generates and exports the metadata corresponding to the data in <paramref name="results"/>.
  /// </summary>
  /// 
  public void ExportMetadata(IEnumerable<Result> results)
  {
    string metadata = GenerateMetadataFor(results);
    string filePath = Path.Combine(config.TargetFolder, "score-metadata.json");
    File.WriteAllText(filePath, metadata);
  }
}
