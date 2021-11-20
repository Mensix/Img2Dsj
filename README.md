# Img2Dsj

The program which generates Deluxe Ski Jump 4 v1.10.0 XML file from input image or customly defined text. Implementation is based on SkiaSharp and RichTextKit library. Compatible with .NET6, I am not sure if it can be build on Windows, as I have written it on WSL2 :)

## Usage

You need settings.json file, the model can be found in Img2Dsj.Models folder in Settings.cs folder.

Example:
```jsonc
{
  "drawText": [ // Draw white text Webdings with font Webdings and size set to 72
    {
      "text": "Webdings",
      "color": "#FFFFFF",
      "font": "Webdings",
      "size": 72
      
    }
  ],
  "scalingFactor": 5, // Scale input text 5x down
  "originDistance": { // Set center origin distance on 67 meter relative to the X-axis and on 0 meter relative to the Z-axis
    "x": 67,
    "z": 0
  },
  "includeTags": ["twigs"], // Generate twigs
  "pixelSize": 0.05, // Set marking size
  "useColor": "#FFFFFF" // Override all image colors to white, note twigs generation already includes this optimization by default
}
```

## License

MIT