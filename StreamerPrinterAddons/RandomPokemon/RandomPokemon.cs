using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

/* 
 * Random Pokémon Addon by CareFreeBomb
 * For DJ_Teo's Streamer Printer Tools
 * Created for KatLink
 * Consumes: https://pokeapi.co/
 * https://github.com/lucasgerrits/stream-tools-and-widgets
 * @version 2024-03-16
 */
public class CPHInline
{
	public class Pokemon {
		public int Num { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public string Genus { get; set; }
		public string Sprite { get; set; }
		public string Type1 { get; set; }
		public string Type2 { get; set; }
		public string TypeString { get; set; }
	}
	
	/*
	 * The localized flavor text is left unprocessed as it is found in game files.
	 * This means that it contains special characters that one might want to replace.
	 *
	 * Page breaks are treated just like newlines.
     * Soft hyphens followed by newlines vanish.
     * Letter-hyphen-newline becomes letter-hyphen, to preserve real hyphenation.
     * Any other newline becomes a space.
     *
     * https://github.com/veekun/pokedex/issues/218#issuecomment-339841781
	 */
	public string RemoveGameFormatting(string flavorText) {
		flavorText = flavorText.Replace("\f", "\n")
			.Replace("\u00ad\n", "")
			.Replace("\u00ad", "")
			.Replace(" -\n", " - ")
			.Replace("-\n", "-")
			.Replace("\n", " ");
		return flavorText;
	}
	
	public bool Execute()
	{
		// Get SBot args
		string user = args["user"].ToString();
		
		string allowedGens = args["pokeGens"].ToString();
		allowedGens = allowedGens.Replace(" ", "");
		int[] gens = allowedGens.Split(',').Select(int.Parse).ToArray();
		
		// Pick random allowed gen
		Random rand = new Random();
		int randGenIndex = rand.Next(0, gens.Length);
		int randGen = gens[randGenIndex];
		
		// Get random gen's National Dex range
		int[][] validRanges = {
			new int[] {1, 151}, // Kanto
			new int[] {152, 251}, // Johto
			new int[] {252, 386}, // Hoenn
			new int[] {387, 493}, // Sinnoh
			new int[] {494, 649}, // Unova
			new int[] {650, 721}, // Kalos
			new int[] {722, 809}, // Alola
			new int[] {810, 905}, // Galar/Hisui
			new int[] {906, 1025} // Paldea
		};
		
		// Back to 0-based
		int[] selectedRange = validRanges[randGen - 1];
		
		// The chosen Pokedude
		Pokemon poke = new Pokemon();
		poke.Num = rand.Next(selectedRange[0], selectedRange[1] + 1);
		
		// Call the API
		using (HttpClient client = new HttpClient()) {
			try {
				// Perform the FIRST HTTP GET request synchronously
				string url = "https://pokeapi.co/api/v2/pokemon/" + poke.Num;
				HttpResponseMessage response = client.GetAsync(url).Result;
				
				// Check if the request was successful (Status code 200)
				if (response.IsSuccessStatusCode)
				{
					// Read the response content synchronously
					string responseBody = response.Content.ReadAsStringAsync().Result;
					JObject data = JObject.Parse(responseBody);
					
					// Set usable (non-display) name
					poke.Name = data.GetValue("name").ToString();
					
					// Set sprites
					JObject sprites = (JObject)data.GetValue("sprites");
					poke.Sprite = sprites.GetValue("front_default").ToString();
					
					// Set poke types
					JArray typeArray = (JArray)data["types"];
					JObject type1 = (JObject)typeArray[0]["type"];
					poke.Type1 = type1["name"].ToString();
					if (typeArray.Count > 1) {
						JObject type2 = (JObject)typeArray[1]["type"];
						poke.Type2 = type2["name"].ToString();
						poke.TypeString = char.ToUpper(poke.Type1[0]) + poke.Type1.Substring(1) +
							" / " + char.ToUpper(poke.Type2[0]) + poke.Type2.Substring(1) + " Type";
					} else {
						poke.Type2 = "";
						poke.TypeString = char.ToUpper(poke.Type1[0]) + poke.Type1.Substring(1) + " Type";
					}
				}
				else
				{
					CPH.LogInfo($"Failed to fetch data from v2/pokemon/. Status code: {response.StatusCode}");
				}
				
				// Perform the SECOND HTTP GET request synchronously
				url = "https://pokeapi.co/api/v2/pokemon-species/" + poke.Num;
				response = client.GetAsync(url).Result;
				
				// Check if the next request was successful
				if (response.IsSuccessStatusCode)
				{
					// Read the response content synchronously
					string responseBody = response.Content.ReadAsStringAsync().Result;
					JObject data = JObject.Parse(responseBody);
					
					// Iterate through to find the English genus
					JArray generaArray = (JArray)data["genera"];
					foreach (JObject item in generaArray) {
						JObject language = (JObject)item["language"];
						if (language["name"].ToString() == "en") {
							poke.Genus = item["genus"].ToString();
							break;
						}
					}
					
					// Iterate through to find the English display name
					JArray namesArray = (JArray)data["names"];
					foreach (JObject item in namesArray) {
						JObject language = (JObject)item["language"];
						if (language["name"].ToString() == "en") {
							poke.DisplayName = RemoveGameFormatting(item["name"].ToString());
							break;
						}
					}
					
					// Iterate through to find the English flavor text
					JArray flavorArray = (JArray)data["flavor_text_entries"];
					if (flavorArray.Count > 0) {
						foreach (JObject item in flavorArray) {
							JObject language = (JObject)item["language"];
							if (language["name"].ToString() == "en") {
								poke.Description = RemoveGameFormatting(item["flavor_text"].ToString());
								break;
							}
						}
					} else {
						poke.Description = "No Pokédex data available.";
					}
				}
				else
				{
					CPH.LogInfo($"Failed to fetch data from v2/pokemon-species/. Status code: {response.StatusCode}");
				}
				
				// Create HTML string
				string htmlStr = $@"
					<div>
						<h5>Random Pokémon For: {user}</h5>
						<h3>#{poke.Num}: {poke.DisplayName}</h3>
						<div class='profileImage'>
							<img src='{poke.Sprite}'>
						</div>
						<h5 style='font-style: italic'>{poke.Genus}</h5>
						<h5>{poke.TypeString}</h5>
						<h5>{poke.Description}</h5>
					</div>
				";
				
				// Set Streamer.bot Args
				CPH.SetArgument("html", htmlStr);
				
				// Sending to Discord Webhook
				bool sendToDiscord = bool.Parse(args["sendToDiscord"].ToString());
				string discordWebhookUrl = args["discordWebhookUrl"].ToString();
				
				if (sendToDiscord == true) {
					string hookUser = "Kat's Pokédex";
					string hookAvatar = "https://raw.githubusercontent.com/lucasgerrits/stream-tools-and-widgets/" +
						"master/StreamerPrinterAddons/RandomPokemon/assets/pokedex.png";
					string content = $"Random Pokémon for **{user}**[:]({poke.Sprite})\n" +
						$"[**#{poke.Num}: {poke.DisplayName}**]" +
						$"(<https://www.serebii.net/pokemon/{poke.Name}>)\n" +
						$"{poke.TypeString}\n*{poke.Genus}*\n```{poke.Description}```";
					CPH.DiscordPostTextToWebhook(discordWebhookUrl, content, hookUser, hookAvatar, false);
				}
			} catch (Exception ex){
				CPH.LogError($"An error occurred: {ex.Message}");
			}
		}
		return true;
	}
}