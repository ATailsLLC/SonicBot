//-----------------------
//The name of our chatbot
//-----------------------
$chatBot::name = "Sonic the Hedgehog";
 
//-------------------------------------------------------------
//Our Chatbot's Responses, anything that uses multiple
//Words should instead use an underscore "_" instead of a space
//Users can constantly add new keywords to respond to
//-------------------------------------------------------------
$chatBot::response["Hi"] = "Wazzap!";
$chatBot::response["Lol"] = "Er,_i_dont_get_whats_so_funny..." ;
$chatBot::responce["Whats your name"] = "Sonic,_Sonic_the_Hedgehog!";
$chatBot::responce["sthhelp"] = "SBHelp:_Hi,Lol,_Whats_your_name";
$chatBot::responce[""] = "";
 
//--------------------------------------------------------
//Number of milliseconds that our chatbot will not respond
//After having responded to any phrase at all
//--------------------------------------------------------
$chatBot::timeOut = 5000;
 
 
package myFirstChatBot
{
        //---------------------------------------------------------
        //There are only a few parameters we need to focus on here
        //Which are %name and %msg
        //---------------------------------------------------------
       
        function clientCmdChatMessage(%a,%b,%c,%fmsg,%cp,%name,%cs,%msg)
        {
                parent::clientCmdChatMessage(%a,%b,%c,%fmsg,%cp,%name,%cs,%msg);
               
                //We want to ignore our name
                        //Strip things we dont want
                        %msg = stripMlControlChars(%msg);
                       
                        %hasTrigger = chatBot_messageHasTriggerWord(%msg);
                       
                        if(%hasTrigger !$= "0")
                        {
                                %phrase = chatBot_getRandomPhraseFromWord(%hasTrigger);
                                chatBot_say(%phrase);
                        }
                }
        }
};
activatePackage(myFirstChatBot);
 
//----------------------------------------------
//This function picks one of our random phrases
//And returns it so we can use it
//----------------------------------------------
 
function chatBot_getRandomPhraseFromWord(%word)
{
        %phrases = $chatBot::response[%word];
       
        if(%phrases !$= "")
        {
                %count = getWordCount(%phrases);
                %ranPhrase = getWord(%phrases,getRandom(0,%count));
                %ranPhrase = strReplace(%ranPhrase,"_"," ");
               
                return %ranPhrase;
        }
        else
                return false;
}
 
//---------------------------------------------------
//This function checks a string sent by another user
//And if it has one of our response triggers
//It'll return that word, otherwise it returns false
//---------------------------------------------------
 
function chatBot_messageHasTriggerWord(%string)
{
        %wordCount = getWordCount(%string);
       
        for(%i=0;%i<%wordCount;%i++)
        {
                %word = getWord(%string,%i);
               
                if($chatBot::response[%word] !$= "")
                {
                        return %word;
                }
        }
        return false;
}
 
//-------------------------------------------
//This function will attempt to say a string
//As the chat bot, and checks things like
//Flood protection triggering and spamming
//-------------------------------------------
 
function chatBot_say(%string)
{
        %time = getSimTime();
       
        if(%time - $chatBot::lastTalkTime >= $chatBot::timeOut)
        {
                if($chatBot::lastTalkMessage $= %string)
                {
                        error("Our chatbot will trigger the flood protection, returning false");
                        return false;
                }
                else
                {
                        commandToServer('messageSent',$chatBot::name @ " => " @ %string);
                        $chatBot::lastTalkMessage = %string;
                        $chatBot::lastTalkTime = %time;
                }
               
                return true;
        }
        return false;
}
