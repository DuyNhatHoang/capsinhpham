using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Service.Utilities
{
    
    public static class FormFileImageProcessing
    {
        public static string ResizeAndEncodeToBase64(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    using (var image = Image.Load(stream))
                    {
                        // Resize the image to a maximum width of 450 pixels
                        var maxWidth = 450;
                        if (image.Width > maxWidth)
                        {
                         
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size((int)(maxWidth), (int)(maxWidth)),
                                Mode = ResizeMode.Max
                            }));
                        }
                        // Convert the image to base64
                        stream.Position = 0;
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            var base64String = Convert.ToBase64String(memoryStream.ToArray());
                            return base64String;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }

}
