using System;
using Color = Xamarin.Forms.Color;

namespace ScrollyFun.Helpers {
    public static class ColorHelpers {
        /// <summary>
        /// Convert HSV/HSB values to a System.Drawing.Color (based on RGB, with some HSL mismatch thrown in).
        /// </summary>
        /// <param name="h">Hue: [0..360)</param>
        /// <param name="s">Saturation: [0..1]</param>
        /// <param name="v">Value/Brightness: [0..1]</param>
        public static Color HsvToRgb(int h, float s, float v) {
            var hue = h;
            var saturation = s;
            var brightness = v;

            var r = brightness;
            var g = brightness;
            var b = brightness;

            // If saturation is 0, all colors are the same (some flavor of gray).
            if (saturation != 0) {
                // Calculate appropriate sector of a 6-part color wheel.
                var sectorPosition = hue / 60f;
                var sectorNumber = (int)Math.Floor(sectorPosition);

                // Fractional part of the sector (how many degrees into sector).
                var fractionalSector = sectorPosition - sectorNumber;

                // Calculate values on the three axes of the color.
                var p = brightness * (1 - saturation);
                var q = brightness * (1 - (saturation * fractionalSector));
                var t = brightness * (1 - (saturation * (1 - fractionalSector)));

                // Assign fractional colors to red, green, and blue based on angle's sector.
                switch (sectorNumber) {
                case 0:
                case 6:
                    r = brightness;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = brightness;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = brightness;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = brightness;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = brightness;
                    break;
                case 5:
                    r = brightness;
                    g = p;
                    b = q;
                    break;
                }
            }

            r *= 255;
            g *= 255;
            b *= 255;

            return Color.FromRgb((int)r, (int)g, (int)b);
        }

        public static Color GetRandomColor(Random rand) {
            int hue = rand.Next(360 - 1);
            return HsvToRgb(hue, 1, 1);
        }
    }
}