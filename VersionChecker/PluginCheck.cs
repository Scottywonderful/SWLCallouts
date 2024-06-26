﻿// Author: Scottywonderful
// Created: 16th Feb 2024
// Version: 0.5.0.5

#region

using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace SWLCallouts.VersionChecker;

public class PluginCheck
{
    public static bool IsUpdateAvailable()
    {
        /////////////////////////////////////////////////////////
        ////**********GITHUB + LSPDFR CHECKER BELOW**********////
        /////////////////////////////////////////////////////////


        Normal("Checking installed version type");
        string versionType = Settings.VersionType;
        string curType = (versionType == "Alpha" || versionType == "Beta" || versionType == "alpha" || versionType == "beta") ? "Unstable" : "Stable";

        if (curType == "Stable")
        {
            Normal("Stable Version Detected.");
        }
        else if (curType == "Unstable")
        {
            Normal("UNSTABLE VERSION DETECTED.");
        }

        // Below is the version checker against the github releases //
        Normal("Checking installed version");
        string curVersion = Settings.PluginVersion;
        Normal("Checking LSPDFR version");
        Uri latestPubVersionUri = new($"https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=46914&textOnly=1");
        Normal("Checking Github version");
        string owner = "Scottywonderful";
        string repo = "SWLCallouts";
        Uri latestTestingVersionUri = new($"https://api.github.com/repos/{owner}/{repo}/releases/latest");

        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "request");

        try
        {
            Normal("Get a response for Pub");
            HttpResponseMessage responsePub = httpClient.GetAsync(latestPubVersionUri).Result;
            Normal("Get a response for Test");
            HttpResponseMessage responseTest = httpClient.GetAsync(latestTestingVersionUri).Result;

            if (curType == "Stable" && responsePub.IsSuccessStatusCode)
            {
                Normal("Response from LSPDFR received");
                string receivedPubData = responsePub.Content.ReadAsStringAsync().Result;
                string latestPubVersion = receivedPubData.Trim();
                    
                if (latestPubVersion == curVersion)
                {
                    Normal("Playing on latest public build.");
                    NotifyP("3dtextures", "mpgroundlogo_cops", "~w~SWLCallouts", "~g~Latest ~w~Build", $"{Arrays.PluginLoadText.PickRandom()}");
                    return false;
                }
                else if (String.Compare(curVersion, latestPubVersion) > 0)
                {
                    Log("[ERROR]: Current version is greater than public released version.");
                    // Display a message thanking the user for helping with testing
                    NotifyP("3dtextures", "mpgroundlogo_cops", "~w~SWLCallouts", $"{Arrays.PluginLoadTestingSubtitle.PickRandom()}", "~r~[ERROR] ~w~Version Type issue!<br>~w~Please update this plugin ~r~ASAP~w~!<br>~y~Public release version detected!");
                    GameFiber.Sleep(5000);
                    NotifyP("3dtextures", "mpgroundlogo_cops", "~w~SWLCallouts", "~o~Unstable Build", "This must be a mistake, please check, as you are on a ~p~future ~y~test build ~w~and may notice ~o~bugs ~w~while playing this version.");
                    return false;
                }
                else
                {
                    Log("New update available. Please update ASAP!");
                    NotifyP("commonmenu", "mp_alerttriangle", "~w~SWLCallouts ~y~Warning", "~y~A new Update is available!", $"Current Version: ~r~{curVersion}~w~<br>New Version: ~o~{latestPubVersion}~w~<br>Please ~b~update ~w~to the latest build ~r~ASAP!");

                    Print("================================================== SWLCallouts ===================================================");
                    Print("[WARNING]: A new version of SWLCallouts is available! Update to the latest build or play on your own risk.");
                    Print("[LOG]: Current Version:  " + curVersion);
                    Print("[LOG]: New Version:  " + latestPubVersion);
                    Print("================================================== SWLCallouts ===================================================");
                    return true;
                }
            }
            else if (curType == "Unstable" && responseTest.IsSuccessStatusCode)
            {
                Normal("Response from Github received");
                string receivedTestData = responseTest.Content.ReadAsStringAsync().Result;
                Normal("Response from LSPDFR received");
                string receivedPubData = responsePub.Content.ReadAsStringAsync().Result;
                string latestPubVersion = receivedPubData.Trim();

                string startTag = "\"tag_name\":\"";
                int startIndex = receivedTestData.IndexOf(startTag);
                if (startIndex != -1)
                {
                    startIndex += startTag.Length;
                    int endIndex = receivedTestData.IndexOf('"', startIndex);
                    if (endIndex != -1)
                    {
                        string latestTestVersion = receivedTestData.Substring(startIndex, endIndex - startIndex);

                        if (latestTestVersion == curVersion && String.Compare(curVersion, latestPubVersion) < 0)
                        {
                            Normal("New public update is available. Please update ASAP!");
                            NotifyP("commonmenu", "mp_alerttriangle", "~w~SWLCallouts ~y~Warning", "~y~A new Update is available!", $"Current Version: ~r~{curVersion}~w~<br>New Version: ~o~{latestPubVersion}~w~<br>Current Type: ~b~{curType}~w~<br>~r~Please update to the latest Public build from LSPDFR!");

                            Print("================================================== SWLCallouts ===================================================");
                            Print("[WARNING]: A new version of SWLCallouts is available! Update to the latest build or play on your own risk.");
                            Print("[LOG]: Current Version:  " + curVersion);
                            Print("[LOG]: New Public Version:  " + latestPubVersion);
                            Print("================================================== SWLCallouts ===================================================");
                            return true;
                        }
                        else if (latestTestVersion == curVersion)
                        {
                            Normal("Playing on latest Test build.");
                            // Display a message thanking the user for helping with testing
                            NotifyP("3dtextures", "mpgroundlogo_cops", "~w~SWLCallouts", $"{Arrays.PluginLoadTestingSubtitle.PickRandom()}", $"{Arrays.PluginLoadTestingText.PickRandom()}");
                            GameFiber.Sleep(5000);
                            NotifyP("3dtextures", "mpgroundlogo_cops", "~w~SWLCallouts", "~o~Unstable Test Build", "This must be a ~r~unstable ~p~wonder build ~w~of SWLCallouts. You may notice ~o~bugs ~w~while playing this unstable build.");
                            return false;
                        }
                        else if (String.Compare(curVersion, latestTestVersion) > 0)
                        {
                            Log("Current version is greater than current Test released version.");
                            // Display a message thanking the user for helping with testing
                            NotifyP("3dtextures", "mpgroundlogo_cops", "~w~SWLCallouts", $"{Arrays.PluginLoadTestingSubtitle.PickRandom()}", $"{Arrays.PluginLoadTestingText.PickRandom()}");
                            GameFiber.Sleep(5000);
                            NotifyP("3dtextures", "mpgroundlogo_cops", "~w~SWLCallouts", "~o~Unstable Test Build", "This must be a ~r~unstable ~p~wonder build ~w~of SWLCallouts. You may notice ~o~bugs ~w~while playing this unstable build.");
                            return false;
                        }
                        else
                        {
                            Log("New testing update is available. Please update ASAP!");
                            NotifyP("commonmenu", "mp_alerttriangle", "~w~SWLCallouts ~y~Warning", "~y~A new Update is available!", $"Current Version: ~r~{curVersion}~w~<br>New Version: ~o~{latestTestVersion}~w~<br>Current Type: ~b~{curType}~w~<br>~r~Please update to the latest Testing build from Github!");

                            Print("================================================== SWLCallouts ===================================================");
                            Print("[WARNING]: A new version of SWLCallouts is available! Update to the latest build or play on your own risk.");
                            Print("[LOG]: Current Version:  " + curVersion);
                            Print("[LOG]: New Testing Version:  " + latestTestVersion);
                            Print("================================================== SWLCallouts ===================================================");
                            return true;
                        }
                    }
                }

            }
            else
            {
                Log("Failed to receive current version type response");
                NotifyP("commonmenu", "mp_alerttriangle", "~w~SWLCallouts ~y~Warning", "~r~Failed to check for an update", "Please make sure you are ~y~connected~w~ to the internet or try to ~y~reload~w~ the plugin.");

                Print("================================================== SWLCallouts ===================================================");
                Print("[WARNING]: Failed to check for an update.");
                Print("[LOG]: Please make sure you are connected to the internet or try to reload the plugin.");
                Print("================================================== SWLCallouts ===================================================");

                return false;
            }
        }
        catch (HttpRequestException)
        {
            Log("No internet connection? - Unable to check for the latest version on LSPDFR or Github.");
            NotifyP("commonmenu", "mp_alerttriangle", "~w~SWLCallouts Warning", "~r~Failed to check for an update", "Please make sure you are ~y~connected~w~ to the internet or try to ~y~reload~w~ the plugin.");

            Print("================================================== SWLCallouts ===================================================");
            Print("[WARNING]: Failed to check for an update.");
            Print("[LOG]: Please make sure you are connected to the internet or try to reload the plugin.");
            Print("================================================== SWLCallouts ===================================================");

            return false;
        }
        catch (Exception ex)
        {
            Log("Unable to check for the latest version on LSPDFR or Github.");
            Error(ex, nameof(IsUpdateAvailable));
            NotifyP("commonmenu", "mp_alerttriangle", "~w~SWLCallouts Warning", "~r~Failed to check for an update", "Please make sure you are ~y~connected~w~ to the internet or please check your ~y~firewall~w~.");

            Print("================================================== SWLCallouts ===================================================");
            Print("[WARNING]: Failed to check for an update.");
            Print("[LOG]: Please make sure you are connected to the internet or please check your firewall.");
            Print("================================================== SWLCallouts ===================================================");

            return false;
        }
        return false;
    }
}
