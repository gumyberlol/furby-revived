using System.Collections.Generic;
using UnityEngine;

public class LocalyticsAndroid
{
    // this thing has been spamming my damn logcat

    static LocalyticsAndroid() {}

    public static void startSession(
        string apiKey) {}

    public static void startSession(
        string apiKey,
        string[] customDimensions) {}

    public static void endSession() {}

    public static void setCustomerData(
        string key, string value) {}

    public static void tagEvent(
        string eventName) {}

    public static void tagEventWithAttributes(
        string eventName,
        Dictionary<string, string>
        attributes) {}

    public static void tagScreen(
        string screenName) {}
}