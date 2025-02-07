﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NickvisionMoney.Shared.Models;

/// <summary>
/// A model for the configuration of the application
/// </summary>
public class Configuration
{
    private static readonly string ConfigDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Nickvision{Path.DirectorySeparatorChar}{AppInfo.Current.Name}";
    private static readonly string ConfigPath = $"{ConfigDir}{Path.DirectorySeparatorChar}config.json";
    private static Configuration? _instance;

    /// <summary>
    /// The preferred theme for the application
    /// </summary>
    public Theme Theme { get; set; }
    /// <summary>
    /// The first recent account
    /// </summary>
    [JsonInclude]
    public RecentAccount RecentAccount1 { get; private set; }
    /// <summary>
    /// The second recent account
    /// </summary>
    [JsonInclude]
    public RecentAccount RecentAccount2 { get; private set; }
    /// <summary>
    /// The third recent account
    /// </summary>
    [JsonInclude]
    public RecentAccount RecentAccount3 { get; private set; }
    /// <summary>
    /// The default color of a transaction
    /// </summary>
    public string TransactionDefaultColor { get; set; }
    /// <summary>
    /// The default color of a transfer
    /// </summary>
    public string TransferDefaultColor { get; set; }
    /// <summary>
    /// The color of accounts with Checking type
    /// </summary>
    public string AccountCheckingColor { get; set; }
    /// <summary>
    /// The color of accounts with Savings type
    /// </summary>
    public string AccountSavingsColor { get; set; }
    /// <summary>
    /// The color of accounts with Business type
    /// </summary>
    public string AccountBusinessColor { get; set; }

    /// <summary>
    /// Occurs when the configuration is saved to disk
    /// </summary>
    public event EventHandler? Saved;

    /// <summary>
    /// Constructs a Configuration
    /// </summary>
    public Configuration()
    {
        if (!Directory.Exists(ConfigDir))
        {
            Directory.CreateDirectory(ConfigDir);
        }
        Theme = Theme.System;
        RecentAccount1 = new RecentAccount();
        RecentAccount2 = new RecentAccount();
        RecentAccount3 = new RecentAccount();
        TransactionDefaultColor = "rgb(53,132,228)";
        TransferDefaultColor = "rgb(192,97,203)";
        AccountCheckingColor = "rgb(129,61,156)";
        AccountSavingsColor = "rgb(53,132,228)";
        AccountBusinessColor = "rgb(38,162,105)";
    }

    /// <summary>
    /// Gets the singleton object
    /// </summary>
    internal static Configuration Current
    {
        get
        {
            if (_instance == null)
            {
                try
                {
                    _instance = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(ConfigPath)) ?? new Configuration();
                }
                catch
                {
                    _instance = new Configuration();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Gets the list of recent accounts available
    /// </summary>
    public List<RecentAccount> RecentAccounts
    {
        get
        {
            var recents = new List<RecentAccount>();
            if (File.Exists(RecentAccount1.Path))
            {
                recents.Add(RecentAccount1);
            }
            if (File.Exists(RecentAccount2.Path))
            {
                recents.Add(RecentAccount2);
            }
            if (File.Exists(RecentAccount3.Path))
            {
                recents.Add(RecentAccount3);
            }
            return recents;
        }
    }

    /// <summary>
    /// Saves the configuration to disk
    /// </summary>
    public void Save()
    {
        File.WriteAllText(ConfigPath, JsonSerializer.Serialize(this));
        Saved?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Adds a recent account
    /// </summary>
    /// <param name="newRecentAccount">The new recent account</param>
    public void AddRecentAccount(RecentAccount newRecentAccount)
    {
        if (newRecentAccount == RecentAccount1)
        {
            RecentAccount1.Name = newRecentAccount.Name;
            RecentAccount1.Type = newRecentAccount.Type;
            return;
        }
        else if (newRecentAccount == RecentAccount2)
        {
            var temp = RecentAccount1;
            RecentAccount1 = RecentAccount2;
            RecentAccount2 = temp;
            RecentAccount1.Name = newRecentAccount.Name;
            RecentAccount1.Type = newRecentAccount.Type;
        }
        else if (newRecentAccount == RecentAccount3)
        {
            var temp1 = RecentAccount1;
            var temp2 = RecentAccount2;
            RecentAccount1 = RecentAccount3;
            RecentAccount2 = temp1;
            RecentAccount3 = temp2;
            RecentAccount1.Name = newRecentAccount.Name;
            RecentAccount1.Type = newRecentAccount.Type;
        }
        else
        {
            RecentAccount3 = RecentAccount2;
            RecentAccount2 = RecentAccount1;
            RecentAccount1 = newRecentAccount;
        }
    }
}