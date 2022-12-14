using System.Runtime.Versioning;

namespace backend;

public static class Api
{
    public static void ConfigureApi(this WebApplication app)
    {
        app.MapPost("/img", PostImage);
    }
    [SupportedOSPlatform("windows")]
    private static async Task<IResult> PostImage(HttpRequest request)
    {
        if (!request.HasFormContentType)
            return Results.BadRequest();

        var form = await request.ReadFormAsync();
        IFormFile image = form.Files[0];

        if (image == null)
            return Results.BadRequest();

        await using Stream stream = image.OpenReadStream();
        
        ImgToASCIIConverter converter = new(stream, int.Parse(form["width"]), int.Parse(form["height"]));
        var asciiImg = await converter.GetAsciiPicture();

        return Results.Ok(asciiImg);
    }
}
