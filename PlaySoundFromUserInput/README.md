# Streamer.bot Action - Play Sound From User Input

This is a Streamer.bot Action import that allows you to create a folder of sounds on your PC named by what chat can type in to play said sounds via a text box in a channel point reward or as a parameter for a chat command (example: "!sfx freshpots" will play "freshpots.mp3" from the folder).

## Video Tutorial

[!["Let Your Twitch Viewers Play SFX On Your Stream" via Hadriel on YouTube](https://img.youtube.com/vi/Hje2hAqQSac/0.jpg)](https://www.youtube.com/watch?v=Hje2hAqQSac)

Thanks to [Hadriel](https://www.twitch.tv/iamhadriel) for this [wonderful video tutorial](https://www.youtube.com/watch?v=Hje2hAqQSac) on how to install and use this Streamer.bot widget (and also for helping me test it [live on his twitch channel](https://www.twitch.tv/iamhadriel)).

## Text Tutorial

### Initial Steps:
1. Create a folder where you will put all of your sound files.
2. Make sure they are named what will be typed into the redemption prompt and/or the chat command (example: `freshpots.mp3`)
3. Ensure that all filetypes are one of the following: .mp3, .wav, .m4a, .ogg

### To add the Action to Streamer.bot:
1. Click into the `PlaySoundStreamerBotAction.cfb` file here in the github repo.
2. Click the "Copy raw file" button.
3. In Streamer.bot, click the "Import" button at the top.
4. Paste the contents from Step 2 into the "Import String" box, then hit the "Import" button at the bottom.

### Configuring the Action:
1. Click into the freshly imported `Play Sound From User Input` Action in Streamer.bot.
2. Locate the Sub-Action for: `Set argument %soundFolder% to 'C:...'` and double click to edit.
3. Change the `Value` input to wherever your folder of sound files is located on your PC and hit "Ok".
4. [Optional] If you would like to change the error message that is displayed in chat when a not-found sound's name is entered, you can edit the next Sub-Action.
5. [Optional] The third Sub-Action can be edited to `False` if you want the error message to come from your broadcaster account as opposed to a bot's.

## Contact:
For questions, concerns, or to show thanks, you can find and follow me here:

[Twitter](https://twitter.com/carefreeb0mb)

[Twitch](https://www.twitch.tv/carefreebomb)

[Discord](https://discord.gg/0X84YV4Sn1v0wyUa)