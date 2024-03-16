using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CPHInline
{
	public bool Execute()
	{
		/* 
		 * Yu-Gi-Oh! Card Addon by CareFreeBomb
		 * For DJ_Teo's Streamer Printer Tools
		 * Idea from and created for KatLink
		 * https://github.com/lucasgerrits/stream-tools-and-widgets
		 * @version 2024-02-28
		 */
		
		
		string user = args["user"].ToString();
		
		using (HttpClient client = new HttpClient())
		{
			try
			{
				string apiUrl = "https://db.ygoprodeck.com/api/v7/cardinfo.php";
				HttpResponseMessage response = client.GetAsync(apiUrl).GetAwaiter().GetResult();
				
				if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string synchronously
                    string jsonStr = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    
					// Parse result string
                    JObject jsonObj = JObject.Parse(jsonStr);
                    JArray cardData = (JArray)jsonObj.GetValue("data");
                    
                    // Draw random card
                    Random rand = new Random();
                    int randIndex = rand.Next(0, cardData.Count);
                    JObject randCard = (JObject)cardData[randIndex];
                    
                    // Pull card info from JSON
                    string cardName = (string)randCard["name"];
                    string cardDesc = (string)randCard["desc"];
                    JArray cardImages = (JArray)randCard.GetValue("card_images");
                    JObject firstCardImage = (JObject)cardImages[0];
                    string cardImg = (string)firstCardImage["image_url"];
                    
                    string htmlStr = $@"
						<div>
							<div class='profileImage'>
								<img src='{cardImg}'>
							</div>
							<h3>{cardName}</h3>
							<h5>{cardDesc}</h5>
							<h5>Yu-Gi-Oh!™ CARD FOR:</h5>
							<h3>{user}</h3>
						</div>
					";
                    
                    // Set Streamer.bot Args
                    CPH.SetArgument("html", htmlStr);
                    
                    // Sending to Discord Webhook
                    bool sendToDiscord = bool.Parse(args["sendToDiscord"].ToString());
                    string discordWebhookUrl = args["discordWebhookUrl"].ToString();
                    
                    if (sendToDiscord == true) {
                    	string hookUser = "Kaiba Corp";
                    	string hookAvatar = "https://static.wikia.nocookie.net/fictionalcompanies/images/f/fc/KaibaCorp.jpg";
                    	string content = $"Yu-Gi-Oh! card for {user} \n [{cardName}]({cardImg})";
						CPH.DiscordPostTextToWebhook(discordWebhookUrl, content, hookUser, hookAvatar, false);
                    }
                }
                else
                {
                    CPH.LogInfo($"Failed with code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                CPH.LogInfo($"Error: {ex.Message}");
            }
		}
		
		return true;
	}
}