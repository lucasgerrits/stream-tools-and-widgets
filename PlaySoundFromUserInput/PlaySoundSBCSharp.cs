using System;
using System.IO;

public class CPHInline
{
	public bool Execute()
	{
		/* 
		 * CFB's Play Sound From User Input Action for Streamer.Bot
		 * Testing shoutouts to IamHadriel
		 * @author https://www.twitch.tv/carefreebomb
		 * @version 2023-05-07
		 */
		
		/* VARIABLE DECLARATION + INITIALIZATION */
		
		string message;
		string soundFolder = args["soundFolder"].ToString();
		if (soundFolder[soundFolder.Length-1] != '\\') { soundFolder += "\\"; }
		bool useBotAccount = Convert.ToBoolean(args["useBotAccount"]);
		bool wasRedemption = true;
		
		/* DETERMINE IF ACTION CALLED VIA REDEEM OR COMMAND */
		
		// Chat command
		if (args.ContainsKey("message")) { 
			message = args["message"].ToString();
			wasRedemption = false;
		// Channel point reward
		} else if (args.ContainsKey("rawInput")){ 
			message =  args["rawInputEscaped"].ToString();
		// Other
		} else {
			CPH.LogDebug("Arrived at the 'CFB Play Sound' Action via a method other than redeem or command?");
			return false;
		}
		
		/* CHECK IF FILE EXISTS */
		
		string fileName = soundFolder + message;
		if (File.Exists(fileName + ".mp3")) {
			fileName += ".mp3";
		} else if (File.Exists(fileName + ".wav")) {
			fileName += ".wav";
		} else if (File.Exists(fileName + ".m4a")) {
			fileName += ".m4a";
		} else if (File.Exists(fileName + ".ogg")) {
			fileName += ".ogg";
		} else {
			CPH.SendMessage(args["inputErrorMessage"] + " " + message);
			if (wasRedemption) {
				CPH.TwitchRedemptionCancel(args["rewardId"].ToString(), args["redemptionId"].ToString());
			}
			return false;
		}
		
		/* PLAY THE SOUND */
		
		bool finishBeforeContinuing = false;
		CPH.PlaySound(fileName, 1.0f, finishBeforeContinuing);
		
		if (wasRedemption) {
			CPH.TwitchRedemptionFulfill(args["rewardId"].ToString(), args["redemptionId"].ToString());
		}
		
		return true;
	}
}
