# Img to ASCII API
The converter class takes the image and turns it into a Bitmap instance then goes through each pixel using GetPixel() and turns it into grayscale (r + g + b / 3) <br/>
As a grayscale for ASCII I used [this](http://paulbourke.net/dataformats/asciiart/) " .:-=+*#%@" after every pixel is turned to character representation of its greyness <br/> 
Normal height to width adjustment is made because text line height is usually larger than width. Finally it takes the expected output values and resizes the image according to that <br/> 
while still keeping the original width to height ratio intact.

## Endpoint

`POST /img`

It reads from form data which has to have the image file, width and height. I used on the frontend FormData() class.
Returns picture converted to ASCII art with the expected dimensions as List<string> where every object in the collection is a row in the picture.
