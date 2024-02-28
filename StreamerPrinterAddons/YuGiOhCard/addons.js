/*------------------------------------------------------------------------------- */

/**
 * Yu-Gi-Oh! Card Addon by CareFreeBomb 
 * for DJTeo's Streamer Printer Tools
 * (Bother Me Not Him)
 * https://github.com/lucasgerrits/stream-tools-and-widgets
 * @version 2024-02-28
 */
function yugiohCardAddon() {
    this.addonName = "yugioh_card";

    /* returns true if you use this template */
    this.useThisAddon = function(streamerBotArgs) {
        return ("addon" in streamerBotArgs) && (streamerBotArgs['addon'] == this.addonName);
    }

	this.template = function(streamerBotArgs) {
        return "yugioh_card_template";
    }

    this.getCard = function() {
        // Uses the free Yu-Gi-Oh! Card Database API: https://ygoprodeck.com/api-guide/
        const yugiohURL = "https://db.ygoprodeck.com/api/v7/cardinfo.php";
        const xhr = new XMLHttpRequest();
        xhr.open('GET', yugiohURL, false); // synchronous
        xhr.send();
        if (xhr.status >= 200 && xhr.status < 300) {
            const response = JSON.parse(xhr.responseText);
            const allCards = response.data;
            const rand = Math.floor(Math.random() * allCards.length);
            const randCard = allCards[rand];
            const card = {
                name: randCard.name,
                img: randCard.card_images[0].image_url,
                desc: randCard.desc,
                race: randCard.race,
                type: randCard.type,
                level: randCard.level,
                attr: randCard.attribute,
            };
            return card;
        } else {
            console.error('Request failed with status:', xhr.status);
        }
    }

    this.addTemplateData = function (streamerBotArgs, templateVars) {
        const card = this.getCard();
        templateVars['cardName'] = card.name;
        templateVars['cardImg'] = card.img;
        templateVars['cardDesc'] = card.desc;
    }

    this.modifyTemplateHtml = function (streamerbotArgs, templateHtml) {
        templateHtml = templateHtml + "";
        return templateHtml;
    }
}

registerAddon(new yugiohCardAddon());