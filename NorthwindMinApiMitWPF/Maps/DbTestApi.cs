namespace NorthwindMinApiMitWPF.Maps;
public static class DbTestApi
{
  public record struct OkMessage(bool IsOk, int Nr);
  public static IEndpointRouteBuilder MapDbTests(this IEndpointRouteBuilder routes)
  {
    routes.MapGet("/dbtest/Categories", (NORTHWNDContext db, ILoggerFactory logger) =>
    {
      int nr = db.Categories.Count();
      logger.Log($"{nr} products");
      return new OkMessage { IsOk = true, Nr = nr };
    });
    return routes;
  }
}