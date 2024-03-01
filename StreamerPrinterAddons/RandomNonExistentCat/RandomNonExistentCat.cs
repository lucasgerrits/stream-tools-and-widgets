using System;

public class CPHInline
{
    public bool Execute()
    {
        /* 
         * Random Non-Existent Cat Addon by CareFreeBomb
         * For DJ_Teo's Streamer Printer Tools
         * Created for KatLink
         * Images used from https://thesecatsdonotexist.com/
         * which were generated using Nvidia StyleGAN 1 and 2
         * https://github.com/lucasgerrits/stream-tools-and-widgets
         * @version 2024-03-01
         */
        
        string user = args["user"].ToString();
        
        // Doubles image pool from 15k to 30k, but uses old model that produces 'weird' images
        bool useStyleGAN1 = (bool)args["useStyleGAN1"];
        
        // Determine folder number
        Random rand = new Random();
        int folderNumber;
        if (useStyleGAN1 == true) {
            folderNumber = rand.Next(1, 7); // Between 1 and 6 inclusive
        } else {
            folderNumber = rand.Next(4, 7); // Between 4 and 6 inclusive
        }
        
        // Determine pic number
        Random rand2 = new Random();
        int picNumber = rand2.Next(5000); // Between 0 and 4999 inclusive
        
        // Create image url
        string baseUrl = "https://d2ph5fj80uercy.cloudfront.net";
        string imgUrl = $"{baseUrl}/0{folderNumber}/cat{picNumber}.jpg";
        
        // Choosing what string to output
        string output = "";
        if (args.ContainsKey("bits")) {
            string bits = args["bits"].ToString();
            output = $"{bits} bits were cheered for this cat to not exist by:";
        } else {
            output = $"This cat does not exist for:";
        }
        
        // Create HTML string for printer template
        string htmlStr = $@"
            <div class='profileImage'>
                <img src='{imgUrl}'>
            </div>
            <h5>{output}</h5>
            <h3>{user}</h3>
        ";
        
        // Set Streamer.bot Args
        CPH.SetArgument("html", htmlStr);
        
        // Sending to Discord Webhook
        bool sendToDiscord = bool.Parse(args["sendToDiscord"].ToString());
        string discordWebhookUrl = args["discordWebhookUrl"].ToString();
        
        if (sendToDiscord == true) {
            string hookUser = "Cat Factory";
            string hookAvatar = imgUrl;
            string content = $"{output} **{user}**[.]({imgUrl})";
            CPH.DiscordPostTextToWebhook(discordWebhookUrl, content, hookUser, hookAvatar, false);
        }
        
        return true;
    }
}