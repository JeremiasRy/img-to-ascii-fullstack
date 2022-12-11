namespace backend;
using System.Drawing;

public static class Api
{
    public static void ConfigureApi(this WebApplication app)
    {
        app.MapPost("/img", PostImage);
    }
    private static async Task<IResult> PostImage(HttpRequest request)
    {
        if (!request.HasFormContentType)
        {
            return Results.BadRequest();
        }

        var form = await request.ReadFormAsync();
        IFormFile image = form.Files[0];

        if (image == null)
            return Results.BadRequest();

        await using Stream stream = image.OpenReadStream();
        StreamReader sr = new (stream);

        Bitmap img = Image.FromStream(stream) as Bitmap;

        return Results.Ok();
        
    }
}
