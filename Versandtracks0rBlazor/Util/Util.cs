namespace Versandtracks0rBlazor
{
    public static class Util
    {

#if DEBUG
        public const string ApiUrl = "http://localhost:5000/";
#endif

#if RELEASE
                public const string ApiUrl = "https://api.versandtracks0r.treize1412.xyz/";
#endif


    }
}