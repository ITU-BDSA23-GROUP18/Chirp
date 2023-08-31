﻿
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using System.Text.RegularExpressions;

try
{
    using(var sr = new StreamReader("chirp_cli_db.csv")){
        

        //This regex is not the best and should be changes later on, or something else should take it's place
        Regex regex = new Regex("(?<author>.*?,)(?<message>\".*?\")(?<time>.*?$)");
        //we match the to the first "," that includes the ","
        //we match the message that is souranded by "" should be changed such that "" is not included
        //lastly we match for everything after the message, such that the time is matched with a "," at the start

        var split = sr.ReadToEnd().Split("\n");
        //i was uable to use the normale method of reading line by line therefor it is done in a skuffed way

        for(int i = 1; i<split.Length; i++)
        {
            
            var unhandledChirp = split[i];

            var regexMatch = regex.Match(unhandledChirp);
            if(!regexMatch.Success)
            {
                return;
            }

            var author = regexMatch.Groups["author"].ToString();
            author = author.Remove(4);

            var message = regexMatch.Groups["message"].ToString();
            message = message.Remove(message.Length-1);
            message = message.Remove(0,1);
            
            var unhandledTime = regexMatch.Groups["time"].ToString();
            unhandledTime = unhandledTime.Remove(0,1);

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = epoch.AddSeconds(long.Parse(unhandledTime)).ToLocalTime();

            Console.WriteLine(author+" @ "+ date+": "+message);
        }
    }
}catch(Exception e)
{
    Console.Write(e.StackTrace);
}


