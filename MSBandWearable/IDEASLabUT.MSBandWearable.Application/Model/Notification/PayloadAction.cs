using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Model.Notification
{
    public enum PayloadAction
    {
        SendMessage
    }

    public static class PayloadActionExtension
    {
        public static string GetDescription(this PayloadAction payloadAction)
        {
            switch (payloadAction)
            {
                case PayloadAction.SendMessage:
                    return "sendMessage";
                default:
                    return null;
            }
        }

        public static PayloadAction? FromDescription(string description)
        {
            if (description == null)
            {
                return null;
            }

            switch (description)
            {
                case "sendMessage":
                    return PayloadAction.SendMessage;
                default:
                    return null;
            }
        }
    }
}
