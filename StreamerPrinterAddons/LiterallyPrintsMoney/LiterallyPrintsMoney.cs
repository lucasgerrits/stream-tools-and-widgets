using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.IO;

/* 
 * Money Printer Addon by CareFreeBomb
 * For DJ_Teo's Streamer Printer Tools
 * Created for KatLink
 * US Guidelines for Currency Image Use: https://www.uscurrency.gov/media/currency-image-use
 * Specimen Dollar Images From: https://www.uscurrency.gov/denominations/
 * Blank Dollar Template From: https://fundred.org/make/
 * https://github.com/lucasgerrits/stream-tools-and-widgets
 * @version 2024-03-16
 */
public class CPHInline
{
    private static string fontFile = "One_dance.ttf";
    private static string remoteFontUrl = "https://raw.githubusercontent.com/lucasgerrits/" + 
		"stream-tools-and-widgets/master/StreamerPrinterAddons/LiterallyPrintsMoney/assets/" + 
		"fonts/asset/One_dance.ttf";
	
    private static bool debug = true;
    private static string tempPath;
    private static string fontFileWithPath;
    
    private void Debug(string logLine)
    {
        if (CPHInline.debug == false) {
            CPH.LogInfo("CFB: " + logLine);
        }
    }

    private void SetPath()
    {
        string path = Directory.GetCurrentDirectory();
        string tempPath = path + "\\teotools\\streamerprinter\\temp\\";
        CPHInline.tempPath = tempPath;
        CPHInline.fontFileWithPath = tempPath + CPHInline.fontFile;
    }

    private void SaveRemoteFont()
    {
        string fontUrl = CPHInline.remoteFontUrl;
        using (HttpClient client = new HttpClient()) {
            // Download the font file as a byte array
            Debug("Before downloading font data");
            byte[] fontData = client.GetByteArrayAsync(fontUrl).GetAwaiter().GetResult();
            Debug("AFter downloading font data");
            // Write the font data to the temporary file
            File.WriteAllBytes(CPHInline.tempPath + CPHInline.fontFile, fontData);
        }
    }

    private int GetTextWidth(string text, Font font)
    {
        using (Bitmap bitmap = new Bitmap(1, 1))
        using (Graphics graphics = Graphics.FromImage(bitmap)) {
            SizeF size = graphics.MeasureString(text, font);
            return (int)Math.Ceiling(size.Width);
        }
    }

    private string CreateTextImageString(string text)
    {
        // Check for font in temp folder, else download it
        if (!File.Exists(CPHInline.fontFileWithPath)) {
            Debug("Font file does not exist");
            SaveRemoteFont();
        } else {
            Debug("Font file exists locally");
        }

        // Load the font from the file
        PrivateFontCollection privateFonts = new PrivateFontCollection();
        Debug(CPHInline.fontFileWithPath);
        privateFonts.AddFontFile(CPHInline.fontFileWithPath);
        int fontSize = 30; // Issues going above 30 with provided font
        Font font = new Font(privateFonts.Families[0], fontSize, FontStyle.Regular);
        
        // Create a bitmap to draw the text on
        Bitmap bitmap = new Bitmap(GetTextWidth(text, font), font.Height);
        // Bitmap bitmap = new Bitmap(300, 100);
        using (Graphics graphics = Graphics.FromImage(bitmap)) {
            graphics.Clear(Color.Transparent);
            
            graphics.DrawString(text, font, Brushes.Black, new PointF(1, 1));
        }

        // Convert the bitmap to base64
        string base64Image;
        using (MemoryStream memoryStream = new MemoryStream()) {
            bitmap.Save(memoryStream, ImageFormat.Png);
            byte[] imageBytes = memoryStream.ToArray();
            base64Image = Convert.ToBase64String(imageBytes);
        }

        // Dispose of resources
        font.Dispose();
        bitmap.Dispose();
        privateFonts.Dispose();
        return base64Image;
    }

    public string CreateHTMLString(string dollarAmount, string bits, string imageFile, string base64Str = "")
    {
        string htmlStr = $@"
			<style>
				img {{
					max-width: 950px;
				}}
				.rotate {{
					-webkit-transform: rotate(90deg);
					padding: 0px;
					margin: 0px;
				}}
				.bits {{
					position: absolute;
					margin: 0px;
					padding: 0px;
					text-align: center;
					width: 200px;
					height: 60px;
				}}
				.bits img {{
					max-width: 100%;
					height: 100%;
					border: 0px solid purple;
				}}
				#textTopLeft {{
					top: 11px;
					left: 67px;
					border: 0px solid red;
				}}
				#textTopRight {{
					top: 11px;
					left: 690px;
					border: 0px solid blue;
				}}
				#textBottomLeft {{
					top: 315px;
					left: 70px;
					width: 150px;
					border: 0px solid green;
				}}
				#textBottomRight {{
					top: 295px;
					left: 700px;
					height: 50px;
					border: 0px solid orange;
				}}
			</style>
			<div class='rotate'>
				<img src='{imageFile}' class=''>
		";
        if (dollarAmount == "0") {
            htmlStr += $@"
				<div class='bits' id='textTopLeft'><img src='data:image/png;base64,{base64Str}'></div>
				<div class='bits' id='textTopRight'><img src='data:image/png;base64,{base64Str}'></div>
				<div class='bits' id='textBottomLeft'><img src='data:image/png;base64,{base64Str}'></div>
				<div class='bits' id='textBottomRight'><img src='data:image/png;base64,{base64Str}'></div>
			";
        }
        htmlStr += $@"
			</div>
		";
        return htmlStr;
    }

    public bool Execute()
    {
        SetPath();
        
        // CUSTOMIZE THESE PATHS IF YOU WOULD LIKE TO HOST OR USE LOCAL FILES
        string usdPath = "https://raw.githubusercontent.com/lucasgerrits/stream-tools-and-widgets/" + 
			"master/StreamerPrinterAddons/LiterallyPrintsMoney/assets/usd/";
        
        // Get Sbot args
        string user = args["user"].ToString();
        string bits = "";
        if (args.ContainsKey("bits")) {
            bits = args["bits"].ToString();
        }

        string dollarAmount = args["dollarAmount"].ToString();
        string imageFile = "";
        if (dollarAmount == "0") {
            if (args["customImage"].ToString() != "") {
                imageFile = args["customImage"].ToString();
            }
        } else {
            imageFile = $@"{usdPath}{dollarAmount}.jpg";
        }

        // Create HTML string for printer template
        string htmlStr;
        
        // If using custom bill, create dollar amount image string
        if (dollarAmount == "0") {
            string base64Str = CreateTextImageString(bits);
            Debug(base64Str);
            htmlStr = CreateHTMLString(dollarAmount, bits, imageFile, base64Str);
        } else {
            htmlStr = CreateHTMLString(dollarAmount, bits, imageFile);
        }

        // Set Streamer.bot Args
        CPH.SetArgument("html", htmlStr);
        return true;
    }
}