namespace IDEASLabUT.MSBandWearable.Application.Model.Notification
{
    public enum PayloadType
    {
        E4Band
    }

    public static class PayloadTypeExtension
    {
        public static string GetDescription(this PayloadType payloadType)
        {
            switch (payloadType)
            {
                case PayloadType.E4Band:
                    return "E4Band";
                default:
                    return null;
            }
        }

        public static PayloadType? FromDescription(string description)
        {
            if (description == null)
            {
                return null;
            }

            switch (description)
            {
                case "E4Band":
                    return PayloadType.E4Band;
                default:
                    return null;
            }
        }
    }
}
